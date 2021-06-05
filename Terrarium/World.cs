using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terrarium
{
	public class World
	{
		int[,] _tiles;

		const int MIN_GROUND_HEIGHT = 7;
		const int MAX_GROUND_HEIGHT = 35;

		public int Width { get; private set; }
		public int Height { get; private set; }

		public World()
		{
			Width = 1280 / TileData.TILE_SIZE;
			Height = 720 / TileData.TILE_SIZE;

			_tiles = new int[Width, Height];

			GenerateWorld();
		}

		void GenerateWorld()
		{
			// Offsets from top
			int minOffset = Height - MAX_GROUND_HEIGHT;
			int maxOffset = Height - MIN_GROUND_HEIGHT;

			// Generate noise map
			int[] elevations = new int[Width];
			float[] values = NoiseGenerator.GenerateNoiseMap(Width, 15);

			for (int x = 0; x < Width; x++)
				elevations[x] = (int)Util.Map(values[x], -1f, 1f, minOffset, maxOffset);

			// Fill in tiles
			for (int x = 0; x < Width; x++)
			{
				_tiles[x, elevations[x]] = GameData.GetTileIdFromStrId("tile.grass");

				for (int y = elevations[x] + 1; y < Height; y++)
					_tiles[x, y] = GameData.GetTileIdFromStrId("tile.dirt");
			}
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
		}

		public int GetTile(int x, int y)
		{
			if (x < 0 || x >= Width || y < 0 || y >= Height)
				return -1;

			return _tiles[x, y];
		}

		public Rectangle GetTileBounds(int x, int y)
		{
			int tileSize = TileData.TILE_SIZE;
			return new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
		}
	}
}
