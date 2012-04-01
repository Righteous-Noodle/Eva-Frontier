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


namespace EvaFrontier.Lib
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ResourcesManager 
    {
        #region Fields & Properties
        private ResourceBuilding _headquarter;

        public int Money {get;set;}
        public int Energy { get; set; }
        public int MaxMoney { get; set; }
        public int MaxEnergy { get; set; }
        public int MoneyIncome { get; set; }
        public int FoodRate { get; set; }
        public int MedicineRate { get; set; }

        protected float timer = 0;
        public float Period { get; set; }
        public ControlManager Controls;
        #endregion
        

        public ResourcesManager(ResourceBuilding headquarter)
        {
            Money = Settings.StartingMoneyAmount;
            MaxMoney = Settings.MaxMoneyAllowed;
            MoneyIncome = Settings.MoneyIncome;
            FoodRate = Settings.FoodIncome;
            MedicineRate = Settings.MedicineIncome;
            MaxEnergy = 20;
            Period = (float)5f;
            Controls = new ControlManager();
            _headquarter = headquarter;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timer>= Period)
            {
                Money = Math.Min(MaxMoney, Money + (int)(MoneyIncome / (60.0 / Period)));
                _headquarter.Food = Math.Min(_headquarter.MaxFood, _headquarter.Food + (int)(FoodRate / (60.0 / Period)));
                _headquarter.Medicine = Math.Min(_headquarter.MaxMedicine, _headquarter.Medicine + (int)(MedicineRate / (60.0 / Period)));

                timer = 0;
            }
            Controls[0].Text = Money.ToString();
            Controls[1].Text = _headquarter.Food.ToString();
            Controls[2].Text = _headquarter.Medicine.ToString();
            Controls[3].Text = Energy.ToString() + " / " + MaxEnergy.ToString();
        }

        public void LoadContent(ContentManager content)
        {
            SpriteFont resourceCountFont = content.Load<SpriteFont>(@"Fonts\ResourceCount");

            Label lblMoney = new Label(resourceCountFont, "");
            lblMoney.Text = Money.ToString();
            lblMoney.Color = Color.Black;
            lblMoney.Size = resourceCountFont.MeasureString(lblMoney.Text);
            lblMoney.Position = new Vector2(65, 530);
            Controls.Add(lblMoney);

            Label lblFood = new Label(resourceCountFont, "");
            lblFood.Text = _headquarter.Food.ToString();
            lblFood.Color = Color.Black;
            lblFood.Size = resourceCountFont.MeasureString(lblFood.Text);
            lblFood.Position = new Vector2(65, 570);
            Controls.Add(lblFood);

            Label lblMedicine = new Label(resourceCountFont, "");
            lblMedicine.Text = _headquarter.Medicine.ToString();
            lblMedicine.Color = Color.Black;
            lblMedicine.Size = resourceCountFont.MeasureString(lblFood.Text);
            lblMedicine.Position = new Vector2(65, 610);
            Controls.Add(lblMedicine);

            Label lblEnergy = new Label(resourceCountFont, "");
            lblEnergy.Text = Energy.ToString() + " / " + MaxEnergy.ToString();
            lblEnergy.Color = Color.Black;
            lblEnergy.Size = resourceCountFont.MeasureString(lblEnergy.Text);
            lblEnergy.Position = new Vector2(65, 655);
            Controls.Add(lblEnergy);

            Label lblMoneyRate = new Label(resourceCountFont, "");
            lblMoneyRate.Text = "+" + MoneyIncome.ToString();
            lblMoneyRate.Color = Color.Green;
            lblMoneyRate.Size = resourceCountFont.MeasureString(lblMoneyRate.Text);
            lblMoneyRate.Position = new Vector2(116, 530);
            Controls.Add(lblMoneyRate);

            Label lblFoodRate = new Label(resourceCountFont, "");
            lblFoodRate.Text = "+" + FoodRate.ToString();
            lblFoodRate.Color = Color.Green;
            lblFoodRate.Size = resourceCountFont.MeasureString(lblFoodRate.Text);
            lblFoodRate.Position = new Vector2(116, 570);
            Controls.Add(lblFoodRate);

            Label lblMedicineRate = new Label(resourceCountFont, "");
            lblMedicineRate.Text = "+" + MedicineRate.ToString();
            lblMedicineRate.Color = Color.Green;
            lblMedicineRate.Size = resourceCountFont.MeasureString(lblMedicineRate.Text);
            lblMedicineRate.Position = new Vector2(116, 610);
            Controls.Add(lblMedicineRate);
        }
    }
}