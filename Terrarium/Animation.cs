using Microsoft.Xna.Framework;

namespace Terrarium
{
	/// <summary>
	/// Stores animation data
	/// </summary>
	public class Animation
	{
		public readonly bool Loop;
		public readonly float Fps;
		public readonly Rectangle[] Frames;

		/// <summary>
		/// Creates a new animation
		/// </summary>
		public Animation(bool loop, float fps, params Rectangle[] frames)
		{
			Loop = loop;
			Fps = fps;
			Frames = frames;
		}
	}
}
