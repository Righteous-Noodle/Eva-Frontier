using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvaFrontier.Lib.Sprites
{
    public class AnimatedSprite : Sprite
    {
        #region Fields

        protected float timer = 0;
        protected int currentFrame = 0;
        protected int totalFrames = -1;
        private Rectangle? _SourceRectangle = null;

        #endregion

        #region Properties

        public override Vector2 Center
        {
            get
            {
                return new Vector2(Position.X + FrameSize.X * Scale.X / 2,
                    Position.Y + FrameSize.Y * Scale.Y / 2);
            }
        }

        public override Rectangle Bounds
        {
            get {
                return new Rectangle((int)(Position.X),
                    (int)(Position.Y), 
                    (int)(FrameSize.X * Scale.X),
                    (int)(FrameSize.Y * Scale.Y));
            }
        }
        public Vector2 FrameSize { get; set; }
        public bool IsAnimating { get; set; }
        public float FrameLength { get; set; }

        protected int SpritesPerRow
        {
            get
            {
                return (int)(Texture.Width / FrameSize.X);
            }
        }

        protected int SpritesPerColumn
        {
            get
            {
                return (int)(Texture.Height / FrameSize.Y);
            }
        }

        public override Rectangle? SourceRectangle
        {
            get
            {
                if (currentFrame >= 0)
                {
                    int x = (int)((currentFrame % SpritesPerRow) * FrameSize.X);
                    int y = (int)((currentFrame / SpritesPerRow) * FrameSize.Y);
                    int width = (int)FrameSize.X;
                    int height = (int)FrameSize.Y;

                    _SourceRectangle = new Rectangle(x, y, width, height);
                }

                return _SourceRectangle;
            }
            set
            {
                _SourceRectangle = value;
            }
        }

        #endregion

        #region Constructor

        public AnimatedSprite():this(new Vector2(64f, 64f))
        {
        }

        public AnimatedSprite(Vector2 frameSize) {
            FrameSize = frameSize;
            IsAnimating = true;
            FrameLength = 0.05f;
        }

        #endregion

        #region Methods

        public virtual void Update(GameTime gameTime)
        {
            if (!IsAnimating) return;

            if (totalFrames == -1) totalFrames = SpritesPerRow * SpritesPerColumn;

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= FrameLength)
            {
                timer = 0f;

                currentFrame = (currentFrame + 1) % totalFrames;
            }
        }

        #endregion
    }
}
