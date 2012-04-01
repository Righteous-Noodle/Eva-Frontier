using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvaFrontier.Lib.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace EvaFrontier.Lib.Units
{
    public class Truck : Unit
    {
        #region Fields
 
        #endregion
 
        #region Properties
 
        #endregion
 
        #region Constructor
 
        public Truck(Vector2 startLocation, int engineLevel, Vector2 frameSize, int framesPerDirection)
        {
            AssetName = @"Textures\Units\Truck";
            Position = startLocation;
            FrameSize = frameSize;
            //Origin = frameSize / 2;
            MaxFramesPerDirection = framesPerDirection;
            //Scale = new Vector2(0.5f, 0.5f);
            Direction = Direction.South;
            Destination = Position;
            MovementTolerance = 8f;

            PurchasingCost = Settings.TruckCost;
            Speeds = Settings.TruckSpeeds;
            MaxSpeed = Speeds[engineLevel];
            Capacity = 6;
            InMobility = new List<string>()
            {
                "Mountain",
                "Water"                
            };
            EngineLevel = engineLevel;
            EngineTypes = Settings.TruckEngineTypes;
            EnergyCosts = Settings.TruckEnergyCosts;
            EnergyUpgradeCosts = Settings.TruckEnergyUpgradeCosts;

            Range = 300;
        }
 
        #endregion
 
        #region Methods

        public override void LoadContent(ContentManager content) {
            base.LoadContent(content);
            Controls.Add(new Button("BuildWindTurbine", new Vector2(875, 580), new Vector2(60, 60), content));
            Controls.Add(new Button("BuildSolarPanel", new Vector2(875, 640), new Vector2(60, 60), content));
        }

        protected override void DrawRangeCircle(SpriteBatch spriteBatch)
        {
            if (IsBuilding)
            {
                spriteBatch.Draw(RangeCircle, new Rectangle((int)(Position.X + FrameSize.X / 2 - Range),
                    (int)(Position.Y + FrameSize.Y / 2 - Range),
                Range * 2, Range * 2), null, new Color(Color.White, 0.3f), 0, Vector2.Zero, SpriteEffects.None, 0);
            }
        }


        #endregion

    }
}
