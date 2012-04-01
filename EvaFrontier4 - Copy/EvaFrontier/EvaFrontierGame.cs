using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RTSEngine.Components;
using TiledLib;
using Microsoft.Xna.Framework.Input.Touch;
using System;

namespace EvaFrontier
{
	public class EvaFrontierGame : RTSEngine.RTSEngine
	{
		//GraphicsDeviceManager _graphics;
	    //private readonly ScreenManager _screenManager;
        Cursor _cursor;

        // By preloading any assets used by UI rendering, we avoid framerate glitches
        // when they suddenly need to be loaded in the middle of a menu transition.
        static readonly string[] PreloadAssets =
        {
            @"Textures\Gradient",
        };

		public EvaFrontierGame()
		{
            /*_graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720
            };*/

			// on the phone, we want to be fullscreen and we're locked to 30Hz
#if WINDOWS_PHONE
			_graphics.IsFullScreen = true;
			TargetElapsedTime = TimeSpan.FromTicks(333333);
#endif

			Content.RootDirectory = "Content";

            _screenManager = new ScreenManager(this);
            Components.Add(_screenManager);
            // Activate the first screens.
            _screenManager.AddScreen(new BackgroundScreen(), null);
            _screenManager.AddScreen(new MainMenuScreen(), null);

            _cursor = new Cursor(this, 50);
		    _cursor.BorderColor = Color.White;
		    _cursor.FillColor = Color.Black;
            Components.Add(_cursor);

			// we are using a frame rate counter to compare the performance of the drawing
			Components.Add(new FrameRateComponent(this));

			// utilize drag and flick to move the Camera
			TouchPanel.EnabledGestures = GestureType.FreeDrag | GestureType.Flick;
		}

        /// <summary>
        /// Loads graphics content.
        /// </summary>
		protected override void LoadContent()
		{
            foreach (string asset in PreloadAssets)
            {
                Content.Load<object>(asset);
            }
		}

		protected override void Update(GameTime gameTime)
		{

#if WINDOWS_PHONE
			// if we have a finger on the screen, set the velocity to 0
			if (TouchPanel.GetState().Count > 0)
			{
				cameraVelocity = Vector2.Zero;
			}

			// update our Camera with the velocity
			MoveCamera(cameraVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);

			// apply some friction to the Camera velocity
			cameraVelocity *= 1f - (.95f * (float)gameTime.ElapsedGameTime.TotalSeconds);

			while (TouchPanel.IsGestureAvailable)
			{
				GestureSample gesture = TouchPanel.ReadGesture();

				// just move the Camera if we have a drag
				if (gesture.GestureType == GestureType.FreeDrag)
				{
					MoveCamera(-gesture.Delta);
				}

				// set our velocity if we see a flick
				else if (gesture.GestureType == GestureType.Flick)
				{
					cameraVelocity = -gesture.Delta;
				}
			}
#else

#endif

			base.Update(gameTime);
		}

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
		protected override void Draw(GameTime gameTime)
		{
            // Color of loading screen
            _graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
			base.Draw(gameTime);
		}
	}
}
