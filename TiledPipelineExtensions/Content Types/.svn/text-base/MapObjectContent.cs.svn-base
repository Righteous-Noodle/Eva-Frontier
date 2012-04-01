using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TiledPipelineExtensions
{
	public class MapObjectContent
	{
		public string Name = string.Empty;
		public string Type = string.Empty;
		public Rectangle Location;
		public List<Property> Properties = new List<Property>();

		public MapObjectContent(XmlNode node, ContentImporterContext context)
		{
			if (node.Attributes["name"] != null)
			{
				Name = node.Attributes["name"].Value;
			}

			if (node.Attributes["type"] != null)
			{
				Type = node.Attributes["type"].Value;
			}

			// values default to 0 if the attribute is missing from the node
			int x = node.Attributes["x"] != null ? int.Parse(node.Attributes["x"].Value, CultureInfo.InvariantCulture) : 0;
			int y = node.Attributes["y"] != null ? int.Parse(node.Attributes["y"].Value, CultureInfo.InvariantCulture) : 0;
			int width = node.Attributes["width"] != null ? int.Parse(node.Attributes["width"].Value, CultureInfo.InvariantCulture) : 0;
			int height = node.Attributes["height"] != null ? int.Parse(node.Attributes["height"].Value, CultureInfo.InvariantCulture) : 0;

			Location = new Rectangle(x, y, width, height);

			XmlNode propertiesNode = node["properties"];
			if (propertiesNode != null)
			{
				Properties = Property.ReadProperties(propertiesNode, context);
			}
		}
	}
}
 