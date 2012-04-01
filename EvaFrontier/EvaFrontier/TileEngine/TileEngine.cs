#region File Description
//-----------------------------------------------------------------------------
// TitleEngine.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using EvaFrontier.Lib;
#endregion

namespace EvaFrontier
{
    /// <summary>
    /// Static class for a tileable map
    /// </summary>
    static class TileEngine
    {
        #region Map

        public static ContentManager ContentManager = null;
        private static KeyboardState _previousKeyboardState;
        
        private static Camera2D _camera;

        public static Camera2D Camera { get { return _camera; } }
        private static KeyboardState keyboard;

        static int scrollValue = 5; // how much the screen moves when scrolling
        static float zoomValue = 0.05f;

        /// <summary>
        /// The map being used by the tile engine.
        /// </summary>
        private static Map _map = null;

        /// <summary>
        /// The map being used by the tile engine.
        /// </summary>
        public static Map Map
        {
            get { return _map; }
        }


        /// <summary>
        /// The position of the outside 0,0 corner of the map, in pixels.
        /// </summary>
        private static Vector2 _mapOriginPosition;


        /// <summary>
        /// Calculate the screen position of a given map location (in tiles).
        /// </summary>
        /// <param name="mapPosition">A map location, in tiles.</param>
        /// <returns>The current screen position of that location.</returns>
        public static Vector2 GetScreenPosition(Point mapPosition)
        {
            return new Vector2(
                _mapOriginPosition.X + mapPosition.X * _map.TileSize.X,
                _mapOriginPosition.Y + mapPosition.Y * _map.TileSize.Y);
        }


        /// <summary>
        /// Set the map in use by the tile engine.
        /// </summary>
        /// <param name="map">The new map for the tile engine.</param>
        public static void SetMap(Map newMap)
        {
            // check the parameter
            if (newMap == null)
            {
                throw new ArgumentNullException("newMap");
            }

            // assign the new map
            _map = newMap;

            _camera = new Camera2D(Viewport, _map.GetSize());

            // reset the map origin, which will be recalculate on the first update
            _mapOriginPosition = Vector2.Zero;

            // move the party to its initial position
            //_partyLeaderPosition.TilePosition = new Point(10, 10);
            //_partyLeaderPosition.TileOffset = Vector2.Zero;
            //_partyLeaderPosition.Direction = Direction.South;
        }


        #endregion


        #region Graphics Data


        /// <summary>
        /// The viewport that the tile engine is rendering within.
        /// </summary>
        private static Viewport _viewport;

        /// <summary>
        /// The viewport that the tile engine is rendering within.
        /// </summary>
        public static Viewport Viewport
        {
            get { return _viewport; }
            set
            {
                _viewport = value;
                _viewportCenter = new Vector2(
                    _viewport.X + _viewport.Width / 2f,
                    _viewport.Y + _viewport.Height / 2f);
            }
        }


        /// <summary>
        /// The center of the current viewport.
        /// </summary>
        private static Vector2 _viewportCenter;


        #endregion


        #region Party


        /// <summary>
        /// The speed of the party leader, in units per second.
        /// </summary>
        /// <remarks>
        /// The movementCollisionTolerance constant should be a multiple of this number.
        /// </remarks>
        private const float _partyLeaderMovementSpeed = 5f;


        /// <summary>
        /// The current position of the party leader.
        /// </summary>
        private static PlayerPosition _partyLeaderPosition = new PlayerPosition();
        public static PlayerPosition PartyLeaderPosition
        {
            get { return _partyLeaderPosition; }
            set { _partyLeaderPosition = value; }
        }


        /// <summary>
        /// The automatic movement remaining for the party leader.
        /// </summary>
        /// <remarks>
        /// This is typically used for automatic movement when spawning on a map.
        /// </remarks>
        private static Vector2 _autoPartyLeaderMovement = Vector2.Zero;


        /// <summary>
        /// Updates the automatic movement of the party.
        /// </summary>
        /// <returns>The automatic movement for this update.</returns>
        private static Vector2 _UpdatePartyLeaderAutoMovement(GameTime gameTime)
        {
            // check for any remaining auto-movement
            if (_autoPartyLeaderMovement == Vector2.Zero)
            {
                return Vector2.Zero;
            }

            // get the remaining-movement direction
            Vector2 autoMovementDirection = Vector2.Normalize(_autoPartyLeaderMovement);

            // calculate the potential movement vector
            Vector2 movement = Vector2.Multiply(autoMovementDirection,
                _partyLeaderMovementSpeed);

            // limit the potential movement vector by the remaining auto-movement
            movement.X = Math.Sign(movement.X) * MathHelper.Min(Math.Abs(movement.X),
                Math.Abs(_autoPartyLeaderMovement.X));
            movement.Y = Math.Sign(movement.Y) * MathHelper.Min(Math.Abs(movement.Y),
                Math.Abs(_autoPartyLeaderMovement.Y));

            // remove the movement from the total remaining auto-movement
            _autoPartyLeaderMovement -= movement;

            return movement;
        }


