using StbImageSharp;
using StbImageWriteSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Xml;
using System.Xml.Linq;

namespace SplitAtlas
{
	internal static class Program
	{
		/// <summary>
		/// Describes how a child element is horizontally positioned or stretched within a parent's layout
		/// slot. 
		/// </summary>
		public enum HorizontalAlignment
		{
			/// <summary>
			/// The child element is aligned to the left of the parent's layout slot. 
			/// </summary>
			Left,

			/// <summary>
			/// The child element is aligned to the center of the parent's layout slot.
			/// </summary>
			Center,

			/// <summary>
			/// The child element is aligned to the right of the parent's layout slot.
			/// </summary>
			Right,

			/// <summary>
			/// The child element stretches to fill the parent's layout slot. 
			/// </summary>
			Stretch
		}

		/// <summary>
		/// Describes how a child element is vertically positioned or stretched within a parent's layout
		/// slot. 
		/// </summary>
		public enum VerticalAlignment
		{
			/// <summary>
			/// The child element is aligned to the top of the parent's layout slot.
			/// </summary>
			Top,

			/// <summary>
			/// The child element is aligned to the center of the parent's layout slot.
			/// </summary>
			Center,

			/// <summary>
			/// The child element is aligned to the bottom of the parent's layout slot.
			/// </summary>
			Bottom,

			/// <summary>
			/// The child element stretches to fill the parent's layout slot. 
			/// </summary>
			Stretch
		}

		// Accepted list formats are:
		//   "R,G,B,A"
		//   "R;G;B;A"
		//   "R G B A"
		private static readonly char[] ListSeparators = { ',', ';', ' ' };

		class ThemeState
		{
			public string Name;
			public bool IsInherited;
			public List<ThemeImage> Images = new List<ThemeImage>();
		}

		class ThemeImage
		{
			public string Name;
			public Rectangle SourceRectangle;
			public Padding Border;
			public HorizontalAlignment HorizontalAlignment;
			public VerticalAlignment VerticalAlignment;
			public Color Color;
			public ImageResult Texture;
		}

		class ThemeStyle
		{
			public string Name;
			public List<ThemeState> States = new List<ThemeState>();
			public Rectangle? IconSourceRectangle;
		}

		class Theme
		{
			public ImageResult DefaultTexture;
			public Dictionary<string, ImageResult> Textures = new Dictionary<string, ImageResult>();
			public Dictionary<string, ThemeStyle> Styles = new Dictionary<string, ThemeStyle>();
		}

		struct Padding
		{
			public static readonly Padding Zero = new Padding
			{
				Left = 0,
				Top = 0,
				Right = 0,
				Bottom = 0
			};

			public int Left, Top, Right, Bottom;

			public static bool operator ==(Padding left, Padding right)
			{
				return left.Left == right.Left &&
					left.Top == right.Top &&
					left.Right == right.Right &&
					left.Bottom == right.Bottom;
			}

			public static bool operator !=(Padding left, Padding right)
			{
				return !(left == right);
			}

			public override string ToString()
			{
				return $"{Left}, {Top}, {Right}, {Bottom}";
			}
		}

		struct ImageInfo
		{
			public Rectangle Rectangle;
			public Padding Padding;
			public bool IsNinePatch;
			public string StyleName;
			public string StateName;
			public string ImageName;

			public static bool operator ==(ImageInfo left, ImageInfo right)
			{
				return left.Rectangle == right.Rectangle &&
					left.Padding == right.Padding &&
					left.IsNinePatch == right.IsNinePatch;
			}

			public static bool operator !=(ImageInfo left, ImageInfo right)
			{
				return !(left == right);
			}

			public override string ToString()
			{
				return $"{Rectangle.ToString()}; {Padding.ToString()}; {IsNinePatch}";
			}
		}

