using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvaFrontier.Lib.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EvaFrontier.Lib.Controls
{
    public class Button : Control
    {
        #region Fields and Properties

        protected Texture2D _texture;
        private SpriteFont _font;

        public Texture2D Texture;

        #endregion

        #region Constructors

        public Button(string name, Vector2 position, Vector2 size, ContentManager content) {
            Name = name;
            Position = position;
            Size = size;
            IsTabStop = false;
            IsEnabled = true;
            IsVisible = true;
            Color = Color.White;

            Texture = content.Load<Texture2D>(@"Textures\Buttons\" + name);
            _font = content.Load<SpriteFont>(@"Fonts\ButtonHoverText");
        }

        #endregion

        #region Methods

        public override void Update(GameTime gameTime) {
            
        }

        public override void HandleInput(InputState input) {
            Vector2 mouse = new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y);
            Rectangle mouseRec = new Rectangle((int)mouse.X, (int)mouse.Y, 1, 1);
            Rectangle bounds = new Rectangle((int)Position.X, (int)Position.Y,
                Texture.Width, Texture.Height);

            if (IsEnabled) {
                IsMouseOver = bounds.Contains(mouseRec);
            
                if (IsMouseOver && input.IsNewLeftMouseClick()) {
                    IsSelected = true;
                }

                if (IsSelected && !IsMouseOver) {
                    if (input.IsNewRightMouseClick()) {
                        IsSelected = false;
                    }
                }
            }    
        }

        public override void Draw(SpriteBatch spriteBatch) {
            if (IsEnabled) {
                Color = (IsSelected || IsMouseOver) ? Color.Lime : Color.White;
                if (IsMouseOver) {
                    string toolTip = Name;

                    _DrawHoverText(spriteBatch, Name, new Vector2(Position.X, Position.Y - 50));
                    if (Name == "DeployCar") {
                        //toolTip += "\nCost: " + Settings.CarCost +
                        //           "\nCapacity " + Settings.CarCapacity;
                        _DrawHoverText(spriteBatch, "Cost: " + Settings.CarCost, new Vector2(Position.X, Position.Y - 40));
                        _DrawHoverText(spriteBatch, "Capacity: " + Settings.CarCapacity, new Vector2(Position.X, Position.Y - 30));
                    } else if (Name == "DeployTruck") {
                        //toolTip += "\nCost: " + Settings.TruckCost +
                        //           "\nCapacity " + Settings.TruckCapacity;
                        _DrawHoverText(spriteBatch, "Cost: " + Settings.TruckCost, new Vector2(Position.X, Position.Y - 40));
                        _DrawHoverText(spriteBatch, "Capacity: " + Settings.TruckCapacity, new Vector2(Position.X, Position.Y - 30));
                    } else if (Name == "DeployUAV") {
                        //toolTip += "\nCost: " + Settings.UAVCost +
                        //    "\nCapacity " + Settings.UAVCapacity;
                        _DrawHoverText(spriteBatch, "Cost: " + Settings.UAVCost, new Vector2(Position.X, Position.Y - 40));
                        _DrawHoverText(spriteBatch, "Capacity: " + Settings.UAVCapacity, new Vector2(Position.X, Position.Y - 30));
                    } else if (Name == "DeployOsprey"){
                        //toolTip += "\nCost: " + Settings.OspreyCost +
                        //    "\nCapacity " + Settings.OspreyCapacity;
                        _DrawHoverText(spriteBatch, "Cost: " + Settings.OspreyCost, new Vector2(Position.X, Position.Y - 40));
                        _DrawHoverText(spriteBatch, "Capacity: " + Settings.OspreyCapacity, new Vector2(Position.X, Position.Y - 30));
                    } else if (Name == "BuildAirfield") {
                        //toolTip += "\nCost: " + Settings.AirfieldCost +
                        //           "\nAllows to deploy UAV";
                        _DrawHoverText(spriteBatch, "Cost: " + Settings.AirfieldCost, new Vector2(Position.X, Position.Y - 40));
                        _DrawHoverText(spriteBatch, "Allows to deploy UAV", new Vector2(Position.X, Position.Y - 30));
                    } else if (Name == "BuildControlTower") {
                        //toolTip += "\nCost: " + Settings.ControlTowerCost +
                        //           "\nAllows to control Osprey";
                        _DrawHoverText(spriteBatch, "Cost: " + Settings.ControlTowerCost, new Vector2(Position.X, Position.Y - 40));
                        _DrawHoverText(spriteBatch, "Allows to control Osprey", new Vector2(Position.X, Position.Y - 30));
                    }

                    //_DrawHoverText(spriteBatch, toolTip, new Vector2(Position.X, Position.Y - 40));
                }
            }
            else {
                float alpha = .50f;
                //Color = new Color(255, 255, 255, 255 * (byte)alpha);
                Color = Color.DarkBlue;
            }
            spriteBatch.Draw(Texture, new Rectangle((int) Position.X, (int) Position.Y, (int) Size.X, (int) Size.Y), Color);
        }

        private void _DrawHoverText(SpriteBatch spriteBatch, string text, Vector2 position) {
            spriteBatch.DrawString(_font, text, position, Color.Black);
        }

        #endregion
    }
}
