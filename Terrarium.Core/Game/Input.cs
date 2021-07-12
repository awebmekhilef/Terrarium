using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Terrarium
{
	public static class Input
	{
		private static KeyboardState _prevKState;
		private static KeyboardState _kState;

		private static MouseState _prevMState;
		private static MouseState _mState;

		public static bool IsKeyHeld(Keys key)
		{
			return _kState.IsKeyDown(key);
		}

		public static bool IsKeyDown(Keys key)
		{
			return _kState.IsKeyDown(key) && _prevKState.IsKeyUp(key);
		}

		public static bool IsKeyReleased(Keys key)
		{
			return _kState.IsKeyUp(key) && _prevKState.IsKeyDown(key);
		}

		public static bool IsMouseButtonHeld(MouseButton mouseButton)
		{
			switch (mouseButton)
			{
				case MouseButton.Left: return _mState.LeftButton == ButtonState.Pressed;
				case MouseButton.Right: return _mState.RightButton == ButtonState.Pressed;
				case MouseButton.Middle: return _mState.MiddleButton == ButtonState.Pressed;
			}

			return false;
		}

		public static bool IsMouseButtonDown(MouseButton mouseButton)
		{
			switch (mouseButton)
			{
				case MouseButton.Left:
					return _mState.LeftButton == ButtonState.Pressed &&
						_prevMState.LeftButton == ButtonState.Released;
				case MouseButton.Right:
					return _mState.RightButton == ButtonState.Pressed &&
						_prevMState.RightButton == ButtonState.Released;
				case MouseButton.Middle:
					return _mState.MiddleButton == ButtonState.Pressed &&
						_prevMState.MiddleButton == ButtonState.Released;
			}

			return false;
		}

		public static bool IsMouseButtonReleased(MouseButton mouseButton)
		{
			switch (mouseButton)
			{
				case MouseButton.Left:
					return _mState.LeftButton == ButtonState.Released &&
						_prevMState.LeftButton == ButtonState.Pressed;
				case MouseButton.Right:
					return _mState.RightButton == ButtonState.Released &&
						_prevMState.RightButton == ButtonState.Pressed;
				case MouseButton.Middle:
					return _mState.MiddleButton == ButtonState.Released &&
						_prevMState.MiddleButton == ButtonState.Pressed;
			}

			return false;
		}

		public static int ScrollWheelDelta 
			=> MathF.Sign(_mState.ScrollWheelValue - _prevMState.ScrollWheelValue);

		public static Vector2 MousePosition
			=> new Vector2(_mState.X, _mState.Y);

		public static void Update()
		{
			_prevKState = _kState;
			_kState = Keyboard.GetState();

			_prevMState = _mState;
			_mState = Mouse.GetState();
		}
	}

	public enum MouseButton
	{
		Left,
		Right,
		Middle
	}
}