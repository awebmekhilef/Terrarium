using Microsoft.Xna.Framework;
using System;

namespace Terrarium
{
	public static class Util
	{
		public static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
		{
			return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
		}

		public static Vector2 GetIntersectionDepth(Rectangle rectA, Rectangle rectB)
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
	}
}
