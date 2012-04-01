using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvaFrontier.Models;
using Microsoft.Xna.Framework;

namespace EvaFrontier.Controllers
{
    public class CameraManager : Microsoft.Xna.Framework.GameComponent
    {
        /// <summary>
        /// This is a game component that implements IUpdateable.
        /// </summary>
        public CameraManager(Game game, Camera camera, InputType type, PlayerIndex index) 
            : base(game) {
            _camera = camera;
            _manager = new InputManager(type, index);
            _keyMap = new Dictionary<CameraAction, Inputs>();

            _keyMap.Add(CameraAction.MoveUp, Inputs.Up);
            _keyMap.Add(CameraAction.MoveDown, Inputs.Down);
            _keyMap.Add(CameraAction.MoveLeft, Inputs.Left);
            _keyMap.Add(CameraAction.MoveRight, Inputs.Right);
            _keyMap.Add(CameraAction.ZoomIn, Inputs.RightTrigger);
            _keyMap.Add(CameraAction.ZoomOut, Inputs.LeftTrigger);
        }

         /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            _manager.Update();
            if (_manager.IsInputDown(_keyMap[CameraAction.MoveUp]))
            {
                _camera.DoAction(CameraAction.MoveUp);
            }
            else if (_manager.IsInputDown(_keyMap[CameraAction.MoveDown]))
            {
                _camera.DoAction(CameraAction.MoveDown);
            }
            else if (_manager.IsInputDown(_keyMap[CameraAction.MoveLeft]))
            {
                _camera.DoAction(CameraAction.MoveLeft);
            }
            else if (_manager.IsInputDown(_keyMap[CameraAction.MoveRight]))
            {
                _camera.DoAction(CameraAction.MoveRight);
            }
            else if (_manager.IsInputDown(_keyMap[CameraAction.ZoomIn]))
            {
                _camera.DoAction(CameraAction.ZoomIn);
            }
            else if (_manager.IsInputDown(_keyMap[CameraAction.ZoomOut]))
            {
                _camera.DoAction(CameraAction.ZoomOut);
            }
        }

        public Matrix Transform
        {
            get { return _camera.Transform; }
        }

        private InputManager _manager;
        private Camera _camera;
        private Dictionary<CameraAction, Inputs> _keyMap;
        public Rectangle VisibleArea
        {
            get { return _camera.VisibleArea; }
        }
    }
}
