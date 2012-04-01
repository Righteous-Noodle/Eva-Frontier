using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace TiledLib
{
	/// <summary>
	/// A layer comprised of objects.
	/// </summary>
	public class MapObjectLayer : Layer
	{
		private readonly List<MapObject> objects = new List<MapObject>();
		private readonly Dictionary<string, MapObject> namedObjects = new Dictionary<string, MapObject>();

		/// <summary>
		/// Gets or sets this layer's color.
		/// </summary>
		public Color Color { get; set; }

		/// <summary>
		/// Gets the objects on the current layer.
		/// </summary>
		public ReadOnlyCollection<MapObject> Objects { get; private set; }

		internal MapObjectLayer(string name, int width, int height, bool visible, float opacity, PropertyCollection properties, List<MapObject> initialObjects)
			: base(name, width, height, visible, opacity, properties)
		{
			Objects = new ReadOnlyCollection<MapObject>(objects);
			initialObjects.ForEach(AddObject);
		}

		/// <summary>
		/// Adds a MapObject to the layer.
		/// </summary>
		/// <param name="mapObject">The MapObject to add.</param>
		public void AddObject(MapObject mapObject)
		{
			// avoid adding the object to the layer twice
			if (objects.Contains(mapObject))
				return;

			namedObjects.Add(mapObject.Name, mapObject);
			objects.Add(mapObject);
		}

		/// <summary>
		/// Gets a MapObject by name.
		/// </summary>
		/// <param name="objectName">The name of the object to retrieve.</param>
		/// <returns>The MapObject with the given name.</returns>
		public MapObject GetObject(string objectName)
		{
			return namedObjects[objectName];
		}

		/// <summary>
		/// Removes an object from the layer.
		/// </summary>
		/// <param name="mapObject">The object to remove.</param>
		/// <returns>True if the object was found and removed, false otherwise.</returns>
		public bool RemoveObject(MapObject mapObject)
		{
			return RemoveObject(mapObject.Name);
		}

		/// <summary>
		/// Removes an object from the layer.
		/// </summary>
		/// <param name="objectName">The name of the object to remove.</param>
		/// <returns>True if the object was found and removed, false otherwise.</returns>
		public bool RemoveObject(string objectName)
		{
			MapObject obj;
			if (namedObjects.TryGetValue(objectName, out obj))
			{
				objects.Remove(obj);
				namedObjects.Remove(objectName);
				return true;
			}
			return false;
		}
	}
}
