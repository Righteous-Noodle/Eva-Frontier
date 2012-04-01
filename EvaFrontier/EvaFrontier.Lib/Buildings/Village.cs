using System;
using System.Collections.Generic;
using EvaFrontier.Lib.Controls;
using EvaFrontier.Lib.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EvaFrontier.Lib.Buildings
{
    public class Village : ResourceBuilding
    {
        #region Constants

        private const int MaxPopulation = 500;
        private const float Interval = 5;
        private const float FoodConsumptionRate = 0.2f;
        private const float MedicineConsumptionRate = 0.5f;
        private int _populationHungerDropRate = 10;
        private const int PopulationDiseaseDropRate = 40;
        #endregion

        #region Fields & Properties

        private int _population;
        private float _timer;

        private readonly Texture2D[] _moreTextures = new Texture2D[5];
        private readonly Vector2[] _morePosition = new Vector2[5];
        private Texture2D _farmTexture;

        private Texture2D _deadTexture;
        private Texture2D _deadSymbol;

        public bool IsFarmBuilt { get; set;}

        private float DiseasePercentage {
            get { return (float)(_population / (float) MaxPopulation * 0.1); }
        }

        private bool _hasDisease;

        public bool IsDead {
            get { IsAlive = (_population > 0); return !IsAlive; }
        }

        #endregion

        #region Constructors

        public Village(Vector2 position, int food, int maxFood, int medicine, int maxMedicine, int population) :
            base(position, @"Textures\Buildings\MainHut", 300, food, maxFood, medicine, maxMedicine)
        {
            Food = food;
            MaxFood = maxFood;
            Medicine = medicine;
            MaxMedicine = maxMedicine;

            _population = population;

            _morePosition[0] = new Vector2(-80, 45);   //left
            _morePosition[1] = new Vector2(95, 60);     //right
            _morePosition[2] = new Vector2(-25, 65);
            _morePosition[3] = new Vector2(145, 70);
            _morePosition[4] = new Vector2(-55, 62);
        }

        #endregion

        #region Methods

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            for (int i = 0; i < 5; i++)
                _moreTextures[i] = content.Load<Texture2D>(@"Textures\Buildings\House" + (i + 1));
            
            _deadTexture = content.Load<Texture2D>(@"Textures\Buildings\DeadHut");
            _deadSymbol = content.Load<Texture2D>(@"Textures\Effects\Disease");
            
            SpriteFont resourceCountFont = content.Load<SpriteFont>(@"Fonts\ResourceCount");

            Controls.Add(new Image("Portrait",
                @"Textures\HUD\Portraits\" + GetType().Name,
                new Vector2(176, 519), new Vector2(147, 147), content));

            // Stat
            Label lblStatText = new Label(resourceCountFont, "StatLabelText");
            lblStatText.Text = "STAT";
            lblStatText.Color = Color.Black;
            lblStatText.Size = resourceCountFont.MeasureString(lblStatText.Text);
            lblStatText.Position = new Vector2(500, 555);
            Controls.Add(lblStatText);

            Controls.Add(new Image("Population",
                @"Textures\Buttons\Population",
                new Vector2(495, 585), new Vector2(60, 60), content));

            Label lblPopulationValue = new Label(resourceCountFont, "PopulationLabelValue");
            lblPopulationValue.Text = Food.ToString();
            lblPopulationValue.Color = Color.Black;
            lblPopulationValue.Size = resourceCountFont.MeasureString(lblPopulationValue.Text);
            lblPopulationValue.Position = new Vector2(575, 594);
            Controls.Add(lblPopulationValue);

            _farmTexture = content.Load<Texture2D>(@"Textures\Buildings\Farm");

            Controls.Add(new Button("BuildFarm", new Vector2(875, 525), new Vector2(60, 60), content));
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsDead)
                Texture = _deadTexture;

            base.Draw(spriteBatch);

            if (IsVisible && Texture != null)
            {
                Color tint = new Color(Tint.R, Tint.G, Tint.B, Convert.ToByte(Opacity * Tint.A));
                for (int i = 0; i < 5; i++)
                    if (_population >= ( MaxPopulation / 5 * (i + 1)) )
                        spriteBatch.Draw(_moreTextures[i], Position + _morePosition[i], SourceRectangle, tint, MathHelper.ToRadians(RotationAngle), Origin, Scale, Orientation, LayerDepth);
            }

            if (_hasDisease) {
                spriteBatch.Draw(_deadSymbol, new Vector2(Position.X + Texture.Width/2, Position.Y - 40), Color.White);
            }
            if (IsFarmBuilt) {
                Vector2 farmPosition = new Vector2(Position.X - 60, Position.Y + 105);
                spriteBatch.Draw(_farmTexture, farmPosition, Color.White);
            }
        }
        public override void Update(GameTime gameTime, List<Sprite> worldSprites) {
                       if (IsDead) {
                _population = 0;
                return;
            }
            base.Update(gameTime, worldSprites);

            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer>= Interval) {
                int consumedFood = (int)(_population*FoodConsumptionRate);

                if (Food > consumedFood) {
                    Food -= consumedFood;
                } else {
                    _population -= _populationHungerDropRate; 
                }

                if (_hasDisease == false &&
                    ((_random.NextDouble() * double.MaxValue) % 100) <= DiseasePercentage) {
                    _hasDisease = true;
                    int consumedMedicine = (int)(_population * MedicineConsumptionRate);

                    if (Medicine > consumedMedicine) {
                        Medicine -= consumedMedicine;
                        _hasDisease = false;
                    }
                    else {
                        _population -= PopulationDiseaseDropRate;
                    }
                }

                _timer = 0;
            }

            Controls.Find(c => c.Name == "PopulationLabelValue").Text = _population.ToString();

            
        }

        public void HandleBuildFarm(Control control, ResourcesManager resourcesManager) {
            if (resourcesManager.Money >= Settings.FarmCost) {
                IsFarmBuilt = true;
                resourcesManager.Money -= Settings.FarmCost;
                control.IsEnabled = false;
                _populationHungerDropRate = 0;
            }
            control.IsSelected = false;
        }

        protected override void DrawBars(SpriteBatch spriteBatch) {
            
            if (IsSelected || HasUnitInRange) {
                spriteBatch.Draw(HealthBar, new Rectangle((int) Position.X, (int) Position.Y,
                                                          Bounds.Width, 10), new Rectangle(0, 45, Bounds.Width, 20),
                                 Color.Gray);

                spriteBatch.Draw(HealthBar, new Rectangle((int)Position.X, (int)Position.Y,
                    (int)(Bounds.Width * ((double)_population / MaxPopulation)), 10), new Rectangle(0, 45, Bounds.Width, 10), Color.LimeGreen);
            }
            base.DrawBars(spriteBatch);
        }

        #endregion
    }
}
