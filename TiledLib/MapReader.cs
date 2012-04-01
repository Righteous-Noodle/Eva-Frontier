using Microsoft.Xna.Framework.Content;

namespace TiledLib
{
	/// <summary>
	/// Reads in a Map from an XNB through a ContentManager.
	/// </summary>
	public sealed class MapReader : ContentTypeReader<Map>
	{
		/// <summary>
		/// Reads a map from the ContentReader.
		/// </summary>
		/// <param name="input">The ContentReader for reading the file.</param>
		/// <param name="existingInstance">The existing Map instance.</param>
		/// <returns>A new Map instance.</returns>
		protected override Map Read(ContentReader input, Map existingInstance)
		{
			// we defer all loading to the Map class itself so as to remove
			// the need to make Map properties 'public' or 'internal' set
			return new Map(input);
		}
	}
}
