#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EvaFrontier.Lib;
using Microsoft.Xna.Framework.Input;
#endregion

namespace EvaFrontier
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class MenuScreen1 : GameScreen
    {
        #region Fields

        List<MenuEntry1> menuEntries = new List<MenuEntry1>();
        int selectedEntry = 0;
        string menuTitle;

        int width, height;                                      
        SpriteFont spriteFont;                                      
        MouseState mouseState;                                  
        Vector2 position = new Vector2();                           

        #endregion

        #region Properties
        public Vector2 MenuPosition { get; set; }
        public Vector2 TitlePosition { get; set; }
        public float LineSpacing { get; set; }
        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry1> MenuEntries
        {
            get { return menuEntries; }
        }
        #endregion

        #region Initialization

                                                                    
        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

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

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public void SetMenuItems(List<MenuEntry1> entry)
        {
            menuEntries.Clear();
            menuEntries.AddRange(entry);
            CalculateBounds();
        }

        private void CalculateBounds()
        {
            height = 0;
            width = 0;

            foreach (MenuEntry1 item in menuEntries)
            {
                spriteFont = item.GetSpriteFont(this);
                height += spriteFont.LineSpacing;
                if (item.GetMeasureString(this).X > width)
                    width = (int)item.GetMeasureString(this).X;
            }
        }                                                           

        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen1(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            MenuPosition = new Vector2(100, 150);
            TitlePosition = new Vector2(426, 80);
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

            if (input.IsMenuSelect(ControllingPlayer, out playerIndex) || input.IsNewLeftMouseClick()) 
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
            Point mouseLocation = new Point(mouseState.X, mouseState.Y);

            Rectangle itemRectangle = new Rectangle(
                (int)position.X,
                (int)position.Y + 120,
                0,
                50);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                itemRectangle.Width = (int)menuEntries[i].GetMeasureString(this).X;
                if (itemRectangle.Contains(mouseLocation))
                    selectedEntry = i;

                itemRectangle.Y += 50;

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
                MenuEntry1 menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, position, isSelected, gameTime);

                position.Y += menuEntry.GetHeight(this) + LineSpacing;
            }
            
            // Draw the menu title.
            //Vector2 titlePosition = new Vector2(426, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192, TransitionAlpha);
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

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