		/// <summary>
		/// Converts the specified string representation of a horizontal alignment to its 
		/// <see cref="HorizontalAlignment"/> equivalent, or throws an exception if the string cannot be
		/// converted to a <see cref="HorizontalAlignment"/>.
		/// </summary>
		/// <param name="value">
		/// The value. If this value is <see langword="null"/> or an empty string, 
		/// <see cref="HorizontalAlignment.Left"/> is returned as the default value.
		/// </param>
		/// <returns>The <see cref="HorizontalAlignment"/>.</returns>
		/// <exception cref="FormatException">
		/// Cannot convert <paramref name="value"/> to <see cref="HorizontalAlignment"/>.
		/// </exception>
		public static HorizontalAlignment ParseHorizontalAlignment(string value)
		{
			if (string.IsNullOrEmpty(value))
				return HorizontalAlignment.Left;

			switch (value.Trim().ToUpperInvariant())
			{
				case "LEFT": return HorizontalAlignment.Left;
				case "CENTER": return HorizontalAlignment.Center;
				case "RIGHT": return HorizontalAlignment.Right;
				case "STRETCH": return HorizontalAlignment.Stretch;
				default:
					string message = string.Format(CultureInfo.InvariantCulture, "Could not parse horizontal alignment: '{0}'", value);
					throw new FormatException(message);
			}
		}

		/// <summary>
		/// Converts the specified string representation of a vertical alignment to its 
		/// <see cref="VerticalAlignment"/> equivalent, or throws an exception if the string cannot be
		/// converted to a <see cref="VerticalAlignment"/>.
		/// </summary>
		/// <param name="value">
		/// The value. If this value is <see langword="null"/> or an empty string, 
		/// <see cref="VerticalAlignment.Top"/> is returned as the default value.
		/// </param>
		/// <returns>The <see cref="VerticalAlignment"/>.</returns>
		/// <exception cref="FormatException">
		/// Cannot convert <paramref name="value"/> to <see cref="VerticalAlignment"/>.
		/// </exception>
		public static VerticalAlignment ParseVerticalAlignment(string value)
		{
			if (string.IsNullOrEmpty(value))
				return VerticalAlignment.Top;

			switch (value.Trim().ToUpperInvariant())
			{
				case "TOP": return VerticalAlignment.Top;
				case "CENTER": return VerticalAlignment.Center;
				case "BOTTOM": return VerticalAlignment.Bottom;
				case "STRETCH": return VerticalAlignment.Stretch;
				default:
					string message = string.Format(CultureInfo.InvariantCulture, "Could not parse vertical alignment: '{0}'", value);
					throw new FormatException(message);
			}
		}

		/// <summary>
		/// Converts the specified string representation of a 4-dimensional vector to its 
		/// <see cref="Vector4"/> equivalent, or throws an exception if the string cannot be
		/// converted to a <see cref="Vector4"/>.
		/// </summary>
		/// <param name="value">
		/// The value. If this value is <see langword="null"/> or an empty string, 
		/// <see cref="Vector4.Zero"/> is returned as the default value.
		/// </param>
		/// <returns>The <see cref="Vector4"/>.</returns>
		/// <exception cref="FormatException">
		/// Cannot convert <paramref name="value"/> to <see cref="Vector4"/>.
		/// </exception>
		public static Vector4 ParseVector4(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Vector4.Zero;

			var values = value.Split(ListSeparators, StringSplitOptions.RemoveEmptyEntries);
			if (values.Length != 4)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "Could not parse 4-element vector: '{0}'", value);
				throw new FormatException(message);
			}

