using System;
using System.Xml.Linq;
using DigitalRise.UI.Rendering;
using System.Xml;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text.Json.Nodes;
using System.IO;
using System.Text.Json;
using DigitalRise.UI.TextureAtlases;

#if !MONOGAME
using static SDL2.SDL;
using MouseCursor = System.Nullable<System.IntPtr>;
#endif

namespace AssetManagementBase
{
	public static partial class DRGameGuiXNAssetsExt
	{
#if !MONOGAME
		private static readonly Dictionary<SDL_SystemCursor, IntPtr> _systemCursors = new Dictionary<SDL_SystemCursor, IntPtr>();

		private static IntPtr GetSystemCursor(SDL_SystemCursor type)
		{
			IntPtr result;
			if (_systemCursors.TryGetValue(type, out result))
			{
				return result;
			}

			result = SDL_CreateSystemCursor(type);
			_systemCursors[type] = result;

			return result;
		}
#endif

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
#if MONOGAME
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
						image = Texture2D.FromStream(theme.GraphicsDevice, stream);
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
#else
			AddDefaultCursorIfNotExists(theme, "Arrow", GetSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_ARROW), true);
			AddDefaultCursorIfNotExists(theme, "IBeam", GetSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_IBEAM), false);
			AddDefaultCursorIfNotExists(theme, "Wait", GetSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_WAIT), false);
			AddDefaultCursorIfNotExists(theme, "Crosshair", GetSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_CROSSHAIR), false);
			AddDefaultCursorIfNotExists(theme, "WaitArror", GetSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_WAITARROW), false);
			AddDefaultCursorIfNotExists(theme, "SizeNWSE", GetSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENWSE), false);
			AddDefaultCursorIfNotExists(theme, "SizeNESW", GetSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENESW), false);
			AddDefaultCursorIfNotExists(theme, "SizeWE", GetSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZEWE), false);
			AddDefaultCursorIfNotExists(theme, "SizeNS", GetSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENS), false);
			AddDefaultCursorIfNotExists(theme, "SizeAll", GetSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZEALL), false);
			AddDefaultCursorIfNotExists(theme, "No", GetSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_NO), false);
			AddDefaultCursorIfNotExists(theme, "Hand", GetSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_HAND), false);
#endif
		}

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

		private static void ProcessStyles(Theme theme, XDocument document, TextureAtlas textureAtlas)
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
								Margin = ThemeHelper.ParseVector4((string)imageElement.Attribute("Margin")),
								HorizontalAlignment = ThemeHelper.ParseHorizontalAlignment((string)imageElement.Attribute("HorizontalAlignment")),
								VerticalAlignment = ThemeHelper.ParseVerticalAlignment((string)imageElement.Attribute("VerticalAlignment")),
								TileMode = ThemeHelper.ParseTileMode((string)imageElement.Attribute("TileMode")),
								IsOverlay = (bool?)imageElement.Attribute("IsOverlay") ?? false,
								Color = ThemeHelper.ParseColor((string)imageElement.Attribute("Color"), Color.White),
							};

							TextureRegion region;
							var imageName = (string)imageElement.Attribute("Image");
							if (!textureAtlas.TextureRegions.TryGetValue(imageName, out region))
							{
								throw new Exception($"Could not find region '{imageName}'");
							}

							image.TextureRegion = region;

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
					else if (element.Name == "IconSourceRectangle")
					{
						TextureRegion region;
						var imageName = element.Value;
						if (!textureAtlas.TextureRegions.TryGetValue(imageName, out region))
						{
							throw new Exception($"Could not find region '{imageName}'");
						}

						var r = region.Rectangle;
						var attribute = new ThemeAttribute
						{
							Name = "IconResourceRectangle",
							Value = $"{r.X},{r.Y},{r.Width},{r.Height}"
						};
						style.Attributes.Add(attribute);
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

			var graphicsDevice = (GraphicsDevice)tag;
			var atlasPathAttr = document.Root.Attribute("TextureAtlas");
			if (atlasPathAttr == null)
			{
				throw new Exception($"The UI theme lacks texture atlas attribute");
			}

			var theme = new Theme(graphicsDevice);
			ProcessCursors(theme, document, manager);
			ProcessFonts(theme, document, manager);

			var xml = manager.LoadString(atlasPathAttr.Value);
			var textureAtlas = TextureAtlas.FromXml(xml, name => manager.LoadTexture2D(graphicsDevice, name));
			theme.TextureAtlas = textureAtlas;
			ProcessStyles(theme, document, textureAtlas);

			return theme;
		};

		public static Theme LoadTheme(this AssetManager manager, GraphicsDevice graphicsDevice, string assetName)
		{
			return manager.UseLoader(_themeLoader, assetName, tag: graphicsDevice);
		}
	}
}