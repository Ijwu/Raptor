﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;

namespace Raptor
{
	/// <summary>
	/// Manages input.
	/// </summary>
	public static class Input
	{
		/// <summary>
		/// Represents special keys.
		/// </summary>
		[Flags]
		public enum SpecialKeys
		{
			Up = 1,
			Down = 2,
			Backspace = 4,
			V = 8
		}

		static int keys;
		static int specialKeys;
		static char[] keyCodes = new char[10];
		static byte[] specialKeyCodes = new byte[10];

		static KeyboardState LastKeyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
		static MouseState LastMouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
		static KeyboardState Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
		static MouseState Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();

		/// <summary>
		/// Gets the active special keys.
		/// </summary>
		public static SpecialKeys ActiveSpecialKeys { get; private set; }
		/// <summary>
		/// Gets if an alt key is down.
		/// </summary>
		public static bool Alt
		{
			get { return Keyboard.IsKeyDown(Keys.LeftAlt) || Keyboard.IsKeyDown(Keys.RightAlt); }
		}
		/// <summary>
		/// Gets if a control key is down.
		/// </summary>
		public static bool Control
		{
			get { return Keyboard.IsKeyDown(Keys.LeftControl) || Keyboard.IsKeyDown(Keys.RightControl); }
		}
		/// <summary>
		/// Gets the mouse delta X position.
		/// </summary>
		public static int MouseDX
		{
			get { return Mouse.X - LastMouse.X; }
		}
		/// <summary>
		/// Gets the mouse delta Y position.
		/// </summary>
		public static int MouseDY
		{
			get { return Mouse.Y - LastMouse.Y; }
		}
		/// <summary>
		/// Gets if the LMB is clicked.
		/// </summary>
		public static bool MouseLeftClick
		{
			get { return Mouse.LeftButton == ButtonState.Pressed && LastMouse.LeftButton == ButtonState.Released; }
		}
		/// <summary>
		/// Gets if the LMB is pressed.
		/// </summary>
		public static bool MouseLeftDown
		{
			get { return Mouse.LeftButton == ButtonState.Pressed; }
		}
		/// <summary>
		/// Gets if the LMB is released.
		/// </summary>
		public static bool MouseLeftRelease
		{
			get { return Mouse.LeftButton != ButtonState.Pressed && LastMouse.LeftButton == ButtonState.Pressed; }
		}
		/// <summary>
		/// Gets if the RMB is clicked.
		/// </summary>
		public static bool MouseRightClick
		{
			get { return Mouse.RightButton == ButtonState.Pressed && LastMouse.RightButton == ButtonState.Released; }
		}
		/// <summary>
		/// Gets if the RMB is pressed.
		/// </summary>
		public static bool MouseRightDown
		{
			get { return Mouse.RightButton == ButtonState.Pressed; }
		}
		/// <summary>
		/// Gets if the RMB is released.
		/// </summary>
		public static bool MouseRightRelease
		{
			get { return Mouse.RightButton != ButtonState.Pressed && LastMouse.RightButton == ButtonState.Pressed; }
		}
		/// <summary>
		/// Gets the mouse scroll wheel value.
		/// </summary>
		public static int MouseScroll
		{
			get { return Mouse.ScrollWheelValue - LastMouse.ScrollWheelValue; }
		}
		/// <summary>
		/// Gets the mouse X position.
		/// </summary>
		public static int MouseX
		{
			get { return Mouse.X; }
		}
		/// <summary>
		/// Gets the mouse Y position.
		/// </summary>
		public static int MouseY
		{
			get { return Mouse.Y; }
		}
		/// <summary>
		/// Gets if a shift key is down.
		/// </summary>
		public static bool Shift
		{
			get { return Keyboard.IsKeyDown(Keys.LeftShift) || Keyboard.IsKeyDown(Keys.RightShift); }
		}
		/// <summary>
		/// Gets the typed string.
		/// </summary>
		public static string TypedString
		{
			get;
			internal set;
		}

		internal static void FilterMessage(System.Windows.Forms.Message m)
		{
			if (m.Msg == 0x100)
			{
				if (specialKeys < 10)
					specialKeyCodes[specialKeys++] = (byte)m.WParam;

				IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(m));
				Marshal.StructureToPtr(m, ptr, true);
				TranslateMessage(ptr);
			}
			// WM_CHAR
			else if (m.Msg == 0x102)
			{
				if (keys < 10)
					keyCodes[keys++] = (char)m.WParam;
			}
		}
		/// <summary>
		/// Gets if a key is down.
		/// </summary>
		public static bool IsKeyDown(Keys key)
		{
			return Keyboard.IsKeyDown(key);
		}
		/// <summary>
		/// Gets if a key was released; that is, if the key is currently depressed but was pressed before.
		/// </summary>
		public static bool IsKeyReleased(Keys key)
		{
			return Keyboard.IsKeyUp(key) && LastKeyboard.IsKeyDown(key);
		}
		/// <summary>
		/// Gets if a key was tapped; that is, if the key is currently pressed but was depressed before.
		/// </summary>
		public static bool IsKeyTapped(Keys key)
		{
			return Keyboard.IsKeyDown(key) && LastKeyboard.IsKeyUp(key);
		}
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
		static extern bool TranslateMessage(IntPtr message);
		internal static void Update()
		{
			LastKeyboard = Keyboard;
			Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
			LastMouse = Mouse;
			Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();

			ActiveSpecialKeys = 0;
			TypedString = String.Join("", keyCodes.Where(c => c >= 32 && c != 127));
			for (int i = 0; i < specialKeys; i++)
			{
				switch (specialKeyCodes[i])
				{
					case 0x08:
						ActiveSpecialKeys |= SpecialKeys.Backspace;
						break;
					case 0x26:
						ActiveSpecialKeys |= SpecialKeys.Up;
						break;
					case 0x28:
						ActiveSpecialKeys |= SpecialKeys.Down;
						break;
					case 0x56:
						ActiveSpecialKeys |= SpecialKeys.V;
						break;
				}
			}
			keys = specialKeys = 0;
		}
	}
}