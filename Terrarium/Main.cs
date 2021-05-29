using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Terrarium
{
	public class Main : Game
	{
		GraphicsDeviceManager _graphics;
		SpriteBatch _spriteBatch;

		World _world;

		public Main()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			base.Initialize();

			_world = new World(64, 32);

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

			Window.Title = $"{1f / (float)gameTime.ElapsedGameTime.TotalSeconds}";
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.LightSkyBlue);

			_spriteBatch.Begin();

			_world.Draw(_spriteBatch);

			_spriteBatch.End();
		}
	}
}
