using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terrarium
{
	public class World
	{
		public int Width { get; private set; }
		public int Height { get; private set; }

		int[,] _tiles;

		const int MIN_GROUND_HEIGHT = 7;
		const int MAX_GROUND_HEIGHT = 35;

		public World(int width, int height)
		{
			Width = width;
			Height = height;

			_tiles = new int[width, height];
			
			// Initialize with air tiles
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					_tiles[x, y] = GameData.GetTileIdFromStrId("tile.air");
				}
			}

			GenerateWorld();
		}

		void GenerateWorld()
		{
			// Offsets from top
			int minOffset = Height - MAX_GROUND_HEIGHT;
			int maxOffset = Height - MIN_GROUND_HEIGHT;

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
						spriteBatch.Draw(GameData.GetTileData(tileId).Texture, new Vector2(x * 8, y * 8), Color.White);
				}
			}
		}
	}
}
