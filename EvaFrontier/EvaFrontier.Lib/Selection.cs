using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvaFrontier.Lib.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using EvaFrontier.Lib.Units;

namespace EvaFrontier.Lib
{
    public class Selection
    {
        #region Fields

        private Texture2D _texture;

        #endregion

        #region Properties

        public Rectangle? Box { get; set;}
        private List<Sprite> _selectedSprites;
        private List<Sprite> _spritesRef;
        public Sprite SelectedSprite { get; set; }

        public bool IsNull { get { return SelectedSprite == null; } }
        public bool IsUnit { get { return SelectedSprite is Unit; } }
        public bool HasControls { get { return SelectedSprite.Controls != null; } }

        #endregion

        #region Methods

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>(@"Textures\DottedLine");
            Box = null;
        }

        public void Set(Sprite sprite) {
            SelectedSprite = sprite;
            SelectedSprite.IsSelected = true;
            if (IsUnit) {
                Unit unit = SelectedSprite as Unit;
                unit.SfxInstance.Play();
            }
        }

        public void UnSet() {
            if (IsUnit){
                Unit unit = SelectedSprite as Unit;
                unit.SfxInstance.Pause();
            }
            SelectedSprite.IsSelected = false;
            SelectedSprite = null;
        }

        public void HandleSingleLeftClick(Vector2 mouseWorldPosition) {
            _CheckAllSprites(mouseWorldPosition);
        }

        private void _CheckAllSprites(Vector2 mouseWorldPosition) {
            foreach (var sprite in _spritesRef) {
                if (sprite.Intersects(mouseWorldPosition)) {
                    _ChangeSelection(sprite);
                    return;
                }
            }
        }

        private void _ChangeSelection(Sprite sprite) {
            if (_selectedSprites.Contains(sprite)) return;

            _UnselectOld();
            _selectedSprites.Clear();

            _selectedSprites.Add(sprite);
            _SelectNew();
        }

        private void _UnselectOld()
        {
            foreach (var sprite in _selectedSprites)
                sprite.IsSelected = false;
        }

        private void _SelectNew()
        {
            foreach (var sprite in _selectedSprites)
                sprite.IsSelected = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Box!=null) {
                _DrawHorizontalLine(spriteBatch, Box.Value.Y);
                _DrawHorizontalLine(spriteBatch, Box.Value.Y + Box.Value.Height);

                _DrawVerticalLine(spriteBatch, Box.Value.X);
                _DrawVerticalLine(spriteBatch, Box.Value.X + Box.Value.Width);
            }
        }

        private void _DrawHorizontalLine(SpriteBatch spriteBatch, int thePositionY)
        {
            if (Box.Value.Width > 0)
            {
                for (int aCounter = 0; aCounter <= Box.Value.Width - 10; aCounter += 10)
                {
                    if (Box.Value.Width - aCounter >= 0)
                    {
                        spriteBatch.Draw(_texture, new Rectangle(Box.Value.X + aCounter, thePositionY, 10, 5), Color.White);
                    }
                }
            }
            else if (Box.Value.Width < 0)
            {
                for (int aCounter = -10; aCounter >= Box.Value.Width; aCounter -= 10)
                {
                    if (Box.Value.Width - aCounter <= 0)
                    {
                        spriteBatch.Draw(_texture, new Rectangle(Box.Value.X + aCounter, thePositionY, 10, 5), Color.White);
                    }
                }
            }
        }

        private void _DrawVerticalLine(SpriteBatch spriteBatch, int thePositionX)
        {
            if (Box.Value.Height > 0)
            {
                for (int aCounter = -2; aCounter <= Box.Value.Height; aCounter += 10)
                {
                    if (Box.Value.Height - aCounter >= 0)
                    {
                        spriteBatch.Draw(_texture, new Rectangle(thePositionX, Box.Value.Y + aCounter, 10, 5),
                            new Rectangle(0, 0, _texture.Width, _texture.Height), Color.White, MathHelper.ToRadians(90),
                            new Vector2(0, 0), SpriteEffects.None, 0);
                    }
                }
            }
            else if (Box.Value.Height < 0)
            {
                for (int aCounter = 0; aCounter >= Box.Value.Height; aCounter -= 10)
                {
                    if (Box.Value.Height - aCounter <= 0)
                    {
                        spriteBatch.Draw(_texture, new Rectangle(thePositionX - 10, Box.Value.Y + aCounter, 10, 5), Color.White);
                    }
                }
            }
        }

        #endregion

    }
}
