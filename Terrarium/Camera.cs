using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Terrarium
{
	public class Camera
	{
		Vector2 _pos;
		float _zoom = 0.25f;

		const float ZOOM_SPEED = 3f;
		const float MOVE_SPEED = 700f;

		public Camera(Vector2 position)
		{
			_pos = position;
		}

		public void Update(float dt)
		{
			_zoom += Input.ScrollWheelDelta * ZOOM_SPEED * dt;
			_zoom = MathHelper.Clamp(_zoom, 0.25f, 5f);

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

		public Vector2 ScreenToWorldSpace(Vector2 point)
		{
			return Vector2.Transform(point, Matrix.Invert(TransformMatrix));
		}
	}
}
