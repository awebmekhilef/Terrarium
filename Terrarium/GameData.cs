using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Terrarium
{
	public static class GameData
	{
		public static Texture2D TileMasks	{ get; private set; }
		public static Texture2D Player		{ get; private set; }

		public static Effect TileMaskEffect { get; private set; }

		static Dictionary<int, TileData> _blockData;
		static Dictionary<string, int> _blocksStrId;

		static Dictionary<int, TileData> _wallData;
		static Dictionary<string, int> _wallsStrId;

		public static void Load(ContentManager content)
		{
			_blockData = new Dictionary<int, TileData>();
			_blocksStrId = new Dictionary<string, int>();

			_wallData = new Dictionary<int, TileData>();
			_wallsStrId = new Dictionary<string, int>();

			LoadEffects(content);
			LoadTextures(content);
			LoadBlockData(content);
			LoadWallData(content);
		}

		static void LoadEffects(ContentManager content)
		{
			TileMaskEffect = content.Load<Effect>("Effects/TileMaskEffect");
		}

		static void LoadTextures(ContentManager content)
		{
			TileMasks = content.Load<Texture2D>("Textures/TileMasks");
			Player = content.Load<Texture2D>("Textures/Player");
		}

		static void LoadBlockData(ContentManager content)
		{
			string json = File.ReadAllText("Content/GameData/Blocks.json");
			var blockDictArray = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(json);

			foreach (var blockDict in blockDictArray)
			{
				int id = int.Parse(blockDict["id"]);

				string name = blockDict["name"];
				string strId = blockDict["strId"];
				string texturePath = blockDict["texturePath"];
				string mergeStrIds = blockDict["mergeBlocks"];

				_blockData.Add(
					id,
					new TileData(
						name,
						strId,
						content.Load<Texture2D>(texturePath),
						mergeStrIds.Split(",")
					)
				);

				_blocksStrId.Add(
					strId,
					id
				);
			}
		}

		static void LoadWallData(ContentManager content)
		{
			string json = File.ReadAllText("Content/GameData/Walls.json");
			var wallDictArray = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(json);

			foreach (var wallDict in wallDictArray)
			{
				int id = int.Parse(wallDict["id"]);

				string name = wallDict["name"];
				string strId = wallDict["strId"];
				string texturePath = wallDict["texturePath"];
				string mergeStrIds = wallDict["mergeWalls"];

				_wallData.Add(
					id,
					new TileData(
						name,
						strId,
						content.Load<Texture2D>(texturePath),
						mergeStrIds.Split(",")
					)
				);

				_wallsStrId.Add(
					strId,
					id
				);
			}
		}

		public static TileData GetBlockData(int id)
		{
			return _blockData[id];
		}

		public static int GetBlockIdFromStrId(string strId)
		{
			return _blocksStrId[strId];
		}

		public static TileData GetWallData(int id)
		{
			return _wallData[id];
		}

		public static int GetWallIdFromStrId(string strId)
		{
			return _wallsStrId[strId];
		}
	}
}
