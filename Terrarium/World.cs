using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terrarium
{
	public class World
	{
		public int Width { get; private set; }
		public int Height { get; private set; }

		int[,] _tiles;

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
			// Generate flat terrain
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					if (y > Height / 2)
						_tiles[x, y] = GameData.GetTileIdFromStrId("tile.dirt");
					else if (y == Height / 2)
						_tiles[x, y] = GameData.GetTileIdFromStrId("tile.grass");
				}
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
