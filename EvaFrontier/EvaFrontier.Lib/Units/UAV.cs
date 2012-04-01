using System;
using System.Collections.Generic;
using System.Linq;
using EvaFrontier.Lib.Buildings;
using EvaFrontier.Lib.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace EvaFrontier.Lib.Units
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>

    public class UAV : AerialUnit
    {
        #region Fields

        private ParachuteBox _parachuteBox;

        #endregion

        #region Properties

        #endregion

        #region Constructor

        public UAV(Vector2 startLocation, Vector2 hqPosition, int engineLevel, Vector2 frameSize, int framesPerDirection)
        {
            AssetName = @"Textures\Units\UAV";
            HQ_Position = hqPosition;
            Position = startLocation;
            FrameSize = frameSize;
            FrameLength = 0.008f;
            //Origin = frameSize / 2;
            MaxFramesPerDirection = framesPerDirection;
            Direction = Direction.SouthWest;
            Destination = Position;
            MovementTolerance = 8f;

            PurchasingCost = Settings.UAVCost;
            Speeds = Settings.UAVSpeeds;
            MaxSpeed = Speeds[engineLevel];
            Capacity = Settings.UAVCapacity;
            EngineLevel = engineLevel;
            EngineTypes = Settings.UAVEngineTypes;
            EnergyCosts = Settings.UAVEnergyCosts;
            EnergyUpgradeCosts = Settings.UAVEnergyUpgradeCosts;

            _parachuteBox = new ParachuteBox();
        }

        #endregion

        #region Methods

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            _parachuteBox.LoadContent(content);
            _parachuteBox.TextureData = new Color[_parachuteBox.Texture.Width * _parachuteBox.Texture.Height];
            _parachuteBox.Texture.GetData(_parachuteBox.TextureData);
        }

        public override void Update(GameTime gameTime, string behavior, Map map)
        {
            _parachuteBox.Update(gameTime);
            base.Update(gameTime, behavior, map);
        }

        public override void HandleResource(InputState input, ResourceBuilding resourceBuilding)
        {
            Control control = Controls.Selection;
            if ((control != null && control.Name.Contains("Unload")) ||
                ((input.IsNewKeyPress(Keys.F, null)) ||
                    (input.IsNewKeyPress(Keys.C, null))))
            {
                _parachuteBox.HandleInput(Position);

                IsWithinTargetRange = false;
                if (_parachuteBox.Bounds.Intersects(resourceBuilding.Bounds)) {

                    /*Color[] buildingTextureData = new Color[resourceBuilding.Texture.Width * resourceBuilding.Texture.Height];
                    resourceBuilding.Texture.GetData(buildingTextureData);

                    if (IntersectPixels(_parachuteBox.Bounds,
                                        _parachuteBox.TextureData,
                                        resourceBuilding.Bounds,
                                        buildingTextureData)) */{
                        IsWithinTargetRange = true;
                    }
                }
            }

            base.HandleResource(input, resourceBuilding);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (_parachuteBox.IsDropping)
            {
                _parachuteBox.Draw(spriteBatch);
            }
        }

        public static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
                                           Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }

        #endregion
    }
}