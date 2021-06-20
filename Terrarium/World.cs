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
		const int TREE_DENSITY = 5;

		public int Width { get; private set; }
		public int Height { get; private set; }

		Random _rand;
		Rectangle[] _tileMaskRects;

		public World(int width, int height)
		{
			Console.WriteLine(CanWallMerge(1,0));

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
			_tileMaskRects = Util.RectsFromTexture(GameData.TileMasks, 8, 8);

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

			// Create trees
			for (int i = 0; i < Width / TREE_DENSITY; i++)
			{
				if (_rand.Next(0, 2) == 0)
				{
					int x = i * TREE_DENSITY;
					CreateTree(x, elevations[x] - 1, _rand.Next(7, 15));
				}
			}
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

		void CreateTree(int x, int y, int height)
		{
			// Check if has grass tile below
			if (GetTile(x, y + 1) != GameData.GetTileIdFromStrId("tile.grass"))
				return;

			int top = y - height - 1;
			int trunkId = GameData.GetWallIdFromStrId("wall.trunk");
			int leavesId = GameData.GetWallIdFromStrId("wall.leaves");

			// Create trunk
			for (int i = y; i >= top; i--)
				_walls[x, i] = trunkId;

			// Create canopy
			SetWall(x - 1, top, leavesId);
			SetWall(x + 1, top, leavesId);

			for (int i = -2; i < 3; i++)
				SetWall(x - i, top - 1, leavesId);

			for (int i = -2; i < 3; i++)
				SetWall(x - i, top - 2, leavesId);

			for (int i = -1; i < 2; i++)
				SetWall(x - i, top - 3, leavesId);
		}

		int GetTileMaskIndex(int x, int y, int tileId)
		{
			// Get adjacent tiles (NWES)
			int[] adjTiles = new int[] { 0, 0, 0, 0 };

			if (CanTileMerge(GetTile(x, y - 1), tileId))
				adjTiles[0] = 1;
			if (CanTileMerge(GetTile(x - 1, y), tileId))
				adjTiles[1] = 1;
			if (CanTileMerge(GetTile(x + 1, y), tileId))
				adjTiles[2] = 1;
			if (CanTileMerge(GetTile(x, y + 1), tileId))
				adjTiles[3] = 1;

			// Calculate mask
			return adjTiles[0] + adjTiles[1] * 2 + adjTiles[2] * 4 + adjTiles[3] * 8;
		}

		bool CanTileMerge(int tileId1, int tileId2)
		{
			if (tileId1 == -1 || tileId2 == -1)
				return false;

			TileData tile1 = GameData.GetTileData(tileId1);
			TileData tile2 = GameData.GetTileData(tileId2);

			return Array.Exists(tile1.MergeTileStrIds, s => s == tile2.StrId) ||
				Array.Exists(tile2.MergeTileStrIds, s => s == tile1.StrId);
		}

		int GetWallMaskIndex(int x, int y, int wallId)
		{
			// Get adjacent walls (NWES)
			int[] adjWalls = new int[] { 0, 0, 0, 0 };

			if (CanWallMerge(GetWall(x, y - 1), wallId))
				adjWalls[0] = 1;
			if (CanWallMerge(GetWall(x - 1, y), wallId))
				adjWalls[1] = 1;
			if (CanWallMerge(GetWall(x + 1, y), wallId))
				adjWalls[2] = 1;
			if (CanWallMerge(GetWall(x, y + 1), wallId))
				adjWalls[3] = 1;

			// Calculate mask
			return adjWalls[0] + adjWalls[1] * 2 + adjWalls[2] * 4 + adjWalls[3] * 8;
		}

		bool CanWallMerge(int wallId1, int wallId2)
		{
			if (wallId1 == -1 || wallId2 == -1)
				return false;

			TileData wall1 = GameData.GetWallData(wallId1);
			TileData wall2 = GameData.GetWallData(wallId2);

			return Array.Exists(wall1.MergeTileStrIds, s => s == wall2.StrId) ||
				Array.Exists(wall2.MergeTileStrIds, s => s == wall1.StrId);
		}

		public void DrawTiles(SpriteBatch spriteBatch)
		{
			Camera cam = Main.Camera;

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
		}

		public void DrawTileMasks(SpriteBatch spriteBatch)
		{
			Camera cam = Main.Camera;

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
						spriteBatch.Draw(GameData.TileMasks,
							new Vector2(x * TileData.TILE_SIZE, y * TileData.TILE_SIZE),
							_tileMaskRects[GetTileMaskIndex(x, y, tileId)], Color.White);
					}
					else if (wallId != -1)
					{
						spriteBatch.Draw(GameData.TileMasks,
							new Vector2(x * TileData.TILE_SIZE, y * TileData.TILE_SIZE),
							_tileMaskRects[GetWallMaskIndex(x, y, wallId)], Color.White);
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

		public int GetWall(int x, int y)
		{
			if (x < 0 || x >= Width || y < 0 || y >= Height)
				return -1;

			return _walls[x, y];
		}

		public void SetTile(int x, int y, int id)
		{
			if (x < 0 || x >= Width || y < 0 || y >= Height)
				return;

			_tiles[x, y] = id;
		}

		public void SetWall(int x, int y, int id)
		{
			if (x < 0 || x >= Width || y < 0 || y >= Height)
				return;

			_walls[x, y] = id;
		}
	}
}
