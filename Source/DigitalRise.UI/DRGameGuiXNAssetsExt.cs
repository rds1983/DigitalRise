using System;
using System.Xml.Linq;
using DigitalRise.UI.Rendering;
using System.Xml;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DigitalRise;

namespace AssetManagementBase
{
	public static partial class DRGameGuiXNAssetsExt
	{
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

#if MONOGAME
		private static void AddDefaultCursorIfNotExists(Theme theme, string name, MouseCursor cursor, bool isDefault)
		{
			if (theme.Cursors.Contains(name))
			{
				return;
			}

			// Default values
			theme.Cursors.Add(new ThemeCursor
			{
				Name = name,
				MouseCursor = cursor,
				IsDefault = isDefault
			});
		}

		private static void ProcessCursors(Theme theme, XDocument document, AssetManager manager)
		{
			var cursorsElement = document.Root.Element("Cursors");
			if (cursorsElement != null)
			{
				foreach (var cursorElement in cursorsElement.Elements("Cursor"))
				{
					string name = GetMandatoryAttributeString(cursorElement, "Name");
					bool isDefault = (bool?)cursorElement.Attribute("IsDefault") ?? false;

					// TODO: Cursor loading(can't copy the existing code, since it works only under win
					string filename = GetMandatoryAttributeString(cursorElement, "File");

					var jsonData = manager.ReadAsString(filename);
					var jObj = (JsonObject)JsonSerializer.Deserialize(jsonData, typeof(JsonObject));
					var bitmapFile = Path.ChangeExtension(filename, "png");

					Texture2D image;
					using (var stream = manager.Open(bitmapFile))
					{
						image = Texture2D.FromStream(DRBase.GraphicsDevice, stream, data =>
						{
							var k = 5;

						});
					}

					var cursor = new ThemeCursor
					{
						Name = name,
						IsDefault = isDefault,
						MouseCursor = MouseCursor.FromTexture2D(image, jObj["HotspotX"].GetValue<int>(), jObj["HotspotY"].GetValue<int>())
					};

					theme.Cursors.Add(cursor);
				}
			}

			AddDefaultCursorIfNotExists(theme, "Arrow", MouseCursor.Arrow, true);
			AddDefaultCursorIfNotExists(theme, "IBeam", MouseCursor.IBeam, false);
			AddDefaultCursorIfNotExists(theme, "Wait", MouseCursor.Wait, false);
			AddDefaultCursorIfNotExists(theme, "Crosshair", MouseCursor.Crosshair, false);
			AddDefaultCursorIfNotExists(theme, "WaitArror", MouseCursor.WaitArrow, false);
			AddDefaultCursorIfNotExists(theme, "SizeNWSE", MouseCursor.SizeNWSE, false);
			AddDefaultCursorIfNotExists(theme, "SizeNESW", MouseCursor.SizeNESW, false);
			AddDefaultCursorIfNotExists(theme, "SizeWE", MouseCursor.SizeWE, false);
			AddDefaultCursorIfNotExists(theme, "SizeNS", MouseCursor.SizeNS, false);
			AddDefaultCursorIfNotExists(theme, "SizeAll", MouseCursor.SizeAll, false);
			AddDefaultCursorIfNotExists(theme, "No", MouseCursor.No, false);
			AddDefaultCursorIfNotExists(theme, "Hand", MouseCursor.Hand, false);
		}
#endif

		private static void ProcessFonts(Theme theme, XDocument document, AssetManager manager)
		{
			var fontsElement = document.Root.Element("Fonts");
			if (fontsElement == null)
				throw new Exception("The given UI theme does not contain a 'Fonts' node.");

			foreach (var fontElement in fontsElement.Elements("Font"))
			{
				string name = GetMandatoryAttributeString(fontElement, "Name");
				bool isDefault = (bool?)fontElement.Attribute("IsDefault") ?? false;
				string filename = GetMandatoryAttributeString(fontElement, "File");
				float size = GetMandatoryAttributeFloat(fontElement, "Size");

				var font = manager.LoadFontSystem(filename).GetFont(size);

				var themeFont = new ThemeFont
				{
					Name = name,
					IsDefault = isDefault,
					Font = font
				};

				theme.Fonts.Add(themeFont);
			}

			if (theme.Fonts.Count == 0)
			{
				throw new Exception("The UI theme does not contain any fonts. At least 1 font is required.");
			}
		}

