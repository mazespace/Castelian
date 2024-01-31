using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.MathF;

namespace Castelian {
	public class Tower {

		int Radius;
		int MaxRadius;
		int Levels;
		int Sections;
		const int Layers = 5;

		/// <summary>
		/// Layer, Section
		/// </summary>
		float[,] coords;
		float[,] coordsDepth;
		Tile[,,] tilemap;
		bool[] drawBackFaces;



		float standardAngle;
		public float speed { get; set; }
		const float RAD = PI / 180.0f;
		int[] minRadius;

		Texture2D wallTile;
		Texture2D background;
		Texture2D stairsTexture;
		Texture2D passageTexture;

		public Texture2D spriteTexture;
		Animation demonhead;
		AnimationPlayer sprite;

		


		public Tower(int _width, int _levels, int _sections, Texture2D _texture, Texture2D _background, Texture2D _texture2, Texture2D _spriteTexture, Texture2D _passage) {
			wallTile = _texture;
			stairsTexture = _texture2;
			background = _background;
			Radius = _width / 2;
			MaxRadius = Radius * 1; //arbitrary distance of viewplane, used in layerDepth calculation.
			Sections = _sections;
			Levels = _levels;
			//Layers = 5;
			coords = new float[Layers, Sections];
			coordsDepth = new float[Layers, Sections];
			drawBackFaces = new bool[Levels];
			standardAngle = 360.0f / Sections;
			speed = 15.0f;
			minRadius = new int[Layers];
			for (int i = 0; i < Layers; i++) {
				int radius = Radius + Tile.width * i;
				minRadius[i] = GetMinRadius(Sections, radius);
			}

			spriteTexture = _spriteTexture;
			passageTexture = _passage;
			demonhead = new Animation(spriteTexture, 2f, true);
			sprite = new AnimationPlayer();
			sprite.PlayAnimation(demonhead);

			tilemap = new Tile[Layers, Levels, Sections];

			DefineLevel();
		}


		public void DefineLevel() {
			for (int j = 0; j < Layers; j++)
				for (int i = 0; i < Sections; i++) {
					coordsDepth[j, i] = GetTrueDepth(j, i * standardAngle);
				}

			for (int j = 0; j < Levels; j++) {
				drawBackFaces[j] = true;
				for (int i = 0; i < Sections; i++) {
					//layer 0
					tilemap[1, j, i] = new(TileType.brick);

					//layer 1
					if (i % 8 == 0) {
						tilemap[4, j, (i + j) % Sections].Type = TileType.step;
						//tilemap[3, j, (i + j + 1) % Sections].Type = TileType.step;
						//tilemap[3, j, (i + j + 2) % Sections].Type = TileType.step;
						tilemap[3, j, (i + j + 1) % Sections].Type = TileType.step;
					}
					if (i % (Sections / 2) == 0 && j % 4 == 0) {
						tilemap[1, wrapIndex(j - 2, Levels), (i + j) % Sections].Type = TileType.passage;
					}
				}
			}
		}



		

		public enum TileType {
			none,
			brick,
			step,
			passage
		}


		public struct Tile {
			public const int width = 64;
			public const int height = 64;
			public TileType Type;
			public Tile(TileType _tileType = TileType.none) {
				Type = _tileType;
			}
		}

		//a point on a cylinder
		public struct tPoint {
			public float z;     //height of point from bottom of tower
			public float angle; //angle in degrees around tower
			public float radius;	//distance from center of tower
			public tPoint(float _z, float _angle, float _radius) {
				z = _z;
				angle = _angle;
				radius = _radius;
			}
		}

		//a coordinate on the tower
		public struct tCoord {
			public int level;		//tower level from bottom
			public int section;		//tower section (pie slice)
			public int layer;		//radius layer from closest to farthest from tower
		}

		public tPoint tPointFromCoord(tCoord coord) {
			float z = coord.level * Tile.height;
			float angle = (coord.section * standardAngle) % 360.0f;
			float radius = Radius + (coord.layer * Tile.width);
			return new tPoint(z, angle, radius);
		}

		/// <summary>
		/// Given a regular polygon whose defined radius is at one of its points,
		/// return the radius at the center of one of its faces.
		/// </summary>
		public int GetMinRadius(int sections, float radius) {
			//float angle = 360.0f / sections;
			float halfSide = radius * (Sin((standardAngle / 2.0f) * RAD) / Sin(90.0f * RAD));
			return (int)Sqrt(radius * radius - halfSide * halfSide);
		}

		public float ViewAngle {
			get; private set;
		}

