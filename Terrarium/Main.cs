using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Terrarium
{
	public class Main : Game
	{
		GraphicsDeviceManager _graphics;
		SpriteBatch _spriteBatch;

		public static World World { get; private set; }
		public static Camera Camera { get; private set; }
		public static Player Player { get; private set; }

		RenderTarget2D _tileRT;
		RenderTarget2D _tileMaskRT;

		public Main()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			base.Initialize();

			World = new World(128, 128);

			Vector2 worldCenter = new Vector2(World.Width * TileData.TILE_SIZE / 2f, World.Height * TileData.TILE_SIZE / 2f);

			Camera = new Camera(worldCenter, 3f);
			Player = new Player(worldCenter);

			PresentationParameters pp = GraphicsDevice.PresentationParameters;

			_tileRT = new RenderTarget2D(GraphicsDevice, 1280, 720, false, pp.BackBufferFormat, DepthFormat.None);
			_tileMaskRT = new RenderTarget2D(GraphicsDevice, 1280, 720, false, pp.BackBufferFormat, DepthFormat.None);

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

			Player.Update(dt);
			Camera.Update(dt);

			Window.Title = $"{1f / dt}";
		}

		protected override void Draw(GameTime gameTime)
		{
			#region Draw Tiles

			GraphicsDevice.SetRenderTarget(_tileRT);
			GraphicsDevice.Clear(Color.Transparent);

			_spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.TransformMatrix);

			World.DrawTiles(_spriteBatch);

			_spriteBatch.End();

			#endregion

			#region Draw Tile Masks

			GraphicsDevice.SetRenderTarget(_tileMaskRT);
			GraphicsDevice.Clear(Color.Transparent);

			_spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.TransformMatrix);

			World.DrawTileMasks(_spriteBatch);

			_spriteBatch.End();

			#endregion
			
			#region Draw Masking Effect

			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.LightSkyBlue);

			_spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: GameData.TileMaskEffect);

			GameData.TileMaskEffect.Parameters["Mask"].SetValue(_tileMaskRT);

			_spriteBatch.Draw(_tileRT, Vector2.Zero, Color.White);

			_spriteBatch.End();

			#endregion

			#region Draw Others

			_spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.TransformMatrix);

			Player.Draw(_spriteBatch);

			DebugDraw.Draw(_spriteBatch);

			_spriteBatch.End();

			#endregion
		}
	}
}
