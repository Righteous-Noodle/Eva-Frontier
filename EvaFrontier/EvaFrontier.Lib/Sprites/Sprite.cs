using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using EvaFrontier.Lib.Controls;
using EvaFrontier.Lib.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EvaFrontier.Lib.Sprites
{
    public class Sprite
    {
        #region Fields

        protected Random _random;

        #endregion

        #region Properties

        public virtual string AssetName { get; set; }
        public virtual Texture2D Texture { get; set; }
        public virtual Texture2D SelectionSquare { get; set; }
        public virtual Texture2D HealthBar { get; set; }
        public virtual Texture2D RangeCircle { get; set; }
        public virtual Vector2 Position { get; set; }
        public virtual Vector2 Center { get; set; }
        public virtual Rectangle? SourceRectangle { get; set; }
        public virtual Color Tint { get; set; }
        public virtual float RotationAngle { get; set; }
        public virtual Vector2 Origin { get; set; }
        public virtual Vector2 Scale { get; set; }
        public virtual SpriteEffects Orientation { get; set; }
        public virtual bool IsVisible { get; set; }
        public virtual float Opacity { get; set; }
        public virtual Rectangle Bounds { get; set; }
        public virtual float LayerDepth { get; set; }

        public virtual bool IsMouseOver { get; set; }
        public virtual bool IsSelected { get; set; }
        public virtual int Range { get; set; }
        public virtual bool HasUnitInRange { get; set; }
        public ControlManager Controls;
        public virtual bool AllowsMultipleSelection { get; set; }

        #endregion

        #region Constructor

        public Sprite()
        {
            AssetName = "";
            Texture = null;
            Position = Vector2.Zero;
            SourceRectangle = null;
            Tint = Color.White;
            RotationAngle = 0f;
            Origin = Vector2.Zero;
            Scale = Vector2.One;
            Orientation = SpriteEffects.None;
            IsVisible = true;
            Opacity = 1f;
            LayerDepth = 0f;
            IsMouseOver = false;

            _random = new Random();
        }

        #endregion

        #region Methods

        public virtual void LoadContent(ContentManager content)
        {
            //AssetName = assetName;
            Texture = content.Load<Texture2D>(AssetName);
            SelectionSquare = content.Load<Texture2D>(@"Textures\SelectionCircle_green");
            RangeCircle = content.Load<Texture2D>(@"Textures\RangeCircle");
            HealthBar = content.Load<Texture2D>(@"Textures\HealthBar");
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (IsVisible && Texture != null)
            {
                Color tint = new Color(Tint.R, Tint.G, Tint.B, Convert.ToByte(Opacity * Tint.A));
                spriteBatch.Draw(Texture, Position, SourceRectangle, tint, MathHelper.ToRadians(RotationAngle), Origin, Scale, Orientation, LayerDepth);
                DrawSelectionCircle(spriteBatch);
                DrawRangeCircle(spriteBatch);
                DrawBars(spriteBatch);
            }
        }

        public void DrawSelectionCircle(SpriteBatch spriteBatch)
        {
            if (IsSelected && this is Unit) {
                spriteBatch.Draw(SelectionSquare, Bounds, new Color(255, 255, 255, 180));
            }
        }

        protected virtual void DrawRangeCircle(SpriteBatch spriteBatch) {
            if (IsSelected || HasUnitInRange) {
                spriteBatch.Draw(RangeCircle, new Rectangle((int)Position.X + Texture.Width/2 - Range,
                    (int)Position.Y + Texture.Height/2 - Range,
                Range * 2, Range * 2), null, new Color(Color.White, 0.3f), 0, Vector2.Zero, SpriteEffects.None, 0);
            }
        }

        protected virtual void DrawBars(SpriteBatch spriteBatch) {
        }

        public bool Intersects(Vector2 location) {
            Rectangle locationRec = new Rectangle((int)location.X, (int)location.Y, 1, 1);
            if (locationRec.Intersects(Bounds)) {
                Color[] pixelData = new Color[1];
                Vector2 pixelPos = location - new Vector2(Bounds.X, Bounds.Y);
                pixelPos.X = Math.Max(0, pixelPos.X);
                pixelPos.Y = Math.Max(0, pixelPos.Y);

                Texture.GetData(0, new Rectangle((int) pixelPos.X, (int) pixelPos.Y, (1), (1)),
                                pixelData, 0, 1);

                return (pixelData[0].A != 0);
            }

            return false;
        }

        public bool IsInRegion(Rectangle box) {
            return box.Contains(Bounds);
        }

        public virtual void HandleInput(InputState input, Vector2 worldMouse)
        {
            Rectangle mouseRec = new Rectangle((int)worldMouse.X, (int)worldMouse.Y, 1, 1);

            /*if (mouseRec.Intersects(Bounds))
            {
                Color[] pixelData = new Color[1];
                //Vector2 pixelPos = worldMouse - Position;
                Vector2 pixelPos = worldMouse - new Vector2(Bounds.X, Bounds.Y);
                pixelPos.X = Math.Max(0, pixelPos.X);
                pixelPos.Y = Math.Max(0, pixelPos.Y);

                Texture.GetData(0, new Rectangle((int)pixelPos.X, (int)pixelPos.Y, (1), (1)),
                    pixelData, 0, 1);

                IsMouseOver = (pixelData[0].A != 0);

                if (IsMouseOver &&
                    input.CurrentMouseState.LeftButton == ButtonState.Pressed &&
                    input.PreviousMouseState.LeftButton == ButtonState.Released)
                {
                    IsSelected = true;
                }

            }
            else
            {
                IsMouseOver = false;
            }*/

            /*if (IsSelected && !IsMouseOver &&
                input.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                IsSelected = false;
            }*/
        }

        #endregion

    }
}
