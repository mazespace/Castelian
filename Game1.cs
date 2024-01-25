using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Castelian {


	public class Game1 : Game {
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		public Game1() {
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		public static int WindowWidth, WindowHeight;

		protected override void Initialize() {
			Point WindowedSize = new(1280, 720);

			_graphics.IsFullScreen = true;
			_graphics.HardwareModeSwitch = false;

			int displayWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
			int displayHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
			if (_graphics.IsFullScreen) {
				WindowWidth = displayWidth;
				WindowHeight = displayHeight;
			} else {
				WindowWidth = WindowedSize.X;
				WindowHeight = WindowedSize.Y;
				Window.Position = new Point(displayWidth / 2 - WindowWidth / 2, displayHeight / 2 - WindowHeight / 2);
			}

			_graphics.PreferredBackBufferWidth = WindowWidth;
			_graphics.PreferredBackBufferHeight = WindowHeight;
			_graphics.ApplyChanges();

			base.Initialize();
		}




		Tower tower;

		protected override void LoadContent() {
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			Texture2D cylinderTexture = Content.Load<Texture2D>("bluebrick");
			Texture2D background = Content.Load<Texture2D>("internetandrej_1_looped");
			Texture2D marble = Content.Load<Texture2D>("marbled");

			tower = new(656, 16, cylinderTexture, background, marble);
		}

		protected override void Update(GameTime gameTime) {
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			MouseState mouseState = Mouse.GetState();
			Point cursor = new(mouseState.X, mouseState.Y);


			tower.Update(gameTime, cursor);

			base.Update(gameTime);
		}



		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.Black);
			_spriteBatch.Begin(samplerState: SamplerState.PointWrap);
			tower.DrawBackground(_spriteBatch);
			_spriteBatch.End();
			_spriteBatch.Begin(samplerState: SamplerState.AnisotropicWrap, sortMode: SpriteSortMode.FrontToBack);
			tower.Draw(_spriteBatch);
			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
