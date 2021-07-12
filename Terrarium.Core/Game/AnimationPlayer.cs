using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Terrarium
{
	/// <summary>
	/// Responsible for calculating the frame of an animation
	/// Drawing is left to the user of this class
	/// </summary>
	public class AnimationPlayer
	{
		Dictionary<string, Animation> _animations;

		Animation _currAnimation;
		AnimationState _state;

		int _currFrame;
		float _timeElapsed;
		float _timePerFrame;

		/// <summary>
		/// Creates a new animation player.
		/// </summary>
		public AnimationPlayer()
		{
			_animations = new Dictionary<string, Animation>();
		}

		/// <summary>
		/// Updates the current playing animation, if any.
		/// </summary>
		public void Update(float dt)
		{
			if (_currAnimation == null || _state == AnimationState.Completed)
				return;

			// Move to next frame
			if (_timeElapsed >= _timePerFrame)
			{
				_currFrame++;

				// End of animation
				if(_currFrame == _currAnimation.Frames.Length)
				{
					if (_currAnimation.Loop)
						_currFrame = 0;
					else
					{
						_state = AnimationState.Completed;
						_currFrame = _currAnimation.Frames.Length - 1;
						return;
					}
				}

				_timeElapsed = 0f;
			}

			_timeElapsed += dt;
		}

		/// <summary>
		/// Adds a new animation with a given name.
		/// </summary>
		public void Add(string name, Animation animation)
		{
			_animations.Add(name, animation);
		}

		/// <summary>
		/// Play the requested animation.
		/// </summary>
		public void Play(string name)
		{
			_currAnimation = _animations[name];
			_currFrame = 0;

			_timeElapsed = 0f;
			_timePerFrame = 1f / _currAnimation.Fps;

			_state = AnimationState.Running;
		}

		/// <summary>
		/// Whether or not the current animation is playing.
		/// </summary>
		public bool IsPlaying(string name)
		{
			return _currAnimation == _animations[name];
		}

		/// <summary>
		/// Get the calculated rectangle of the current frame.
		/// </summary>
		public Rectangle GetFrameBounds()
		{
			return _currAnimation.Frames[_currFrame];
		}

		enum AnimationState
		{
			Running,Completed
		}
	}
}
