using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EvaFrontier.Lib.Controls
{
    public class Image : Control
    {
        private Texture2D _texture;
        public Texture2D Texture {
            get { return _texture; }
            set { _texture = value; }
        }

        #region Constructors

        public Image(string name, string imagePath, Vector2 position, Vector2 size, ContentManager content):
            this(name, position, size) {
            _texture = content.Load<Texture2D>(imagePath);
        }

        public Image(string name, Vector2 position, Vector2 size) {
            Name = name;
            Position = position;
            Size = size;
            IsTabStop = false;
            IsEnabled = true;
            IsVisible = true;
            Color = Color.White;
        }

        #endregion

        public override void Update(GameTime gameTime) {}

        public override void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), Color);
        }

        public override void HandleInput(InputState input) {}
    }
}
