using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvaFrontier.Lib.Controls
{
    public abstract class Control
    {
        protected string _name;
        protected string _text;
        protected Vector2 _size;
        protected Vector2 _position;
        protected object _value;
        protected bool _isSelected;
        protected bool _isMouseOver;
        protected bool _isEnabled;
        protected bool _isVisible;
        protected bool _isTabStop;
        protected SpriteFont _spriteFont;
        protected Color _color;

        public event EventHandler Selected;

        public virtual string Name {
            get { return _name; }
            set { _name = value; }
        }

        public virtual string Text {
            get { return _text; }
            set { _text = value; }
        }

        public virtual Vector2 Size {
            get { return _size;}
            set { _size = value; }
        }

        public virtual Vector2 Position {
            get { return _position; }
            set { _position = value; }
        }

        public virtual object Value {
            get { return _value; }
            set { _value = value; }
        }

        public virtual bool IsSelected {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        public virtual bool IsMouseOver {
            get { return _isMouseOver; }
            set { _isMouseOver = value; }
        }

        public virtual bool IsEnabled {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public virtual bool IsVisible {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        public virtual bool IsTabStop {
            get { return _isTabStop; }
            set { _isTabStop = value; }
        }

        public virtual SpriteFont SpriteFont {
            get { return _spriteFont; }
            set { _spriteFont = value; }
        }

        public virtual Color Color {
            get { return _color; }
            set { _color = value; }
        }

        protected virtual void OnSelected(EventArgs e) {
            if (Selected != null) {
                Selected(this, e);
            }
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void HandleInput(InputState input);
    }
}