		private static void ProcessTextures(Theme theme, XDocument document, AssetManager manager)
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

				var texture = manager.LoadTexture2D(filename, premultiplyAlpha);

				var themeTexture = new ThemeTexture
				{
					Name = name,
					IsDefault = isDefault,
					Texture = texture,
				};

				theme.Textures.Add(themeTexture);
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
								SourceRectangle = ThemeHelper.ParseRectangle((string)imageElement.Attribute("Source")),
								Margin = ThemeHelper.ParseVector4((string)imageElement.Attribute("Margin")),
								HorizontalAlignment = ThemeHelper.ParseHorizontalAlignment((string)imageElement.Attribute("HorizontalAlignment")),
								VerticalAlignment = ThemeHelper.ParseVerticalAlignment((string)imageElement.Attribute("VerticalAlignment")),
								TileMode = ThemeHelper.ParseTileMode((string)imageElement.Attribute("TileMode")),
								Border = ThemeHelper.ParseVector4((string)imageElement.Attribute("Border")),
								IsOverlay = (bool?)imageElement.Attribute("IsOverlay") ?? false,
								Color = ThemeHelper.ParseColor((string)imageElement.Attribute("Color"), Color.White),
							};

							var imageTexture = (string)imageElement.Attribute("Texture");
							if (!string.IsNullOrEmpty(imageTexture))
							{
								ThemeTexture themeTexture;
								if (!theme.Textures.TryGet(imageTexture, out themeTexture))
								{
									string message = string.Format("Missing texture: The image '{0}' in state '{1}' of style '{2}' requires a texture named '{3}'.", image.Name, state.Name, style.Name, image.Texture);
									throw new Exception(message);
								}

								image.Texture = themeTexture;
							}

							state.Images.Add(image);
						}

						var backgroundElement = element.Element("Background");
						if (backgroundElement != null)
							state.Background = ThemeHelper.ParseColor((string)backgroundElement, Color.Transparent);

						var foregroundElement = element.Element("Foreground");
						if (foregroundElement != null)
							state.Foreground = ThemeHelper.ParseColor((string)foregroundElement, Color.Black);

						state.Opacity = (float?)element.Element("Opacity");

						style.States.Add(state);
					}
					else
					{
						// A custom attribute.
						var attribute = new ThemeAttribute
						{
							Name = element.Name.ToString(),
							Value = element.Value,
						};
						style.Attributes.Add(attribute);
					}
				}

				theme.Styles.Add(style);
			}

			// Validate inheritance.
			foreach (var pair in inherits)
			{
				var style = theme.Styles[pair.Key];

				ThemeStyle parent;
				if (!theme.Styles.TryGet(pair.Value, out parent))
				{
					// Parent of the given style not found. Log warning.
					// TODO: AssetManagerBase warnings
					/*					context.Logger.LogWarning(
											null,
											theme.Identity,
											"The parent of style \"{0}\" (Inherits = \"{1}\") not found.",
											style.Name,
											style.Inherits);*/
				}
				else
				{
					style.Inherits = parent;
				}
			}
		}

		private static AssetLoader<Theme> _themeLoader = (manager, assetName, settings, tag) =>
		{
			var document = XDocument.Parse(manager.ReadAsString(assetName), LoadOptions.SetLineInfo);
			if (document.Root == null)
			{
				string message = string.Format("Root element \"<Theme>\" is missing in XML.");
				throw new Exception(message);
			}

			var theme = new Theme();
#if MONOGAME
			ProcessCursors(theme, document, manager);
#endif
			ProcessFonts(theme, document, manager);
			ProcessTextures(theme, document, manager);
			ProcessStyles(theme, document);

			return theme;
		};

		public static Theme LoadTheme(this AssetManager manager, string assetName)
		{
			return manager.UseLoader(_themeLoader, assetName);
		}
	}
}