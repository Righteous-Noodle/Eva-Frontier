using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvaFrontier.Lib.Controls;
using EvaFrontier.Lib.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EvaFrontier.Lib.Buildings
{
    public class HeadQuarter : ResourceBuilding
    {
        #region Fields & Properties

        private Texture2D _airfieldTexture;
        private Texture2D _controlTowerTexture;

        public bool IsAirfieldBuilt { get; set; }
        public bool IsControlTowerBuilt { get; set; }

        #endregion

        #region Constructors

        public HeadQuarter(Vector2 position, string assetName, int range, int food, int maxFood, int medicine, int maxMedicine) :
            base(position, assetName, range, food, maxFood, medicine, maxMedicine)
        {
            Food = food;
            MaxFood = maxFood;
            Medicine = medicine;
            MaxMedicine = maxMedicine;
        }

        #endregion

        #region Methods

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            _airfieldTexture = content.Load<Texture2D>(@"Textures\Buildings\Airfield");
            _controlTowerTexture = content.Load<Texture2D>(@"Textures\Buildings\ControlTower");

            Controls.Add(new Image("Portrait",
                @"Textures\HUD\Portraits\" + GetType().Name,
                new Vector2(176, 519), new Vector2(147, 147), content));

            Controls.Add(new Button("DeployCar", new Vector2(875, 525), new Vector2(60, 60), content));
            Controls.Add(new Button("DeployTruck", new Vector2(935, 525), new Vector2(60, 60), content));
            Controls.Add(new Button("BuildAirfield", new Vector2(875, 580), new Vector2(60, 60), content));
            Controls.Add(new Button("DeployUAV", new Vector2(935, 580), new Vector2(60, 60), content));
            Controls.Add(new Button("BuildControlTower", new Vector2(875, 635), new Vector2(60, 60), content));
            Controls.Add(new Button("DeployOsprey", new Vector2(935, 635), new Vector2(60, 60), content));
        
        }

        public void HandleBuildCommand(Control control, ResourcesManager resourcesManager) {
            switch (control.Name) {
                case "BuildAirfield":
                    if (resourcesManager.Money >= Settings.AirfieldCost) {
                        IsAirfieldBuilt = true;
                        resourcesManager.Money -= Settings.AirfieldCost;
                        control.IsEnabled = false;
                    }
                    control.IsSelected = false;
                    break;
                case "BuildControlTower":
                    if (resourcesManager.Money >= Settings.ControlTowerCost) {
                        IsControlTowerBuilt = true;
                        resourcesManager.Money -= Settings.ControlTowerCost;
                        control.IsEnabled = false;
                    }
                    control.IsSelected = false;
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAirfieldBuilt) {
                Vector2 airfieldPosition = new Vector2(Position.X - 60, Position.Y + 105);
                spriteBatch.Draw(_airfieldTexture, airfieldPosition, Color.White);
            }
            if (IsControlTowerBuilt) {
                Vector2 controlTowerPosition = new Vector2(Position.X + 10, Position.Y + 10);
                spriteBatch.Draw(_controlTowerTexture, controlTowerPosition, Color.White);
            }

            base.Draw(spriteBatch);
        }
      
        #endregion
    }
}
