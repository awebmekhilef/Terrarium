using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Terrarium
{
	public static class DebugDraw
	{
		static Dictionary<Rectangle, Color> _rects = new Dictionary<Rectangle, Color>();

		public static void AddRect(Rectangle rect, Color color)
		{
			_rects.TryAdd(rect, color);
		}

		public static void Draw(SpriteBatch spriteBatch)
		{
			foreach (var rect in _rects)
				spriteBatch.DrawRectangle(rect.Key, rect.Value);

			_rects.Clear();
		}
	}
}
