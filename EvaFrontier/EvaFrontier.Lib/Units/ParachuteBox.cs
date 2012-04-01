using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvaFrontier.Lib.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EvaFrontier.Lib.Units
{
    public class ParachuteBox : AnimatedSprite
    {

        #region Fields and Properties

        public bool CanDrop { get; set; }
        public bool IsDropping { get; set; }
        public int Altitude { get; set; }
        public int Travel { get; set; }

        public Color[] TextureData { get; set; }
        #endregion

        #region Constructors

        public ParachuteBox(): base(new Vector2(120f, 135f)) {
            AssetName = @"Textures\Effects\ParachuteBox";
            FrameLength = 0.01f;
            IsAnimating = true;
            CanDrop = true;
            IsDropping = false;
            Altitude = 360;
            Travel = 0;
        }

        #endregion

        #region Methods

        public override void Update(GameTime gameTime) {
            if (!IsAnimating) return;

            if (IsDropping) {
                if (totalFrames == -1) totalFrames = SpritesPerRow * SpritesPerColumn;

                if (Travel <= Altitude)
                {
                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (timer >= FrameLength)
                    {
                        timer = 0f;
                        currentFrame++;
                        Position += new Vector2(0, 10);
                        Travel += 10;
                        if (currentFrame == totalFrames)
                            currentFrame--;
                    }
                }
               
                else
                {
                    CanDrop = true;
                    IsDropping = false;
                    currentFrame = 0;
                    Travel = 0;
                }
            }
        }

        public void HandleInput(Vector2 unitPosition)
        {
            if (!IsDropping && CanDrop)
            {
                Position = unitPosition;
                IsDropping = true;
            }
        }

        #endregion

    }
}
