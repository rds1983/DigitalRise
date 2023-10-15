// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DigitalRise.UI.Controls;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DigitalRise.GameBase;
using Microsoft.Xna.Framework.Input;

#if !MONOGAME
using MouseCursor = System.Nullable<System.IntPtr>;
#endif

namespace DigitalRise.UI.Rendering
{
	/// <summary>
	/// Manages and renders the visual appearance of a UI. (Default implementation.)
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class implements <see cref="IUIRenderer"/> (see comments of <see cref="IUIRenderer"/>). 
	/// </para>
	/// <para>
	/// When creating the <see cref="UIRenderer"/> a UI theme (see <see cref="Theme"/>) must be 
	/// specified. The renderer will use the attributes and styles of the theme to render the 
	/// controls.
	/// </para>
	/// <para>
	/// <strong>Thread-Safety:</strong> This class is not thread-safe. <see cref="Render"/> must not 
	/// be called simultaneously in concurrent threads.
	/// </para>
	/// <para>
	/// <strong>Render Callbacks:</strong> This class has a dictionary <see cref="RenderCallbacks"/>
	/// which defines the methods used for rendering. When 
	/// <see cref="Render(UIControl, UIRenderContext)"/> is called, the style of the control is
	/// determined and the render callback for the style is used to render the control. If a render
	/// method is not given for a given style, the parent styles are used (a <see cref="ThemeStyle"/>
	/// can inherit from another style, see <see cref="ThemeStyle.Inherits"/>). See also 
	/// <see cref="RenderCallbacks"/>.
	/// </para>
	/// </remarks>
	public partial class UIRenderer : IUIRenderer
	{
		// Controls are rendered back to front because we use alpha blending (text and glow images).
		// TODO: Cache info in UIControl.RenderData. Check UIControl IsVisualValid to see if the cache must be updated.

		//--------------------------------------------------------------
		#region Fields
		//--------------------------------------------------------------

		private MouseCursor _defaultCursor;
		private SpriteFontBase _defaultFont;
		private Texture2D _defaultTexture;
		#endregion


		//--------------------------------------------------------------
		#region Properties & Events
		//--------------------------------------------------------------

		/// <inheritdoc/>
		public GraphicsDevice GraphicsDevice => Theme.GraphicsDevice;


		/// <summary>
		/// Gets the UI theme.
		/// </summary>
		/// <value>The UI theme.</value>
		public Theme Theme { get; private set; }


		/// <inheritdoc/>
		public Dictionary<string, GameObject> Templates
		{
			get { return _templates; }
		}
		private readonly Dictionary<string, GameObject> _templates = new Dictionary<string, GameObject>();
		#endregion


