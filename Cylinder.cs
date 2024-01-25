using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Math;

namespace Castelian {
	public class Cylinder {

		int Radius;
		int sections;
		int[] coords;
		double angle;
		double speed;
		const double RAD = Math.PI / 180.0;
		int crop;

		Texture2D wallTile;
		Texture2D background;


		public Cylinder(int _width, int _sections, Texture2D _texture, Texture2D _background) {
			wallTile = _texture;
			background = _background;
			Radius = _width / 2;
			sections = _sections;
			coords = new int[sections];
			angle = 360.0 / sections;
			speed = 30.0;

			double halfSide = Radius * (Sin((angle / 2.0) * RAD) / Sin(90.0 * RAD));
			int minRadius = (int)Sqrt(Radius * Radius - halfSide * halfSide);
			Console.WriteLine("minRadius: " + minRadius + ", halfside: " + halfSide);
		}

		public double ViewAngle {
			get; private set;
		}


		public void Update(GameTime gameTime, Point cursor) {
			double t = gameTime.TotalGameTime.TotalSeconds * speed;
			double offset = cursor.X / 5.0;

			ViewAngle = t % 360;
			//ViewAngle = offset;

			for (int i = 0; i < sections; i++) {
				coords[i] = (int)(Sin((i * angle + ViewAngle) * RAD) * Radius);
				//coords[i] = (int)(Sin((i * angle + ViewAngle) * RAD) * Radius);
			}
		}

		Rectangle window = new(0, 0, 1024, 768);

		public void Draw(SpriteBatch spriteBatch) {
			int bgPos = (int)((background.Width / 360.0) * -ViewAngle);
			Rectangle bgView = new Rectangle(bgPos, 0, window.Width, window.Height);
			spriteBatch.Draw(background, window, bgView, Color.White);

			for(int i = 0; i < coords.Length; i++) {
				int x1 = 512 + coords[i];
				int x2 = 512 + coords[(i + 1) % coords.Length];

				Point pos = new(x1, 0);
				Point size = new(x2 - x1, 768);

				Rectangle source = new Rectangle(-16, -16, wallTile.Width, 768);
				Rectangle destination = new Rectangle(pos, size);

				//Color color = i % 2 == 0 ? Color.White : Color.DarkBlue;

				spriteBatch.Draw(wallTile, destination, source, Color.White);

				int y = i * 32 - 32;
				while(y < 768) {
					Rectangle stripe = new(x1, y, x2 - x1, 64);
					y += 32 * sections;
					Primitives2D.FillRectangle(spriteBatch, stripe, Color.DarkBlue);
				}
				

				
			}
		}

	}
}
