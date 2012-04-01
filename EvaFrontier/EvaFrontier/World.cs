using System;
using System.Collections.Generic;
using System.Linq;
using EvaFrontier.Lib;
using EvaFrontier.Lib.Buildings;
using EvaFrontier.Lib.Controls;
using EvaFrontier.Lib.Sprites;
using EvaFrontier.Lib.Units;
using EvaFrontier.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EvaFrontier
{
    public static class World
    {
        #region

        #endregion

        #region Fields & Properties

        private static bool _controlPressed;
        private static bool _isBuildingSolarPanel;
        private static bool _isBuildingWindTurbine;

        private static Texture2D _solarPanelTexture;
        private static Texture2D _windTurbineTexture;

        private static Texture2D _gridTexture;

        private static Vector2 _screenMouse;
        public static Vector2 Mouse { get; set; }
        public static Rectangle MouseRec
        {
            get
            {
                return new Rectangle((int)Mouse.X, (int)Mouse.Y, 1, 1);
            }
        }

        public static Map Map { get; set; }
        public static Dictionary<Vector2, Rectangle> CollisionClip { get; set; }
        public static Camera Camera { get; set; }
        public static HUD HUD { get; set; }
        public static HeadQuarter HeadQuarter { get; set; }
        public static List<Sprite> Sprites { get; set; }
        public static Selection Selection { get; set; }
        public static WorldTime Time { get; set; }
        public static ResourcesManager ResourcesManager { get; set; }

        #endregion

        #region Methods

        public static void Update(GameTime gameTime) {
            foreach (var sprite in Sprites) {
                if (sprite is Unit){
                    ((Unit)sprite).Update(gameTime, "steering", Map);
                }
                else if (sprite is ResourceBuilding)
                {
                    ((ResourceBuilding)sprite).Update(gameTime, Sprites);
                }
                else if (sprite is AnimatedSprite)
                    ((AnimatedSprite)sprite).Update(gameTime);
            }

            Time.Update(gameTime);
            ResourcesManager.Update(gameTime);
        }

        public static void HandleInput(InputState input, ContentManager content) {

            _screenMouse = new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y);
            _gridTexture = content.Load<Texture2D>(@"Maps\Grid");

            HUD.HandleInput(input, Selection.SelectedSprite);

            if (Selection.IsNull) {
                foreach (var sprite in Sprites) {
                    if (sprite.Intersects(Mouse) && input.IsNewLeftMouseClick() && !Selection.IsUnit) {
                        Selection.Set(sprite);
                        break;
                    }
                }
            } else {

                if (Selection.HasControls) {
                    Control control = Selection.SelectedSprite.Controls.Selection;

                    _HandleResourceMechanism(input);

                    if (control == null) {
                        if (input.CurrentMouseState.Y < (Camera.ViewSize.Y)) {
                            if (input.IsNewLeftMouseClick()) {
                                Selection.UnSet();
                            }
                            else if (Selection.IsUnit) {
                                var unit = Selection.SelectedSprite as Unit;
                                unit.HandleWayPoint(input, Mouse);
                            }
                        } else {
                            Selection.SelectedSprite.Controls.HandleInput(input);
                        }
                    } else { // some control is selected
                        if (Selection.IsUnit) {
                            _HandleUnitControlSelection(input, control, content);
                        }
                        else if (Selection.SelectedSprite is HeadQuarter) {
                            if (control.Name.Contains("Deploy")) {

                                _HandleDeployCommandAtHeadQuarter(control, content);
                            } else {
                                var hq = Selection.SelectedSprite as HeadQuarter;
                                hq.HandleBuildCommand(control, ResourcesManager);
                            }
                        }
                        else if (Selection.SelectedSprite is Village) {
                            var village = Selection.SelectedSprite as Village;
                            if (control.Name == "BuildFarm") {
                                village.HandleBuildFarm(control, ResourcesManager);
                            }
                        }
                    }
                } else { // sprite with no control
                    if (input.CurrentMouseState.Y < (Camera.ViewSize.Y)) {
                        if (input.IsNewLeftMouseClick()) {
                            Selection.UnSet();
                        }
                    }
                }
            }
        }

        private static void _HandleResourceMechanism(InputState input)
        {
            if (Selection.IsUnit) {
                var unit = Selection.SelectedSprite as Unit;

                ResourceBuilding resourceBuilding = (from sprite in Sprites
                                                     where sprite is ResourceBuilding &&
                                                           Vector2.Distance(unit.Position, sprite.Position) <
                                                           sprite.Range
                                                     select sprite as ResourceBuilding).FirstOrDefault();
                unit.HandleResource(input, resourceBuilding);

                /*if (unit.Controls.Selection.Name.Contains("Load"))
                {
                    EvaFrontier.load.Play();
                }
                else if (unit.Controls.Selection.Name.Contains("Unload"))
                {
                    EvaFrontier.unload.Play();
                }*/
            }
        }

        private static void _HandleDeployCommandAtHeadQuarter(Control control, ContentManager content)
        {
            Unit unit = null;
            if (control.Name == "DeployCar") {
                    unit = new Car(new Vector2(HeadQuarter.Position.X - HeadQuarter.Texture.Width/2,
                                               HeadQuarter.Position.Y + HeadQuarter.Texture.Height),
                                   0, new Vector2(105f, 79f), 1);
            }
            else if (control.Name == "DeployTruck") {
                    unit = new Truck(new Vector2(HeadQuarter.Position.X - HeadQuarter.Texture.Width,
                                                 HeadQuarter.Position.Y + HeadQuarter.Texture.Height),
                                     0, new Vector2(160f, 120f), 6);
            }
            else if (control.Name == "DeployUAV") {
                if (Sprites.FindAll(s => s is UAV).Count < 1 &&
                    HeadQuarter.IsAirfieldBuilt) {
                    unit = new UAV(new Vector2(HeadQuarter.Position.X - 30,
                                                      HeadQuarter.Position.Y + 70),
                                          new Vector2(HeadQuarter.Position.X - 30,
                                                      HeadQuarter.Position.Y + 70), 0, new Vector2(93f, 93f), 1);
                }
            }
            else if (control.Name == "DeployOsprey") {
                if (Sprites.FindAll(s => s is Osprey).Count < 1 &&
                    HeadQuarter.IsControlTowerBuilt) {
                    unit = new Osprey(new Vector2(HeadQuarter.Position.X,
                                                  HeadQuarter.Position.Y + HeadQuarter.Texture.Height),
                                      0, new Vector2(300f, 300f), 4);
                }
            }

            if (unit != null) {
                if (ResourcesManager.MaxEnergy - ResourcesManager.Energy >= unit.EnergyCosts[unit.EngineLevel] &&
                    ResourcesManager.Money >= unit.PurchasingCost) {
                    unit.LoadContent(content);
                    Sprites.Add(unit);
                    ResourcesManager.Money -= unit.PurchasingCost;
                    ResourcesManager.Energy += unit.EnergyCosts[unit.EngineLevel];
                }
            }
            control.IsSelected = false;
        }

        private static void _HandleUnitControlSelection(InputState input, Control control, ContentManager content) {
            if (Selection.IsUnit) {
                var unit = Selection.SelectedSprite as Unit;
                if (control.Name == "Move"){
                    if (input.CurrentMouseState.Y < (Camera.ViewSize.Y) && input.IsNewLeftMouseClick()) {
                        unit.Destination = new Vector2(Mouse.X - unit.Bounds.Width/2,
                                                       Mouse.Y - unit.Bounds.Height/2);
                        control.IsSelected = false;
                    }
                }
                else if (control.Name == "BuildSolarPanel") {
                    if (!_isBuildingSolarPanel) {
                        unit = Selection.SelectedSprite as Truck;
                        unit.IsBuilding = true;
                        _isBuildingSolarPanel = true;
                        _solarPanelTexture = content.Load<Texture2D>(@"Textures\Buildings\SolarPlant");
                    }

                    if (_isBuildingSolarPanel &&
                        input.IsNewLeftMouseClick()) {
                            var solarPanel = new StaticBuilding(Mouse, @"Textures\Buildings\SolarPlant", 30);
                            solarPanel.LoadContent(content);
                            Sprites.Add(solarPanel);
                            ResourcesManager.Money -= Settings.SolarPanelCost;
                            ResourcesManager.MaxEnergy += Settings.SolarPanelEnergy;
                            control.IsSelected = false;
                            _isBuildingSolarPanel = false;
                    }
                    if (input.CurrentMouseState.Y < (Camera.ViewSize.Y) && input.IsNewRightMouseClick())
                    {
                        _isBuildingSolarPanel = false;
                        unit.IsBuilding = false;
                        control.IsSelected = false;
                    }
                }

                else if (control.Name == "BuildWindTurbine")
                {
                    unit = Selection.SelectedSprite as Truck;
                    unit.IsBuilding = true;
                    _isBuildingWindTurbine = true;
                    _windTurbineTexture = content.Load<Texture2D>(@"Textures\Buildings\WindTurbineSingleSprite");

                    if ((Vector2.Distance(Mouse, unit.Position) <= unit.Range) && input.IsNewLeftMouseClick())
                    {
                        var windTurbine = new AnimatedBuilding(Mouse, @"Textures\Buildings\WindTurbine", 30);
                        windTurbine.LoadContent(content);
                        Sprites.Add(windTurbine);
                        ResourcesManager.Money -= Settings.WindTurbineCost;
                        ResourcesManager.MaxEnergy += Settings.WindTurbineEnergy;
                        control.IsSelected = false;
                        _isBuildingWindTurbine = false;
                    }

                    if (input.CurrentMouseState.Y < (Camera.ViewSize.Y) && input.IsNewRightMouseClick())
                    {
                        unit.IsBuilding = false;
                        _isBuildingWindTurbine = false;
                        control.IsSelected = false;
                    }
                }
                else if (!control.Name.ToLower().Contains("load"))
                {
                    unit.HandleEnergyUpgrade(ResourcesManager.Money);
                    control.IsSelected = false;
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Begin();

            if (_isBuildingSolarPanel) {
                //_DrawGrid(spriteBatch);
                spriteBatch.Draw(_solarPanelTexture, 
                    new Vector2(_screenMouse.X - _solarPanelTexture.Width/2,
                        _screenMouse.Y - _solarPanelTexture.Height/2),
                        Color.White);
            }

            if (_isBuildingWindTurbine) {
                //_DrawGrid(spriteBatch);
                spriteBatch.Draw(_windTurbineTexture,
                    new Vector2(_screenMouse.X - _windTurbineTexture.Width / 2,
                        _screenMouse.Y - _windTurbineTexture.Height / 2),
                        Color.White);
            }

            HUD.Draw(spriteBatch);
            HUD.DrawControls(spriteBatch, ResourcesManager.Controls);
            if (Selection.SelectedSprite != null)
            {
                HUD.DrawControls(spriteBatch, Selection.SelectedSprite.Controls);
            }

            spriteBatch.End();
        }

        private static void _DrawGrid(SpriteBatch spriteBatch)
        {
            Color gridColor = new Color(0, 0, 0, 128);
            Rectangle gridSquare;

            int left = Camera.ViewArea.X;
            int right = Camera.ViewArea.Right / Map.TileWidth;
            int top = Camera.ViewArea.Y;
            int bottom = Camera.ViewArea.Bottom / Map.TileHeight;

            for (int x = 0; x < right + 1; x++)
            {
                for (int y = 0; y < bottom; y++)
                {
                    gridSquare = new Rectangle(x *Map.TileWidth, y * Map.TileHeight, Map.TileWidth, Map.TileHeight);
                    spriteBatch.Draw(_gridTexture, gridSquare, gridColor);
                }
            }
        }

        public static Vector2 WorldToScreenPosition(Vector2 worldPosition) {
            return new Vector2((worldPosition.X - Camera.ViewArea.Left) * Camera.Zoom,
                               (worldPosition.Y - Camera.ViewArea.Top) * Camera.Zoom);
        }

        public static Vector2 ScreenToWorldPosition(Vector2 screenPosition) {
            return new Vector2(screenPosition.X / Camera.Zoom + Camera.ViewArea.Left,
                               screenPosition.Y / Camera.Zoom + Camera.ViewArea.Top);
        }

        public static void LoadCollisionClip()
        {
            CollisionClip = new Dictionary<Vector2, Rectangle>();
            TileLayer clipLayer = Map.GetLayer("Collision") as TileLayer;
            if (clipLayer.Tiles.Width > 0)
            {
                for (int y = 0; y < clipLayer.Width; y++)
                {
                    for (int x = 0; x < clipLayer.Height; x++)
                    {
                        Tile tile = clipLayer.Tiles[x, y];
                        if (tile != null)
                        {
                            CollisionClip.Add(new Vector2(x, y), new Rectangle(x * tile.Source.Width, y * tile.Source.Height, tile.Source.Width, tile.Source.Height));
                        }
                    }
                }
            }
        }
    
        public static void CheckCollisions(Unit unit)
        {
            if (!(unit is AerialUnit)) {
                foreach (Rectangle clip in CollisionClip.Values)
                {
                    if (unit.Bounds.Intersects(clip))
                    {
                        unit.Collision();
                        break;
                    }
                } 
            }
        }

        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels
        /// between two sprites.
        /// </summary>
        /// <param name="rectangleA">Bounding rectangle of the first sprite</param>
        /// <param name="dataA">Pixel data of the first sprite</param>
        /// <param name="rectangleB">Bouding rectangle of the second sprite</param>
        /// <param name="dataB">Pixel data of the second sprite</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
                                           Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }


        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites.
        /// </summary>
        /// <param name="transformA">World transform of the first sprite.</param>
        /// <param name="widthA">Width of the first sprite's texture.</param>
        /// <param name="heightA">Height of the first sprite's texture.</param>
        /// <param name="dataA">Pixel color data of the first sprite.</param>
        /// <param name="transformB">World transform of the second sprite.</param>
        /// <param name="widthB">Width of the second sprite's texture.</param>
        /// <param name="heightB">Height of the second sprite's texture.</param>
        /// <param name="dataB">Pixel color data of the second sprite.</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(
                            Matrix transformA, int widthA, int heightA, Color[] dataA,
                            Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }

        #endregion
    }
}
