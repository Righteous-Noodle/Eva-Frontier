using System;
using System.Collections.Generic;
using System.Reflection;
using EvaFrontier.Lib.Buildings;
using EvaFrontier.Lib.Controls;
using EvaFrontier.Lib.PathFinding;
using Microsoft.Xna.Framework;
using EvaFrontier.Lib.Sprites;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EvaFrontier.Lib.Units
{
    public enum Direction : int
    {
        South = 0,
        SouthWest = 1,
        West = 2,
        NorthWest = 3,
        North = 4,
        NorthEast = 5,
        East = 6,
        SouthEast = 7,
    }

    public abstract class Unit : AnimatedSprite
    {
        #region Constants

        /// <summary>
        /// This is how much the Unit can turn in one second in radians, since Pi 
        /// radians makes half a circle the tank can all the way around in one second
        /// </summary>
        public static float MaxAngularVelocity
        {
            get { return maxAngularVelocity; }
        }
        const float maxAngularVelocity = MathHelper.Pi;

        /// <summary>
        /// This is the Unit’s best possible movement speed
        /// </summary>
        public float MaxSpeed
        {
            get { return maxSpeed; }
            set { maxSpeed = value; }
        }
        float maxSpeed;

        /// <summary>
        /// This is most the Unit can speed up or slow down in one second
        /// </summary>
        public float MaxSpeedDelta
        {
            get { return maxSpeed / 2; }
        }

        public const int AmountPerCargo = 50;

        #endregion

        #region Fields

        private List<Texture2D> _energyTextures;
        private bool _wasCtrlPressed;

        private SoundEffect _sfx;
        private SoundEffectInstance _sfxInstance;
        private SoundEffect _sfxLoading;
        private SoundEffect _sfxUnloading;

        #endregion

        #region Properties

        public SoundEffectInstance SfxInstance {
            get { return _sfxInstance; }
            set { _sfxInstance = value; }
        }

        public int MaxFramesPerDirection { get; set; }
        public Vector2 TilePosition { get; set; }
        public Direction Direction { get; set; }
        public float Speed { get; set; }

        // Unit's specific variables
        public int PurchasingCost;
        public int UpgradingCost;
        public int Capacity { get; set; }
        public int FoodCargo { get; set; }
        public int MedicineCargo { get; set; }
        protected bool IsWithinTargetRange { get; set; }
        public List<string> InMobility;
        public List<string> EngineTypes;
        public List<int> EnergyCosts { get; set;}
        public List<int> EnergyUpgradeCosts { get; set; }
        public int EngineLevel { get; set;}
        public List<int> Speeds { get; set; }
        public string Description { get; set;}
        public bool IsBuilding { get; set; }

        // Movement variables
        public Vector2 LastMovement { get; set; }
        public Vector2 Destination { get; set; }
        public Vector2 HQ_Position { get; set; }
        public Vector2 MovingDirection { get; set; }
        public float MovementTolerance { get; set; }
        public Direction PreviousDirection { get; set; }
        public bool IsAtDestination
        {
            get { return Vector2.Distance(Position, Destination) < MovementTolerance; }
        }

        private WaypointList _waypoints;
        public WaypointList Waypoints
        {
            get { return _waypoints; }
        }

        #endregion

        #region Constructor

        public Unit() {
            Speed = MaxSpeed;
            IsAnimating = false;
            PreviousDirection = Direction.North;
            IsWithinTargetRange = true;

            Controls = new ControlManager();

            _waypoints = new WaypointList();

            _energyTextures = new List<Texture2D>();
        }

        public Unit(Vector2 frameSize, int framesPerDirection) {
            FrameSize = frameSize;
            FrameLength = 0.008f;
            //Origin = frameSize / 2;
            MaxFramesPerDirection = framesPerDirection;
            Speed = MaxSpeed;
            IsAnimating = false;
            PreviousDirection = Direction.North;
        }

        #endregion

        #region Methods

        public virtual void LoadContent(ContentManager content) {
            base.LoadContent(content);

            _sfx = content.Load<SoundEffect>(@"SFX\" + GetType().Name);
            _sfxInstance = _sfx.CreateInstance();
            _sfxInstance.Volume = Settings.SFXVolume;

            _sfxLoading = content.Load<SoundEffect>(@"SFX\load");
            _sfxUnloading = content.Load<SoundEffect>(@"SFX\unload");

            _waypoints.LoadContent(content);

            SpriteFont resourceCountFont = content.Load<SpriteFont>(@"Fonts\ResourceCount");

            Controls.Add(new Image("Portrait",
                @"Textures\HUD\Portraits\" + GetType().Name,
                new Vector2(176, 519), new Vector2(147, 147), content));

            Label lblCargoText = new Label(resourceCountFont, "CargoLabelText");
            lblCargoText.Text = "CARGO";
            lblCargoText.Color = Color.Black;
            lblCargoText.Size = resourceCountFont.MeasureString(lblCargoText.Text);
            lblCargoText.Position = new Vector2(345, 555);
            Controls.Add(lblCargoText);

            // Food
            Controls.Add(new Image("FoodCargo",
                @"Textures\Buttons\FoodCargo",
                new Vector2(340, 585), new Vector2(60, 60), content));

            Label lblFoodCargoValue = new Label(resourceCountFont, "FoodCargoLabelValue");
            lblFoodCargoValue.Text = FoodCargo.ToString();
            lblFoodCargoValue.Size = resourceCountFont.MeasureString(lblFoodCargoValue.Text);
            lblFoodCargoValue.Position = new Vector2(377, 617);
            Controls.Add(lblFoodCargoValue);

            // Medicine
            Controls.Add(new Image("MedicineCargo",
                @"Textures\Buttons\MedicineCargo",
                new Vector2(405, 585), new Vector2(60, 60), content));

            Label lblMedicineCargo = new Label(resourceCountFont, "MedicineCargoLabel");
            lblMedicineCargo.Text = MedicineCargo.ToString();
            lblMedicineCargo.Size = resourceCountFont.MeasureString(lblMedicineCargo.Text);
            lblMedicineCargo.Position = new Vector2(442, 617);
            Controls.Add(lblMedicineCargo);

            Label lblEnergySourceText = new Label(resourceCountFont, "EnergySourceLabelText");
            lblEnergySourceText.Text = "ENERGY SOURCE";
            lblEnergySourceText.Color = Color.Black;
            lblEnergySourceText.Size = resourceCountFont.MeasureString(lblEnergySourceText.Text);
            lblEnergySourceText.Position = new Vector2(505, 555);
            Controls.Add(lblEnergySourceText);

            // Engine
            _energyTextures.Add(content.Load<Texture2D>(@"Textures\Buttons\Gasoline"));
            _energyTextures.Add(content.Load<Texture2D>(@"Textures\Buttons\Electric"));
            _energyTextures.Add(content.Load<Texture2D>(@"Textures\Buttons\Solar"));

            Image image = new Image("Energy", new Vector2(500, 585), new Vector2(60, 60));
            image.Texture = _energyTextures[EngineLevel];
            Controls.Add(image);

            Label lblEnergyCost = new Label(resourceCountFont, "EnergyCostLabel");
            lblEnergyCost.Text = EnergyCosts[EngineLevel].ToString();
            lblEnergyCost.Color = Color.Red;
            lblEnergyCost.Size = resourceCountFont.MeasureString(lblEnergyCost.Text);
            lblEnergyCost.Position = new Vector2(580, 600);
            Controls.Add(lblEnergyCost);

            Controls.Add(new Button("Move", new Vector2(875, 525), new Vector2(60, 60), content));
            Controls.Add(new Button("LoadFood", new Vector2(935, 525), new Vector2(60, 60), content));
            Controls.Add(new Button("LoadMedicine", new Vector2(995, 525), new Vector2(60, 60), content));
            Controls.Add(new Button("UnloadFood", new Vector2(935, 580), new Vector2(60, 60), content));
            Controls.Add(new Button("UnloadMedicine", new Vector2(995, 580), new Vector2(60, 60), content));

            Button electricButton = new Button("Electric", new Vector2(935, 640), new Vector2(60, 60), content);
            electricButton.IsEnabled = !(EngineLevel > 0);
            Controls.Add(electricButton);

            Button solarButton = new Button("Solar", new Vector2(995, 640), new Vector2(60, 60), content);
            solarButton.IsEnabled = !(EngineLevel > 1);
            Controls.Add(solarButton);
        }

        public virtual void Update(GameTime gameTime, string behavior, Map map)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_waypoints.Count >= 1) {
                Vector2 mouse = _waypoints.Peek();
                Destination = new Vector2(mouse.X - Bounds.Width / 2,
                                        mouse.Y - Bounds.Height / 2);
            }

            if (IsAtDestination && _waypoints.Count >= 1) {
                _waypoints.Dequeue();
            }

            Vector2 direction = Destination - Position;

            _Move(behavior, direction, map);

            //if (!IsAnimating) return;

            if (totalFrames == -1) totalFrames = SpritesPerRow * SpritesPerColumn;

            if (timer >= FrameLength)
            {
                timer = 0f;
                currentFrame = ((currentFrame + 1) % MaxFramesPerDirection) + ((int)Direction * MaxFramesPerDirection);
            }

            Controls.Find(c => c.Name == "FoodCargoLabelValue").Text = FoodCargo.ToString();
            Controls.Find(c => c.Name == "MedicineCargoLabel").Text = MedicineCargo.ToString();

            if (EngineLevel > 1) {
                Controls.Find(c => c.Name == "Solar").IsEnabled = false;
            } 
            if (EngineLevel > 0) {
                Controls.Find(c => c.Name == "Electric").IsEnabled = false;
            }
        }

        public void HandleWayPoint(InputState input, Vector2 worldMouse) {
            if ((Keyboard.GetState().IsKeyDown(Keys.LeftShift) ||
                Keyboard.GetState().IsKeyDown(Keys.RightShift)) 
                && input.IsNewRightMouseClick())
            {
                //unit.Waypoints.EnqueueWithCollisionChecking(unit.Position, Mouse, Sprites);
                Waypoints.Enqueue(worldMouse);
            }
            else if (input.IsNewRightMouseClick())
            {
                Waypoints.Clear();
                Waypoints.Enqueue(worldMouse);
            }
        }

        public virtual void HandleResource(InputState input, ResourceBuilding resourceBuilding)
        {
            Control control = Controls.Selection;
            if ((control != null && control.Name == "LoadFood") ||
                (input.CurrentKeyboardStates[1].IsKeyDown(Keys.LeftControl) &&
                    input.IsNewKeyPress(Keys.F, null))) 
            {
                LoadResource("Food", resourceBuilding);
                _sfxLoading.Play();
            }
            else if ((control != null && control.Name == "LoadMedicine") ||
                (input.CurrentKeyboardStates[1].IsKeyDown(Keys.LeftControl) &&
                   input.IsNewKeyPress(Keys.C, null))) 
            {
                LoadResource("Medicine", resourceBuilding);
                _sfxLoading.Play();
            }
            else if ((control != null && control.Name == "UnloadFood") ||
                (input.IsNewKeyPress(Keys.F, null)))
            {
                UnloadResource("Food", resourceBuilding);
                _sfxUnloading.Play();
            }
            else if ((control != null && control.Name == "UnloadMedicine") ||
                (input.IsNewKeyPress(Keys.C, null)))
            {
                UnloadResource("Medicine", resourceBuilding);
                _sfxUnloading.Play();
            }

            if (control != null) control.IsSelected = false;
        }

        public void HandleEnergyUpgrade(int currentMoney) {
            Control control = Controls.Selection;

            if (control == null)
                return;

            if (control.Name == "Electric") {
                if (currentMoney > EnergyUpgradeCosts[1]) {
                    EngineLevel = 1;
                    Speed = Speeds[EngineLevel];
                    Image image = Controls.Find(c => c.Name == "Energy") as Image;
                    if (image != null) image.Texture = _energyTextures[EngineLevel];
                }
            }
            else if (control.Name == "Solar") {
                if (currentMoney > EnergyUpgradeCosts[2]) {
                    EngineLevel = 2;
                    Speed = Speeds[EngineLevel];
                    Image image = Controls.Find(c => c.Name == "Energy") as Image;
                    if (image != null) image.Texture = _energyTextures[EngineLevel];
                }
            }
        }

        public virtual void LoadResource(string resourceType, ResourceBuilding resourceBuilding) {
            PropertyInfo resourceProperty = resourceBuilding.GetType().GetProperty(resourceType);
            PropertyInfo resourceCargoProperty = GetType().GetProperty(resourceType + "Cargo");

            int resourceValue = (int) (resourceProperty.GetValue(resourceBuilding, null));
            int resourceCargoValue = (int)(resourceCargoProperty.GetValue(this, null));

            if (FoodCargo + MedicineCargo < Capacity &&
                resourceValue >= AmountPerCargo) {

                resourceCargoProperty.SetValue(this,
                    Convert.ChangeType(resourceCargoValue + 1, resourceCargoProperty.PropertyType),
                    null);
                resourceProperty.SetValue(resourceBuilding,
                    Convert.ChangeType(resourceValue - AmountPerCargo, resourceProperty.PropertyType),
                    null);
            }
        }

        public virtual void UnloadResource(string resourceType, ResourceBuilding resourceBuilding) {
            PropertyInfo resourceProperty = resourceBuilding.GetType().GetProperty(resourceType);
            PropertyInfo resourceCargoProperty = GetType().GetProperty(resourceType + "Cargo");

            int resourceValue = (int)(resourceProperty.GetValue(resourceBuilding, null));
            int resourceCargoValue = (int)(resourceCargoProperty.GetValue(this, null));

            if (resourceCargoValue > 0 && resourceBuilding.IsAlive) {
                if (IsWithinTargetRange) {
                    resourceCargoProperty.SetValue(this,
                    Convert.ChangeType(resourceCargoValue - 1, resourceCargoProperty.PropertyType),
                    null);
                    resourceProperty.SetValue(resourceBuilding,
                    Convert.ChangeType(resourceValue + AmountPerCargo, resourceProperty.PropertyType),
                    null);
                }
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _waypoints.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }

        protected override void DrawBars(SpriteBatch spriteBatch) {
            if (IsSelected)
            {
                spriteBatch.Draw(HealthBar, new Rectangle((int)Position.X, (int)Position.Y,
                    Bounds.Width, 10), new Rectangle(0, 45, Bounds.Width, 10), Color.Gray);

                int foodBarWidth = (int)(Bounds.Width*((double) FoodCargo/Capacity));
                int medicineBarWidth = (int)(Bounds.Width * ((double)MedicineCargo / Capacity));

                spriteBatch.Draw(HealthBar, new Rectangle((int)Position.X, (int)Position.Y,
                    foodBarWidth, 10), new Rectangle(0, 45, Bounds.Width, 10), Color.Orange);
                spriteBatch.Draw(HealthBar, new Rectangle((int)Position.X + foodBarWidth, (int)Position.Y,
                    medicineBarWidth, 10), new Rectangle(0, 45, Bounds.Width, 10), Color.Red);
            }
        }

        private void _Move(string behavior, Vector2 direction, Map map)
        {
            if ((Destination != Position) && !IsAtDestination) {
                if (behavior == "linear") {
                    direction.Normalize();
                }

                else if (behavior == "steering") {
                    float previousSpeed = Speed;
                    float desiredSpeed = _FindMaxSpeed(Destination, direction);
                    Speed = MathHelper.Clamp(desiredSpeed,
                                             previousSpeed - MaxSpeedDelta*timer,
                                             previousSpeed + MaxSpeedDelta*timer);

                    float facingDirection = (float) Math.Atan2(direction.Y, direction.X);
                    facingDirection = _TurnToFace(Position, Destination, facingDirection, MaxAngularVelocity*timer);
                    
                    direction = new Vector2((float) Math.Cos(facingDirection), (float) Math.Sin(facingDirection));
                }

                bool collided = false;
                // If there's no color data, we're not colliding
                foreach (string inMobility in InMobility) {
                    Color? collColor = map.GetColorAt(inMobility, Position + (direction * 5));
                    if (collColor != null && collColor.Value.A > 0)
                    {// There is color data, we're interested in anything that's not fully transparent
                        collided = true;
                        break;
                    }
                }

                // Only move if we've not collided
                if (!collided) {

                    Position += Speed*direction;
                    LastMovement = Speed*direction;
                    IsAnimating = true;
                }
                else {
                    Collision();
                    IsAnimating = false;
                }
            }

            if ((Destination != Position) && IsAtDestination)
            {
                Destination = Position;
                Speed = 0;
                if (GetType().Name == "UAV")
                {
                    Destination = HQ_Position;
                    Direction = Direction.SouthWest;
                }
                //IsAnimating = false;
            }
           if (!IsAtDestination)
            Direction = (Direction)_ConvertDirectionToEnum(direction);
           if (Direction != PreviousDirection)
               Speed = Math.Max(maxSpeed * 0.5f, Speed * 0.7f);
           PreviousDirection = Direction;
        }

        private float _FindMaxSpeed(Vector2 destination, Vector2 direction)
        {
            float finalSpeed = MaxSpeed;
            float turningRadius = MaxSpeed / MaxAngularVelocity;
            Vector2 orth = new Vector2(direction.Y, -direction.X);
            float closestDistance = Math.Min(
                Vector2.Distance(destination, Position + (orth * turningRadius)),
                Vector2.Distance(destination, Position - (orth * turningRadius)));

            if (closestDistance < turningRadius)
            {
                float radius = Vector2.Distance(Position, destination) / 2;
                finalSpeed = MaxAngularVelocity * radius;
            }

            return finalSpeed;
        }

        private static float _TurnToFace(Vector2 position, Vector2 faceThis,
            float currentAngle, float turnSpeed)
        {
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.X;

            float desiredAngle = (float)Math.Atan2(y, x);

            float difference = _WrapAngle(desiredAngle - currentAngle);
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            return _WrapAngle(currentAngle + difference);
        }

        private static float _WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        private static int _ConvertDirectionToEnum(Vector2 direction)
        {
            // Convert a movement vector to face direction
            float angle = ((float)Math.Atan2(-direction.Y, -direction.X) + MathHelper.TwoPi) % MathHelper.TwoPi;
            int polarRegion = (int)Math.Round(angle * 8f / MathHelper.TwoPi) % 8;

            // Do a little bit of jigging because our spritesheet isn't in order
            polarRegion += 2;
            if (polarRegion > 7) polarRegion -= 8;

            return polarRegion;
        }

        public void Collision()
        {
            Position -= LastMovement;
            Destination = Position - LastMovement;
        }

        #endregion
    }
}
