using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Terrarium
{
	public class Main : Game
	{
		GraphicsDeviceManager _graphics;
		SpriteBatch _spriteBatch;

		public static Main Instance { get; private set; }

		public World World { get; private set; }
		public Camera Camera { get; private set; }

		public Main()
		{
			Instance = this;

			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			IsFixedTimeStep = false;
			_graphics.SynchronizeWithVerticalRetrace = false;
		}

		protected override void Initialize()
		{
			base.Initialize();

			World = new World(512, 256);
			Camera = new Camera(new Vector2(World.Width * TileData.TILE_SIZE / 2f, World.Height * TileData.TILE_SIZE / 2f));

			_graphics.PreferredBackBufferWidth = 1280;
			_graphics.PreferredBackBufferHeight = 720;
			_graphics.ApplyChanges();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			GameData.Load(Content);
		}

		protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

			Input.Update();

			Camera.Update(dt);

			Window.Title = $"{1f / dt}";
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.LightSkyBlue);

			_spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.TransformMatrix);

			World.Draw(_spriteBatch);

			DebugDraw.Draw(_spriteBatch);

			_spriteBatch.End();
		}
	}
}
