using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using EvaFrontier.Lib;

namespace EvaFrontier
{
    class CreditsMenuScreen: MenuScreen1
    {
        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>

        public CreditsMenuScreen()
            : base("Continue Game")
        {
            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;

            MenuEntry1 backMenuEntry = new MenuEntry1("Back");
            backMenuEntry.Selected += OnCancel;
            MenuEntries.Add(backMenuEntry);

        }
        #endregion

        #region Drawing
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 4 / 5);

            Vector2 position = new Vector2(100, 250);
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            //spriteBatch.Draw(background, fullscreen, transitionColor);

            spriteBatch.DrawString(ScreenManager.Font, "[ Design /Concept / Programming / Art ] - Righteous Noodle",
                position, Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "[ Design / Testing ] - Righteous Noodle",
                position + new Vector2(0f, ScreenManager.Font.LineSpacing * 2), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "[ Music ] - Righteous Noodle",
                position + new Vector2(0f, ScreenManager.Font.LineSpacing * 4), Color.White);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
