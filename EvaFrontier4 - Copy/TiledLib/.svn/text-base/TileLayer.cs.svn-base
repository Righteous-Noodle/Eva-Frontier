namespace TiledLib
{
	/// <summary>
	/// A map layer containing tiles.
	/// </summary>
	public class TileLayer : Layer
	{
		/// <summary>
		/// Gets the layout of tiles on the layer.
		/// </summary>
		public TileGrid Tiles { get; private set; }

		internal TileLayer(string name, int width, int height, bool visible, float opacity, PropertyCollection properties, Map map, int[] data, bool makeUnique)
			: base(name, width, height, visible, opacity, properties)
		{
			Tiles = new TileGrid(width, height);

			// data is left-to-right, top-to-bottom
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int index = data[y * width + x];
					
					// get the tile
					Tile t = map.Tiles[index];

					// if the tile is non-null and we want unique instances, clone it.
					if (t != null && makeUnique)
						t = t.Clone();

					// put that tile in our grid
					Tiles[x, y] = t;
				}
			}
		}
	}
}
