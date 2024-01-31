#region File Description
//-----------------------------------------------------------------------------
// AnimationPlayer.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Castelian {
	/// <summary>
	/// Controls playback of an Animation.
	/// </summary>
	struct AnimationPlayer {

		/// <summary>
		/// Gets the animation which is currently playing.
		/// </summary>
		public Animation Animation {
			get { return animation; }
		}
		Animation animation;

		/// <summary>
		/// Gets the index of the current frame in the animation.
		/// </summary>
		public int FrameIndex {
			get { return frameIndex; }
		}
		int frameIndex;

		/// <summary>
		/// The amount of time in seconds that the current frame has been shown for.
		/// </summary>
		private float time;

		/// <summary>
		/// Gets a texture origin at the bottom center of each frame.
		/// </summary>
		public Vector2 GetBottomCenter {
			get { return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight); }
		}

		/// <summary>
		/// Gets a texture origin at the bottom center of each frame.
		/// </summary>
		public Vector2 GetCenter {
			get { return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight / 2.0f); }
		}

		/// <summary>
		/// Gets a texture origin from animation object
		/// </summary>
		public Vector2 Origin {
			get { return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight); }
			//get { return Animation.Origin; }
		}

		Random rand;

		/// <summary>
		/// Begins or continues playback of an animation.
		/// </summary>
		public void PlayAnimation(Animation animation, bool randomStartFrame = false) {
			// If this animation is already running, do not restart it.
			if (Animation == animation)
				return;
			Paused = false;

			// Start the new animation.
			this.animation = animation;

			rand = new();

			if (randomStartFrame) {
				frameIndex = rand.Next(0,255) % (animation.FrameCount - 1);
			} else {
				if (animation.Reverse)
					frameIndex = animation.FrameCount - 1;
				else
					frameIndex = 0;
			}

			time = 0.0f;
		}
		/// <summary>
		/// Begins  playback of an animation from 0.
		/// </summary>
		public void RestartAnimation(Animation animation) {
			Paused = false;
			// Start the new animation.
			this.animation = animation;
			if (animation.Reverse)
				this.frameIndex = animation.FrameCount;
			else
				this.frameIndex = 0;
			//this.frameIndex = Animation.FrameCount - 1;
			this.time = 0.0f;
		}

		bool Paused;
		public void PauseAllAnimation() {
			if (!Paused) Paused = true;
			else Paused = false;
		}





		/// <summary>
		/// Advances the time position and draws the current frame of the animation.
		/// </summary>
		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects = SpriteEffects.None, Color? color = null, float layerDepth = 0.0f) {
			if (Animation == null)
				throw new NotSupportedException("No animation is currently playing.");

			// Process passing time.
			if (!Paused) time += (float)gameTime.ElapsedGameTime.TotalSeconds;
			while (time > Animation.FrameTime) {
				time -= Animation.FrameTime;

				// Advance the frame index; looping or clamping as appropriate.
				if (Animation.IsLooping) {
					if (Animation.Reverse) {
						var fc = Animation.FrameCount;
						frameIndex = ((frameIndex - 1 % fc) + fc) % fc;
					} else
						frameIndex = (frameIndex + 1) % Animation.FrameCount;
					//frameIndex = (frameIndex - 1) % Animation.FrameCount + 1;
				} else {
					if (!Animation.Reverse)
						frameIndex = Math.Min(frameIndex + 1, Animation.FrameCount - 1);
					else
						frameIndex = Math.Max(frameIndex - 1, 0);
					//frameIndex = Math.Max (frameIndex - 1, 0);
				}
			}

			// Calculate the source rectangle of the current frame.
			Rectangle source = new Rectangle(FrameIndex * Animation.FrameWidth, Animation.RowNo * Animation.FrameHeight, Animation.FrameWidth, Animation.FrameHeight);

			// Draw the current frame.
			spriteBatch.Draw(Animation.Texture, position, source, color ?? Color.White, Animation.Rotation, Animation.Origin, Animation.Scale, spriteEffects, layerDepth);
		}
	}
}