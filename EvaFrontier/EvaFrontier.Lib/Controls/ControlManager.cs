using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EvaFrontier.Lib.Controls
{
    public class ControlManager : List<Control>
    {

        #region Fields and Properties

        private Control _selection;
        public Control Selection {
            get {
                _selection = Find(c => c.IsSelected);
                return _selection;
            }
            set {
                _selection = value;
            }
        }

        #endregion

        #region Constructors

        public ControlManager() : base() {}
        public ControlManager(int capacity) : base(capacity) {}
        public ControlManager(IEnumerable<Control> collection) : base(collection) {}

        #endregion

        #region Methods

        public void Update(GameTime gameTime) {
            if (Count == 0)
                return;

            foreach (Control control in this) {
                if (control.IsEnabled) {
                    control.Update(gameTime);
                }
            }
        }

        public void HandleInput(InputState input) {
            if (Count == 0)
                return;

            foreach (Control control in this) {
                control.HandleInput(input);
                if (control.IsSelected) {
                    Selection = control;
                    break;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            foreach (Control control in this) {
                if (control.IsVisible) {
                    control.Draw(spriteBatch);
                }
            }
        }

        #endregion
    }
}
