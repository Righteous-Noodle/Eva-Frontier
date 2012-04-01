using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TiledLib
{
	/// <summary>
	/// A single tile in a TileLayer.
	/// </summary>
	public class Tile
	{
		/// <summary>
		/// Gets the Texture2D to use when drawing the tile.
		/// </summary>
		public Texture2D Texture { get; private set; }

		/// <summary>
		/// Gets the source rectangle of the tile.
		/// </summary>
		public Rectangle Source { get; private set; }

		/// <summary>
		/// Gets the collection of properties for the tile.
		/// </summary>
		public PropertyCollection Properties { get; private set; }

		/// <summary>
		/// Gets or sets a color associated with the tile.
		/// </summary>
		public Color Color { get; set; }

		/// <summary>
		/// Creates a new Tile object.
		/// </summary>
		/// <param name="texture">The texture that contains the tile image.</param>
		/// <param name="source">The source rectangle of the tile.</param>
		public Tile(Texture2D texture, Rectangle source) : this(texture, source, new PropertyCollection()) { }

		/// <summary>
		/// Creates a new Tile object.
		/// </summary>
		/// <param name="texture">The texture that contains the tile image.</param>
		/// <param name="source">The source rectangle of the tile.</param>
		/// <param name="properties">The initial property collection or null to create an empty property collection.</param>
		public Tile(Texture2D texture, Rectangle source, PropertyCollection properties)
		{
			if (texture == null)
				throw new ArgumentNullException("texture");

			Texture = texture;
			Source = source;
			Properties = properties ?? new PropertyCollection();
			Color = Color.White;
		}

		/// <summary>
		/// Creates a copy of the current tile.
		/// </summary>
		/// <returns>A new Tile with the same properties as the current tile.</returns>
		public virtual Tile Clone()
		{
			return new Tile(Texture, Source, Properties);
		}

		/// <summary>
		/// Draws the tile with an orthographic perspective.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch to use when rendering the tile.</param>
		/// <param name="destBounds">The destination bounds where the tile should be drawn.</param>
		/// <param name="opacity">The level of opacity to use when drawing the tile.</param>
		public virtual void DrawOrthographic(SpriteBatch spriteBatch, Rectangle destBounds, float opacity)
		{
			spriteBatch.Draw(Texture, destBounds, Source, Color * opacity);
		}
	}
}
