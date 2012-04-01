using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvaFrontier.Lib.Controls;
using EvaFrontier.Lib.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EvaFrontier.Lib.Units
{
    public class Osprey : AerialUnit
    {
        #region Fields
 
        #endregion
 
        #region Properties
 
        #endregion
 
        #region Constructor

        public Osprey(Vector2 startLocation, int engineLevel, Vector2 frameSize, int framesPerDirection)
        {
            AssetName = @"Textures\Units\Osprey";
            Position = startLocation;
            FrameSize = frameSize;
            FrameLength = 0.008f;
            //Origin = frameSize / 2;
            MaxFramesPerDirection = framesPerDirection;
            Direction = Direction.South;
            Destination = Position;
            MovementTolerance = 8f;

            PurchasingCost = 650;
            Speeds = Settings.OspreySpeeds;
            MaxSpeed = Speeds[engineLevel];
            Capacity = 35;
            EngineLevel = engineLevel;
            EngineTypes = Settings.OspreyEngineTypes;
            EnergyCosts = Settings.OspreyEnergyCosts;
            EnergyUpgradeCosts = Settings.OspreyEnergyUpgradeCosts;
        }
 
        #endregion

        #region Methods

        #endregion
    }
}
