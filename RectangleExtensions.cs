#region File Description
//-----------------------------------------------------------------------------
// RectangleExtensions.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;

namespace Castelian {
	/// <summary>
	/// A set of helpful methods for working with rectangles.
	/// </summary>
	///


	public static class RectangleExtensions {

		public static Vector2 ToVector2(this Point point) {
			return new Vector2(point.X, point.Y);
		}


		/// <summary>
		/// adapted from https://www.youtube.com/watch?v=8JJ-4JgR7Dg&t=1643s
		/// </summary>
		/// <param name="rayOrigin"></param>
		/// <param name="rayDirection"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public static bool RayVsRect(Vector2 rayOrigin, Vector2 rayDirection, Rectangle target, ref Vector2 contactPoint, ref Vector2 contactNormal, ref float tHitNear) {
			contactPoint = new();
			contactNormal = new();

			Vector2 tNear = (target.Location.ToVector2() - rayOrigin) / rayDirection;
			Vector2 tFar = (target.Location.ToVector2() + target.Size.ToVector2() - rayOrigin) / rayDirection;



			if (tNear.X > tFar.X)
				(tNear.X, tFar.X) = (tFar.X, tNear.X);
			if (tNear.Y > tFar.Y)
				(tNear.Y, tFar.Y) = (tFar.Y, tNear.Y);


			if (tNear.X > tFar.Y || tNear.Y > tFar.X)
				return false;

			tHitNear = Math.Max(tNear.X, tNear.Y);
			float tHitFar = Math.Min(tFar.X, tFar.Y);

			if (tHitFar < 0)
				return false;

			contactPoint = rayOrigin + tHitNear * rayDirection;

			if (tNear.X > tNear.Y)
				if (rayDirection.X < 0)
					contactNormal = new Vector2(1, 0);
				else
					contactNormal = new Vector2(-1, 0);
			else if (tNear.X < tNear.Y)
				if (rayDirection.Y < 0)
					contactNormal = new Vector2(0, 1);
				else
					contactNormal = new Vector2(0, -1);

			return true;
		}

		public static bool DynamicRectVsRect(Rectangle source, Rectangle target, Vector2 velocity, ref Vector2 contactPoint, ref Vector2 contactNormal, ref float contactTime, float elapsedTime) {
			if (velocity == Vector2.Zero)
				return false;

			Rectangle expandedTarget = new();
			int expX = target.Location.X - source.Size.X / 2;
			int expY = target.Location.Y - source.Size.Y / 2;
			expandedTarget.Location = new(expX, expY);
			expandedTarget.Size = target.Size + source.Size;

			if (RayVsRect(source.GetCenter() * elapsedTime, velocity, expandedTarget, ref contactPoint, ref contactNormal, ref contactTime)) {
				if (contactTime <= 1.0f)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Calculates the signed depth of intersection between two rectangles.
		/// </summary>
		/// <returns>
		/// The amount of overlap between two intersecting rectangles. These
		/// depth values can be negative depending on which wides the rectangles
		/// intersect. This allows callers to determine the correct direction
		/// to push objects in order to resolve collisions.
		/// If the rectangles are not intersecting, Vector2.Zero is returned.
		/// </returns>
		public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB) {
			// Calculate half sizes.
			float halfWidthA = rectA.Width / 2.0f;
			float halfHeightA = rectA.Height / 2.0f;
			float halfWidthB = rectB.Width / 2.0f;
			float halfHeightB = rectB.Height / 2.0f;

			// Calculate centers.
			Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
			Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

			// Calculate current and minimum-non-intersecting distances between centers.
			float distanceX = centerA.X - centerB.X;
			float distanceY = centerA.Y - centerB.Y;
			float minDistanceX = halfWidthA + halfWidthB;
			float minDistanceY = halfHeightA + halfHeightB;

			// If we are not intersecting at all, return (0, 0).
			if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
				return Vector2.Zero;

			// Calculate and return intersection depths.
			float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
			float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
			return new Vector2(depthX, depthY);
		}

		/// <summary>
		/// Gets the position of the center of the bottom edge of the rectangle.
		/// </summary>
		public static Vector2 GetBottomCenter(this Rectangle rect) {
			return new Vector2(rect.X + rect.Width / 2.0f, rect.Bottom);
		}
		public static Vector2 GetCenter(this Rectangle rect) {
			return new Vector2(rect.X + rect.Width / 2.0f, rect.Y + rect.Height / 2.0f);
		}
	}
}