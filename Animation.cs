#region File Description
//-----------------------------------------------------------------------------
// Animation.cs
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
	/// Represents an animated texture.
	/// </summary>
	/// <remarks>
	/// Currently, this class assumes that each frame of animation is
	/// as wide as each animation is tall. The number of frames in the
	/// animation are inferred from this.
	/// </remarks>
	///

	class Animation {
		/// <summary>
		/// All frames in the animation arranged horizontally.
		/// </summary>
		public Texture2D Texture {
			get { return texture; }
		}
		Texture2D texture;

		/// <summary>
		/// Duration of time to show each frame.
		/// </summary>
		public float FrameTime {
			get { return frameTime; }
		}
		float frameTime;

		public bool Reverse {
			get { return reverse; }
		}
		bool reverse;

		/// <summary>
		/// When the end of the animation is reached, should it
		/// continue playing from the beginning?
		/// </summary>
		public bool IsLooping {
			get { return isLooping; }
		}
		bool isLooping;

		public int Rows {
			get { return rows; }
		}
		int rows;

		public int RowNo {
			get { return rowNo; }
		}
		int rowNo;

		/// <summary>
		/// Gets the number of frames in the animation.
		/// </summary>
		public int FrameCount {
			// Assume square frames.
			get { return Texture.Width / FrameWidth; }
		}

		/// <summary>
		/// Gets the width of a frame in the animation.
		/// </summary>
		public int FrameWidth {
			// Assume square frames.
			get { return FrameHeight; }
		}

		/// <summary>
		/// Gets the height of a frame in the animation.
		/// </summary>
		public int FrameHeight {
			get { return Texture.Height / Rows; }
		}

		public float Rotation {
			get; set;
		}

		public Vector2 Origin {
			get; set;
		} = new Vector2(64 / 2, 64);

		public Vector2 Scale { get; set; } = Vector2.One;

		/// <summary>
		/// Constructors a new animation.
		/// </summary>        
		public Animation(Texture2D texture, float frameTime, bool isLooping, float rotation = 0.0f, bool reverse = false, int rows = 1, int rowNo = 0) {
			this.texture = texture;
			this.frameTime = frameTime;
			this.isLooping = isLooping;
			this.reverse = reverse;
			this.rows = rows;
			this.rowNo = rowNo;
			Rotation = rotation;
			//this.origin = origin;
		}
	}
}