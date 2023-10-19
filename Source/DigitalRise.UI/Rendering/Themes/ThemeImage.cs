// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using DigitalRise.Mathematics.Algebra;
using DigitalRise.UI.TextureAtlases;
using Microsoft.Xna.Framework;


namespace DigitalRise.UI.Rendering
{
	/// <summary>
	/// Represents an image of the UI theme.
	/// </summary>
	/// <remarks>
	/// The image is a region in a texture atlas of the theme. Images support 9-grid scaling.
	/// </remarks>
	public class ThemeImage : INamedObject
	{
		/// <summary>
		/// Gets or sets the name of the image.
		/// </summary>
		/// <value>The name of the image.</value>
		public string Name { get; set; }

		/// <summary>
		/// Texture Region
		/// </summary>
		public TextureRegion TextureRegion { get; set; }


		/// <summary>
		/// Gets or sets the margin (left, top, right, bottom).
		/// </summary>
		/// <value>
		/// The margin (left, top, right, bottom). Can be negative to draw outside of the control area.
		/// </value>
		public Vector4 Margin { get; set; }


		/// <summary>
		/// Gets or sets the horizontal alignment.
		/// </summary>
		/// <value>The horizontal alignment.</value>
		public HorizontalAlignment HorizontalAlignment { get; set; }


		/// <summary>
		/// Gets or sets the vertical alignment.
		/// </summary>
		/// <value>The vertical alignment.</value>
		public VerticalAlignment VerticalAlignment { get; set; }


		/// <summary>
		/// Gets or sets the tile mode.
		/// </summary>
		/// <value>
		/// The tile mode that defines whether the image is repeated and how. The default value is
		/// <see cref="Rendering.TileMode.None"/>.
		/// </value>
		/// <remarks>
		/// Note that, when either the <see cref="HorizontalAlignment"/> or the 
		/// <see cref="VerticalAlignment"/> is set to <see cref="UI.HorizontalAlignment.Stretch"/> then
		/// the image is never tiled.
		/// </remarks>
		public TileMode TileMode { get; set; }


		/// <summary>
		/// Gets or sets a value indicating whether this image is drawn on top of the control.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this image is drawn on top of the control; otherwise, 
		/// <see langword="false"/>.
		/// </value>
		public bool IsOverlay { get; set; }


		/// <summary>
		/// Gets or sets the tint color.
		/// </summary>
		/// <value>The tint color.</value>
		public Color Color { get; set; }
	}
}
