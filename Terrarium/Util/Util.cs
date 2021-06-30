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
