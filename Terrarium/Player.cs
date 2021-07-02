using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Terrarium
{
	/// <summary>
	/// The brave adventurer!
	/// </summary>
	public class Player
	{
		public RectangleF Bounds
			=> new RectangleF(_pos + LocalBounds.Location, LocalBounds.Size);

		public RectangleF LocalBounds
			=> new RectangleF(3, 0, 10, 24);

		Vector2 _prevPos;
		Vector2 _pos;
		Vector2 _vel;

		bool _isGrounded;
		bool _flip;

		AnimationPlayer _animPlayer;
		string _currAnim;

		World _world;
		Camera _cam;

		const float GRAVITY = 700;
		const float MOVE_SPEED = 100;
		const float JUMP_FORCE = 250;
		const float MAX_FALL_SPEED = 400;

		/// <summary>
		/// Spawns a player at a position and sets up animations.
		/// </summary>
		public Player(Vector2 position)
		{
			_pos = position;

			Rectangle[] frames = Util.RectsFromTexture(GameData.Player, 16, 24);

			_animPlayer = new AnimationPlayer();

			_animPlayer.Add("Idle", new Animation(true, 1, frames[0]));
			_animPlayer.Add("Jump", new Animation(true, 1, frames[1]));
			_animPlayer.Add("Run", new Animation(true, 20, frames[2..15]));

			_currAnim = "Idle";

			_world = Main.World;
			_cam = Main.Camera;
		}

		/// <summary>
		/// Updates player movement, tile placing and animation
		/// </summary>
		public void Update(float dt)
		{
			#region Movement and Collisons

			// Gravity
			_vel.Y += GRAVITY * dt;
			_vel.Y = MathHelper.Clamp(_vel.Y, -MAX_FALL_SPEED, MAX_FALL_SPEED);

			// Horizontal movement
			if (Input.IsKeyHeld(Keys.A))
			{
				_vel.X = -MOVE_SPEED;
				_flip = true;
			}
			else if (Input.IsKeyHeld(Keys.D))
			{
				_vel.X = MOVE_SPEED;
				_flip = false;
			}
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
			HandleCollision(CollisionDirection.Horizontal);

			_pos.Y += _vel.Y * dt;
			HandleCollision(CollisionDirection.Vertical);

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

			#region Animations

			if (!_animPlayer.IsPlaying(_currAnim))
				_animPlayer.Play(_currAnim);

			if (_isGrounded)
			{
				if (_vel.X != 0f)
					_currAnim = "Run";
				else
					_currAnim = "Idle";
			}
			else
				_currAnim = "Jump";

			_animPlayer.Update(dt);

			#endregion
		}

		/// <summary>
		/// Draws the player
		/// </summary>
		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(GameData.Player, _pos, _animPlayer.GetFrameBounds(), Color.White,
				0f, Vector2.Zero, 1f, _flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
		}

		/// <summary>
		/// Displace the player in the given direction based on intersection depth
		/// </summary>
		void HandleCollision(CollisionDirection direction)
		{
			RectangleF bounds = Bounds;

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
						Vector2 depth = Util.GetIntersectionDepth(bounds, tileBounds);

						if (direction == CollisionDirection.Horizontal)
							_pos.X += depth.X;
						else
						{
							_pos.Y += depth.Y;

							if (depth.Y <= 0)
								_isGrounded = true;
						}

						bounds = Bounds;
					}
				}
			}
		}

		enum CollisionDirection
		{
			Horizontal, Vertical
		}
	}
}
