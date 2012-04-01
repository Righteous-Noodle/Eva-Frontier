using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvaFrontier.Lib.Buildings;
using EvaFrontier.Lib.Controls;
using EvaFrontier.Lib.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace EvaFrontier.Lib.Units
{
    public class AerialUnit : Unit
    {
        #region Fields
 
        #endregion
 
        #region Properties
 
        #endregion
 
        #region Constructor
 
        public AerialUnit()
        {
            FrameLength = 0.05f;
            IsAnimating = true;

            InMobility = new List<string>();
        }
 
        #endregion
 
        #region Methods
        
        #endregion
    }
}