        /// <summary>
        /// Update the user-controlled movement of the party.
        /// </summary>
        /// <returns>The controlled movement for this update.</returns>
        private static Vector2 _UpdateUserMovement(GameTime gameTime)
        {
            Vector2 desiredMovement = Vector2.Zero;
            KeyboardState currentKeyboardState = Keyboard.GetState();

            // accumulate the desired direction from user input
            //if (InputManager.IsActionPressed(InputManager.Action.MoveCharacterUp))
            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                if (_CanPartyLeaderMoveUp())
                {
                    desiredMovement.Y -= _partyLeaderMovementSpeed;
                }
            }
            //if (InputManager.IsActionPressed(InputManager.Action.MoveCharacterDown))
            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                if (_CanPartyLeaderMoveDown())
                {
                    desiredMovement.Y += _partyLeaderMovementSpeed;
                }
            }
            //if (InputManager.IsActionPressed(InputManager.Action.MoveCharacterLeft))
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                if (_CanPartyLeaderMoveLeft())
                {
                    desiredMovement.X -= _partyLeaderMovementSpeed;
                }
            }
            //if (InputManager.IsActionPressed(InputManager.Action.MoveCharacterRight))
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                if (_CanPartyLeaderMoveRight())
                {
                    desiredMovement.X += _partyLeaderMovementSpeed;
                }
            }

            // if there is no desired movement, then we can't determine a direction
            if (desiredMovement == Vector2.Zero)
            {
                return Vector2.Zero;
            }

            _previousKeyboardState = currentKeyboardState;

            return desiredMovement;
        }


        #endregion


        #region Collision


        /// <summary>
        /// The number of pixels that characters should be allowed to move into 
        /// blocking tiles.
        /// </summary>
        /// <remarks>
        /// The partyMovementSpeed constant should cleanly divide this number.
        /// </remarks>
        const int movementCollisionTolerance = 12;


        /// <summary>
        /// Returns true if the player can move up from their current position.
        /// </summary>
        private static bool _CanPartyLeaderMoveUp()
        {
            // if they're not within the tolerance of the next tile, then this is moot
            if (_partyLeaderPosition.TileOffset.Y > -movementCollisionTolerance)
            {
                return true;
            }

            // if the player is at the outside left and right edges, 
            // then check the diagonal tiles
            if (_partyLeaderPosition.TileOffset.X < -movementCollisionTolerance)
            {
                if (_map.IsBlocked(new Point(
                    _partyLeaderPosition.TilePosition.X - 1,
                    _partyLeaderPosition.TilePosition.Y - 1)))
                {
                    return false;
                }
            }
            else if (_partyLeaderPosition.TileOffset.X > movementCollisionTolerance)
            {
                if (_map.IsBlocked(new Point(
                    _partyLeaderPosition.TilePosition.X + 1,
                    _partyLeaderPosition.TilePosition.Y - 1)))
                {
                    return false;
                }
            }

            // check the tile above the current one
            return !_map.IsBlocked(new Point(
                    _partyLeaderPosition.TilePosition.X,
                    _partyLeaderPosition.TilePosition.Y - 1));
        }


        /// <summary>
        /// Returns true if the player can move down from their current position.
        /// </summary>
        private static bool _CanPartyLeaderMoveDown()
        {
            // if they're not within the tolerance of the next tile, then this is moot
            if (_partyLeaderPosition.TileOffset.Y < movementCollisionTolerance)
            {
                return true;
            }

            // if the player is at the outside left and right edges, 
            // then check the diagonal tiles
            if (_partyLeaderPosition.TileOffset.X < -movementCollisionTolerance)
            {
                if (_map.IsBlocked(new Point(
                    _partyLeaderPosition.TilePosition.X - 1,
                    _partyLeaderPosition.TilePosition.Y + 1)))
                {
                    return false;
                }
            }
            else if (_partyLeaderPosition.TileOffset.X > movementCollisionTolerance)
            {
                if (_map.IsBlocked(new Point(
                    _partyLeaderPosition.TilePosition.X + 1,
                    _partyLeaderPosition.TilePosition.Y + 1)))
                {
                    return false;
                }
            }

            // check the tile below the current one
            return !_map.IsBlocked(new Point(
                    _partyLeaderPosition.TilePosition.X,
                    _partyLeaderPosition.TilePosition.Y + 1));
        }


        /// <summary>
        /// Returns true if the player can move left from their current position.
        /// </summary>
        private static bool _CanPartyLeaderMoveLeft()
        {
            // if they're not within the tolerance of the next tile, then this is moot
            if (_partyLeaderPosition.TileOffset.X > -movementCollisionTolerance)
            {
                return true;
            }

            // if the player is at the outside left and right edges, 
            // then check the diagonal tiles
            if (_partyLeaderPosition.TileOffset.Y < -movementCollisionTolerance)
            {
                if (_map.IsBlocked(new Point(
                    _partyLeaderPosition.TilePosition.X - 1,
                    _partyLeaderPosition.TilePosition.Y - 1)))
                {
                    return false;
                }
            }
            else if (_partyLeaderPosition.TileOffset.Y > movementCollisionTolerance)
            {
                if (_map.IsBlocked(new Point(
                    _partyLeaderPosition.TilePosition.X - 1,
                    _partyLeaderPosition.TilePosition.Y + 1)))
                {
                    return false;
                }
            }

            // check the tile to the left of the current one
            return !_map.IsBlocked(new Point(
                    _partyLeaderPosition.TilePosition.X - 1,
                    _partyLeaderPosition.TilePosition.Y));
        }


        /// <summary>
        /// Returns true if the player can move right from their current position.
        /// </summary>
        private static bool _CanPartyLeaderMoveRight()
        {
            // if they're not within the tolerance of the next tile, then this is moot
            if (_partyLeaderPosition.TileOffset.X < movementCollisionTolerance)
            {
                return true;
            }

            // if the player is at the outside left and right edges, 
            // then check the diagonal tiles
            if (_partyLeaderPosition.TileOffset.Y < -movementCollisionTolerance)
            {
                if (_map.IsBlocked(new Point(
                    _partyLeaderPosition.TilePosition.X + 1,
                    _partyLeaderPosition.TilePosition.Y - 1)))
                {
                    return false;
                }
            }
            else if (_partyLeaderPosition.TileOffset.Y > movementCollisionTolerance)
            {
                if (_map.IsBlocked(new Point(
                    _partyLeaderPosition.TilePosition.X + 1,
                    _partyLeaderPosition.TilePosition.Y + 1)))
                {
                    return false;
                }
            }

            // check the tile to the right of the current one
            return !_map.IsBlocked(new Point(
                    _partyLeaderPosition.TilePosition.X + 1,
                    _partyLeaderPosition.TilePosition.Y));
        }


        #endregion


        #region Updating


        /// <summary>
        /// Update the tile engine.
        /// </summary>
        public static void Update(GameTime gameTime)
        {
            //// check for auto-movement
            //Vector2 autoMovement = _UpdatePartyLeaderAutoMovement(gameTime);

            //// if there is no auto-movement, handle user controls
            //Vector2 userMovement = Vector2.Zero;
            //if (autoMovement == Vector2.Zero)
            //{
            //    userMovement = _UpdateUserMovement(gameTime);
            //    // calculate the desired position
            //    if (userMovement != Vector2.Zero)
            //    {
            //        Point desiredTilePosition = _partyLeaderPosition.TilePosition;
            //        Vector2 desiredTileOffset = _partyLeaderPosition.TileOffset;
            //        PlayerPosition.CalculateMovement(
            //            Vector2.Multiply(userMovement, 15f),
            //            ref desiredTilePosition, ref desiredTileOffset);
            //        // check for collisions
            //        if ((_partyLeaderPosition.TilePosition != desiredTilePosition) &&
            //            !_CanMoveIntoTile(desiredTilePosition))
            //        {
            //            userMovement = Vector2.Zero;
            //        }
            //    }
            //}

            //// move the party
            //Point oldPartyLeaderTilePosition = _partyLeaderPosition.TilePosition;
            //_partyLeaderPosition.Move(autoMovement + userMovement);

            //// adjust the _map origin so that the party is at the center of the viewport
            //_mapOriginPosition += _viewportCenter - _partyLeaderPosition.ScreenPosition;

            //// make sure the boundaries of the _map are never inside the viewport
            //_mapOriginPosition.X = MathHelper.Min(_mapOriginPosition.X, _viewport.X);
            //_mapOriginPosition.Y = MathHelper.Min(_mapOriginPosition.Y, _viewport.Y);
            //_mapOriginPosition.X += MathHelper.Max(
            //    (_viewport.X + _viewport.Width) -
            //    (_mapOriginPosition.X + _map.MapDimensions.X * _map.TileSize.X), 0f);
            //_mapOriginPosition.Y += MathHelper.Max(
            //    (_viewport.Y + _viewport.Height /*- Hud.HudHeight*/) -
            //    (_mapOriginPosition.Y + _map.MapDimensions.Y * _map.TileSize.Y), 0f);

            keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Down))
            //if (true)
                _camera.Move(new Vector2(0, scrollValue));
            else if (keyboard.IsKeyDown(Keys.Up))
                _camera.Move(new Vector2(0, -scrollValue));
            if (keyboard.IsKeyDown(Keys.Left))
                _camera.Move(new Vector2(-scrollValue, 0));
            else if (keyboard.IsKeyDown(Keys.Right))
                _camera.Move(new Vector2(scrollValue, 0));
            if (keyboard.IsKeyDown(Keys.OemPlus))
                _camera.ZoomIn(zoomValue);
            else if (keyboard.IsKeyDown(Keys.OemMinus))
                _camera.ZoomOut(zoomValue);

        }


        /// <summary>
        /// Performs any actions associated with moving into a new tile.
        /// </summary>
        /// <returns>True if the character can move into the tile.</returns>
        private static bool _CanMoveIntoTile(Point mapPosition)
        {
            // if the tile is blocked, then this is simple
            if (_map.IsBlocked(mapPosition))
            {
                return false;
            }

            // check for anything that might be in the tile
            //if (Session.EncounterTile(mapPosition))
            //{
            //    return false;
            //}

            // nothing stops the party from moving into the tile
            return true;
        }


        #endregion


        #region Drawing


        /// <summary>
        /// Draw the visible tiles in the given _map layers.
        /// </summary>
        public static void DrawLayers(SpriteBatch spriteBatch, bool drawBase,
            bool drawFringe, bool drawObject)
        {
            // check the parameters
            if (spriteBatch == null)
            {
                throw new ArgumentNullException("spriteBatch");
            }
            if (!drawBase && !drawFringe && !drawObject)
            {
                return;
            }

            Rectangle destinationRectangle =
                new Rectangle(0, 0, _map.TileSize.X, _map.TileSize.Y);

            for (int y = 0; y < _map.MapDimensions.Y; y++)
            {
                for (int x = 0; x < _map.MapDimensions.X; x++)
                {
                    destinationRectangle.X =
                        (int)_mapOriginPosition.X + x * _map.TileSize.X;
                    destinationRectangle.Y =
                        (int)_mapOriginPosition.Y + y * _map.TileSize.Y;

                    // If the tile is inside the screen
                    if (CheckVisibility(destinationRectangle))
                    {
                        Point mapPosition = new Point(x, y);
                        if (drawBase)
                        {
                            Rectangle sourceRectangle =
                                _map.GetBaseLayerSourceRectangle(mapPosition);
                            if (sourceRectangle != Rectangle.Empty)
                            {
                                spriteBatch.Draw(_map.Texture, destinationRectangle,
                                    sourceRectangle, Color.White);
                            }
                        }
                        if (drawFringe)
                        {
                            Rectangle sourceRectangle =
                                _map.GetFringeLayerSourceRectangle(mapPosition);
                            if (sourceRectangle != Rectangle.Empty)
                            {
                                spriteBatch.Draw(_map.Texture, destinationRectangle,
                                    sourceRectangle, Color.White);
                            }
                        }
                        if (drawObject)
                        {
                            Rectangle sourceRectangle =
                                _map.GetObjectLayerSourceRectangle(mapPosition);
                            if (sourceRectangle != Rectangle.Empty)
                            {
                                spriteBatch.Draw(_map.Texture, destinationRectangle,
                                    sourceRectangle, Color.White);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Returns true if the given rectangle is within the viewport.
        /// </summary>
        public static bool CheckVisibility(Rectangle screenRectangle)
        {
            return ((screenRectangle.X > _viewport.X - screenRectangle.Width) &&
                (screenRectangle.Y > _viewport.Y - screenRectangle.Height) &&
                (screenRectangle.X < _viewport.X + _viewport.Width) &&
                (screenRectangle.Y < _viewport.Y + _viewport.Height));
        }


        #endregion
    }
}
