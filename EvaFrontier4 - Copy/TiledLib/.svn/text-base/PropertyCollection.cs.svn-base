using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace TiledLib
{
	/// <summary>
	/// An enumerable collection of properties.
	/// </summary>
	public class PropertyCollection : IEnumerable<Property>
	{
		// cheating under the hood :)
		private readonly Dictionary<string, Property> values = new Dictionary<string, Property>();

		/// <summary>
		/// Gets a Property with the given name.
		/// </summary>
		/// <param name="name">The name of the property to retrieve.</param>
		/// <returns>The Property if a matching one is found or null if no Property exists for the given name.</returns>
		public Property this[string name]
		{
			get 
			{
				Property p;
				if (values.TryGetValue(name, out p))
					return p;
				return null;
			}
		}

		/// <summary>
		/// Creates a new PropertyCollection.
		/// </summary>
		public PropertyCollection() { }

		/// <summary>
		/// Adds a property to the collection.
		/// </summary>
		/// <param name="property">The property to add.</param>
		public void Add(Property property)
		{
			values.Add(property.Name, property);
		}

		/// <summary>
		/// Attempts to get a property by name.
		/// </summary>
		/// <param name="name">The name of the property to retrieve.</param>
		/// <param name="property">The property that is found, if one matches.</param>
		/// <returns>True if the property was found, false otherwise.</returns>
		public bool TryGetValue(string name, out Property property)
		{
			return values.TryGetValue(name, out property);
		}

		/// <summary>
		/// Removes a property with the given name.
		/// </summary>
		/// <param name="name">The name of the property to remove.</param>
		/// <returns>True if the property was removed, false otherwise.</returns>
		public bool Remove(string name)
		{
			return values.Remove(name);
		}

		// internal constructor because games shouldn't make their own PropertyCollections
		internal PropertyCollection(ContentReader reader)
		{
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				string key = reader.ReadString();
				string value = reader.ReadString();

				values.Add(key, new Property(key, value));
			}
		}

		/// <summary>
		/// Gets an enumerator that can be used to iterate over the properties in the collection.
		/// </summary>
		/// <returns>An enumerator over the properties.</returns>
		public IEnumerator<Property> GetEnumerator()
		{
			return values.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return values.Values.GetEnumerator();
		}
	}
}