		static int GetSectionNo (float angle, int sections) {
			return (int)((angle / 360.0f) * sections);
		}
		int ViewSection() {
			return GetSectionNo(ViewAngle, Sections);
		}



		public static int wrapIndex(int i, int i_max) {
			return ((i % i_max) + i_max) % i_max;
		}

		public struct Viewport {
			public Rectangle rect;
			public Point position => rect.Location;
			public Point size => rect.Size;
			public int width => rect.Width;
			public int height => rect.Height;
			public int hCenter => rect.X + (rect.Width / 2);
			public int vCenter => rect.Y + (rect.Height / 2);
			public Point centerTop => new Point(hCenter, rect.Y);
			public Point center => new Point(hCenter, vCenter);
			public Viewport(int _x, int _y, int _width, int _height) {
				rect = new(_x, _y, _width, _height);
			}
		}

		Viewport viewport => new(0, 0, Game1.WindowWidth, Game1.WindowHeight);



		public void DrawBackground(SpriteBatch spriteBatch) {
			float zoom = 2f; // more zoomed in, faster scrolling
			int width = (int)(viewport.width / zoom);
			int height = (int)(viewport.height / zoom);

			int X = (int)((background.Width / 360.0f) * ViewAngle);
			int Y = (background.Height / 2) - (height / 2);

			Rectangle bgView = new Rectangle(X, Y, width, height);
			spriteBatch.Draw(background, viewport.rect, bgView, Color.White);
		}


		public bool Paused { get; set; } = true;

		public int Move { get; set; } = 0;

		float t;
		float move;
		public void Update(GameTime gameTime, Point cursor) {
			float elapsed = (float)(gameTime.ElapsedGameTime.TotalSeconds);
			if(!Paused) t += elapsed * speed;
			move += elapsed * speed * Move;

			float offset = (float)(cursor.X / 2.0);


			ViewAngle = (t - move - (offset % 360.0f)) % 360.0f;

			for (int layer = 0; layer < Layers; layer++)
			for (int i = 0; i < Sections; i++) {
				float coord = Sin((i * standardAngle + ViewAngle) * RAD) * (Radius + Tile.width * layer);
				coord = MathHelper.Clamp(coord, -minRadius[layer], minRadius[layer]);
				coords[layer, i] = coord;
			}
		}


		struct spritePlayer {

		}


		static Rectangle GetAnimationFrame(int frame, Texture2D texture) {
			int frameCount = texture.Width / texture.Height;
			frame = frame % frameCount;
			int frameWidth = texture.Height;
			return new Rectangle(frameWidth * frame, 0, frameWidth, texture.Height);
		}


		static float GetTrueDepth(float radius, float angle, float viewDistance, float viewAngle) {
			float dp = -Cos((angle + viewAngle) * RAD) * radius;   //distance from center plane
			float d = viewDistance - dp;
			return (d / (viewDistance * 2));
		}

		float GetTrueDepth(int layer, float angle) {
			float radius = (layer * Tile.width) + Radius;
			return GetTrueDepth(radius, angle, MaxRadius, ViewAngle);
		}

		static float GetLayerDepth(int layers, float layer, bool receeding) {
			int b = receeding ? -1 : 1;
			return (float)(b * -layer / (layers * 2.0f)) + 0.5f;
		}


		static float GetFaceDepth(float layer, int x0, int x1) {
			float b = x0 > x1 ? -1.0f : 1.0f;
			return (float)(b * -layer / (Layers * 2.0f)) + 0.5f;
		}


		static void DrawTextureStrip(SpriteBatch spriteBatch, Texture2D texture, Viewport viewport, int x1, int x2, float layerDepth) {
			Rectangle source = new Rectangle(0, 0, texture.Width, viewport.height);
			if (x2 > x1) {  //only calculate front faces
				Point pos = new(x1, 0);
				Point size = new(x2 - x1, viewport.height);
				Rectangle destination = new Rectangle(pos + viewport.centerTop, size);
				spriteBatch.Draw(texture, destination, source, Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth: layerDepth);
			}
		}

