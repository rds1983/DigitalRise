// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System.ComponentModel;
using DigitalRise.GameBase;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace DigitalRise.UI.Controls
{
	/// <summary>
	/// Represents a control that displays an image.
	/// </summary>
	/// <remarks>
	/// The <see cref="UIControl.Foreground"/> color can be used to tint the image. 
	/// (The default value is <see cref="Color.White"/>.)
	/// </remarks>
	/// <example>
	/// The following examples shows how to create a button containing an icon and a text label.
	/// <code lang="csharp">
	/// <![CDATA[
	/// // Create a horizontal stack panel containing an icon and a label.
	/// var buttonContentPanel = new StackPanel { Orientation = Orientation.Horizontal };
	/// 
	/// buttonContentPanel.Children.Add(new Image
	/// {
	///   Width = 16,
	///   Height = 16,
	///   Texture = content.Load<Texture2D>("Icons"),   // Load existing texture.
	///   SourceRectangle = new Rectangle(0, 0, 16, 16) // Optional: Select region in texture.
	/// });
	/// 
	/// buttonContentPanel.Children.Add(new TextBlock
	/// {
	///   Margin = new Vector4(4, 0, 0, 0),
	///   Text = "Label",
	///   VerticalAlignment = VerticalAlignment.Center,
	/// });
	/// 
	/// var button = new Button
	/// {
	///   Content = buttonContentPanel,
	///   Margin = new Vector4(4),
	/// };
	/// 
	/// // To show the button, add it to an existing content control or panel.
	/// panel.Children.Add(button);
	/// 
	/// // To handle button clicks simply add an event handler to the Click event.
	/// button.Click += OnButtonClicked;
	/// ]]>
	/// </code>
	/// </example>
	public class Image : UIControl
	{
		//--------------------------------------------------------------
		#region Fields
		//--------------------------------------------------------------
		#endregion


		//--------------------------------------------------------------
		#region Properties & Events
		//--------------------------------------------------------------
		#endregion


		//--------------------------------------------------------------
		#region Game Object Properties & Events
		//--------------------------------------------------------------

		/// <summary> 
		/// The game object property for <see cref="Texture"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<Texture2D> TextureProperty = CreateProperty<Texture2D>(
			typeof(Image), "Texture", GamePropertyCategories.Appearance, null, null,
			UIPropertyOptions.AffectsMeasure);

		/// <summary>
		/// Gets or sets the texture with the image that should be displayed. 
		/// This is a game object property.
		/// </summary>
		/// <value>The texture with the image that should be displayed.</value>
		public Texture2D Texture
		{
			get => TextureProperty.GetValue(this);
			set => TextureProperty.SetValue(this, value);
		}


		/// <summary> 
		/// The game object property for <see cref="SourceRectangle"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<Rectangle?> SourceRectangleProperty = CreateProperty<Rectangle?>(
			typeof(Image), "SourceRectangle", GamePropertyCategories.Appearance, null, null,
			UIPropertyOptions.AffectsMeasure);

		/// <summary>
		/// Gets or sets the source rectangle that defines the region of the <see cref="Texture"/>
		/// that should be displayed. This is a game object property.
		/// </summary>
		/// <value>
		/// The source rectangle. Can be <see langword="null"/> if the whole texture should be
		/// displayed.
		/// </value>
		/// <remarks>
		/// <para>
		/// <strong>Special notes for Windows Universal (UWP):</strong> <br/>
		/// Usually, the type of this property is a nullable rectangle. In UWP nullable game object
		/// properties cannot be used because of a bug in .NET Native. Therefore, the type of this 
		/// property is not nullable in the UWP build. Use an <see cref="Rectangle.Empty"/> rectangle if
		/// the whole texture should be displayed.
		/// </para>
		/// </remarks>
		public Rectangle? SourceRectangle
		{
			get => SourceRectangleProperty.GetValue(this);
			set => SourceRectangleProperty.SetValue(this, value);
		}

		#endregion


		//--------------------------------------------------------------
		#region Creation & Cleanup
		//--------------------------------------------------------------

		/// <summary>
		/// Initializes static members of the <see cref="Image"/> class.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static Image()
		{
			// The default Foreground color must be white because the Foreground color is used for
			// tinting.
			OverrideDefaultValue(typeof(Image), ForegroundProperty.Id, Color.White);
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="Image"/> class.
		/// </summary>
		public Image()
		{
			Style = "Image";
		}
		#endregion


		//--------------------------------------------------------------
		#region Methods
		//--------------------------------------------------------------

		/// <inheritdoc/>
		protected override Vector2 OnMeasure(Vector2 availableSize)
		{
			// If nothing else is set, the desired size is determined by the SourceRectangle or 
			// the whole texture.

			Vector2 result = base.OnMeasure(availableSize);

			if (Texture == null)
				return result;

			float width = Width;
			float height = Height;
			Vector4 padding = Padding;
			Vector2 desiredSize = Vector2.Zero;

			if (Numeric.IsPositiveFinite(width))
			{
				desiredSize.X = width;
			}
			else
			{
				int imageWidth = (SourceRectangle != null) ? SourceRectangle.Value.Width : Texture.Width;
				desiredSize.X = padding.X + padding.Z + imageWidth;
			}

			if (Numeric.IsPositiveFinite(height))
			{
				desiredSize.Y = height;
			}
			else
			{
				int imageHeight = (SourceRectangle != null) ? SourceRectangle.Value.Height : Texture.Height;
				desiredSize.Y = padding.Y + padding.W + imageHeight;
			}

			return desiredSize;
		}
		#endregion
	}
}
