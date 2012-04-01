using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvaFrontier.Actors
{
    public class Sprite1
    {
        #region Declarations
        /// <summary>
        /// The sprite sheet that contains all animation frames
        /// for any individual sprite.
        /// </summary>
        public Texture2D Texture;

        private Vector2 _worldPosition = Vector2.Zero;
        private Vector2 _velocity = Vector2.Zero;

        /// <summary>
        /// Holds a single Rectangle object for each animation
        /// frame defined for the sprite.
        /// </summary>
        private List<Rectangle> _frames = new List<Rectangle>();

        /// <summary>
        /// The index of the frame that is being displayed at any given time.
        /// </summary>
        private int _currentFrameIndex;

        /// <summary>
        /// Pre-determined amount of time for which each frame is displayed.
        /// </summary>
        private float _frameTime = 0.1f;
        /// <summary>
        /// Used to compare with _frameTime to determine when _currentFrameIndex
        /// should be incremented.
        /// </summary>
        private float _timeForCurrentFrame = 0.0f;

        private Color _tintColor = Color.White;

        private float _rotation = 0.0f;
        
        public bool IsExpired = false;
        public bool IsAnimating = true;
        public bool IsAnimatingWhenStopped = true;

        public bool IsCollidable = true;
        public int CollisionRadius = 0;
        public int BoundingXPadding = 0;
        public int BoundingYPadding = 0;
        #endregion

        #region Drawing and Animation Properties
        public int FrameWidth { get { return _frames[0].Width; } }

        public int FrameHeight { get { return _frames[0].Height; } }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value % MathHelper.TwoPi; }
        }

        public int FrameIndex
        {
            get { return _currentFrameIndex; }
            set
            {
                _currentFrameIndex = (int)MathHelper.Clamp(value, 0,
                                                            _frames.Count - 1);
            }
        }

        public float FrameTime
        {
            get { return _frameTime; }
            set { _frameTime = MathHelper.Max(0, value); }
        }

        public Rectangle Source { get { return _frames[_currentFrameIndex]; } }
        #endregion

        #region Positional Properties
        public Vector2 WorldLocation
        {
            get { return _worldPosition; }
            set { _worldPosition = value; }
        }
        public Vector2 ScreenLocation
        {
            get { return Camera.WorldToScreen(_worldPosition); }
        }

        public Rectangle WorldRectangle
        {
            get
            {
                return new Rectangle(
                    (int)_worldPosition.X,
                    (int)_worldPosition.Y,
                    FrameWidth, FrameHeight);
            }
        }
        public Rectangle ScreenRectangle
        {
            get { return Camera.WorldToScreen(WorldRectangle); }
        }

        /// <summary>
        /// The center relative to the current frame's upper left corner.
        /// </summary>
        public Vector2 RelativeCenter
        {
            get { return new Vector2(FrameWidth / 2, FrameHeight / 2); }
        }
        /// <summary>
        /// The center of the frame in world coordinates
        /// </summary>
        public Vector2 WorldCenter
        {
            get { return _worldPosition + RelativeCenter; }
        }
        /// <summary>
        /// The center of the frame in screen coordinates
        /// </summary>
        public Vector2 ScreenCenter
        {
            get { return Camera.WorldToScreen(_worldPosition + RelativeCenter); }
        }
        #endregion

        #region Collision Related Properties
        public Rectangle BoundingBoxRect {
            get { return new Rectangle(
                (int)_worldPosition.X + BoundingXPadding,
                (int)_worldPosition.Y + BoundingYPadding,
                FrameWidth - (BoundingXPadding * 2),
                FrameHeight - (BoundingYPadding * 2));}
        }
        #endregion

        #region Constructors
        public Sprite1(Vector2 worldPosition, 
            Texture2D texture, 
            Rectangle initialFrame,
            Vector2 velocity) {
            _worldPosition = worldPosition;
            Texture = texture;
            _velocity = velocity;

            _frames.Add(initialFrame);
            }
        #endregion

        #region Update and Draw Methods
        public virtual void Update(GameTime gameTime) {
            if (!IsExpired) {
                float elapsed = (float) gameTime.ElapsedGameTime.TotalSeconds;

                _timeForCurrentFrame += elapsed;

                if (IsAnimating) {
                    if (_timeForCurrentFrame >= FrameTime) {
                        if (IsAnimatingWhenStopped || _velocity != Vector2.Zero)
                        {
                            _currentFrameIndex = (_currentFrameIndex + 1)%(_frames.Count);
                            _timeForCurrentFrame = 0.0f;
                        }
                    }
                }

                _worldPosition += (_velocity*elapsed);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch) {
            if (!IsExpired) {
                if (Camera.IsObjectVisible(WorldRectangle)) {
                    spriteBatch.Draw(
                        Texture,
                        ScreenCenter,
                        Source,
                        _tintColor,
                        _rotation,
                        RelativeCenter,
                        1.0f,
                        SpriteEffects.None,
                        0.0f);
                }
            }
        }
        #endregion

        #region Collision Detection Methods
        public bool IsBoxColliding(Rectangle otherBox) {
            if ((IsCollidable) && (!IsExpired)) {
                return BoundingBoxRect.Intersects(otherBox);
            } else {
                return false;
            }
        }

        public bool IsCircleColliding(Vector2 otherCenter, float otherRadius) {
            if ((IsCollidable) && (!IsExpired)) {
                if (Vector2.Distance(WorldCenter, otherCenter) <
                    (CollisionRadius + otherRadius)) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }
        #endregion

        #region Animation-Related Methods
        public void AddFrame(Rectangle frameRectangle) {
            _frames.Add(frameRectangle);
        }

        public void RotateTo(Vector2 direction) {
            Rotation = (float)Math.Atan2(direction.Y, direction.X);
        }
        #endregion
    }
}