			Vector4 result;
			result.X = float.Parse(values[0], CultureInfo.InvariantCulture);
			result.Y = float.Parse(values[1], CultureInfo.InvariantCulture);
			result.Z = float.Parse(values[2], CultureInfo.InvariantCulture);
			result.W = float.Parse(values[3], CultureInfo.InvariantCulture);
			return result;
		}

		/// <summary>
		/// Converts the specified string representation of a rectangle to its <see cref="Rectangle"/> 
		/// equivalent, or throws an exception if the string cannot be converted to a 
		/// <see cref="Rectangle"/>.
		/// </summary>
		/// <param name="value">
		/// The value. If this value is <see langword="null"/> or an empty string, a rectangle is 
		/// returned where all values are 0.
		/// </param>
		/// <returns>
		/// The <see cref="Rectangle"/>.
		/// </returns>
		/// <exception cref="FormatException">
		/// Cannot convert <paramref name="value"/> to <see cref="Rectangle"/>.
		/// </exception>
		public static Rectangle ParseRectangle(string value)
		{
			try
			{
				Vector4 vector = ParseVector4(value);
				return new Rectangle((int)vector.X, (int)vector.Y, (int)vector.Z, (int)vector.W);
			}
			catch (Exception exception)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "Could not parse rectangle: '{0}'", value);
				throw new FormatException(message, exception);
			}
		}

		static Padding ParsePadding(string value)
		{
			var vector = ParseVector4(value);
			return new Padding
			{
				Left = (int)vector.X,
				Top = (int)vector.Y,
				Right = (int)vector.Z,
				Bottom = (int)vector.W,
			};
		}

		/// <summary>
		/// Converts the specified string representation of a color to its <see cref="Color"/> 
		/// equivalent, or throws an exception if the string cannot be converted to a 
		/// <see cref="Color"/>.
		/// </summary>
		/// <param name="value">
		/// The value. If this value is <see langword="null"/> or an empty string, the 
		/// <paramref name="defaultColor"/> is returned.
		/// </param>
		/// <param name="defaultColor">The default color that is used for empty strings.</param>
		/// <returns>The <see cref="Color"/>.</returns>
		/// <exception cref="FormatException">
		/// Cannot convert <paramref name="value"/> to <see cref="Color"/>.
		/// </exception>
		public static Color ParseColor(string value, Color defaultColor)
		{
			if (string.IsNullOrEmpty(value))
				return defaultColor;

			try
			{
				Vector4 vector = ParseVector4(value);
				return Color.FromArgb((byte)vector.W, (byte)vector.X, (byte)vector.Y, (byte)vector.Z);
			}
			catch (Exception exception)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "Could not parse color: '{0}'", value);
				throw new FormatException(message, exception);
			}
		}

		static void Log(string message)
		{
			Console.WriteLine(message);
		}

		private static string GetExceptionMessage(XElement element, string format, params object[] args)
		{
			string message = string.Format(format, args);

			var lineInfo = (IXmlLineInfo)element;
			if (lineInfo.HasLineInfo())
				message += string.Format(" (Element: \"{0}\", Line: {1}, Position {2})", element.Name, lineInfo.LineNumber, lineInfo.LinePosition);
			else
				message += string.Format(" (Element: \"{0}\")", element.Name);

			return message;
		}

		private static XAttribute EnsureAttribute(XElement element, string name)
		{
			var attribute = element.Attribute(name);
			if (attribute == null)
			{
				string message = GetExceptionMessage(element, "\"{0}\" attribute is missing.", name);
				throw new Exception(message);
			}

			return attribute;
		}

		private static string GetMandatoryAttributeString(XElement element, string name)
		{
			var attribute = EnsureAttribute(element, name);

			string s = (string)attribute;
			if (s.Length == 0)
			{
				string message = GetExceptionMessage(element, "\"{0}\" attribute must not be empty.", name);
				throw new Exception(message);
			}

			return s;
		}

		private static float GetMandatoryAttributeFloat(XElement element, string name)
		{
			var attribute = EnsureAttribute(element, name);

			return (float)attribute;
		}


		private static void ProcessTextures(Theme theme, XDocument document, string path)
		{
			if (document.Root.Elements("Texture").Any())
			{
				// Issue error because theme file is using old alpha version format.
				throw new Exception("Given theme file is using a format which is no longer supported. All textures need to be defined inside a 'Textures' node.");
			}

			var texturesElement = document.Root.Element("Textures");
			if (texturesElement == null)
				throw new Exception("Given theme file does not contain a 'Textures' node.");

			foreach (var textureElement in texturesElement.Elements("Texture"))
			{
				string name = GetMandatoryAttributeString(textureElement, "Name");
				bool isDefault = (bool?)textureElement.Attribute("IsDefault") ?? false;
				string filename = GetMandatoryAttributeString(textureElement, "File");
				bool premultiplyAlpha = (bool?)textureElement.Attribute("PremultiplyAlpha") ?? true;

				var texturePath = Path.Combine(path, filename);
				using (var stream = File.OpenRead(texturePath))
				{
					theme.Textures[name] = ImageResult.FromStream(stream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);
				}

				if (isDefault)
				{
					theme.DefaultTexture = theme.Textures[name];
				}
			}

			if (theme.Textures.Count == 0)
				throw new Exception("The UI theme does not contain any textures. At least 1 texture is required.");
		}

		private static void ProcessStyles(Theme theme, XDocument document)
		{
			var stylesElement = document.Root.Element("Styles");
			if (stylesElement == null)
				return;

			var inherits = new Dictionary<string, string>();
			foreach (var styleElement in stylesElement.Elements("Style"))
			{
				var style = new ThemeStyle
				{
					Name = GetMandatoryAttributeString(styleElement, "Name")
				};

				var inheritValue = (string)styleElement.Attribute("Inherits");
				if (!string.IsNullOrEmpty(inheritValue))
				{
					inherits[style.Name] = inheritValue;
				}

				foreach (var element in styleElement.Elements())
				{
					if (element.Name == "State")
					{
						var state = new ThemeState
						{
							Name = GetMandatoryAttributeString(element, "Name"),
							IsInherited = (bool?)element.Attribute("IsInherited") ?? false,
						};

						foreach (var imageElement in element.Elements("Image"))
						{
							var image = new ThemeImage
							{
								Name = (string)imageElement.Attribute("Name"),
								SourceRectangle = ParseRectangle((string)imageElement.Attribute("Source")),
								HorizontalAlignment = ParseHorizontalAlignment((string)imageElement.Attribute("HorizontalAlignment")),
								VerticalAlignment = ParseVerticalAlignment((string)imageElement.Attribute("VerticalAlignment")),
								Border = ParsePadding((string)imageElement.Attribute("Border")),
								Color = ParseColor((string)imageElement.Attribute("Color"), Color.White),
							};

							var imageTexture = (string)imageElement.Attribute("Texture");
							if (!string.IsNullOrEmpty(imageTexture))
							{
								ImageResult themeTexture;
								if (!theme.Textures.TryGetValue(imageTexture, out themeTexture))
								{
									string message = string.Format("Missing texture: The image '{0}' in state '{1}' of style '{2}' requires a texture named '{3}'.", image.Name, state.Name, style.Name, image.Texture);
									throw new Exception(message);
								}

								image.Texture = themeTexture;
							}
							else
							{
								image.Texture = theme.DefaultTexture;
							}

							state.Images.Add(image);
						}

						style.States.Add(state);
					}
					else if (element.Name == "IconSourceRectangle")
					{
						style.IconSourceRectangle = ParseRectangle((string)element.Value);
					}
				}

				theme.Styles[style.Name] = style;
			}
		}

		static string BuildKey(this Rectangle r) => $"{r.X}, {r.Y}, {r.Width}, {r.Height}";

		static void Process(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Usage: SplitAtlas <fileName>");
				return;
			}

			var path = Path.GetFullPath(args[0]);
			var document = XDocument.Parse(File.ReadAllText(path));

			var theme = new Theme();

			var folder = Path.GetDirectoryName(path);
			ProcessTextures(theme, document, folder);
			ProcessStyles(theme, document);

			// Validate borders
			var rects = new Dictionary<string, ImageInfo>();
			foreach (var pair in theme.Styles)
			{
				foreach (var state in pair.Value.States)
				{
					foreach (var image in state.Images)
					{
						var key = image.SourceRectangle.BuildKey();
						var imageInfo = new ImageInfo
						{
							Rectangle = image.SourceRectangle,
							Padding = image.Border,
							IsNinePatch = image.HorizontalAlignment == HorizontalAlignment.Stretch ||
								image.VerticalAlignment == VerticalAlignment.Stretch,
							StyleName = pair.Value.Name,
							StateName = state.Name,
							ImageName = image.Name
						};

						if (rects.TryGetValue(key, out ImageInfo oldImageInfo))
						{
							if (oldImageInfo != imageInfo)
							{
								throw new Exception($"ImageInfo changed. Rect: {key}. Old: {oldImageInfo}, New: {imageInfo}");
							}
						}

						rects[key] = imageInfo;
					}
				}

				if (pair.Value.IconSourceRectangle != null)
				{
					var key = pair.Value.IconSourceRectangle.Value.BuildKey();
					var imageInfo = new ImageInfo
					{
						Rectangle = pair.Value.IconSourceRectangle.Value,
						Padding = Padding.Zero,
						IsNinePatch = false,
						StyleName = pair.Value.Name,
						StateName = "IconSourceRectangle"
					};
					if (rects.TryGetValue(key, out ImageInfo oldImageInfo))
					{
						if (oldImageInfo != imageInfo)
						{
							throw new Exception($"ImageInfo changed. Rect: {key}. Old: {oldImageInfo}, New: {imageInfo}");
						}
					}

					rects[key] = imageInfo;
				}
			}

			// Write images
			var images = new Dictionary<string, string>();
			var imageWriter = new ImageWriter();
			var t = theme.DefaultTexture;
			foreach (var pair in rects)
			{
				var imageInfo = pair.Value;
				var r = imageInfo.Rectangle;

				var key = r.BuildKey();
				if (images.ContainsKey(key))
				{
					continue;
				}

				var width = r.Width;
				var height = r.Height;
				var data = new byte[r.Width * r.Height * 4];
				for (var x = 0; x < r.Width; x++)
				{
					for (var y = 0; y < r.Height; y++)
					{
						var destIndex = (y * r.Width + x) * 4;
						var srcIndex = ((r.Top + y) * t.Width + r.Left + x) * 4;
						for (var i = 0; i < 4; i++)
						{
							data[destIndex + i] = t.Data[srcIndex + i];
						}
					}
				}

				if (pair.Value.IsNinePatch)
				{
					var newWidth = r.Width + 2;
					var newHeight = r.Height + 2;

					var newData = new byte[newWidth * newHeight * 4];
					for (var x = 0; x < r.Width; x++)
					{
						for (var y = 0; y < r.Height; y++)
						{
							var srcIndex = (y * r.Width + x) * 4;
							var destIndex = ((1 + y) * newWidth + 1 + x) * 4;
							for (var i = 0; i < 4; i++)
							{
								newData[destIndex + i] = data[srcIndex + i];
							}
						}
					}

					// White border
					for (var x = 0; x < newWidth; ++x)
					{
						var destIndex = x * 4;
						newData[destIndex] = newData[destIndex + 1] =
							newData[destIndex + 2] = newData[destIndex + 3] = 255;

						destIndex = ((newHeight - 1) * newWidth + x) * 4;
						newData[destIndex] = newData[destIndex + 1] =
							newData[destIndex + 2] = newData[destIndex + 3] = 255;
					}

					for (var y = 0; y < newHeight; ++y)
					{
						var destIndex = y * newWidth * 4;
						newData[destIndex] = newData[destIndex + 1] =
							newData[destIndex + 2] = newData[destIndex + 3] = 255;

						destIndex = ((newWidth - 1) + y * newWidth) * 4;

						newData[destIndex] = newData[destIndex + 1] =
							newData[destIndex + 2] = newData[destIndex + 3] = 255;
					}

					// Black part
					var border = imageInfo.Padding;
					for (var x = border.Left; x < r.Width - border.Right; ++x)
					{
						var destIndex = (x + 1) * 4;

						newData[destIndex] = 0;
						newData[destIndex + 1] = 0;
						newData[destIndex + 2] = 0;
						newData[destIndex + 3] = 255;
					}

					for (var y = border.Top; y < r.Height - border.Bottom; ++y)
					{
						var destIndex = (y + 1) * newWidth * 4;
						newData[destIndex] = 0;
						newData[destIndex + 1] = 0;
						newData[destIndex + 2] = 0;
						newData[destIndex + 3] = 255;
					}

					width = newWidth;
					height = newHeight;
					data = newData;
				}

				string imagePath;

				var imageName = $"{imageInfo.StyleName}";
				if (!string.IsNullOrEmpty(imageInfo.StateName))
				{
					imageName += $"_{imageInfo.StateName}";
				}

				if (!string.IsNullOrEmpty(imageInfo.ImageName))
				{
					imageName += $"_{imageInfo.ImageName}";
				}

				if (!imageInfo.IsNinePatch)
				{
					imagePath = $"{imageName}.png";
				}
				else
				{
					imagePath = $"{imageName}.9.png";
				}

				Log($"Writing to {imagePath}");
				using (var stream = File.OpenWrite(imagePath))
				{
					imageWriter.WritePng(data, width, height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
				}

				images[key] = imageName;
			}

			// Update xml
			var stylesElement = document.Root.Element("Styles");
			foreach (var styleElement in stylesElement.Elements("Style"))
			{
				foreach (var element in styleElement.Elements())
				{
					if (element.Name == "State")
					{
						foreach (var imageElement in element.Elements("Image"))
						{
							var sourceRectangle = ParseRectangle((string)imageElement.Attribute("Source"));
							var imageName = images[sourceRectangle.BuildKey()];

							imageElement.Attribute("Source").Remove();

							var borderAttribute = imageElement.Attribute("Border");
							if (borderAttribute != null) borderAttribute.Remove();
							imageElement.Add(new XAttribute("Image", imageName));
						}
					} else if (element.Name == "IconSourceRectangle")
					{
						var sourceRectangle = ParseRectangle(element.Value);
						var imageName = images[sourceRectangle.BuildKey()];

						element.Value = imageName;
					}
				}
			}

			document.Save("test.xml");
		}

		static void Main(string[] args)
		{
			try
			{
				Process(args);
			}
			catch (Exception ex)
			{
				Log(ex.ToString());
			}
		}
	}
}