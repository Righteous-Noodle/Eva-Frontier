using Microsoft.Xna.Framework;

namespace EvaFrontier.Models
{
    public class Camera
    {
        public Camera(Rectangle bounds, Vector2 viewSize)
        {
            _bounds = bounds;
            _viewSize = viewSize;
            _position = Vector2.Zero;
            _minZoom = MathHelper.Max(_viewSize.X / _bounds.Width,
                                      _viewSize.Y / _bounds.Height);
            _zoom = 1.0f;
        }

        //public void Update(float elapsedSeconds)
        //{
            
        //}

        public void DoAction(CameraAction action)
        {
            switch (action)
            {
                case CameraAction.MoveUp:
                    DoActionMove(new Vector2(0, -1));
                    break;
                case CameraAction.MoveDown:
                    DoActionMove(new Vector2(0, 1));
                    break;
                case CameraAction.MoveLeft:
                    DoActionMove(new Vector2(-1, 0));
                    break;
                case CameraAction.MoveRight:
                    DoActionMove(new Vector2(1, 0));
                    break;
                case CameraAction.ZoomIn:
                    DoActionZoom(ZoomValue);
                    break;
                case CameraAction.ZoomOut:
                    DoActionZoom(-ZoomValue);
                    break;
            }
        }

        private void DoActionZoom(float amount)
        {
            _zoom = MathHelper.Clamp(_zoom + amount, _minZoom, _maxZoom);
        }

        private void DoActionMove(Vector2 movement)
        {
            // to match the thumbstick behavior, we need to normalize non-zero vectors in case the user
            // is pressing a diagonal direction.
            if (movement != Vector2.Zero)
                movement.Normalize();

            // scale our movement to move 25 pixels per second
            movement *= 25f;

            // move the Camera
            _position += movement;

            // clamp the Camera so it never leaves the visible area of the map
            var cameraMax = new Vector2(
                _bounds.Width - _viewSize.X,
                _bounds.Height - _viewSize.Y);
            _position = Vector2.Clamp(_position, Vector2.Zero, cameraMax);
        }

        private const float ZoomValue = 0.05f;

        private float _zoom = 1.0f;
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        private float _minZoom;
        private float _maxZoom = 5.0f;
        private Rectangle _bounds;
        private Vector2 _viewSize;

        private Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
        }

        public Matrix Transform
        {
            get
            {
                return Matrix.Identity *
                   Matrix.CreateTranslation(-(int)_position.X, -(int)_position.Y, 0) *
                    Matrix.CreateScale(_zoom, _zoom, 0);
            }
        }

        public Rectangle VisibleArea
        {
            get
            {
                float newWidth = _viewSize.X / Zoom;
                float newHeight = _viewSize.Y / Zoom;

                return new Rectangle((int)_position.X,
                    (int)_position.Y,
                    (int)newWidth,
                    (int)newHeight);
            }
        }
    }

    public enum CameraAction
    {
        Nothing,
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        GoToLeftEnd,
        GoToRightEnd,
        GoToTopEnd,
        GoToBottomEnd,
        ZoomIn,
        ZoomOut,
        ZoomNormal,
        Focus
    };
}
