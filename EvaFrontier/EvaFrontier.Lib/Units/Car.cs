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
    public class Car : Unit
    {
        #region Fields
 
        #endregion
 
        #region Properties
 
        #endregion
 
        #region Constructor
 
        public Car(Vector2 startLocation, int engineLevel, Vector2 frameSize, int framesPerDirection)
        {
            AssetName = @"Textures\Units\Car";
            Position = startLocation;
            FrameSize = frameSize;
            //Origin = frameSize / 2;
            MaxFramesPerDirection = framesPerDirection;
            //Scale = new Vector2(0.5f, 0.5f);
            Direction = Direction.South;
            Destination = Position;
            MovementTolerance = 8f;

            PurchasingCost = Settings.CarCost;
            Speeds = Settings.CarSpeeds;
            MaxSpeed = Speeds[engineLevel];
            Capacity = Settings.CarCapacity;
            InMobility = new List<string>()
            {           
                "Mountain",
                "Water"                
            };
            EngineLevel = engineLevel;
            EngineTypes = Settings.CarEngineTypes;
            EnergyCosts = Settings.CarEnergyCosts;
            EnergyUpgradeCosts = Settings.CarEnergyUpgradeCosts;
        }
 
        #endregion
 
        #region Methods

        
        #endregion

    }
}
