using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Terrarium
{
	public static class Util
	{
		public static float Map(float v, float iMin, float iMax, float oMin, float oMax)
		{
			return (v - iMin) / (iMax - iMin) * (oMax - oMin) + oMin;
		}

		public static float InverseLerp(float a, float b, float v)
		{
			return (v - a) / (b - a);
		}

		public static bool IsBetween(int v, int a, int b)
		{
			return v >= a && v < b;
		}

		public static Vector2 GetIntersectionDepth(RectangleF rectA, RectangleF rectB)
		{
			Vector2 halfSizeA = new Vector2(rectA.Width / 2f, rectA.Height / 2f);
			Vector2 halfSizeB = new Vector2(rectB.Width / 2f, rectB.Height / 2f);

			Vector2 centerA = new Vector2(rectA.Left + halfSizeA.X, rectA.Top + halfSizeA.Y);
			Vector2 centerB = new Vector2(rectB.Left + halfSizeB.X, rectB.Top + halfSizeB.Y);

			Vector2 dist = centerA - centerB;
			Vector2 minDist = halfSizeA + halfSizeB;

			if (Math.Abs(dist.X) >= minDist.X || Math.Abs(dist.Y) >= minDist.Y)
				return Vector2.Zero;

			return new Vector2(
				dist.X > 0 ? minDist.X - dist.X : -minDist.X - dist.X,
				dist.Y > 0 ? minDist.Y - dist.Y : -minDist.Y - dist.Y
			);
		}

		public static Rectangle[] RectsFromTexture(Texture2D texture, int sizeX, int sizeY)
		{
			int cols = texture.Width / sizeX;
			int rows = texture.Height / sizeY;

			Rectangle[] rects = new Rectangle[rows * cols];

			for (int y = 0; y < rows; y++)
			{
				for (int x = 0; x < cols; x++)
				{
					rects[x + y * cols] = new Rectangle(x * sizeX, y * sizeY, sizeX, sizeY);
				}
			}

			return rects;
		}
	}
}
