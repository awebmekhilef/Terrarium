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

		public static void Load(ContentManager content)
		{
			_tileData = new Dictionary<int, TileData>();
			_tilesStrId = new Dictionary<string, int>();

			using (StreamReader sr = new StreamReader("Content/GameData/Tiles.json"))
			{
				string json = sr.ReadToEnd();
				var tileDictArray = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(json);

				foreach (var tileDict in tileDictArray)
				{
					int id = int.Parse(tileDict["id"]);
					string texturePath = tileDict["texturePath"];

					Texture2D texture = null;

					if (!string.IsNullOrEmpty(texturePath))
						texture = content.Load<Texture2D>(texturePath);

					_tileData.Add(
						id,
						new TileData(
							tileDict["name"],
							texture
						)
					);

					_tilesStrId.Add(
						tileDict["strId"],
						id
					);
				}
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
	}
}