		static void DrawBrick(SpriteBatch spriteBatch, int section, Texture2D texture, Viewport viewport, int y0, int x1, int x2, float layerDepth) {
			Color tint = Color.White;
			if (x1 >= x2) {
				(x1, x2) = (x2, x1);
				layerDepth += 0.005f;
				tint = new Color(96, 96, 96, 255);
			} else {
				layerDepth -= 0.005f;
			}
			Rectangle source = new Rectangle(section * texture.Width, y0, texture.Width, texture.Height / 2);
			Point pos = new(x1, y0);
			Point size = new(x2 - x1, Tile.height);
			Rectangle destination = new Rectangle(pos + viewport.centerTop, size);
			spriteBatch.Draw(texture, destination, source, tint, 0f, Vector2.Zero, SpriteEffects.None, layerDepth: layerDepth);

			//Rectangle source = new Rectangle(0, 0, wallTile.Width, viewport.height);
			//if (x2 > x1) {  //only calculate front faces
			//	Point pos = new(x1, 0);
			//	Point size = new(x2 - x1, viewport.height);
			//	Rectangle destination = new Rectangle(pos + viewport.centerTop, size);
			//	spriteBatch.Draw(wallTile, destination, source, Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth: 0.5f);
			//}
		}

		static void DrawPassage(SpriteBatch spriteBatch, Texture2D texture, int y, Viewport viewport, int x0, int x1) {
			int farPoint0 = -x1;
			int w0 = (int)((farPoint0 - x0) * 0.51f);

			Rectangle side0 = new Rectangle(x0 + viewport.hCenter, y, w0, Tile.height);
			Rectangle side1 = new Rectangle(-x1 - w0 + viewport.hCenter, y, w0, Tile.height);

			//Primitives2D.FillRectangle(spriteBatch, side0, Color.DarkBlue, 0.0f, 0.5f);
			//Primitives2D.FillRectangle(spriteBatch, side1, Color.DarkMagenta, 0.0f, 0.5f);

			spriteBatch.Draw(texture, side0, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth: 0.5f);
			spriteBatch.Draw(texture, side1, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, layerDepth: 0.5f);
		}


		static Color tintColor(Color color, float tint) {
			Vector3 vColor = color.ToVector3() * tint;
			return new Color(vColor);
		}




