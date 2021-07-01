using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Terrarium
{
	// Responsible for return a frame of an animatino
	// Drawing is left for the class user

	public class AnimationPlayer
	{
		Dictionary<string, Animation> _animations;

		Animation _currAnimation;
		AnimationState _state;

		int _currFrame;
		float _timeElapsed;
		float _timePerFrame;

		public AnimationPlayer()
		{
			_animations = new Dictionary<string, Animation>();
		}

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
						_state = AnimationState.Completed;
				}

				_timeElapsed = 0f;
			}

			_timeElapsed += dt;
		}

		public void Add(string name, Animation animation)
		{
			_animations.Add(name, animation);
		}

		public void Play(string name)
		{
			_currAnimation = _animations[name];
			_currFrame = 0;

			_timeElapsed = 0f;
			_timePerFrame = 1f / _currAnimation.Fps;

			_state = AnimationState.Running;
		}

		public bool IsPlaying(string name)
		{
			return _currAnimation == _animations[name];
		}

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
