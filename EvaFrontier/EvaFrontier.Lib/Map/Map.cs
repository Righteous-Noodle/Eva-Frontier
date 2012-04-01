#region File Description
//-----------------------------------------------------------------------------
// Map.cs
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
using Microsoft.Xna.Framework.Content;
#endregion

namespace EvaFrontier.Lib
{
    /// <summary>
    /// One section of the world, and all of the data in it.
    /// </summary>
    public class Map : ContentObject, ICloneable
    {
        #region Description


        /// <summary>
        /// The name of this section of the world.
        /// </summary>
        private string name;

        /// <summary>
        /// The name of this section of the world.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        #endregion


        #region Dimensions


        /// <summary>
        /// The dimensions of the map, in tiles.
        /// </summary>
        private Point mapDimensions;

        /// <summary>
        /// The dimensions of the map, in tiles.
        /// </summary>
        public Point MapDimensions
        {
            get { return mapDimensions; }
            set { mapDimensions = value; }
        }


        /// <summary>
        /// The size of the tiles in this map, in pixels.
        /// </summary>
        private Point tileSize;

        /// <summary>
        /// The size of the tiles in this map, in pixels.
        /// </summary>
        public Point TileSize
        {
            get { return tileSize; }
            set { tileSize = value; }
        }

        public Rectangle GetSize()
        {
            return new Rectangle(0, 0, mapDimensions.X * tileSize.X, mapDimensions.Y * tileSize.X);
        }

        /// <summary>
        /// The number of tiles in a row of the map texture.
        /// </summary>
        /// <remarks>
        /// Used to determine the source rectangle from the map layer value.
        /// </remarks>
        private int tilesPerRow;

        /// <summary>
        /// The number of tiles in a row of the map texture.
        /// </summary>
        /// <remarks>
        /// Used to determine the source rectangle from the map layer value.
        /// </remarks>
        [ContentSerializerIgnore]
        public int TilesPerRow
        {
            get { return tilesPerRow; }
        }


        #endregion


        #region Spawning


        /// <summary>
        /// A valid spawn position for this map. 
        /// </summary>
        private Point spawnMapPosition;

        /// <summary>
        /// A valid spawn position for this map. 
        /// </summary>
        public Point SpawnMapPosition
        {
            get { return spawnMapPosition; }
            set { spawnMapPosition = value; }
        }


        #endregion


        #region Graphics Data


        /// <summary>
        /// The content name of the texture that contains the tiles for this map.
        /// </summary>
        private string textureName;

        /// <summary>
        /// The content name of the texture that contains the tiles for this map.
        /// </summary>
        public string TextureName
        {
            get { return textureName; }
            set { textureName = value; }
        }


        /// <summary>
        /// The texture that contains the tiles for this map.
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// The texture that contains the tiles for this map.
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D Texture
        {
            get { return texture; }
        }


        #endregion


        #region Music

        /*
        /// <summary>
        /// The name of the music cue for this map.
        /// </summary>
        private string musicCueName;

        /// <summary>
        /// The name of the music cue for this map.
        /// </summary>
        public string MusicCueName
        {
            get { return musicCueName; }
            set { musicCueName = value; }
        }


        /// <summary>
        /// The name of the music cue for combats that occur while traveling on this map.
        /// </summary>
        private string combatMusicCueName;

        /// <summary>
        /// The name of the music cue for combats that occur while traveling on this map.
        /// </summary>
        public string CombatMusicCueName
        {
            get { return combatMusicCueName; }
            set { combatMusicCueName = value; }
        }

        */
        #endregion


        #region Map Layers


        #region Base Layer


        /// <summary>
        /// Spatial array for the ground tiles for this map.
        /// </summary>
        private int[] baseLayer;

        /// <summary>
        /// Spatial array for the ground tiles for this map.
        /// </summary>
        public int[] BaseLayer
        {
            get { return baseLayer; }
            set { baseLayer = value; }
        }


        /// <summary>
        /// Retrieves the base layer value for the given map position.
        /// </summary>
        public int GetBaseLayerValue(Point mapPosition)
        {
            // check the parameter
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                throw new ArgumentOutOfRangeException("mapPosition");
            }

            return baseLayer[mapPosition.Y * mapDimensions.X + mapPosition.X];
        }


