using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using EvaFrontier.Lib;

namespace EvaFrontier.Lib
{
    public class Camera
    {
        private Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private Matrix _matrix;
        public Matrix Matrix
        {
            get { return _matrix; }
        }

        private Vector2 _viewCenter;
        public Vector2 ViewCenter
        {
            get { return _viewCenter; }
            set { _viewCenter = value; }
        }

        private Rectangle _bounds;

        private Vector2 _viewSize;
        public Vector2 ViewSize
        {
            get { return _viewSize; }
            set { _viewSize = value; }
        }

        private Vector2 _tileSize;
        public Vector2 TileSize
        {
            get { return _tileSize; }
            set { _tileSize = value; }
        }

        private Rectangle _viewArea;
        public Rectangle ViewArea
        {
            get { return _viewArea; }
        }

        private float _zoom = 1.0f;
        public float Zoom { get { return _zoom; } }

        private float _minZoom;
        private float _maxZoom = 5.0f;
        private float _scrollValue = 0.05f; // how much the screen moves when scrolling
        private float _zoomValue = 0.05f;

        public Camera(Vector2 mapSize, Vector2 tileSize, Vector2 viewSize)
        {
            _bounds = new Rectangle(0, 0, (int)(mapSize.X*tileSize.X), (int)(mapSize.Y*tileSize.Y));
            _tileSize = tileSize;
            _viewSize = viewSize;
            _scrollValue = TileSize.X * 0.625f;
            _viewCenter = new Vector2(_viewSize.X / 2, _viewSize.Y / 2);
            _position = _viewCenter;
            _minZoom = MathHelper.Max(
                (float)_viewSize.X / _bounds.Width,
                (float)_viewSize.Y / _bounds.Height);

            _UpdateCamera();
        }

        public void HandleInput(InputState input, PlayerIndex? controllingPlayer)
        {
            int playerIndex = (int)controllingPlayer;
            if (input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.S) ||
                input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Down))
                _Move(new Vector2(0, _scrollValue));
            else if (input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.W) ||
                input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Up))
                _Move(new Vector2(0, -_scrollValue));
            if (input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.D) ||
                input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Right))
                _Move(new Vector2(_scrollValue, 0));
            else if (input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.A) ||
                input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Left))
                _Move(new Vector2(-_scrollValue, 0));

            float zoomValueFromMouseWheel = input.CurrentMouseState.ScrollWheelValue - input.PreviousMouseState.ScrollWheelValue;
            if (zoomValueFromMouseWheel >= 10)
            {
                _ZoomIn(_zoomValue * 5);
            }
            else if (zoomValueFromMouseWheel <= -10) {
                _ZoomOut(_zoomValue * 5);
            }

            if (input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.OemPlus))
                _ZoomIn(_zoomValue);
            else if (input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.OemMinus))
                _ZoomOut(_zoomValue);
            else if (input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Back))
                _ZoomOriginal();
        }

        private void _UpdateCamera()
        {
            float newWidth = ViewSize.X / Zoom;
            float newHeight = ViewSize.Y / Zoom;
            _viewArea = new Rectangle(
                (int)(_position.X - newWidth / 2),
                (int)(_position.Y - newHeight / 2),
                (int)(newWidth), (int)(newHeight));

            _matrix = Matrix.CreateTranslation(-_position.X, -_position.Y, 0) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(_viewCenter.X, _viewCenter.Y, 0);
        }

        private void _Move(Vector2 movement)
        {
            _position.X = MathHelper.Clamp(
                _position.X + movement.X,
                (_bounds.Left + _viewCenter.X) / _zoom,
                _bounds.Right - _viewCenter.X / _zoom);
            _position.Y = MathHelper.Clamp(
                _position.Y + movement.Y,
                (_bounds.Top + _viewCenter.Y) / _zoom,
                _bounds.Bottom - _viewCenter.Y / _zoom);

            _UpdateCamera();
        }

        private void _ZoomIn(float amount)
        {
            _zoom = MathHelper.Clamp(_zoom + amount, _minZoom, _maxZoom);
            // zooming in will never produce a visibile area outside the map bounds, so
            // updating the matrix is sufficient
            _UpdateCamera();
        }

        private void _ZoomOut(float amount)
        {
            _zoom = MathHelper.Clamp(_zoom - amount, _minZoom, _maxZoom);
            // zooming out might extend the view area outside the bounds, so the camera position must be clamped
            // the move method does this already and will update the camera variables
            _Move(Vector2.Zero);
        }

        private void _ZoomOriginal()
        {
            _zoom = 1.0f;
            _UpdateCamera();
        }

        public void Focus(Vector2 point)
        {
            _Move(point - _position);
        }
    }
}
