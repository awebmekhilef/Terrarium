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
		public static Effect TileMaskEffect { get; private set; }

		static Dictionary<int, TileData> _tileData;
		static Dictionary<string, int> _tilesStrId;

		static Dictionary<int, TileData> _wallData;
		static Dictionary<string, int> _wallsStrId;

		public static void Load(ContentManager content)
		{
			_tileData = new Dictionary<int, TileData>();
			_tilesStrId = new Dictionary<string, int>();

			_wallData = new Dictionary<int, TileData>();
			_wallsStrId = new Dictionary<string, int>();

			LoadEffects	(content);
			LoadTextures(content);
			LoadTileData(content);
			LoadWallData(content);
		}

		static void LoadEffects(ContentManager content)
		{
			TileMaskEffect = content.Load<Effect>("Effects/TileMaskEffect");
		}

		static void LoadTextures(ContentManager content)
		{
			TileMasks = content.Load<Texture2D>("Textures/TileMasks");
		}

		static void LoadTileData(ContentManager content)
		{
			string json = File.ReadAllText("Content/GameData/Tiles.json");
			var tileDictArray = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(json);

			foreach (var tileDict in tileDictArray)
			{
				int id = int.Parse(tileDict["id"]);

				string name = tileDict["name"];
				string strId = tileDict["strId"];
				string texturePath = tileDict["texturePath"];
				string mergeStrIds = tileDict["mergeTiles"];

				_tileData.Add(
					id,
					new TileData(
						name,
						strId,
						content.Load<Texture2D>(texturePath),
						mergeStrIds.Split(",")
					)
				);

				_tilesStrId.Add(
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

		public static TileData GetTileData(int id)
		{
			return _tileData[id];
		}

		public static int GetTileIdFromStrId(string strId)
		{
			return _tilesStrId[strId];
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
