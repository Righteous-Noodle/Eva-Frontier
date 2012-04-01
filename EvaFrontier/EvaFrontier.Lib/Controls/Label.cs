using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvaFrontier.Lib.Controls
{
    public class Label : Control
    {
        #region Constructors

        public Label(SpriteFont spriteFont, string name) {
            Name = name;
            SpriteFont = spriteFont;
            IsTabStop = false;
            IsEnabled = true;
            IsVisible = true;
            Color = Color.White;
        }

        #endregion

        #region Abstract Methods

        public override void Update(GameTime gameTime) {
            
        }

        public override void HandleInput(InputState input) {
            
        }

        public override void Draw(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(SpriteFont, Text, Position, Color);
        }

        #endregion
    }
}
