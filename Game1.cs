using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Castelian {
	public class Game1 : Game {
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		public int WindowWidth { get; private set; }
		public int WindowHeight { get; private set; }

		public Game1() {
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			WindowWidth = 1024;
			WindowHeight = 768;

			_graphics.PreferredBackBufferWidth = WindowWidth;
			_graphics.PreferredBackBufferHeight = WindowHeight;
			_graphics.ApplyChanges();

			base.Initialize();
		}

		protected override void Initialize() {
			// TODO: Add your initialization logic here

			base.Initialize();
		}




		Cylinder cylinder;

		protected override void LoadContent() {
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			Texture2D cylinderTexture = Content.Load<Texture2D>("bluebrick");
			Texture2D background = Content.Load<Texture2D>("internetandrej_1_looped");
			cylinder = new(656, 16, cylinderTexture, background);
		}

		protected override void Update(GameTime gameTime) {
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			MouseState mouseState = Mouse.GetState();
			Point cursor = new(mouseState.X, mouseState.Y);


			cylinder.Update(gameTime, cursor);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.Black);

			_spriteBatch.Begin(samplerState: SamplerState.AnisotropicWrap);
			cylinder.Draw(_spriteBatch);
			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
