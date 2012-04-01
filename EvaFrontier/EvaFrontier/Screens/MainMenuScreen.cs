#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using EvaFrontier.Lib;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using EvaFrontier;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;
#endregion

namespace EvaFrontier.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        ContentManager content;
        Texture2D background;
        private Song _menuSound;

        static public Texture2D textNG;
        static public Texture2D textLG;
        static public Texture2D textOp;
        static public Texture2D textCr;
        static public Texture2D textEx;
        static public Texture2D gameLogo;
        static public Vector2 centerScreen;

        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("EVA FRONTIER")
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("New Game");
            MenuEntry continueGameMenuEntry = new MenuEntry("Load Game");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry creditsMenuEntry = new MenuEntry("Credits");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            continueGameMenuEntry.Selected += ContinueGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            creditsMenuEntry.Selected += CreditsGameMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(continueGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(creditsMenuEntry);
            MenuEntries.Add(exitMenuEntry);

            MenuPosition = new Vector2(100f, 300f);
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen());
        }

        /// <summary>
        /// Event handler for when the Continue Game menu entry is selected.
        /// </summary>
        void ContinueGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
            //                   new GameplayScreen());
            ScreenManager.AddScreen(new ContinueGameMenuScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// Event handler for when the Credits Game menu entry is selected.
        /// </summary>
        void CreditsGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new CreditsMenuScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit Eva Frontier?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
       
        public override void LoadContent()
        {
            if (content == null) content = new ContentManager(ScreenManager.Game.Services , "Content");

            _menuSound = content.Load<Song>(@"SFX\menuSound1");

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = Settings.MainVolume;
            MediaPlayer.Play(_menuSound);

            background = content.Load<Texture2D>(@"Textures\islands");
            textNG = content.Load<Texture2D>(@"Textures\newGameText");
            textLG = content.Load<Texture2D>(@"Textures\loadGameText");
            textOp = content.Load<Texture2D>(@"Textures\optionsText");
            textCr = content.Load<Texture2D>(@"Textures\creditsText");
            textEx = content.Load<Texture2D>(@"Textures\exitText");
            gameLogo = content.Load<Texture2D>(@"Textures\gameLogo");
        }
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            centerScreen = new Vector2(viewport.Width / 2, 20f);
            byte fade = TransitionAlpha;

            Color transitionColor = new Color(fade, fade, fade);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(background, fullscreen, transitionColor);
            spriteBatch.Draw(gameLogo, new Vector2(centerScreen.X - gameLogo.Width / 2, 20f), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion
    }
}
