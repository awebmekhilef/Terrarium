using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Terrarium
{
	public class World
	{
		int[,] _tiles;
		int[,] _walls;

		const int MIN_GROUND_HEIGHT = 192;
		const int MAX_GROUND_HEIGHT = 256;

		public int Width { get; private set; }
		public int Height { get; private set; }

		Random _rand;

		public World(int width, int height)
		{
			Width = width;
			Height = height;

			_tiles = new int[Width, Height];
			_walls = new int[Width, Height];

			// Initialze with air tiles/walls
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					_tiles[x, y] = -1;
					_walls[x, y] = -1;
				}
			}

			_rand = new Random();

			GenerateWorld();
		}

		void GenerateWorld()
		{
			// Offsets from top
			int minOffset = Height - MAX_GROUND_HEIGHT;
			int maxOffset = Height - MIN_GROUND_HEIGHT;

			// Generate elevations
			int[] elevations = new int[Width];
			float[,] elevationNoise = NoiseGenerator.GenerateNoiseMap(Width, 1, 100f, 4, 0.5f, 2f);

			for (int x = 0; x < Width; x++)
				elevations[x] = (int)Util.Map(elevationNoise[x, 0], 0f, 1f, minOffset, maxOffset);

			// Fill in tiles/walls
			for (int x = 0; x < Width; x++)
			{
				_tiles[x, elevations[x]] = GameData.GetTileIdFromStrId("tile.grass");
				_walls[x, elevations[x]] = GameData.GetWallIdFromStrId("wall.dirt");

				for (int y = elevations[x] + 1; y < Height; y++)
				{
					_tiles[x, y] = GameData.GetTileIdFromStrId("tile.dirt");
					_walls[x, y] = GameData.GetWallIdFromStrId("wall.dirt");
				}
			}

			// Create caves
			float[,] caveNoise = NoiseGenerator.GenerateNoiseMap(Width, Height - minOffset, 70, 5, 0.35f, 2f);

			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height - minOffset; y++)
				{
					if (caveNoise[x, y] > 0.5f && caveNoise[x, y] < 0.6f)
						_tiles[x, minOffset + y] = -1;
				}
			}

			// Create ores
			for (int i = 0; i < Width * Height / 1200; i++)
				CreateOreVein(_rand.Next(0, Width), _rand.Next(minOffset, Height), GameData.GetTileIdFromStrId("tile.stone"), _rand.Next(3, 5));

			for (int i = 0; i < Width * Height / 1200; i++)
				CreateOreVein(_rand.Next(0, Width), _rand.Next(minOffset, Height), GameData.GetTileIdFromStrId("tile.copper"), _rand.Next(3, 5));
		}

		void CreateOreVein(int x, int y, int tileId, int size)
		{
			if (GetTile(x, y) == -1 || size <= 0)
				return;

			if (_rand.Next(0, 10) == 0)
				size += 1;

			_tiles[x, y] = tileId;

			CreateOreVein(x + 1, y, tileId, size - 1);
			CreateOreVein(x - 1, y, tileId, size - 1);
			CreateOreVein(x, y + 1, tileId, size - 1);
			CreateOreVein(x, y - 1, tileId, size - 1);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			Camera cam = Main.Instance.Camera;

			int left = Math.Max(0, cam.Bounds.Left / TileData.TILE_SIZE);
			int right = Math.Min(Width - 1, cam.Bounds.Right / TileData.TILE_SIZE) + 1;
			int top = Math.Max(0, cam.Bounds.Top / TileData.TILE_SIZE);
			int bottom = Math.Min(Height - 1, cam.Bounds.Bottom / TileData.TILE_SIZE) + 1;

			for (int x = left; x < right; x++)
			{
				for (int y = top; y < bottom; y++)
				{
					int tileId = _tiles[x, y];
					int wallId = _walls[x, y];

					if (tileId != -1)
					{
						spriteBatch.Draw(GameData.GetTileData(tileId).Texture,
							new Vector2(x * TileData.TILE_SIZE, y * TileData.TILE_SIZE), Color.White);
					}
					else if (wallId != -1)
					{
						spriteBatch.Draw(GameData.GetWallData(wallId).Texture,
							new Vector2(x * TileData.TILE_SIZE, y * TileData.TILE_SIZE), Color.White);
					}
				}
			}

			DebugDraw.AddRect(new Rectangle(0, 0, Width * TileData.TILE_SIZE, Height * TileData.TILE_SIZE), Color.Blue);
		}

		public int GetTile(int x, int y)
		{
			if (x < 0 || x >= Width || y < 0 || y >= Height)
				return -1;

			return _tiles[x, y];
		}
		
		public int GetWall(int x, int y)
		{
			if (x < 0 || x >= Width || y < 0 || y >= Height)
				return -1;

			return _walls[x, y];
		}
	}
}
