using Microsoft.Xna.Framework;

namespace Terrarium
{
	public struct RectangleF
	{
		public float X;
		public float Y;
		public float Width;
		public float Height;

		public float Left => X;
		public float Right => (X + Width);
		public float Top => Y;
		public float Bottom => (Y + Height);

		public Vector2 Location
		{
			get => new Vector2(X, Y);
			set
			{
				X = value.X;
				Y = value.Y;
			}
		}

		public Vector2 Size
		{
			get => new Vector2(Width, Height);
			set
			{
				Width = value.X;
				Height = value.Y;
			}
		}

		public Vector2 Center => new Vector2(X + (Width / 2), Y + (Height / 2));

		public RectangleF(float x, float y, float width, float height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public RectangleF(Vector2 location, Vector2 size)
		{
			X = location.X;
			Y = location.Y;
			Width = size.X;
			Height = size.Y;
		}

		public bool Intersects(RectangleF value)
		{
			return value.Left < Right &&
				   Left < value.Right &&
				   value.Top < Bottom &&
				   Top < value.Bottom;
		}

		public override string ToString()
		{
			return string.Format("X:{0}, Y:{1}, Width: {2}, Height: {3}", X, Y, Width, Height);
		}

		public static bool operator ==(RectangleF a, RectangleF b)
		{
			return ((a.X == b.X) && (a.Y == b.Y) && (a.Width == b.Width) && (a.Height == b.Height));
		}

		public static bool operator !=(RectangleF a, RectangleF b)
		{
			return !(a == b);
		}

		public static explicit operator RectangleF(Rectangle r)
		{
			return new RectangleF(r.X, r.Y, r.Width, r.Height);
		}

		public override bool Equals(object obj)
		{
			return (obj is RectangleF) && this == ((RectangleF)obj);
		}

		public override int GetHashCode()
		{
			return ((int)X ^ (int)Y ^ (int)Width ^ (int)Height);
		}
	}
}