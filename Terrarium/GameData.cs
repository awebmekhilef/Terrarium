using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Terrarium
{
	public static class GameData
	{
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

			LoadTileData(content);
			LoadWallData(content);
		}

		static void LoadTileData(ContentManager content)
		{
			string json = File.ReadAllText("Content/GameData/Tiles.json");
			var tileDictArray = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(json);

			foreach (var tileDict in tileDictArray)
			{
				int id = int.Parse(tileDict["id"]);
				string texturePath = tileDict["texturePath"];

				_tileData.Add(
					id,
					new TileData(
						tileDict["name"],
						content.Load<Texture2D>(texturePath)
					)
				);

				_tilesStrId.Add(
					tileDict["strId"],
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
				string texturePath = wallDict["texturePath"];

				_wallData.Add(
					id,
					new TileData(
						wallDict["name"],
						content.Load<Texture2D>(texturePath)
					)
				);

				_wallsStrId.Add(
					wallDict["strId"],
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
