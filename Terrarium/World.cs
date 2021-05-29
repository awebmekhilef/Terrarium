using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Terrarium
{
	public class World
	{
		public int Width { get; private set; }
		public int Height { get; private set; }

		int[,] _tiles;

		const int MIN_GROUND_HEIGHT = 10;
		const int MAX_GROUND_HEIGHT = 22;

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
			int[] elevations = new int[Width];

			// Offsets from top
			int minOffset = Height - MAX_GROUND_HEIGHT;
			int maxOffset = Height - MIN_GROUND_HEIGHT;

			Random rand = new Random();

			//Randomly ocsillate up or down
			for (int i = 0; i < Width; i++)
			{
				// Move up or down
				int dir = rand.Next(0, 2) == 1 ? 1 : -1;

				if (i > 0)
				{
					if (elevations[i - 1] + dir > maxOffset || elevations[i - 1] + dir < minOffset)
						dir *= -1;

					elevations[i] = elevations[i - 1] + dir;
				}
				else
				{
					elevations[0] = rand.Next(minOffset, maxOffset);
				}
			}

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
