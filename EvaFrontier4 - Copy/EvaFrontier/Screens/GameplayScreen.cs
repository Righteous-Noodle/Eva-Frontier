#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using EvaFrontier.Components;
using EvaFrontier.Controllers;
using EvaFrontier.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledLib;

#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager _content;
        SpriteFont _gameFont;
        private GraphicsDevice _graphicsDevice;
        private GameComponentCollection _components;

        private KeyboardState _keyboardState;
        private GamePadState _gamePadState;
        private MouseState _mouseState;

        Random _random = new Random();

        float pauseAlpha;

        // a blank 1x1 texture for drawing the box and point
        Texture2D blank;
        private Texture2D gridTexture;

        private Vector2 _tileMouseOnWorldPosition;

        private World _world;

        CameraManager _cameraManager;

        // some objects in the map
        MapObject box;
        MapObject point;

        // the color to draw our box object
        Color boxColor;

        // change to see the effects of each drawing method
        // if set to true, we use the efficient Draw(SpriteBatch, Rectangle) method.
        // if set to false, we use the slower Draw(SpriteBatch) method.
        bool useEfficientDrawing = true;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _components = ScreenManager.Game.Components;
            _graphicsDevice = ScreenManager.GraphicsDevice;

            _gameFont = _content.Load<SpriteFont>(@"Fonts\GameFont");

            blank = new Texture2D(_graphicsDevice, 1, 1);
            blank.SetData(new[] { Color.White });

            gridTexture = _content.Load<Texture2D>(@"Textures\Grid");

            Vector2 viewSize = new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
            _world = new World(_content.Load<Map>(@"Maps\1experiment"), viewSize);

            _cameraManager = new CameraManager(ScreenManager.Game, _world.Camera, InputType.Keyboard, PlayerIndex.One);
            _components.Add(_cameraManager);

            // find the "Box" and "Point" objects we made in the level
            /*MapObjectLayer objects = _world.Map.GetLayer("Objects") as MapObjectLayer;
            point = objects.GetObject("Point");
            box = objects.GetObject("Box");*/

            // attempt to read the box color from the box object properties
            /*try
            {
                boxColor.R = (byte)box.Properties["R"];
                boxColor.G = (byte)box.Properties["G"];
                boxColor.B = (byte)box.Properties["B"];
                boxColor.A = 255;
            }
            catch
            {
                // on failure, default to yellow
                boxColor = Color.Yellow;
            }*/

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            //Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            _content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                // insert something more interesting in this space :-)
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            _keyboardState = input.CurrentKeyboardStates[playerIndex];
            _gamePadState = input.CurrentGamePadStates[playerIndex];
            _mouseState = Mouse.GetState();

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !_gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // Insert input handling here
                
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // we use 0 and null to indicate that we just want the default values
            //spriteBatch.Begin(0, null, null, null, null, null, _cameraManager.Transform);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, _cameraManager.Transform);

            if (useEfficientDrawing)
            {
                // if using the efficient drawing, we use an overload of Draw that takes in the area to
                // draw in order to compensate for large maps that only need to draw what's on screen.
                // our visible area is computed using the Camera and the viewport size.
                _world.Map.Draw(spriteBatch, _cameraManager.VisibleArea);
            }
            else
            {
                // if not using the efficient drawing, just call Draw to draw the whole map. this is substantially
                // slower for such a large map.
                _world.Map.Draw(spriteBatch);
            }

            // fill in our box object
            //spriteBatch.Draw(blank, box.Bounds, boxColor);

            // draw a box around our point
            //spriteBatch.Draw(blank, new Rectangle(point.Bounds.X - 5, point.Bounds.Y - 5, 10, 10), Color.Red);

            DrawMapGrid(spriteBatch, new Color(0, 0, 0, 128));

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        private void DrawMapGrid(SpriteBatch spriteBatch, Color gridColor) {
            Rectangle gridRectangle;

            int tileWidth = _world.Map.TileWidth;
            int tileHeight = _world.Map.TileHeight;

            int leftTile = _cameraManager.VisibleArea.X / tileWidth;
            int rightTile = _cameraManager.VisibleArea.Right / tileWidth;
            int topTile = _cameraManager.VisibleArea.Y / tileHeight;
            int bottomTile = _cameraManager.VisibleArea.Bottom / tileHeight;

            for (int i=leftTile; i<rightTile+1; i++)
            {
                for (int j=topTile; j<bottomTile+1; j++) {
                    gridRectangle = new Rectangle(i*tileWidth, j*tileHeight, tileWidth, tileHeight);
                    // Draw vertical lines
                    spriteBatch.Draw(blank, new Rectangle(i*tileWidth, j*tileHeight, tileWidth, 1), gridColor);
                    // Draw horizontal lines
                    spriteBatch.Draw(blank, new Rectangle(i*tileWidth, j*tileHeight, 1, tileHeight), gridColor);

                    // Draw hightlight curson on mouse's coordinates.
                    if (gridRectangle.Contains((int)_world.Mouse.X, (int)_world.Mouse.Y)) {
                        for (int a=gridRectangle.X; a<gridRectangle.X + gridRectangle.Width; a++ ) {
                            for (int b=gridRectangle.Y; b<gridRectangle.Y+gridRectangle.Height; b++) {
                                if (a==gridRectangle.X || a==gridRectangle.X + gridRectangle.Width-1) {
                                    
                                }
                            }
                        }
                            //spriteBatch.Draw(gridTexture, new Rectangle(i * tileWidth, j * tileHeight, tileWidth, tileHeight), Color.White);
                    }
                }
            }
        }

        #endregion
    }
}
