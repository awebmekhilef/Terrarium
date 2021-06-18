using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Terrarium
{
	public class Camera
	{
		Vector2 _pos;
		float _zoom = 2.5f;

		const float MOVE_SPEED = 500f;

		public Camera(Vector2 position)
		{
			_pos = position;
		}

		public void Update(float dt)
		{
			// Zooming
			if (Input.IsKeyDown(Keys.OemPlus))
				_zoom += 0.25f;
			else if(Input.IsKeyDown(Keys.OemMinus))
				_zoom -= 0.25f;

			_zoom = MathHelper.Clamp(_zoom, 0.25f, 5f);

			// Movement
			if (Input.IsKeyHeld(Keys.A))
				_pos.X -= MOVE_SPEED * dt;
			else if (Input.IsKeyHeld(Keys.D))
				_pos.X += MOVE_SPEED * dt;

			if (Input.IsKeyHeld(Keys.W))
				_pos.Y -= MOVE_SPEED * dt;
			else if (Input.IsKeyHeld(Keys.S))
				_pos.Y += MOVE_SPEED * dt;
		}

		public Matrix TransformMatrix
			=> Matrix.CreateTranslation(new Vector3(-_pos, 0)) *
				Matrix.CreateScale(_zoom) *
				Matrix.CreateTranslation(new Vector3(1280 / 2, 720 / 2, 0));

		public Rectangle Bounds
			=> new Rectangle(
				(int)(_pos.X - 1280 * 1f / _zoom / 2),
				(int)(_pos.Y - 720 * 1f / _zoom / 2),
				(int)(1280 * 1f / _zoom), 
				(int)(720 * 1f / _zoom)
			);

		public Vector2 ScreenToWorldSpace(Vector2 point)
		{
			return Vector2.Transform(point, Matrix.Invert(TransformMatrix));
		}
	}
}
