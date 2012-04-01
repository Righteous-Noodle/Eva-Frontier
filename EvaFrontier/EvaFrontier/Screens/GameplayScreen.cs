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
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using EvaFrontier.Lib.Buildings;
using EvaFrontier.Lib.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using EvaFrontier.Lib;
using EvaFrontier.Lib.Units;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace EvaFrontier.Screens
{
    class GameplayScreen : GameScreen
    {
        #region Fields & Properties

        ContentManager _content;
        SpriteFont _gameFont;

        private Song _gameSound;

        private MouseState _mouse, _pMouse;
        private Rectangle _mouseRec;
        private Rectangle _mouseTile;
        private Vector2 rMouse = new Vector2(-1,-1);
        private AnimatedBuilding turbine;
        private StaticBuilding solarPlant;

        Random _random = new Random();

        public bool DidWin {
            get {
                int numberOfFarms = 0;
                foreach (var sprite in World.Sprites) {
                    if (sprite is Village) {
                        Village village = sprite as Village;
                        if (village.IsFarmBuilt) {
                            numberOfFarms++;
                        }
                    }
                }

                return (numberOfFarms >=4 && 
                    World.Sprites.FindAll(s => s is UAV || s is Osprey).Count > 0);
            }
        }

        public bool DidLose {
            get {
                int numberOfDeadVillage = 0;
                foreach (var sprite in World.Sprites) {
                    if (sprite is Village) {
                        Village village = sprite as Village;
                        if (village.IsDead) {
                            numberOfDeadVillage++;
                        }
                    }
                }
                return (numberOfDeadVillage >= 4);
            }
        }

        #endregion

        #region Initialization

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            World.Selection = new Selection();
        }

        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            //_gameFont = _content.Load<SpriteFont>(@"Fonts\gamefont");

            _gameSound = _content.Load<Song>(@"SFX\gameSound");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = Settings.MainVolume;
            MediaPlayer.Play(_gameSound);

            _CreateWorld();

            World.Selection.LoadContent(_content);

            MapObjectLayer objects = World.Map.GetLayer("Objects") as MapObjectLayer;
            if (objects != null) {

                MapObject headQuarterLocation = objects.GetObject("HQ");
                World.HeadQuarter = new HeadQuarter(new Vector2(headQuarterLocation.Bounds.X, headQuarterLocation.Bounds.Y),
                                                        @"Textures\Buildings\HQ", 300, 500, 999, 500, 999);
                World.HeadQuarter.LoadContent(_content);

                World.ResourcesManager = new ResourcesManager(World.HeadQuarter);
                World.ResourcesManager.LoadContent(_content);

                foreach (MapObject obj in objects.GetObjectsByType("Village"))
                {
                    int initFood = Convert.ToInt32(obj.Properties["Food"].RawValue);
                    int initMedicine = Convert.ToInt32(obj.Properties["Medicine"].RawValue);
                    int initPopulation = Convert.ToInt32(obj.Properties["Population"].RawValue);

                    Village village = new Village(new Vector2(obj.Bounds.X, obj.Bounds.Y), initFood, 999, initMedicine, 999, initPopulation);
                    village.LoadContent(_content);
                    World.Sprites.Add(village);
                }

                World.Sprites.Add(World.HeadQuarter);
            }

            World.Camera.Focus(World.HeadQuarter.Position);

            World.LoadCollisionClip();

            World.Time = new WorldTime(12, 10, 2012, 6, 0);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

        private void _CreateWorld() {
            World.Map = _content.Load<Map>(@"Maps\1");

            int viewWidth = ScreenManager.GraphicsDevice.Viewport.Width;
            int viewHeight = ScreenManager.GraphicsDevice.Viewport.Height;

            World.Camera = new Camera(new Vector2(World.Map.Width, World.Map.Height),
                                new Vector2(World.Map.TileWidth, World.Map.TileHeight),
                                new Vector2(viewWidth, viewHeight - HUD.HUDHeight));

            World.HUD = new HUD(World.Map.GetSize());
            World.HUD.LoadContent(_content);

            World.Sprites = new List<Sprite>();
        }

        public override void UnloadContent()
        {
            MediaPlayer.Pause();
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
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                World.Update(gameTime);
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null) throw new ArgumentNullException("input");

            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];
            _mouse = input.CurrentMouseState;
            _pMouse = input.PreviousMouseState;

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else if (DidLose) {
                ScreenManager.AddScreen(new GameOverScreen(), ControllingPlayer);
            }
            else if (DidWin) {
                ScreenManager.AddScreen(new GameWonScreen(), ControllingPlayer);
            }
            else
            {
                if (keyboardState.IsKeyDown(Keys.H)) {
                    World.Camera.Focus(World.HeadQuarter.Position);
                }
                World.Camera.HandleInput(input, ControllingPlayer);
               
                World.HandleInput(input, _content);
  
                _HandleMouse(input, ControllingPlayer);
            }
        }

        private void _HandleMouse(InputState input, PlayerIndex? ControllingPlayer)
        {
            Boolean isMouseMoved = (_mouse != _pMouse);

            if (isMouseMoved) {
                _mouseRec = new Rectangle(_mouse.X, _mouse.Y, 1, 1);
                World.Mouse = World.ScreenToWorldPosition(new Vector2(_mouse.X, _mouse.Y));
            }

            if (_mouse.LeftButton == ButtonState.Pressed) {
                if (rMouse.X == -1)
                {
                    rMouse.X = _mouse.X;
                    rMouse.Y = _mouse.Y;
                }

                if (_pMouse.LeftButton == ButtonState.Released &&
                    _mouse.Y <= World.Camera.ViewArea.Height) {
                
                    World.Selection.Box = new Rectangle(_mouse.X, _mouse.Y, 0, 0);
                    
                }
                if (rMouse.X != -1)
                if (World.Selection.Box.HasValue) {
                    if (_mouse.Y <= World.Camera.ViewArea.Height) {
                        //_selection.Box = new Rectangle(_selection.Box.Value.X, _selection.Box.Value.Y,
                        //                               _mouse.X - _selection.Box.Value.X, _mouse.Y - _selection.Box.Value.Y);
                        int currentX = (int)rMouse.X;
                        int currentY = (int)rMouse.Y;
                        int newX = _mouse.X;
                        int newY = _mouse.Y;
                        int width = Math.Abs(currentX - newX);
                        int height = Math.Abs(currentY - newY);

                        World.Selection.Box = new Rectangle(Math.Min(currentX, newX), Math.Min(currentY, newY), width, height); 
                    //} else {
                    //    _selection.Box = new Rectangle(_selection.Box.Value.X, _selection.Box.Value.Y,
                    //                                   _mouse.X - _selection.Box.Value.X, World.Camera.ViewArea.Height - _selection.Box.Value.Y);
                        //World.Camera.Focus(new Vector2(World.Camera.Position.X, World.Camera.Position.Y + (_mouse.Y - World.Camera.ViewArea.Height)));
                    }
                }
            }

            if (_mouse.LeftButton == ButtonState.Released)
            {
                World.Selection.Box = null;
                rMouse.X = -1;
                rMouse.Y = -1;
            }

            _pMouse = _mouse;
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None, World.Camera.Matrix);

            
            World.Map.Draw(spriteBatch, World.Camera.ViewArea); // Draw all layers at once

            //World.Map.DrawLayer(spriteBatch, "Ocean", World.Camera.ViewArea);
            //World.Map.DrawLayer(spriteBatch, "Ground", World.Camera.ViewArea);
            //World.Map.DrawLayer(spriteBatch, "Mountain", World.Camera.ViewArea);
            //World.Map.DrawLayer(spriteBatch, "Decoration", World.Camera.ViewArea);
            //World.Map.DrawLayer(spriteBatch, "Tree", World.Camera.ViewArea);

            World.HeadQuarter.Draw(spriteBatch);

            foreach (var sprite in World.Sprites)
            {
                sprite.Draw(spriteBatch);
            }

            /*if (World.Selection.SelectedSprite != null)
            {
                World.Selection.SelectedSprite.DrawSelectionSquare(spriteBatch);
            }*/

            //World.Map.DrawLayer(spriteBatch, "Clouds 1", World.Camera.ViewArea);
            //World.Map.DrawLayer(spriteBatch, "Clouds 2", World.Camera.ViewArea);

            spriteBatch.End();

            World.Draw(spriteBatch);

            spriteBatch.Begin(/*SpriteBlendMode.None, SpriteSortMode.Deferred, SaveStateMode.None, World.Camera.Matrix*/);
            World.Selection.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin();

            Vector2 timeSize = ScreenManager.Font.MeasureString(World.Time.ToString()) / 2;
            Vector2 timePosition = new Vector2((ScreenManager.Game.GraphicsDevice.Viewport.Width / 2) - (timeSize.X / 2), 0f);
            //spriteBatch.DrawString(ScreenManager.Font, World.Time.ToString(), timePosition, Color.Yellow, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            //spriteBatch.DrawString(ScreenManager.Font, "Mouse Position: " + World.Mouse + "-" + Mouse.GetState(), new Vector2(0f, 110f), Color.Yellow);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }

        #endregion

    }
}
