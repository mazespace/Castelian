using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Math;

namespace Castelian {
	public class Tower {

		int Radius;
		int sections;
		int[] coords;
		int[] coordsOuter;
		double angle;
		double speed;
		const double RAD = Math.PI / 180.0;
		int minRadius;

		Texture2D wallTile;
		Texture2D background;
		Texture2D stairsTexture;


		int[] tilemap;


		public Tower(int _width, int _sections, Texture2D _texture, Texture2D _background, Texture2D _texture2) {
			wallTile = _texture;
			stairsTexture = _texture2;
			background = _background;
			Radius = _width / 2;
			sections = _sections;
			coords = new int[sections];
			coordsOuter = new int[sections];
			angle = 360.0 / sections;
			speed = 15.0;

			double halfSide = Radius * (Sin((angle / 2.0) * RAD) / Sin(90.0 * RAD));
			minRadius = (int)Sqrt(Radius * Radius - halfSide * halfSide);

			tilemap = new int[sections * 32];
			//for (int y = 0; y < 32; y++)
			//	for(int x = 0; x < sections; x++) {
			//		tilemap[sections * y + x] = 1;
			//	}
			for(int i = 0; i < sections * 32; i++) {
				if (i % 7 == 0) tilemap[i] = 1;
			}
		}

		public double ViewAngle {
			get; private set;
		}




		public static int wrapIndex(int i, int i_max) {
			return ((i % i_max) + i_max) % i_max;
		}

		Rectangle window = new(0, 0, Game1.WindowWidth, Game1.WindowHeight);
		Point centerTop = new(Game1.WindowWidth / 2, 0);

		public void DrawBackground(SpriteBatch spriteBatch) {
			int X = (int)((background.Width / 360.0) * ViewAngle);

			decimal zoom = 2M; // more zoomed in, faster scrolling
			int width = (int)(window.Width / zoom);
			int height = (int)(window.Height / zoom);

			int Y = (background.Height / 2) - (height / 2);

			Rectangle bgView = new Rectangle(X, Y, width, height);
			spriteBatch.Draw(background, window, bgView, Color.White);
		}

		public void Update(GameTime gameTime, Point cursor) {
			double t = gameTime.TotalGameTime.TotalSeconds * -speed;
			double offset = cursor.X / 2.0;

			//ViewAngle = t % 360;
			//ViewAngle = offset % 360;
			ViewAngle = (t - (offset % 360)) % 360;


			for (int i = 0; i < sections; i++) {
				double coord = Sin((i * angle + ViewAngle) * RAD) * Radius;
				//coord = Clamp(coord, -minRadius, minRadius);
				coords[i] = (int)coord;
				coordsOuter[i] = (int)(coord * 1.15f);
			}
		}

		public void Draw(SpriteBatch spriteBatch) {
			for(int i = 0; i < coords.Length; i++) {
				int x1 = coords[i];
				int x2 = coords[(i + 1) % coords.Length];
				float outerRadiusDepth = 1.0f;
				bool behindTower = false;

				//only draw front
				Rectangle source = new Rectangle(0, 0, wallTile.Width, Game1.WindowHeight);
				if (x2 > x1) {	//negative width means we are viewing back of face
					Point pos = new(x1, 0);
					Point size = new(x2 - x1, Game1.WindowHeight);
					Rectangle destination = new Rectangle(pos + centerTop, size);
					spriteBatch.Draw(wallTile, destination, source, Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth: 0.5f);
				}
				//begin outer radius
				else {
					//(x1, x2) = (x2, x1);        //negative width is invalid, so swap the left and right edges
					outerRadiusDepth = 0f;            //and send to back layer
					behindTower = true;
				}
				
				int stairX1 = coordsOuter[i];
				int stairX2 = coordsOuter[(i + 1) % coords.Length];
					
				Point stairSize = new(stairX2 - stairX1, 64);

				int sideX1 = x1;
				int sideX2 = stairX1;
				float sideDepth = 0.75f;
				if (behindTower) {
					sideDepth = 0.25f;
				}

				Color sideColor = Color.DarkBlue;

				if (sideX1 > sideX2) {
					sideX1 = stairX2;
					sideX2 = x2;
				}

				//Rectangle stairSource = new Rectangle(0, 0, wallTile.Width, Game1.WindowHeight);
				//Rectangle sideSource = new Rectangle(0, 0, wallTile.Width, Game1.WindowHeight);

				int y2 = i * 64 - (64 * sections) + 64;
				while (i % 2 != 0 && y2 < window.Height) {
					Point stairPos = new(stairX1, y2);
					Rectangle stairSource = new Rectangle(i * 64, y2 * 13, stairsTexture.Width / 2, stairsTexture.Height / 4); //position offset pseudo random
					Rectangle stairDest = new(stairPos + centerTop, stairSize);
					Rectangle sideDest = new(new Point(sideX1, y2) + centerTop, new(sideX2 - sideX1, 64));
					spriteBatch.Draw(stairsTexture, stairDest, stairSource, Color.WhiteSmoke, 0f, Vector2.Zero, SpriteEffects.None, outerRadiusDepth);
					spriteBatch.Draw(stairsTexture, sideDest, stairSource, sideColor, 0f, Vector2.Zero, SpriteEffects.None, sideDepth);
					y2 += 32 * sections;
				}
			}
		}

		//end class
	}
}
