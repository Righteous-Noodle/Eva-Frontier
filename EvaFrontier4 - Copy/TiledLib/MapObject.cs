using Microsoft.Xna.Framework;
using System;

namespace TiledLib
{
	/// <summary>
	/// An arbitrary object placed on an ObjectLayer.
	/// </summary>
	public class MapObject
	{
		/// <summary>
		/// Gets the name of the object.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the type of the object.
		/// </summary>
		public string Type { get; private set; }

		/// <summary>
		/// Gets or sets the bounds of the object.
		/// </summary>
		public Rectangle Bounds { get; set; }

		/// <summary>
		/// Gets a list of the object's properties.
		/// </summary>
		public PropertyCollection Properties { get; private set; }

		/// <summary>
		/// Creates a new MapObject.
		/// </summary>
		/// <param name="name">The name of the object.</param>
		/// <param name="type">The type of object to create.</param>
		public MapObject(string name, string type) : this(name, type, new Rectangle(), new PropertyCollection()) { }

		/// <summary>
		/// Creates a new MapObject.
		/// </summary>
		/// <param name="name">The name of the object.</param>
		/// <param name="type">The type of object to create.</param>
		/// <param name="bounds">The initial bounds of the object.</param>
		public MapObject(string name, string type, Rectangle bounds) : this(name, type, bounds, new PropertyCollection()) { }

		/// <summary>
		/// Creates a new MapObject.
		/// </summary>
		/// <param name="name">The name of the object.</param>
		/// <param name="type">The type of object to create.</param>
		/// <param name="bounds">The initial bounds of the object.</param>
		/// <param name="properties">The initial property collection or null to create an empty property collection.</param>
		public MapObject(string name, string type, Rectangle bounds, PropertyCollection properties)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException(null, "name");

			Name = name;
			Type = type;
			Bounds = bounds;
			Properties = properties ?? new PropertyCollection();
		}
	}
}
