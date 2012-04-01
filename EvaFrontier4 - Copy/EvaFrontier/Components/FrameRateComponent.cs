using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvaFrontier.Components
{
	/// <summary>
	/// A real basic frame rate counter.
	/// </summary>
	public class FrameRateComponent : DrawableGameComponent
	{
		private int drawCount;
		private float drawTimer;
		private string drawString = "FPS: ";

		private SpriteBatch spriteBatch;
		private SpriteFont font;

		public FrameRateComponent(Game game)
			: base(game)
		{
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			font = Game.Content.Load<SpriteFont>(@"Fonts\Font");
		}

		public override void Draw(GameTime gameTime)
		{
			drawCount++;
			drawTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (drawTimer >= 1f)
			{
				drawTimer -= 1f;
				drawString = "FPS: " + drawCount;
				drawCount = 0;
			}

			spriteBatch.Begin();
			spriteBatch.DrawString(font, drawString, new Vector2(10f, 10f), Color.White);
			spriteBatch.End();
		}
	}
}
