using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Terrarium
{
	public class World
	{
		int[,] _tiles;

		const int MIN_GROUND_HEIGHT = 48;
		const int MAX_GROUND_HEIGHT = 128;

		public int Width { get; private set; }
		public int Height { get; private set; }

		Random _rand;

		public World(int width, int height)
		{
			Width = width;
			Height = height;

			_tiles = new int[Width, Height];
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
			float[] values = NoiseGenerator.GenerateNoiseMap(Width, 100, 6, 0.5f, 2f);

			for (int x = 0; x < Width; x++)
				elevations[x] = (int)Util.Map(values[x], -1f, 1f, minOffset, maxOffset);

			// Fill in tiles
			for (int x = 0; x < Width; x++)
			{
				_tiles[x, elevations[x]] = GameData.GetTileIdFromStrId("tile.grass");

				for (int y = elevations[x] + 1; y < Height; y++)
					_tiles[x, y] = GameData.GetTileIdFromStrId("tile.dirt");
			}

			// Create ores
			for (int i = 0; i < Width * Height / 1200; i++)
				CreateOreVein(_rand.Next(0, Width), _rand.Next(minOffset, Height), GameData.GetTileIdFromStrId("tile.stone"), _rand.Next(3, 4));

			for (int i = 0; i < Width * Height / 1200; i++)
				CreateOreVein(_rand.Next(0, Width), _rand.Next(minOffset, Height), GameData.GetTileIdFromStrId("tile.copper"), _rand.Next(3, 5));
		}

		void CreateOreVein(int x, int y, int tileId, int size)
		{
			if (GetTile(x, y) <= 0 || size <= 0)
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
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					int tileId = _tiles[x, y];

					if (tileId != GameData.GetTileIdFromStrId("tile.air"))
					{
						spriteBatch.Draw(GameData.GetTileData(tileId).Texture,
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
	}
}
