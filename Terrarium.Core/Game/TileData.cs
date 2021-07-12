using Microsoft.Xna.Framework.Graphics;

namespace Terrarium
{
	public class TileData
	{
		public const int TILE_SIZE = 8;

		public readonly string Name;
		public readonly string StrId;
		public readonly Texture2D Texture;
		public readonly string[] MergeTileStrIds; // TODO: Maybe a script that converts str ids to int ids

		public TileData(string name, string strId, Texture2D texture, string[] mergeTileStrIds)
		{
			Name = name;
			StrId = strId;
			Texture = texture;
			MergeTileStrIds = mergeTileStrIds;
		}
	}
}
