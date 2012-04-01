using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TiledLib
{
	/// <summary>
	/// A delegate used for searching for map objects.
	/// </summary>
	/// <param name="layer">The current layer.</param>
	/// <param name="mapObj">The current object.</param>
	/// <returns>True if this is the map object desired, false otherwise.</returns>
	public delegate bool MapObjectFinder(MapObjectLayer layer, MapObject mapObj);

	/// <summary>
	/// A full map from Tiled.
	/// </summary>
	public class Map
	{
		private readonly Dictionary<string, Layer> namedLayers = new Dictionary<string, Layer>();
		
		/// <summary>
		/// Gets the version of Tiled used to create the Map.
		/// </summary>
		public Version Version { get; private set; }

		/// <summary>
		/// Gets the orientation of the map.
		/// </summary>
		public Orientation Orientation { get; private set; }

		/// <summary>
		/// Gets the width (in tiles) of the map.
		/// </summary>
		public int Width { get; private set; }

		/// <summary>
		/// Gets the height (in tiles) of the map.
		/// </summary>
		public int Height { get; private set; }

		/// <summary>
		/// Gets the width of a tile in the map.
		/// </summary>
		public int TileWidth { get; private set; }

		/// <summary>
		/// Gets the height of a tile in the map.
		/// </summary>
		public int TileHeight { get; private set; }

		/// <summary>
		/// Gets a list of the map's properties.
		/// </summary>
		public PropertyCollection Properties { get; private set; }

		/// <summary>
		/// Gets a collection of all of the tiles in the map.
		/// </summary>
		public ReadOnlyCollection<Tile> Tiles { get; private set; }

		/// <summary>
		/// Gets a collection of all of the layers in the map.
		/// </summary>
		public ReadOnlyCollection<Layer> Layers { get; private set; }
	
		internal Map(ContentReader reader) 
		{
			// read in the basic map information
			Version = new Version(reader.ReadString());
			Orientation = (Orientation)reader.ReadByte();
			Width = reader.ReadInt32();
			Height = reader.ReadInt32();
			TileWidth = reader.ReadInt32();
			TileHeight = reader.ReadInt32();
			Properties = new PropertyCollection(reader);
			bool makeTilesUnique = reader.ReadBoolean();

			// create a list for our tiles
			List<Tile> tiles = new List<Tile>();
			Tiles = new ReadOnlyCollection<Tile>(tiles);

			// read in each tile set
			int numTileSets = reader.ReadInt32();
			for (int i = 0; i < numTileSets; i++)
			{
				// get the id and texture
				int firstId = reader.ReadInt32();
				Texture2D texture = reader.ReadExternalReference<Texture2D>();

				// read in each individual tile
				int numTiles = reader.ReadInt32();
				for (int j = 0; j < numTiles; j++)
				{
					int id = firstId + j;
					Rectangle source = reader.ReadObject<Rectangle>();
					PropertyCollection props = new PropertyCollection(reader);

					Tile t = new Tile(texture, source, props);
					while (id >= tiles.Count)
					{
						tiles.Add(null);
					}
					tiles.Insert(id, t);
				}
			}

			// read in all the layers
			List<Layer> layers = new List<Layer>();
			Layers = new ReadOnlyCollection<Layer>(layers);
			int numLayers = reader.ReadInt32();
			for (int i = 0; i < numLayers; i++)
			{
				Layer layer = null;

				// read generic layer data
				string type = reader.ReadString();
				string name = reader.ReadString();
				int width = reader.ReadInt32();
				int height = reader.ReadInt32();
				bool visible = reader.ReadBoolean();
				float opacity = reader.ReadSingle();
				PropertyCollection props = new PropertyCollection(reader);

				// using the type, figure out which object to create
				if (type == "layer")
				{
					int[] data = reader.ReadObject<int[]>();
					layer = new TileLayer(name, width, height, visible, opacity, props, this, data, makeTilesUnique);
				}
				else if (type == "objectgroup")
				{
					List<MapObject> objects = new List<MapObject>();

					// read in all of our objects
					int numObjects = reader.ReadInt32();
					for (int j = 0; j < numObjects; j++)
					{
						string objName = reader.ReadString();
						string objType = reader.ReadString();
						Rectangle objLoc = reader.ReadObject<Rectangle>();
						PropertyCollection objProps = new PropertyCollection(reader);

						objects.Add(new MapObject(objName, objType, objLoc, objProps));
					}

					layer = new MapObjectLayer(name, width, height, visible, opacity, props, objects);

					// read in the layer's color
					(layer as MapObjectLayer).Color = reader.ReadColor();
				}
				else
				{
					throw new Exception("Invalid type: " + type);
				}

				layers.Add(layer);
				namedLayers.Add(name, layer);
			}
		}
		
		/// <summary>
		/// Converts a point in world space into tile indices that can be used to index into a TileLayer.
		/// </summary>
		/// <param name="worldPoint">The point in world space to convert into tile indices.</param>
		/// <returns>A Point containing the X/Y indices of the tile that contains the point.</returns>
		public Point WorldPointToTileIndex(Vector2 worldPoint)
		{
			if (worldPoint.X < 0 || worldPoint.Y < 0 || worldPoint.X > Width * TileWidth || worldPoint.Y > Height * TileHeight)
			{
				throw new ArgumentOutOfRangeException("worldPoint");
			}

			Point p = new Point();
			
			// simple conversion to tile indices
			p.X = (int)Math.Floor(worldPoint.X / TileWidth);
			p.Y = (int)Math.Floor(worldPoint.Y / TileHeight);

			// check the upper limit edges. if we are on the edge, we need to decrement the index to keep in bounds.
			if (worldPoint.X == Width * TileWidth)
			{
				p.X--;
			}
			if (worldPoint.Y == Height * TileHeight)
			{
				p.Y--;
			}

			return p;
		}

		/// <summary>
		/// Returns the set of all objects in the map.
		/// </summary>
		/// <returns>A new set of all objects in the map.</returns>
		public IEnumerable<MapObject> GetAllObjects()
		{
			foreach (var layer in Layers)
			{
				MapObjectLayer objLayer = layer as MapObjectLayer;
				if (objLayer == null)
					continue;

				foreach (var obj in objLayer.Objects)
				{
					yield return obj;
				}
			}
		}

		/// <summary>
		/// Finds an object in the map using a delegate.
		/// </summary>
		/// <remarks>
		/// This method is used when an object is desired, but there is no specific
		/// layer to find the object on. The delegate allows the caller to create 
		/// any logic they want for finding the object. A simple example for finding
		/// the first object named "goal" in any layer would be this:
		/// 
		/// var goal = map.FindObject((layer, obj) => return obj.Name.Equals("goal"));
		/// 
		/// You could also use the layer name or any other logic to find an object.
		/// The first object for which the delegate returns true is the object returned
		/// to the caller. If the delegate never returns true, the method returns null.
		/// </remarks>
		/// <param name="finder">The delegate used to search for the object.</param>
		/// <returns>The MapObject if the delegate returned true, null otherwise.</returns>
		public MapObject FindObject(MapObjectFinder finder)
		{
			foreach (var layer in Layers)
			{
				MapObjectLayer objLayer = layer as MapObjectLayer;
				if (objLayer == null)
					continue;

				foreach (var obj in objLayer.Objects)
				{
					if (finder(objLayer, obj))
						return obj;
				}
			}

			return null;
		}

		/// <summary>
		/// Finds a collection of objects in the map using a delegate.
		/// </summary>
		/// <remarks>
		/// This method performs basically the same process as FindObject, but instead
		/// of returning the first object for which the delegate returns true, it returns
		/// a collection of all objects for which the delegate returns true.
		/// </remarks>
		/// <param name="finder">The delegate used to search for the object.</param>
		/// <returns>A collection of all MapObjects for which the delegate returned true.</returns>
		public IEnumerable<MapObject> FindObjects(MapObjectFinder finder)
		{
			foreach (var layer in Layers)
			{
				MapObjectLayer objLayer = layer as MapObjectLayer;
				if (objLayer == null)
					continue;

				foreach (var obj in objLayer.Objects)
				{
					if (finder(objLayer, obj))
						yield return obj;
				}
			}
		}

		/// <summary>
		/// Gets a layer by name.
		/// </summary>
		/// <param name="name">The name of the layer to retrieve.</param>
		/// <returns>The layer with the given name.</returns>
		public Layer GetLayer(string name)
		{
			return namedLayers[name];
		}

		/// <summary>
		/// Performs a basic rendering of the map.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch to use to render the map.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			Draw(spriteBatch, new Rectangle(0, 0, Width * TileWidth, Height * TileHeight));
		}

		/// <summary>
		/// Draws an area of the map defined in world space (pixel) coordinates.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch to use to render the map.</param>
		/// <param name="worldArea">The area of the map to draw in world coordinates.</param>
		public void Draw(SpriteBatch spriteBatch, Rectangle worldArea)
		{
			if (spriteBatch == null)
				throw new ArgumentNullException("spriteBatch");

			if (Orientation == Orientation.Orthogonal)
			{
				// figure out the min and max tile indices to draw
				int minX = Math.Max((int)Math.Floor((float)worldArea.Left / TileWidth), 0);
				int maxX = Math.Min((int)Math.Ceiling((float)worldArea.Right / TileWidth), Width);

				int minY = Math.Max((int)Math.Floor((float)worldArea.Top / TileHeight), 0);
				int maxY = Math.Min((int)Math.Ceiling((float)worldArea.Bottom / TileHeight), Height);

				foreach (var l in Layers)
				{
					if (!l.Visible)
						continue;

					TileLayer tileLayer = l as TileLayer;
					if (tileLayer != null)
					{
						for (int x = minX; x < maxX; x++)
						{
							for (int y = minY; y < maxY; y++)
							{
								Tile tile = tileLayer.Tiles[x, y];

								if (tile == null)
									continue;

								Rectangle r = new Rectangle(x * TileWidth, y * TileHeight, TileWidth, TileHeight);
								tile.DrawOrthographic(spriteBatch, r, tileLayer.Opacity);
							}
						}
					}
				}
			}
			else
			{
				throw new NotSupportedException("TiledLib does not have built in support for rendering isometric tile maps.");
			}
		}
	}
}
