using Microsoft.Xna.Framework.Graphics;

namespace Terrarium
{
	public class TileData
	{
		public readonly string Name;
		public readonly Texture2D Texture;

		public TileData(string name, Texture2D texture)
		{
			Name = name;
			Texture = texture;
		}
	}
}
