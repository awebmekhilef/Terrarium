using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terrarium
{
	public class World
	{
		int[,] _tiles;

		const int MIN_GROUND_HEIGHT = 7;
		const int MAX_GROUND_HEIGHT = 35;

		const int WIDTH = 160;
		const int HEIGHT = 90;

		public World()
		{
			_tiles = new int[WIDTH, HEIGHT];

			GenerateWorld();
		}

		void GenerateWorld()
		{
			// Offsets from top
			int minOffset = HEIGHT - MAX_GROUND_HEIGHT;
			int maxOffset = HEIGHT - MIN_GROUND_HEIGHT;

			// Generate noise map
			int[] elevations = new int[WIDTH];
			float[] values = NoiseGenerator.GenerateNoiseMap(WIDTH, 15);

			for (int x = 0; x < WIDTH; x++)
				elevations[x] = (int)Util.Map(values[x], -1f, 1f, minOffset, maxOffset);

			// Fill in tiles
			for (int x = 0; x < WIDTH; x++)
			{
				_tiles[x, elevations[x]] = GameData.GetTileIdFromStrId("tile.grass");

				for (int y = elevations[x] + 1; y < HEIGHT; y++)
					_tiles[x, y] = GameData.GetTileIdFromStrId("tile.dirt");
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			for (int x = 0; x < WIDTH; x++)
			{
				for (int y = 0; y < HEIGHT; y++)
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
			if (x < 0 || x >= WIDTH || y < 0 || y >= HEIGHT)
				return -1;

			return _tiles[x, y];
		}
	}
}
