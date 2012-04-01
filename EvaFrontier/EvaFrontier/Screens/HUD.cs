using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvaFrontier.Lib.Buildings;
using EvaFrontier.Lib.Controls;
using EvaFrontier.Lib.Sprites;
using EvaFrontier.Lib.Units;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using EvaFrontier.Lib;
using Microsoft.Xna.Framework.Input;

namespace EvaFrontier.Screens
{
    public class HUD
    {
        private const int MiniMapSide = 200;
        public const int HUDHeight = 210;

        private Texture2D _backgroundTexture;
        private Vector2 backgroundPosition;
        private Texture2D _blankTexture;
        private Texture2D _objectTexture;

        private Texture2D _miniMapTexture;

        public Rectangle MiniMap {
            get { return _miniMap;}
        }
        private Rectangle _miniMap;

        private Rectangle originalMiniMap;
        private Vector2 _miniMapRatio;
        private Vector2 _mapSize;

        public HUD(Vector2 mapSize/*, Camera camera*/)
        {
            _mapSize = mapSize;
            backgroundPosition = new Vector2(0, World.Camera.ViewArea.Height);
            _miniMap = new Rectangle(World.Camera.ViewArea.Width-MiniMapSide - 5,
                                    World.Camera.ViewArea.Height - MiniMapSide + HUDHeight - 5,
                                    MiniMapSide, MiniMapSide);
            originalMiniMap = _miniMap;
            _miniMapRatio = new Vector2(_miniMap.Width / _mapSize.X, _miniMap.Height / _mapSize.Y);
        }

        public void LoadContent(ContentManager content)
        {
            _backgroundTexture = content.Load<Texture2D>(@"Textures\HUD\HudBkgd");
            _miniMapTexture = content.Load<Texture2D>(@"Maps\MiniMap");
            _blankTexture = content.Load<Texture2D>(@"Textures\BlankTexture");
            _objectTexture = content.Load<Texture2D>(@"Textures\Circle");
        }

        public void HandleInput(InputState input, Sprite selectedSprite) {
            MouseState mouse = input.CurrentMouseState;
            Rectangle mouseRec = new Rectangle(mouse.X, mouse.Y, 1, 1);

            if (mouseRec.Intersects(_miniMap)) {
                Vector2 mousePositionOnMiniMap = new Vector2((mouse.X - _miniMap.X)/_miniMapRatio.X,
                                                            (mouse.Y - _miniMap.Y)/_miniMapRatio.Y);

                if (input.CurrentMouseState.LeftButton == ButtonState.Pressed) {
                    World.Camera.Focus(mousePositionOnMiniMap);
                }

                if (input.CurrentMouseState.RightButton == ButtonState.Pressed) {
                    var unit = selectedSprite as Unit;
                    if (unit != null) {
                        unit.Destination = mousePositionOnMiniMap;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_backgroundTexture, backgroundPosition, Color.White);

            _DrawMiniMap(spriteBatch);
        }

        public void DrawControls(SpriteBatch spriteBatch, ControlManager controls) {
            if (controls != null) {
                controls.Draw(spriteBatch);
            }
        }

        private void _DrawMiniMap(SpriteBatch spriteBatch)
        {
            Rectangle miniCamera;
            spriteBatch.Draw(_miniMapTexture, _miniMap, Color.White);
            miniCamera = new Rectangle((int)(World.Camera.ViewArea.X * _miniMapRatio.X + _miniMap.X),
                                       (int)(World.Camera.ViewArea.Y * _miniMapRatio.Y + _miniMap.Y),
                                       (int)(World.Camera.ViewArea.Width * _miniMapRatio.X),
                                       (int)(World.Camera.ViewArea.Height * _miniMapRatio.Y));
            spriteBatch.Draw(_blankTexture, miniCamera, null, new Color(200, 200, 255, 80),
                             0, Vector2.Zero, SpriteEffects.None, 0.13f);
            _DrawMapObjectsOnMiniMap(spriteBatch);
        }

        private void _DrawMapObjectsOnMiniMap(SpriteBatch spriteBatch)
        {
            Rectangle miniObj;
            Color color = Color.White;
            foreach (var sprite in World.Sprites)
            {
                if (sprite is Unit) color = Color.Blue;
                else if (sprite is Village) {
                    if  (!((sprite as Village).IsDead))
                        color = Color.Orange;
                    else
                        color = new Color(0,0,0,0);
                }
                else if (sprite is ResourceBuilding) color = Color.Red;

                miniObj = new Rectangle((int)(sprite.Position.X * _miniMapRatio.X + _miniMap.X),
                                        (int)(sprite.Position.Y * _miniMapRatio.Y + _miniMap.Y), 5, 5);
                spriteBatch.Draw(_objectTexture, miniObj, color);
            }
        }
    }
}
