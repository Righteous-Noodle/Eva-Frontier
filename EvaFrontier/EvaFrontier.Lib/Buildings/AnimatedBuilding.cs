using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvaFrontier.Lib.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace EvaFrontier.Lib.Buildings
{
    public class AnimatedBuilding : AnimatedSprite
    {
        public AnimatedBuilding(Vector2 location, string assetName, int range)
        {
            AssetName = assetName;
            Position = location;
            FrameSize = new Vector2(145, 170);
            Origin = FrameSize / 2;
            //Scale = new Vector2(0.3f, 0.3f);
            FrameLength = 0.1f;
            Range = range;
        }
    }
}
