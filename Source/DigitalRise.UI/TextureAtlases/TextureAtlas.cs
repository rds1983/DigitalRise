using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DigitalRise.UI.TextureAtlases
{
	public class TextureAtlas
	{
		private const string ImageName = "Image";
		private const string NinePatchRegionName = "NinePatchRegion";
		private const string LeftName = "Left";
		private const string TopName = "Top";
		private const string WidthName = "Width";
		private const string HeightName = "Height";
		private const string NinePatchLeftName = "NinePatchLeft";
		private const string NinePatchTopName = "NinePatchTop";
		private const string NinePatchRightName = "NinePatchRight";
		private const string NinePatchBottomName = "NinePatchBottom";

		public Texture2D Texture { get; private set; }
		public readonly Dictionary<string, TextureRegion> TextureRegions = new Dictionary<string, TextureRegion>();

		public TextureRegion this[string id]
		{
			get => TextureRegions[id];
			set => TextureRegions[id] = value;
		}

		public static TextureAtlas FromXml(string xml, Func<string, Texture2D> textureLoader)
		{
			var doc = XDocument.Parse(xml);

			var root = doc.Root;

			var result = new TextureAtlas();
			var imageFileAttr = root.Attribute(ImageName);
			if (imageFileAttr == null)
			{
				throw new Exception("Mandatory attribute 'ImageFile' doesnt exist");
			}

			var texture = textureLoader(imageFileAttr.Value);
			result.Texture = texture;
			foreach (XElement entry in root.Elements())
			{
				var id = entry.Attribute("Id").Value;

				var bounds = new Rectangle(
					int.Parse(entry.Attribute(LeftName).Value),
					int.Parse(entry.Attribute(TopName).Value),
					int.Parse(entry.Attribute(WidthName).Value),
					int.Parse(entry.Attribute(HeightName).Value)
				);

				var isNinePatch = entry.Name == NinePatchRegionName;

				TextureRegion region;
				if (!isNinePatch)
				{
					region = new TextureRegion(texture, bounds);
				}
				else
				{
					var padding = new Padding
					{
						Left = int.Parse(entry.Attribute(NinePatchLeftName).Value),
						Top = int.Parse(entry.Attribute(NinePatchTopName).Value),
						Right = int.Parse(entry.Attribute(NinePatchRightName).Value),
						Bottom = int.Parse(entry.Attribute(NinePatchBottomName).Value)
					};

					region = new NinePatchRegion(texture, bounds, padding);
				}

				result.TextureRegions[id] = region;
			}

			return result;
		}
	}
}
