using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RTSEngine.Components;
using GameStateManagement;
using TiledLib;

namespace RTSEngine
{
    public partial class RTSEngine : Game
    {
        /// <summary>
        /// The graphics device, used to render.  
        /// </summary>
        protected static GraphicsDeviceManager _graphicsDeviceManager = null;

        /// <summary>
        /// Screen manager, using GameStateManagement
        /// </summary>
        protected static ScreenManager _screenManager;

        /// <summary>
        ///  The map of the game, using TiledLib
        /// </summary>
        protected static Map _map;

        /// <summary>
        /// Head-Up Display with controls, minimap, etc..
        /// </summary>
        //protected static HUD _hud;

        private static bool _deviceChangesApplied = false;

        protected RTSEngine() {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            GameSettings.Initialize();
            ApplyResolutionChange();
        }

        public static void ApplyResolutionChange()
        {
            int resolutionWidth = GameSettings.Default.ResolutionWidth;
            int resolutionHeight = GameSettings.Default.ResolutionHeight;

            if (resolutionWidth <= 0 || resolutionWidth <= 0)
            {
                resolutionWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                resolutionHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
#if XBOX360
            // Xbox 360 graphics settings are fixed
            _graphicsDeviceManager.IsFullScreen = true;
            _graphicsDeviceManager.PreferredBackBufferWidth =
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphicsDeviceManager.PreferredBackBufferHeight =
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
#else
            _graphicsDeviceManager.PreferredBackBufferWidth = resolutionWidth;
            _graphicsDeviceManager.PreferredBackBufferHeight = resolutionHeight;
            _graphicsDeviceManager.IsFullScreen = GameSettings.Default.Fullscreen;
 
            _deviceChangesApplied = true;
#endif
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Apply device changes
            if (_deviceChangesApplied) {
                _graphicsDeviceManager.ApplyChanges();
                _deviceChangesApplied = false;
            }
        }
    }
}
