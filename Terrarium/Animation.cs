using Microsoft.Xna.Framework;

namespace Terrarium
{
	public class Animation
	{
		public readonly bool Loop;
		public readonly float Fps;
		public readonly Rectangle[] Frames;

		public Animation(bool loop, float fps, params Rectangle[] frames)
		{
			Loop = loop;
			Fps = fps;
			Frames = frames;
		}
	}
}