        /// <summary>
        /// Retrieves the source rectangle for the tile in the given position
        /// in the base layer.
        /// </summary>
        /// <remarks>This method allows out-of-bound (blocked) positions.</remarks>
        public Rectangle GetBaseLayerSourceRectangle(Point mapPosition)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                return Rectangle.Empty;
            }

            int baseLayerValue = GetBaseLayerValue(mapPosition);
            if (baseLayerValue < 0)
            {
                return Rectangle.Empty;
            }

            return new Rectangle(
                (baseLayerValue % tilesPerRow) * tileSize.X,
                (baseLayerValue / tilesPerRow) * tileSize.Y,
                tileSize.X, tileSize.Y);
        }


        #endregion


        #region Fringe Layer

        /// <summary>
        /// Spatial array for the fringe tiles for this map.
        /// </summary>
        private int[] fringeLayer;

        /// <summary>
        /// Spatial array for the fringe tiles for this map.
        /// </summary>
        public int[] FringeLayer
        {
            get { return fringeLayer; }
            set { fringeLayer = value; }
        }


        /// <summary>
        /// Retrieves the fringe layer value for the given map position.
        /// </summary>
        public int GetFringeLayerValue(Point mapPosition)
        {
            // check the parameter
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                throw new ArgumentOutOfRangeException("mapPosition");
            }

            return fringeLayer[mapPosition.Y * mapDimensions.X + mapPosition.X];
        }


        /// <summary>
        /// Retrieves the source rectangle for the tile in the given position
        /// in the fringe layer.
        /// </summary>
        /// <remarks>This method allows out-of-bound (blocked) positions.</remarks>
        public Rectangle GetFringeLayerSourceRectangle(Point mapPosition)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                return Rectangle.Empty;
            }

            int fringeLayerValue = GetFringeLayerValue(mapPosition);
            if (fringeLayerValue < 0)
            {
                return Rectangle.Empty;
            }

            return new Rectangle(
                (fringeLayerValue % tilesPerRow) * tileSize.X,
                (fringeLayerValue / tilesPerRow) * tileSize.Y,
                tileSize.X, tileSize.Y);
        }


        #endregion


        #region Object Layer


        /// <summary>
        /// Spatial array for the object images on this map.
        /// </summary>
        private int[] objectLayer;

        /// <summary>
        /// Spatial array for the object images on this map.
        /// </summary>
        public int[] ObjectLayer
        {
            get { return objectLayer; }
            set { objectLayer = value; }
        }


        /// <summary>
        /// Retrieves the object layer value for the given map position.
        /// </summary>
        public int GetObjectLayerValue(Point mapPosition)
        {
            // check the parameter
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                throw new ArgumentOutOfRangeException("mapPosition");
            }

            return objectLayer[mapPosition.Y * mapDimensions.X + mapPosition.X];
        }


        /// <summary>
        /// Retrieves the source rectangle for the tile in the given position
        /// in the object layer.
        /// </summary>
        /// <remarks>This method allows out-of-bound (blocked) positions.</remarks>
        public Rectangle GetObjectLayerSourceRectangle(Point mapPosition)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                return Rectangle.Empty;
            }

            int objectLayerValue = GetObjectLayerValue(mapPosition);
            if (objectLayerValue < 0)
            {
                return Rectangle.Empty;
            }

            return new Rectangle(
                (objectLayerValue % tilesPerRow) * tileSize.X,
                (objectLayerValue / tilesPerRow) * tileSize.Y,
                tileSize.X, tileSize.Y);
        }


        #endregion


        #region Collision Layer


        /// <summary>
        /// Spatial array for the collision properties of this map.
        /// </summary>
        private int[] collisionLayer;

        /// <summary>
        /// Spatial array for the collision properties of this map.
        /// </summary>
        public int[] CollisionLayer
        {
            get { return collisionLayer; }
            set { collisionLayer = value; }
        }


        /// <summary>
        /// Retrieves the collision layer value for the given map position.
        /// </summary>
        public int GetCollisionLayerValue(Point mapPosition)
        {
            // check the parameter
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                throw new ArgumentOutOfRangeException("mapPosition");
            }

            return collisionLayer[mapPosition.Y * mapDimensions.X + mapPosition.X];
        }


        /// <summary>
        /// Returns true if the given map position is blocked.
        /// </summary>
        /// <remarks>This method allows out-of-bound (blocked) positions.</remarks>
        public bool IsBlocked(Point mapPosition)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                return true;
            }

            //return (GetCollisionLayerValue(mapPosition) != 0);
            return false;
        }


        #endregion


        #endregion


        #region Map Contents


        #endregion


        #region ICloneable Members


        public object Clone()
        {
            Map map = new Map();

            map.AssetName = AssetName;
            map.baseLayer = BaseLayer.Clone() as int[];
            map.collisionLayer = CollisionLayer.Clone() as int[];
            //map.combatMusicCueName = CombatMusicCueName;
            map.fringeLayer = FringeLayer.Clone() as int[];
            map.mapDimensions = MapDimensions;
            //map.musicCueName = MusicCueName;
            map.name = Name;
            map.objectLayer = ObjectLayer.Clone() as int[];
            map.spawnMapPosition = SpawnMapPosition;
            map.texture = Texture;
            map.textureName = TextureName;
            map.tileSize = TileSize;
            map.tilesPerRow = tilesPerRow;

            return map;
        }


        #endregion


        #region Content Type Reader


        /// <summary>
        /// Read a Map object from the content pipeline.
        /// </summary>
        public class MapReader : ContentTypeReader<Map>
        {
            protected override Map Read(ContentReader input, Map existingInstance)
            {
                Map map = existingInstance;
                if (map == null)
                {
                    map = new Map();
                }

                map.AssetName = input.AssetName;

                map.Name = input.ReadString();
                map.MapDimensions = input.ReadObject<Point>();
                map.TileSize = input.ReadObject<Point>();
                map.SpawnMapPosition = input.ReadObject<Point>();

                map.TextureName = input.ReadString();
                map.texture = input.ContentManager.Load<Texture2D>(
                    System.IO.Path.Combine(@"Textures\Maps",
                    map.TextureName));
                map.tilesPerRow = map.texture.Width / map.TileSize.X;

                //map.MusicCueName = input.ReadString();
                //map.CombatMusicCueName = input.ReadString();

                map.BaseLayer = input.ReadObject<int[]>();
                map.FringeLayer = input.ReadObject<int[]>();
                map.ObjectLayer = input.ReadObject<int[]>();
                map.CollisionLayer = input.ReadObject<int[]>();

                return map;
            }
        }


        #endregion
    }
}
