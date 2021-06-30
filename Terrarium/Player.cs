using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Terrarium
{
	public class Player
	{
		public RectangleF Bounds
			=> new RectangleF(_pos.X, _pos.Y, 12, 23);

		Vector2 _prevPos;
		Vector2 _pos;
		Vector2 _vel;

		bool _isGrounded;

		World _world;
		Camera _cam;

		const float GRAVITY = 700;
		const float MOVE_SPEED = 100;
		const float JUMP_FORCE = 250;
		const float MAX_FALL_SPEED = 400;

		public Player(Vector2 position)
		{
			_pos = position;

			_world = Main.World;
			_cam = Main.Camera;
		}

		public void Update(float dt)
		{
			#region Movement and Collisons

			// Gravity
			_vel.Y += GRAVITY * dt;
			_vel.Y = MathHelper.Clamp(_vel.Y, -MAX_FALL_SPEED, MAX_FALL_SPEED);

			// Horizontal movement
			if (Input.IsKeyHeld(Keys.A))
				_vel.X = -MOVE_SPEED;
			else if (Input.IsKeyHeld(Keys.D))
				_vel.X = MOVE_SPEED;
			else
				_vel.X = 0f;

			// Jumping
			if (Input.IsKeyDown(Keys.Space) && _isGrounded)
			{
				_vel.Y = -JUMP_FORCE;
				_isGrounded = false;
			}

			// Calculate position and collsions
			_prevPos = _pos;

			_pos.X += _vel.X * dt;
			HandleCollisionsX();

			_pos.Y += _vel.Y * dt;
			HandleCollisionsY();

			// Reset velocity if colliding
			if (_prevPos.X == _pos.X)
				_vel.X = 0f;

			if (_prevPos.Y == _pos.Y)
				_vel.Y = 0f;

			#endregion

			#region Tile placing and removing

			Vector2 mousePos = _cam.ScreenToWorldSpace(Input.MousePosition);
			Point tilePos = new Point((int)mousePos.X / 8, (int)mousePos.Y / 8);

			// Dont place tile in player bounds
			if (!Bounds.Intersects((RectangleF)_world.GetTileBounds(tilePos.X, tilePos.Y)))
			{
				// Place block
				if (Input.IsMouseButtonHeld(MouseButton.Left) && _world.GetBlock(tilePos.X, tilePos.Y) == -1)
					_world.SetBlock(tilePos.X, tilePos.Y, GameData.GetBlockIdFromStrId("block.dirt"));
				// Remove blocks
				else if (Input.IsMouseButtonHeld(MouseButton.Right) && _world.GetBlock(tilePos.X, tilePos.Y) != -1)
					_world.SetBlock(tilePos.X, tilePos.Y, -1);
			}

			#endregion
		}

		void HandleCollisionsX()
		{
			RectangleF bounds = Bounds;
			RectangleF prevBounds = new RectangleF(_prevPos, bounds.Size);

			int left = (int)bounds.Left / TileData.TILE_SIZE;
			int right = (int)bounds.Right / TileData.TILE_SIZE;
			int top = (int)bounds.Top / TileData.TILE_SIZE;
			int bottom = (int)bounds.Bottom / TileData.TILE_SIZE;

			for (int x = left; x <= right; x++)
			{
				for (int y = top; y <= bottom; y++)
				{
					if (_world.GetBlock(x, y) == -1)
						continue;

					RectangleF tileBounds = (RectangleF)_world.GetTileBounds(x, y);

					if (bounds.Intersects(tileBounds))
					{
						// Was previously to left of block
						if (prevBounds.Right <= tileBounds.Left)
							_pos.X = tileBounds.Left - bounds.Width;

						// Was previoulsy below block
						if (prevBounds.Left >= tileBounds.Right)
							_pos.X = tileBounds.Right;
					}
				}
			}
		}

		void HandleCollisionsY()
		{
			RectangleF bounds = Bounds;
			RectangleF prevBounds = new RectangleF(_prevPos, bounds.Size);

			int left = (int)bounds.Left / TileData.TILE_SIZE;
			int right = (int)bounds.Right / TileData.TILE_SIZE;
			int top = (int)bounds.Top / TileData.TILE_SIZE;
			int bottom = (int)bounds.Bottom / TileData.TILE_SIZE;

			_isGrounded = false;

			for (int x = left; x <= right; x++)
			{
				for (int y = top; y <= bottom; y++)
				{
					if (_world.GetBlock(x, y) == -1)
						continue;

					RectangleF tileBounds = (RectangleF)_world.GetTileBounds(x, y);

					if (bounds.Intersects(tileBounds))
					{
						// Was previously above block
						if (prevBounds.Bottom <= tileBounds.Top)
						{
							_pos.Y = tileBounds.Top - bounds.Height;

							_isGrounded = true;
						}

						// Was previoulsy below block
						if (prevBounds.Top >= tileBounds.Bottom)
							_pos.Y = tileBounds.Bottom;
					}
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.FillRectangle(Bounds.Location, Bounds.Size, Color.White);
		}
	}
}
