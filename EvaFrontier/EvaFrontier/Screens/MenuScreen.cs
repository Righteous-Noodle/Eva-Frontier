#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EvaFrontier.Lib;
using EvaFrontier.Screens;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
#endregion

namespace EvaFrontier
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class MenuScreen : GameScreen
    {
        #region Fields

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        int selectedEntry = 0;
        string menuTitle;

        MouseState mouseState;
        Point mouseLocation;
        Rectangle itemRectangle0, itemRectangle1, itemRectangle2, itemRectangle3, itemRectangle4;

        #endregion

        #region Properties
        public Vector2 MenuPosition { get; set; }
        public Vector2 TitlePosition { get; set; }
        public float LineSpacing { get; set; }
        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }
        #endregion

        #region Initialization                       

        public int SelectedEntry              
        {
            get { return selectedEntry; }
            set
            {
                selectedEntry = (int)MathHelper.Clamp(
                    value,
                    0,
                    menuEntries.Count - 1);
            }
        }                                   

        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            MenuPosition = new Vector2(100, 150);
            TitlePosition = new Vector2(750, 40);
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            // Move to the previous menu entry?
            if (input.IsMenuUp(ControllingPlayer))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;

                EvaFrontier.buttonOver.Play();
            }

            // Move to the next menu entry?
            if (input.IsMenuDown(ControllingPlayer))
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;

                EvaFrontier.buttonOver.Play();
            }

            // Accept or cancel the menu? We pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex;

            if (input.IsMenuSelect(ControllingPlayer, out playerIndex) || (itemRectangle0.Contains(mouseLocation) && input.IsNewLeftMouseClick() || 
                itemRectangle1.Contains(mouseLocation) && input.IsNewLeftMouseClick() || itemRectangle2.Contains(mouseLocation) && input.IsNewLeftMouseClick() ||
                    itemRectangle3.Contains(mouseLocation) && input.IsNewLeftMouseClick() || itemRectangle4.Contains(mouseLocation) && input.IsNewLeftMouseClick()))  
            {                
                OnSelectEntry(selectedEntry, playerIndex);
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[selectedEntry].OnSelectEntry(playerIndex);
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }


        #endregion

        #region Update and Draw
        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            mouseState = Mouse.GetState();                          
            mouseLocation = new Point(mouseState.X, mouseState.Y);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                switch (i)
                {
                    case 0: // New Game
                        itemRectangle0 = new Rectangle(190, 230, MainMenuScreen.textNG.Width, MainMenuScreen.textNG.Height);
                        if (itemRectangle0.Contains(mouseLocation))
                            selectedEntry = i;
                        break;
                    case 1: // Load Game
                        itemRectangle1 = new Rectangle(1020, 250, MainMenuScreen.textLG.Width, MainMenuScreen.textLG.Height);
                        if (itemRectangle1.Contains(mouseLocation))
                            selectedEntry = i;
                        break;
                    case 2: // Option
                        itemRectangle2 = new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2 - MainMenuScreen.textOp.Width / 2, 530, MainMenuScreen.textOp.Width, MainMenuScreen.textOp.Height);
                        if (itemRectangle2.Contains(mouseLocation))
                            selectedEntry = i;
                        break;
                    case 3: // Credit
                        itemRectangle3 = new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2 - MainMenuScreen.textCr.Width / 2, 580, MainMenuScreen.textCr.Width, MainMenuScreen.textCr.Height);
                        if (itemRectangle3.Contains(mouseLocation))
                            selectedEntry = i;
                        break;
                    case 4: // Exit
                        itemRectangle4 = new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2 - MainMenuScreen.textEx.Width / 2, 630, MainMenuScreen.textEx.Width, MainMenuScreen.textEx.Height);
                        if (itemRectangle4.Contains(mouseLocation))
                            selectedEntry = i;
                        break;
                }

                bool isSelected = IsActive && (i == selectedEntry);
                menuEntries[i].Update(this, isSelected, gameTime);
            }              

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
            Vector2 position = MenuPosition;
            Vector2 titlePosition = TitlePosition;
            
            
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                switch (i)
                {
                    case 0: // New Game
                        position = new Vector2(190, 250);
                        break;
                    case 1: // Load Game
                        position = new Vector2(1020, 270);
                        break;
                    case 2: // Option
                        position = new Vector2(MainMenuScreen.centerScreen.X - MainMenuScreen.textOp.Width /2, 550);
                        break;
                    case 3: // Credit
                        position = new Vector2(MainMenuScreen.centerScreen.X - MainMenuScreen.textCr.Width /2, 600);
                        break;
                    case 4: // Exit
                        position = new Vector2(MainMenuScreen.centerScreen.X - MainMenuScreen.textEx.Width /2, 650);
                        break;
                }
                menuEntry.Draw(this, position, isSelected, gameTime);
            }
           
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(0, 100, 0, TransitionAlpha);

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.End();
        }
       #endregion

        #region CenterTitle

        public void CenterTitle()
        {
            if (ScreenManager != null)
            {
                TitlePosition = new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 2)
                - (ScreenManager.Font.MeasureString(menuTitle).X / 2), 80);
            }
        }
        #endregion
    }
}
