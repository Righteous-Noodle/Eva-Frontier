#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using EvaFrontier.Lib;
#endregion

namespace EvaFrontier
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen1
    {
        #region Fields

        MenuEntry1 screenResolutionMenuEntry;
        MenuEntry1 fullScreenMenuEntry;
        MenuEntry1 mainVolumeMenuEntry;
        MenuEntry1 sfxVolumeMenuEntry;

        #endregion

        #region Initialization

        public OptionsMenuScreen()
            : base("Options")
        {
            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;

            screenResolutionMenuEntry = new MenuEntry1(string.Empty);
            fullScreenMenuEntry = new MenuEntry1(string.Empty);
            mainVolumeMenuEntry = new MenuEntry1(string.Empty);
            sfxVolumeMenuEntry = new MenuEntry1(string.Empty);

            SetMenuEntryText();

            MenuEntry1 backMenuEntry = new MenuEntry1("Back");

            screenResolutionMenuEntry.Selected += ScreenResolutionEntrySelected;
            fullScreenMenuEntry.Selected += FullScreenEntrySelected;
            mainVolumeMenuEntry.Selected += MainVolumeEntrySelected;
            sfxVolumeMenuEntry.Selected += SFXVolumeEntrySelected;
            backMenuEntry.Selected += OnCancel;

            MenuEntries.Add(screenResolutionMenuEntry);
            MenuEntries.Add(fullScreenMenuEntry);
            MenuEntries.Add(mainVolumeMenuEntry);
            MenuEntries.Add(sfxVolumeMenuEntry);
            MenuEntries.Add(backMenuEntry);
        }

        void SetMenuEntryText()
        {
            screenResolutionMenuEntry.Text = String.Format("Screen Resolution: {0} x {1}",
                Settings.ScreenResolution.Key, Settings.ScreenResolution.Value);
            fullScreenMenuEntry.Text = String.Format("Fullscreen: {0}", Settings.IsFullScreen);
            mainVolumeMenuEntry.Text = String.Format("Main Volume: {0} %", (int)(Settings.MainVolume * 100));
            sfxVolumeMenuEntry.Text = String.Format("SFX Volume: {0} %", (int)(Settings.SFXVolume * 100));
        }


        #endregion

        #region Handle Input

        void ScreenResolutionEntrySelected(object sender, PlayerIndexEventArgs e)
        {

            SetMenuEntryText();
        }

        void FullScreenEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            Settings.IsFullScreen = !Settings.IsFullScreen;

            SetMenuEntryText();
        }

        void MainVolumeEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            Settings.MainVolume += 0.05f;
            if (Settings.MainVolume > 1.0f) Settings.MainVolume = 0;

            SetMenuEntryText();
        }

        void SFXVolumeEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            Settings.SFXVolume += 0.05f;
            if (Settings.SFXVolume > 1.0f) Settings.SFXVolume = 0;

            SetMenuEntryText();
        }
        #endregion

        #region Load Content

        public override void LoadContent()
        {
            EvaFrontier.screenSwitch.Play();
        }

        #endregion

        #region Drawing

        public override void Draw(GameTime gameTime)
        {
            //SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            //Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            //Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            //byte fade = TransitionAlpha;

            //Color TransitionColor = new Color(fade, fade, fade);
            //spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            //spriteBatch.Draw(background, fullscreen, TransitionColor);
            //spriteBatch.End();

            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 4 / 5);

            base.Draw(gameTime);
        }
        #endregion
    }
}
