using System.ComponentModel;
using System.Drawing;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace TiledPipelineExtensions
{
	[ContentProcessor(DisplayName = "TMX Processor")]
	public class TmxProcessor : ContentProcessor<MapContent, MapContent>
	{
		[DisplayName("TileSet Directory")]
		[Description("The directory (relative to the content root) in which the processor will find the tile sheet images. If left blank, the tile sheets are assumed to be in the folder with the map.")]
		public string TileSetDirectory { get; set; }

		[DisplayName("Make Tiles Unique")]
		[DefaultValue(false)]
		[Description("If true, all tiles are cloned when the map is loaded so each tile space is a unique Tile object. If false, Tile instances are shared across the map.")]
		public bool MakeTilesUnique { get; set; }

		public TmxProcessor()
		{
			MakeTilesUnique = false;
		}

		public override MapContent Process(MapContent input, ContentProcessorContext context)
		{
			// make sure we pass this value to the map
			input.MakeTilesUnique = MakeTilesUnique;

			// do some processing on tile sets to load external textures and figure out tile regions
			foreach (var tileSet in input.TileSets)
			{
				// get the full path to the file
				string path = string.Empty;

				// if there is no specified tile set directory, we assume the tile sheet texture
				// is in the directory with the map
				if (string.IsNullOrEmpty(TileSetDirectory))
				{
					path = Path.Combine(input.Directory, tileSet.Image);
				}

				// otherwise we build the path using the TileSetDirectory relative to the Content root directory
				else
				{
					path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine(TileSetDirectory, tileSet.Image));
				}

				// the asset name is the entire path, minus extension, after the content directory
				string asset = string.Empty;
				if (path.StartsWith(Directory.GetCurrentDirectory()))
					asset = path.Remove(path.LastIndexOf('.')).Substring(Directory.GetCurrentDirectory().Length + 1);
				else
					asset = Path.GetFileNameWithoutExtension(path);

				// build the asset as an external reference
				OpaqueDataDictionary data = new OpaqueDataDictionary();
				data.Add("GenerateMipmaps", false);
				data.Add("ResizeToPowerOfTwo", false);
				data.Add("TextureFormat", TextureProcessorOutputFormat.Color);
				data.Add("ColorKeyEnabled", tileSet.ColorKey.HasValue);
				data.Add("ColorKeyColor", tileSet.ColorKey.HasValue ? tileSet.ColorKey.Value : Microsoft.Xna.Framework.Color.Magenta);
				tileSet.Texture = context.BuildAsset<TextureContent, TextureContent>(
					new ExternalReference<TextureContent>(path),
					"TextureProcessor",
					data,
					"TextureImporter",
					asset);

				// load the image so we can compute the individual tile source rectangles
				int imageWidth = 0;
				int imageHeight = 0;
				using (Image image = Image.FromFile(path))
				{
					imageWidth = image.Width;
					imageHeight = image.Height;
				}

				// remove the margins from our calculations
				imageWidth -= tileSet.Margin * 2;
				imageHeight -= tileSet.Margin * 2;

				// figure out how many frames fit on the X axis
				int frameCountX = 1;
				while (frameCountX * tileSet.TileWidth < imageWidth)
				{
					frameCountX++;
					imageWidth -= tileSet.Spacing;
				}

				// figure out how many frames fit on the Y axis
				int frameCountY = 1;
				while (frameCountY * tileSet.TileHeight < imageHeight)
				{
					frameCountY++;
					imageHeight -= tileSet.Spacing;
				}

				// make our tiles. tiles are numbered by row, left to right.
				for (int y = 0; y < frameCountY; y++)
				{
					for (int x = 0; x < frameCountX; x++)
					{
						Tile tile = new Tile();

						// calculate the source rectangle
						int rx = tileSet.Margin + x * (tileSet.TileWidth + tileSet.Spacing);
						int ry = tileSet.Margin + y * (tileSet.TileHeight + tileSet.Spacing);
						tile.Source = new Microsoft.Xna.Framework.Rectangle(rx, ry, tileSet.TileWidth, tileSet.TileHeight);

						// get any properties from the tile set
						int index = tileSet.FirstId + (y * frameCountX + x);
						if (tileSet.TileProperties.ContainsKey(index))
						{
							tile.Properties = tileSet.TileProperties[index];
						}

						// save the tile
						tileSet.Tiles.Add(tile);
					}
				}
			}

			return input;
		}
	}
}
