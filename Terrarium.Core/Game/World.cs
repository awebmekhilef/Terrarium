using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Terrarium
{
	public class World
	{
		public int Width { get; private set; }
		public int Height { get; private set; }

		int[,] _blocks;
		int[,] _walls;
		int[,] _tileMasks;

		Random _rand;
		Rectangle[] _tileMaskRects;

		const int MIN_GROUND_HEIGHT = 16;
		const int MAX_GROUND_HEIGHT = 64;
		const int TREE_DENSITY = 5;

		/// <summary>
		/// Creates a new world with a given width and height
		/// </summary>
		public World(int width, int height)
		{
			Width = width;
			Height = height;

			_blocks = new int[Width, Height];
			_walls = new int[Width, Height];
			_tileMasks = new int[Width, Height];

			// Initialze with empty tiles
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					_blocks[x, y] = -1;
					_walls[x, y] = -1;
					_tileMasks[x, y] = -1;
				}
			}

			_rand = new Random();
			_tileMaskRects = Util.RectsFromTexture(GameData.TileMasks, 8, 8);

			GenerateWorld();
		}

		/// <summary>
		/// Generates elevations, caves, ores and trees
		/// </summary>
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

			// Fill in tiles
			int grassId = GameData.GetBlockIdFromStrId("block.grass");
			int dirtBlockId = GameData.GetBlockIdFromStrId("block.dirt");
			int dirtWallId = GameData.GetWallIdFromStrId("wall.dirt");

			for (int x = 0; x < Width; x++)
			{
				SetBlock(x, elevations[x], grassId);
				SetWall(x, elevations[x], dirtWallId);

				for (int y = elevations[x] + 1; y < Height; y++)
				{
					SetBlock(x, y, dirtBlockId);
					SetWall(x, y, dirtWallId);
				}
			}

			// Create caves
			float[,] caveNoise = NoiseGenerator.GenerateNoiseMap(Width, Height - minOffset, 70, 5, 0.35f, 2f);

			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height - minOffset; y++)
				{
					if (caveNoise[x, y] > 0.5f && caveNoise[x, y] < 0.6f)
						SetBlock(x, minOffset + y, -1);
				}
			}

			// Create ores
			int stoneId = GameData.GetBlockIdFromStrId("block.stone");
			int copperId = GameData.GetBlockIdFromStrId("block.copper");

			for (int i = 0; i < Width * Height / 1200; i++)
				CreateOreCluster(_rand.Next(0, Width), _rand.Next(minOffset, Height), _rand.Next(3, 5), stoneId);

			for (int i = 0; i < Width * Height / 1200; i++)
				CreateOreCluster(_rand.Next(0, Width), _rand.Next(minOffset, Height), _rand.Next(3, 5), copperId);

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

		/// <summary>
		/// Recursively creates an ore cluster at a given starting position and size
		/// </summary>
		void CreateOreCluster(int x, int y, int size, int blockId)
		{
			if (GetBlock(x, y) == -1 || size <= 0)
				return;

			if (_rand.Next(0, 10) == 0)
				size += 1;

			SetBlock(x, y, blockId);

			CreateOreCluster(x + 1, y, size - 1, blockId);
			CreateOreCluster(x - 1, y, size - 1, blockId);
			CreateOreCluster(x, y + 1, size - 1, blockId);
			CreateOreCluster(x, y - 1, size - 1, blockId);
		}

		/// <summary>
		/// Creates a tree at a position with a given height
		/// </summary>
		void CreateTree(int x, int y, int height)
		{
			// Check if has grass block below
			if (GetBlock(x, y + 1) != GameData.GetBlockIdFromStrId("block.grass"))
				return;

			int top = y - height - 1;
			int trunkId = GameData.GetWallIdFromStrId("wall.trunk");
			int leavesId = GameData.GetWallIdFromStrId("wall.leaves");

			// Create trunk
			for (int i = y; i >= top; i--)
			{
				SetWall(x, i, trunkId);

				// Place random side leaves
				if (_rand.Next(0, 5) == 0 && Util.IsBetween(i, top + 2, y - 1))
				{
					if (_rand.Next(0, 2) == 0)
						SetWall(x - 1, i, leavesId);
					else
						SetWall(x + 1, i, leavesId);
				}
			}

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

		/// <summary>
		/// Updates the _tileMasks array at a particular position based on the calculated bitmask.
		/// Optionally updates the tiles around it.
		/// </summary>
		void UpdateTileMasks(int x, int y, bool updateAdjacent = true)
		{
			if (x < 0 || x >= Width || y < 0 || y >= Height)
				return;

			int blockId = GetBlock(x, y);
			int wallId = GetWall(x, y);

			// Blocks get priority over walls for tilemasks
			if (blockId != -1)
				_tileMasks[x, y] = GetMaskIndex(GetBlockBitmask(x, y, blockId));
			else if (wallId != -1)
				_tileMasks[x, y] = GetMaskIndex(GetWallBitmask(x, y, wallId));
			else
				_tileMasks[x, y] = -1;

			// Update adjacent tiles
			if (updateAdjacent)
			{
				UpdateTileMasks(x - 1, y, false);
				UpdateTileMasks(x + 1, y, false);
				UpdateTileMasks(x, y - 1, false);
				UpdateTileMasks(x, y + 1, false);
			}
		}

		/// <summary>
		/// Calculates a bitmask based on the block id, its position and the mergeable block adjacent to it
		/// </summary>
		int GetBlockBitmask(int x, int y, int blockId)
		{
			// Get adjacent blocks (NWES)
			int[] adjBlocks = new int[] { 0, 0, 0, 0 };

			if (CanBlockMerge(GetBlock(x, y - 1), blockId))
				adjBlocks[0] = 1;
			if (CanBlockMerge(GetBlock(x - 1, y), blockId))
				adjBlocks[1] = 1;
			if (CanBlockMerge(GetBlock(x + 1, y), blockId))
				adjBlocks[2] = 1;
			if (CanBlockMerge(GetBlock(x, y + 1), blockId))
				adjBlocks[3] = 1;

			// Calculate mask
			return adjBlocks[0] + adjBlocks[1] * 2 + adjBlocks[2] * 4 + adjBlocks[3] * 8;
		}

		/// <summary>
		/// Checks whether two block ids can merge
		/// </summary>
		bool CanBlockMerge(int blockId1, int blockId2)
		{
			if (blockId1 == -1 || blockId2 == -1)
				return false;

			TileData block1 = GameData.GetBlockData(blockId1);
			TileData block2 = GameData.GetBlockData(blockId2);

			return Array.Exists(block1.MergeTileStrIds, s => s == block2.StrId) ||
				Array.Exists(block2.MergeTileStrIds, s => s == block1.StrId);
		}

		/// <summary>
		/// Calculates a bitmask based on the wall id, its position and the mergeable walls adjacent to it
		/// </summary>
		int GetWallBitmask(int x, int y, int wallId)
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

		/// <summary>
		/// Checks whether two wall ids can merge
		/// </summary>
		bool CanWallMerge(int wallId1, int wallId2)
		{
			if (wallId1 == -1 || wallId2 == -1)
				return false;

			TileData wall1 = GameData.GetWallData(wallId1);
			TileData wall2 = GameData.GetWallData(wallId2);

			return Array.Exists(wall1.MergeTileStrIds, s => s == wall2.StrId) ||
				Array.Exists(wall2.MergeTileStrIds, s => s == wall1.StrId);
		}

		/// <summary>
		/// Calculates a random index in the _tileMaskRects array based on a given bitmask
		/// </summary>
		int GetMaskIndex(int bitmask)
		{
			return (bitmask * 3) + _rand.Next(0, 3);
		}

		/// <summary>
		/// Draws visible blocks and walls that are within the camera bounds.
		/// </summary>
		public void DrawTiles(SpriteBatch spriteBatch)
		{
			Rectangle bounds = Main.Camera.Bounds;

			int left = Math.Max(0, bounds.Left / TileData.TILE_SIZE);
			int right = Math.Min(Width - 1, bounds.Right / TileData.TILE_SIZE) + 1;
			int top = Math.Max(0, bounds.Top / TileData.TILE_SIZE);
			int bottom = Math.Min(Height - 1, bounds.Bottom / TileData.TILE_SIZE) + 1;

			for (int x = left; x < right; x++)
			{
				for (int y = top; y < bottom; y++)
				{
					int blockId = _blocks[x, y];
					int wallId = _walls[x, y];

					if (blockId != -1)
					{
						spriteBatch.Draw(GameData.GetBlockData(blockId).Texture,
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

		/// <summary>
		/// Draws tilemasks that within the camera bounds
		/// </summary>
		public void DrawTileMasks(SpriteBatch spriteBatch)
		{
			Rectangle bounds = Main.Camera.Bounds;

			int left = Math.Max(0, bounds.Left / TileData.TILE_SIZE);
			int right = Math.Min(Width - 1, bounds.Right / TileData.TILE_SIZE) + 1;
			int top = Math.Max(0, bounds.Top / TileData.TILE_SIZE);
			int bottom = Math.Min(Height - 1, bounds.Bottom / TileData.TILE_SIZE) + 1;

			for (int x = left; x < right; x++)
			{
				for (int y = top; y < bottom; y++)
				{
					int maskIndex = _tileMasks[x, y];

					if (maskIndex != -1)
					{
						spriteBatch.Draw(GameData.TileMasks,
							new Vector2(x * TileData.TILE_SIZE, y * TileData.TILE_SIZE),
							_tileMaskRects[maskIndex], Color.White);
					}
				}
			}
		}

		/// <summary>
		/// Gets the block id at a particular position.
		/// Returns -1 if the position is out bounds.
		/// </summary>
		public int GetBlock(int x, int y)
		{
			if (x < 0 || x >= Width || y < 0 || y >= Height)
				return -1;

			return _blocks[x, y];
		}

		/// <summary>
		/// Gets the wall id at a particular position.
		/// Returns -1 if the position is out bounds.
		/// </summary>
		public int GetWall(int x, int y)
		{
			if (x < 0 || x >= Width || y < 0 || y >= Height)
				return -1;

			return _walls[x, y];
		}

		/// <summary>
		/// Sets the block id at a particular position and updates tile masks.
		/// Returns if the position is out bounds.
		/// </summary>
		public void SetBlock(int x, int y, int id)
		{
			if (x < 0 || x >= Width || y < 0 || y >= Height)
				return;

			_blocks[x, y] = id;

			UpdateTileMasks(x, y);
		}

		/// <summary>
		/// Sets the wall id at a particular position and updates tile masks.
		/// Returns if the position is out bounds.
		/// </summary>
		public void SetWall(int x, int y, int id)
		{
			if (x < 0 || x >= Width || y < 0 || y >= Height)
				return;

			_walls[x, y] = id;

			UpdateTileMasks(x, y);
		}

		/// <summary>
		/// Get the bounding rectangle of a tile
		/// </summary>
		public Rectangle GetTileBounds(int x, int y)
		{
			return new Rectangle(x * TileData.TILE_SIZE, y * TileData.TILE_SIZE, TileData.TILE_SIZE, TileData.TILE_SIZE);
		}
	}
}
