using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledLib;

namespace EvaFrontier.Models
{
    public class World
    {
        public World(Map map, Vector2 viewSize) {
            _map = map;

            Rectangle bounds = new Rectangle(0, 0, map.Width * map.TileWidth, map.Height * map.TileHeight);
            _camera = new Camera(bounds, viewSize);

        }
        
        private Map _map;
        public Map Map { get { return _map; } }
        private Camera _camera;
        public Camera Camera { get { return _camera; } }

        public Vector2 Mouse {
            get { 
                MouseState mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
                return new Vector2(mouse.X + _camera.Position.X, mouse.Y + _camera.Position.Y);
            }
        }
    }
}