		//--------------------------------------------------------------
		#region Creation & Cleanup
		//--------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance of the <see cref="UIRenderer"/> class.
		/// </summary>
		/// <param name="theme">The loaded UI theme.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="theme"/> is <see langword="null"/>.
		/// </exception>
		public UIRenderer(Theme theme)
		{
			if (theme == null)
				throw new ArgumentNullException("theme");

			Theme = theme;

			InitializeDefaultCursor();
			InitializeDefaultFont();
			InitializeDefaultTexture();
			InitializeRendering();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UIRenderer"/> class that uses the default theme. 
		/// </summary>
		/// <param name="graphicsDevice"></param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="theme"/> is <see langword="null"/>.
		/// </exception>
		public UIRenderer(GraphicsDevice graphicsDevice) : this(Theme.GetDefault(graphicsDevice))
		{
		}

		private void InitializeDefaultCursor()
		{
			if (Theme.Cursors != null)
			{
				// Get cursor with attribute "IsDefault=true".
				_defaultCursor = (from c in Theme.Cursors where c.IsDefault select c.MouseCursor).FirstOrDefault();
				if (_defaultCursor == null)
				{
					// The theme does not define a font with "IsDefault=true".
					// Check if a font is named "Default".
					_defaultCursor = (from c in Theme.Cursors where c.Name.Equals("default", StringComparison.OrdinalIgnoreCase) select c.MouseCursor).FirstOrDefault();
				}
			}
		}


		private void InitializeDefaultFont()
		{
			if (Theme.Fonts != null)
			{
				_defaultFont = Theme.Fonts
														.Where(f => f.IsDefault)
														.Select(f => f.Font)
														.FirstOrDefault();
				if (_defaultFont == null)
				{
					// The theme does not define a font with "IsDefault=true".
					// Check if a font is named "Default".
					_defaultFont = Theme.Fonts
															.Where(f => f.Name.Equals("default", StringComparison.OrdinalIgnoreCase))
															.Select(f => f.Font)
															.FirstOrDefault();
				}
				if (_defaultFont == null)
				{
					// No default font found so far. --> Just use the first available font.
					_defaultFont = Theme.Fonts.Select(f => f.Font).FirstOrDefault();
				}
			}

			if (_defaultFont == null)
				throw new UIException("No default font found.");
		}


		private void InitializeDefaultTexture()
		{
			if (Theme.Textures != null)
			{
				_defaultTexture = Theme.Textures
															 .Where(t => t.IsDefault)
															 .Select(t => t.Texture)
															 .FirstOrDefault();
				if (_defaultTexture == null)
				{
					// The theme does not define a texture with "IsDefault=true".
					// Check if a texture is named "Default".
					_defaultTexture = Theme.Textures
																 .Where(t => t.Name.Equals("default", StringComparison.OrdinalIgnoreCase))
																 .Select(t => t.Texture)
																 .FirstOrDefault();
				}
				if (_defaultTexture == null)
				{
					// No default texture found so far. --> Just use the first available texture.
					_defaultTexture = Theme.Textures.Select(t => t.Texture).FirstOrDefault();
				}
			}

			if (_defaultTexture == null)
				throw new UIException("No default texture found.");
		}

		#endregion


		//--------------------------------------------------------------
		#region Methods
		//--------------------------------------------------------------

		/// <summary>
		/// Gets a style-specific attribute value.
		/// </summary>
		/// <typeparam name="T">The type of the attribute value.</typeparam>
		/// <param name="style">The style.</param>
		/// <param name="name">The name of the attribute.</param>
		/// <param name="result">The attribute value.</param>
		/// <returns>
		/// <see langword="true"/> if the renderer can provide a value for the attribute; otherwise, 
		/// <see langword="false"/> if the renderer does not know the style or the attribute.
		/// </returns>
		/// <remarks>
		/// This method calls <see cref="OnParseAttribute{T}"/> to convert a 
		/// <see cref="ThemeAttribute"/> to a value of type <typeparamref name="T"/>.
		/// </remarks>
		public bool GetAttribute<T>(string style, string name, out T result)
		{
			result = default(T);

			if (style == null)
				return false;
			if (string.IsNullOrEmpty(name))
				return false;

			// Get style.
			ThemeStyle themeStyle;
			bool found = Theme.Styles.TryGet(style, out themeStyle);
			if (!found)
				return false;

			// Search for attribute including style inheritance.
			ThemeAttribute attribute = null;
			while (attribute == null && themeStyle != null)
			{
				if (!themeStyle.Attributes.TryGet(name, out attribute))
				{
					// Try ancestor.
					themeStyle = themeStyle.Inherits;
				}
			}

			if (attribute == null)
				return false;

			return OnParseAttribute(attribute, out result);
		}


		/// <summary>
		/// Called by <see cref="GetAttribute{T}"/> to convert attributes to values.
		/// </summary>
		/// <typeparam name="T">The target type.</typeparam>
		/// <param name="attribute">The attribute.</param>
		/// <param name="result">The parsed value.</param>
		/// <returns>
		/// <see langword="true"/> if the <see cref="ThemeAttribute.Value"/> could be converted to the
		/// type <typeparamref name="T"/>; otherwise, <see langword="false"/>.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The base implementation of this method supports following types: <see cref="Vector4"/>,
		/// <see cref="Color"/>, <see cref="Rectangle"/>, <see cref="Rectangle"/>?,
		/// <see cref="Texture2D"/>, enumerations, types that have a <see cref="TypeConverter"/>, and
		/// types that implement <see cref="IConvertible"/>.
		/// </para>
		/// <para>
		/// Derived classes can override this method to add support for additional types.
		/// </para>
		/// </remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
		protected bool OnParseAttribute<T>(ThemeAttribute attribute, out T result)
		{
			result = default(T);

			if (result is Vector2)
			{
				result = (T)(object)ThemeHelper.ParseVector2(attribute.Value);
			}
			else if (result is Vector3)
			{
				result = (T)(object)ThemeHelper.ParseVector3(attribute.Value);
			}
			else if (result is Vector4)
			{
				result = (T)(object)ThemeHelper.ParseVector4(attribute.Value);
			}
			else if (typeof(T) == typeof(string) || typeof(T) == typeof(object))
			{
				result = (T)(object)attribute.Value;
			}
			else if (result is Color)
			{
				result = (T)(object)ThemeHelper.ParseColor(attribute.Value, Color.Black);
			}
			else if (typeof(T).IsAssignableFrom(typeof(Rectangle)))
			{
				result = (T)(object)ThemeHelper.ParseRectangle(attribute.Value);
			}
			else if (typeof(T).IsAssignableFrom(typeof(Texture2D)))
			{
				result = (T)(object)GetTexture(attribute.Value);
			}
			else
			{
				try
				{
					result = ObjectHelper.Parse<T>(attribute.Value);
				}
				catch
				{
					return false;
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public MouseCursor GetCursor(string name)
		{
			if (string.IsNullOrEmpty(name))
				return _defaultCursor;

			ThemeCursor cursor;
			bool exists = Theme.Cursors.TryGet(name, out cursor);
			return (exists) ? cursor.MouseCursor : _defaultCursor;
		}


		/// <inheritdoc/>
		public SpriteFontBase GetFont(string name)
		{
			if (string.IsNullOrEmpty(name))
				return _defaultFont;

			ThemeFont font;
			bool exists = Theme.Fonts.TryGet(name, out font);
			return (exists) ? font.Font : _defaultFont;
		}


		/// <inheritdoc/>
		public Texture2D GetTexture(string name)
		{
			if (string.IsNullOrEmpty(name))
				return _defaultTexture;

			ThemeTexture texture;
			bool exists = Theme.Textures.TryGet(name, out texture);
			return (exists) ? texture.Texture : _defaultTexture;
		}

		#endregion
	}
}
