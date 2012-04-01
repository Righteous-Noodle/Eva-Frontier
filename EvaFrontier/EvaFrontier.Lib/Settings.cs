using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvaFrontier.Lib
{
    public static class Settings
    {
        public static Dictionary<int, int> ScreenResolutions = new Dictionary<int, int>()
        {
            {1280, 720},
            {1024, 768},
            {800, 600}
        };

        public static KeyValuePair<int, int> ScreenResolution = 
            new KeyValuePair<int,int>(1280, 720);
        public static float MainVolume = 0.75f;
        public static float SFXVolume = 0.75f;
        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;
        public static bool IsFullScreen = false;

        public const int StartingMoneyAmount = 2000;
        public const int MaxMoneyAllowed = 9999;
        public const int MoneyIncome = 150;
        public const int FoodIncome = 530;
        public const int MedicineIncome = 120;

        public const int CarCost = 100;
        public static readonly List<string> CarEngineTypes = new List<string>()
            { "Gasoline", "Electric", "Solar" };
        public static readonly List<int> CarEnergyCosts = new List<int>()
            { 4, 2, 0};
        public static readonly List<int> CarEnergyUpgradeCosts = new List<int>() 
            { 0, 40, 60 };
        public static readonly List<int> CarSpeeds = new List<int>() 
            { 8, 5, 0 };
        public const int CarCapacity = 4;

        public const int TruckCost = 220;
        public static readonly List<string> TruckEngineTypes = new List<string>()
            { "Gasoline", "Electric", "Solar" };
        public static readonly List<int> TruckEnergyCosts = new List<int>() 
            { 8, 6, 0 };
        public static readonly List<int> TruckEnergyUpgradeCosts = new List<int>() 
            { 0, 70, 60 };
        public static readonly List<int> TruckSpeeds = new List<int>() 
            { 6, 4, 2 };
        public const int TruckCapacity = 8;

        public const int UAVCost = 270;
        public static readonly List<string> UAVEngineTypes = new List<string>() 
            { "Gasoline", "Electric" };
        public static readonly List<int> UAVEnergyCosts = new List<int>()
            { 12, 8 };
        public static readonly List<int> UAVEnergyUpgradeCosts = new List<int>() 
            { 0, 50 };
        public static readonly List<int> UAVSpeeds = new List<int>() 
            { 20, 14 };
        public const int UAVCapacity = 4;

        public const int OspreyCost = 650;
        public static readonly List<string> OspreyEngineTypes = new List<string>() 
            { "Gasoline"};
        public static readonly List<int> OspreyEnergyCosts = new List<int>() 
            { 10 };
        public static readonly List<int> OspreyEnergyUpgradeCosts = new List<int>() 
            { 0 };
        public static readonly List<int> OspreySpeeds = new List<int>()
            { 11 };
        public const int OspreyCapacity = 12;

        public const int SolarPanelCost = 120;
        public const int SolarPanelEnergy = 5;
        public const int WindTurbineCost = 180;
        public const int WindTurbineEnergy = 10;
        public const int AirfieldCost = 200;
        public const int ControlTowerCost = 100;
        public const int FarmCost = 250;
    }
}