		static void DrawStep(	SpriteBatch spriteBatch,
								Texture2D texture,
								Viewport viewport,
								int y0,
								int x0, int x1, int x2, int x3,
								int layer,
								bool drawBackFace
		) {
			

			int sideX0 = x0;
			int sideX1 = x1;
			int sideX2 = x2;
			int sideX3 = x3;

			Color Side0Color;
			if (layer == 2) Side0Color = Color.DarkBlue;
			else Side0Color = Color.DarkRed;
			

			Color Side1Color;
			if (layer == 2) Side1Color = Color.DarkGreen;
			else Side1Color = Color.DarkOrange;

			

			Rectangle stairSource = new Rectangle(0, y0 * 13, texture.Width / 2, texture.Height / 4); //texture position offset pseudo random

			Point stairPos = new(x1, y0);
			Point stairSize = new(x2 - x1, Tile.height);
			

			Rectangle stairFrontDest = new(stairPos + viewport.centerTop, stairSize);
			Rectangle stairBackDest = new(x3 + viewport.hCenter, y0, x0 - x3, Tile.height);

			Rectangle side0Dest = new(new Point(sideX0, y0) + viewport.centerTop, new(sideX1 - sideX0, Tile.height));
			Rectangle side1Dest = new(new Point(sideX2, y0) + viewport.centerTop, new(sideX3 - sideX2, Tile.height));


			float layerDepth = GetFaceDepth(layer, x1, x2);
			float layerDepthBack = GetFaceDepth(layer - 1.0f, x1, x2);
			float sideDepth = GetFaceDepth(layer - 0.5f, x1, x2);

			spriteBatch.Draw(texture, stairFrontDest, stairSource, Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
			spriteBatch.Draw(texture, stairBackDest, stairSource, Color.BlanchedAlmond, 0f, Vector2.Zero, SpriteEffects.None, layerDepthBack);

			spriteBatch.Draw(texture, side0Dest, stairSource, Side0Color, 0f, Vector2.Zero, SpriteEffects.None, sideDepth);
			spriteBatch.Draw(texture, side1Dest, stairSource, Side1Color, 0f, Vector2.Zero, SpriteEffects.None, sideDepth);
		}

		public void Draw2(SpriteBatch spriteBatch, GameTime gameTime) {

			int minLevel = 0;
			int maxLevel = (int)Min((viewport.height / Tile.height) + 1, Levels);

			for (int layer = 0; layer < Layers; layer++) {
				for (int level = minLevel; level < maxLevel; level++) {
					for (int i = 0; i < Sections; i++) {
						int x1 = (int)coords[layer, i];
						int x2 = (int)coords[layer, (i + 1) % Sections];
						int x0 = x1;
						int x3 = x2;
						int y0 = level * Tile.height;

						float layerDepth = GetLayerDepth(3, layer, x1 >= x2);
						//if(behindTower) layerDepth = (float)(layer / (Layers * 2.0f)) + 0.5f;

						if (layer > 0) {
							x0 = (int)coords[layer - 1, i];
							x3 = (int)coords[layer - 1, (i + 1) % Sections];
						}
						var t = tilemap[layer, level, i].Type;
						switch (t) {
							case TileType.none: break;
							case TileType.brick:
								DrawBrick(spriteBatch, i, wallTile, viewport, y0, x1, x2, layerDepth);
								break;
							case TileType.step:
								DrawStep(spriteBatch, stairsTexture, viewport, y0, x0, x1, x2, x3, layer, drawBackFaces[level]);
								break;
							case TileType.passage:
								DrawPassage(spriteBatch, passageTexture, y0, viewport, x1, x2);
								break;
						}
					}
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
			for(int i = 0; i < Sections; i++) {
				int x1 = (int)coords[0, i];
				int x2 = (int)coords[0, (i + 1) % Sections];
				float outerRadiusDepth = 0.1f;
				bool behindTower = false;
				float _tint = coordsDepth[0, i] * 0.8f + 0.2f;
				Color tint = new Color(_tint, _tint, _tint, 1.0f);
				//only draw front

				Rectangle source = new Rectangle(0, 0, wallTile.Width, viewport.height);
				if (x2 > x1) {	//only calculate front faces
					Point pos = new(x1, 0);
					Point size = new(x2 - x1, viewport.height);
					Rectangle destination = new Rectangle(pos + viewport.centerTop, size);
					spriteBatch.Draw(wallTile, destination, source, Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth: 0.5f);
				}
				//begin outer radius
				else {
					//(x1, x2) = (x2, x1);        //swap to draw back of face. todo:fix swap breaking vars further down 
					outerRadiusDepth = 0.9f;            //and send to back layer
					behindTower = true;
				}
				
				int stairX1 = (int)coords[1, i];
				int stairX2 = (int)coords[1, (i + 1) % Sections];
					
				Point stairSize = new(stairX2 - stairX1, Tile.height);

				int sideX1 = x1;
				int sideX2 = stairX1;
				float sideDepth = 0.25f;
				if (behindTower) {
					sideDepth = 0.75f;
				}

				//if side-width is negative, switch to opposite side of stair
				if (sideX1 > sideX2) {
					sideX1 = stairX2;
					sideX2 = x2;
				}

				//Rectangle stairSource = new Rectangle(0, 0, wallTile.Width, Game1.WindowHeight);
				//Rectangle sideSource = new Rectangle(0, 0, wallTile.Width, Game1.WindowHeight);
				

				int y2 = 1 * Tile.height - (Tile.height * Sections) + Tile.height;
				while (y2 < viewport.height) {
					Point stairPos = new(stairX1, y2);
					Rectangle stairSource = new Rectangle(i * Tile.width, y2 * 13, stairsTexture.Width / 2, stairsTexture.Height / 4); //texture position offset pseudo random
					Rectangle stairDest = new(stairPos + viewport.centerTop, stairSize);
					Rectangle sideDest = new(new Point(sideX1, y2) + viewport.centerTop, new(sideX2 - sideX1, Tile.height));
					spriteBatch.Draw(stairsTexture, stairDest, stairSource, Color.White, 0f, Vector2.Zero, SpriteEffects.None, outerRadiusDepth);
					spriteBatch.Draw(stairsTexture, sideDest, stairSource, Color.DarkBlue, 0f, Vector2.Zero, SpriteEffects.None, sideDepth);
					//Primitives2D.FillRectangle(spriteBatch, stairDest, Color.YellowGreen, 0, outerRadiusDepth);
					//Primitives2D.FillRectangle(spriteBatch, sideDest, Color.DarkBlue, 0, sideDepth);
					Point circlePos = new Point((int)MathHelper.Lerp( stairX1 + 16, stairX2 - 32, 0.5f) - 16, y2 - 48);
					//Primitives2D.FillCircle(spriteBatch, RectangleExtensions.ToVector2(circlePos + centerTop), 32, Color.YellowGreen, (outerRadiusDepth - 0.5f) * 1.1f + 0.5f );
					spriteBatch.Draw(
						spriteTexture,
						new Rectangle(circlePos + viewport.centerTop, new(32, 32)),
						GetAnimationFrame(i + ViewSection() + 4, spriteTexture),
						Color.White,
						0f,
						Vector2.Zero,
						SpriteEffects.None,
						layerDepth: (outerRadiusDepth - 0.5f) * 1.1f + 0.5f);

					y2 += 32 * Sections;
				}
			}
		}

		//end class
	}
}
