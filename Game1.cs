using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Castelian {


	public class Game1 : Game {
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		Tower tower;

		public Game1() {
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			IsFixedTimeStep = true;
			Primitives2D.DegreesMode = true;
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





		protected override void LoadContent() {
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			Texture2D cylinderTexture = Content.Load<Texture2D>("bluebrick");
			Texture2D background = Content.Load<Texture2D>("internetandrej_1_looped");
			Texture2D marble = Content.Load<Texture2D>("marbled");
			//magic number for 64px to be square: 656
			Texture2D spriteTexture = Content.Load<Texture2D>("ball_32px_noisy_32colors");
			Texture2D passage = Content.Load<Texture2D>("passage_bluebrick");
			tower = new Tower(656, 30, 16, cylinderTexture, background, marble, spriteTexture, passage);
		}

		KeyboardState prevState;
		protected override void Update(GameTime gameTime) {
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			MouseState mouseState = Mouse.GetState();
			Point cursor = new(mouseState.X, mouseState.Y);

			KeyboardState state = Keyboard.GetState();

			if (prevState.IsKeyUp(Keys.Space) && state.IsKeyDown(Keys.Space))
				tower.Paused = !tower.Paused;
			if (prevState.IsKeyUp(Keys.Up) && state.IsKeyDown(Keys.Up))
				tower.speed += 5.0f;
			if (prevState.IsKeyUp(Keys.Down) && state.IsKeyDown(Keys.Down))
				tower.speed -= 5.0f;

			int left = state.IsKeyDown(Keys.Left) ? -1 : 0;
			int right = state.IsKeyDown(Keys.Right) ? 1 : 0;
			tower.Move = left + right;

			prevState = state;

			tower.Update(gameTime, cursor);
			tower.Move = 0;

			base.Update(gameTime);
		}





		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.Black);
			_spriteBatch.Begin(samplerState: SamplerState.PointWrap);
			tower.DrawBackground(_spriteBatch);
			if (tower.Paused) {
				Primitives2D.FillRectangle(_spriteBatch, new Rectangle(WindowWidth - 96, 32, 16, 64), Color.Yellow);
				Primitives2D.FillRectangle(_spriteBatch, new Rectangle(WindowWidth - 64, 32, 16, 64), Color.Yellow);
			}
				
			_spriteBatch.End();
			_spriteBatch.Begin(samplerState: SamplerState.AnisotropicWrap, sortMode: SpriteSortMode.BackToFront);
			tower.Draw2(_spriteBatch, gameTime);
			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
