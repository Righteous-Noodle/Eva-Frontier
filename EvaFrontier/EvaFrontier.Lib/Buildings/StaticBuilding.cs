using System;
using EvaFrontier.Lib.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EvaFrontier.Lib.Buildings
{
    public class StaticBuilding : Sprite
    {
        public override Rectangle Bounds
        {
            get {
                return new Rectangle((int)(Position.X),
                (int)(Position.Y),
                //return new Rectangle((int)(Position.X),
                //(int)(Position.Y),
                (int)(Texture.Width*Scale.X), 
                (int)(Texture.Height*Scale.Y));
            }
        }

        public StaticBuilding(Vector2 position, string assetName, int range)
        {
            AssetName = assetName;
            Position = position;
            Range = range;
        }

        public void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            Position = new Vector2(Position.X - Texture.Width / 2, Position.Y - Texture.Height / 2);
            //Origin = new Vector2(Texture.Width/2, Texture.Height/2);
        }

        public void Update(GameTime gameTime) {
        }

        public void HandleInput(InputState input) {
        }
    }
}
