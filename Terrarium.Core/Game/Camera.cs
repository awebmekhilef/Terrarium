using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Terrarium
{
	public class Camera
	{
		public Matrix TransformMatrix
			=> Matrix.CreateTranslation(new Vector3(-_pos, 0)) *
				Matrix.CreateScale(_zoom) *
				Matrix.CreateTranslation(new Vector3(1280 / 2, 720 / 2, 0));

		public RectangleF Bounds
			=> new RectangleF(
				_pos.X - 1280 * 1f / _zoom / 2,
				_pos.Y - 720 * 1f / _zoom / 2,
				1280 * 1f / _zoom,
				720 * 1f / _zoom
			);

		Vector2 _pos;
		float _zoom;

		public Camera(Vector2 position, float zoom)
		{
			_pos = position;
			_zoom = zoom;
		}

		public void Update(float dt)
		{
			// Zooming
			if (Input.IsKeyDown(Keys.OemPlus))
				_zoom += 0.2f;
			else if (Input.IsKeyDown(Keys.OemMinus))
				_zoom -= 0.2f;

			_zoom = MathHelper.Clamp(_zoom, 1f, 5f);

			// Follow player
			_pos = Main.Player.Bounds.Center;
		}

		public Vector2 ScreenToWorldSpace(Vector2 point)
		{
			return Vector2.Transform(point, Matrix.Invert(TransformMatrix));
		}
	}
}
