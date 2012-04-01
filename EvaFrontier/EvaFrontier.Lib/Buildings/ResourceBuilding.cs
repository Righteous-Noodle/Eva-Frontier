using System;
using System.Collections.Generic;
using EvaFrontier.Lib.Buildings;
using EvaFrontier.Lib.Controls;
using EvaFrontier.Lib.Units;
using Microsoft.Xna.Framework;
using EvaFrontier.Lib.Sprites;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace EvaFrontier.Lib.Buildings
{
    public class ResourceBuilding : StaticBuilding
    {
        #region Properties
        public int Food { get; set; }
        public int Medicine { get; set; }
        public int MaxFood { get; set; }
        public int MaxMedicine { get; set; }
        public bool IsAlive { get; set; }
        #endregion


        public ResourceBuilding(Vector2 position, string assetName, int range, int food, int maxFood, int medicine, int maxMedicine) : base(position, assetName, range)
        {
            Food = food;
            MaxFood = maxFood;
            Medicine = medicine;
            MaxMedicine = maxMedicine;
            IsAlive = true;
            Controls = new ControlManager();
        }

        public virtual void LoadContent(ContentManager content) {
            base.LoadContent(content);

            SpriteFont resourceCountFont = content.Load<SpriteFont>(@"Fonts\ResourceCount");

            /*Controls.Add(new Image("Portrait",
                @"Textures\HUD\Portraits\" + GetType().Name,
                new Vector2(176, 519), new Vector2(147, 147), content));*/

            Label lblInventoryText = new Label(resourceCountFont, "InventoryLabelText");
            lblInventoryText.Text = "INVENTORY";
            lblInventoryText.Color = Color.Black;
            lblInventoryText.Size = resourceCountFont.MeasureString(lblInventoryText.Text);
            lblInventoryText.Position = new Vector2(345, 555);
            Controls.Add(lblInventoryText);

            // Food
            Controls.Add(new Image("Food",
                @"Textures\Buttons\Food",
                new Vector2(340, 585), new Vector2(60, 60), content));

            Label lblFoodValue = new Label(resourceCountFont, "FoodLabelValue");
            lblFoodValue.Text = Food.ToString();
            lblFoodValue.Color = Color.Black;
            lblFoodValue.Size = resourceCountFont.MeasureString(lblFoodValue.Text);
            lblFoodValue.Position = new Vector2(415, 594);
            Controls.Add(lblFoodValue);

            // Medicine
            Controls.Add(new Image("Medicine",
                @"Textures\Buttons\Medicine",
                new Vector2(340, 647), new Vector2(60, 60), content));

            Label lblMedicineValue = new Label(resourceCountFont, "MedicineLabelValue");
            lblMedicineValue.Text = Food.ToString();
            lblMedicineValue.Color = Color.Black;
            lblMedicineValue.Size = resourceCountFont.MeasureString(lblMedicineValue.Text);
            lblMedicineValue.Position = new Vector2(415, 656);
            Controls.Add(lblMedicineValue);
        }

        public virtual void Update(GameTime gameTime, List<Sprite> worldSprites) {
            
            foreach (var sprite in worldSprites)
            {
                if (sprite is Unit && Vector2.Distance(Position, sprite.Position) < Range) {
                    HasUnitInRange = true;
                    break;
                }
                HasUnitInRange = false;
            }

            Controls.Find(c => c.Name == "FoodLabelValue").Text = Food.ToString();
            Controls.Find(c => c.Name == "MedicineLabelValue").Text = Medicine.ToString();
        }

        protected override void DrawBars(SpriteBatch spriteBatch)
        {
            if (IsSelected || HasUnitInRange)
            {
                spriteBatch.Draw(HealthBar, new Rectangle((int)Position.X, (int)Position.Y+10,
                    Bounds.Width, 7), new Rectangle(0, 45, Bounds.Width, 7), Color.Gray);
                spriteBatch.Draw(HealthBar, new Rectangle((int)Position.X, (int)Position.Y + 17,
                    Bounds.Width, 7), new Rectangle(0, 45, Bounds.Width, 7), Color.Gray);

                spriteBatch.Draw(HealthBar, new Rectangle((int)Position.X, (int)Position.Y+10,
                    (int)(Bounds.Width * ((double)Food / MaxFood)), 7), new Rectangle(0, 45, Bounds.Width, 7), Color.Orange);
                spriteBatch.Draw(HealthBar, new Rectangle((int)Position.X, (int)Position.Y + 17,
                    (int)(Bounds.Width * ((double)Medicine / MaxMedicine)), 7), new Rectangle(0, 45, Bounds.Width, 7), Color.Red);
            }
        }
    }
}