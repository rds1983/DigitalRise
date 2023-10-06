// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using DigitalRise.GameBase;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using FontStashSharp;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;

namespace DigitalRise.UI.Controls
{
  /// <summary>
  /// Provides a lightweight control for displaying small amounts of text, supporting text 
  /// wrapping at word boundaries.
  /// </summary>
  /// <example>
  /// The following examples shows how to create a button and handle the 
  /// <see cref="ButtonBase.Click"/> event.
  /// <code lang="csharp">
  /// <![CDATA[
  /// var button = new Button
  /// {
  ///   Content = new TextBlock { Text = "Click Me!" },
  ///   Margin = new Vector4(4),
  ///   Padding = new Vector4(6),
  ///   HorizontalAlignment = HorizontalAlignment.Stretch,
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
  public class TextBlock : UIControl
  {
		private readonly RichTextLayout _richText = new RichTextLayout
		{
			SupportsCommands = true
		};

		//--------------------------------------------------------------
		#region Properties & Events
		//--------------------------------------------------------------

    public RichTextLayout RichTextLayout => _richText;

    /// <summary>
    /// Gets a value indicating whether the renderer should clip the rendered 
    /// <see cref="VisualText"/>.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the renderer should clip the text; otherwise, 
    /// <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// If this value is <see langword="true"/>, the renderer should clip the text rendering. This
    /// flag is set if text must be clipped within characters (e.g. if the whole text block is not
    /// high enough). The clipping rectangle is defined by the <see cref="UIControl.ActualBounds"/>
    /// and applying the <see cref="UIControl.Padding"/>.
    /// </remarks>
    public bool VisualClip { get; private set; }
    #endregion


    //--------------------------------------------------------------
    #region Game Object Properties & Events
    //--------------------------------------------------------------

/*    /// <summary> 
    /// The ID of the <see cref="UseEllipsis"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int UseEllipsisPropertyId = CreateProperty(
      typeof(TextBlock), "UseEllipsis", GamePropertyCategories.Appearance, null, false,
      UIPropertyOptions.AffectsRender);

    /// <summary>
    /// Gets or sets a value indicating whether an ellipsis ("�") should be appended when the text
    /// must be clipped. This is a game object property.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if an ellipsis ("�") should be appended; otherwise, 
    /// <see langword="false"/>.
    /// </value>
    public bool UseEllipsis
    {
      get { return GetValue<bool>(UseEllipsisPropertyId); }
      set { SetValue(UseEllipsisPropertyId, value); }
    }*/


    /// <summary> 
    /// The ID of the <see cref="WrapText"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int WrapTextPropertyId = CreateProperty(
      typeof(TextBlock), "WrapText", GamePropertyCategories.Layout, null, false,
      UIPropertyOptions.AffectsMeasure);

    /// <summary>
    /// Gets or sets a value indicating whether text is wrapped when the available space is not
    /// wide enough. This is a game object property.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if text is wrapped when the available space is not wide enough; 
    /// otherwise, <see langword="false"/>.
    /// </value>
    public bool WrapText
    {
      get { return GetValue<bool>(WrapTextPropertyId); }
      set { SetValue(WrapTextPropertyId, value); }
    }


    /// <summary> 
    /// The ID of the <see cref="Text"/> game object property.
    /// </summary>
    [Browsable(false)]
    public static readonly int TextPropertyId = CreateProperty(
      typeof(TextBlock), "Text", GamePropertyCategories.Common, null, string.Empty,
      UIPropertyOptions.AffectsMeasure);

    /// <summary>
    /// Gets or sets the text. 
    /// This is a game object property.
    /// </summary>
    /// <value>The text.</value>
    public string Text
    {
      get { return GetValue<string>(TextPropertyId); }
      set { SetValue(TextPropertyId, value); }
    }
    #endregion


    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBlock"/> class.
    /// </summary>
    public TextBlock()
    {
      Style = "TextBlock";
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    protected override Vector2 OnMeasure(Vector2 availableSize)
    {
      // This control can only measure itself if it is in a screen because it needs a font.
      var screen = Screen;
      if (screen == null)
        return base.OnMeasure(availableSize);

      // Clear old renderer info.
      VisualClip = false;
      _richText.Text = Text;
			_richText.Font = screen.Renderer.GetFont(Font);

			string text = Text;
      float width = Width;
      float height = Height;
      bool hasWidth = Numeric.IsPositiveFinite(width);
      bool hasHeight = Numeric.IsPositiveFinite(height);

      if (string.IsNullOrEmpty(text))
      {
        // No text --> Abort.
        return new Vector2(
          hasWidth ? width : 0,
          hasHeight ? height : 0);
      }

      // Limit constraint size by user-defined width and height.
      if (hasWidth && width < availableSize.X)
        availableSize.X = width;
      if (hasHeight && height < availableSize.Y)
        availableSize.Y = height;

      // Remove padding from constraint size.
      Vector4 padding = Padding;
      Vector2 contentSize = availableSize;
      if (Numeric.IsPositiveFinite(availableSize.X))
        contentSize.X -= padding.X + padding.Z;
      if (Numeric.IsPositiveFinite(availableSize.Y))
        contentSize.Y -= padding.Y + padding.W;

      // Measure text size.
      _richText.Width = WrapText ? (int)contentSize.X : null;
      var sz = _richText.Size;
      Vector2 size = new Vector2(sz.X, sz.Y);
      return new Vector2(
        hasWidth ? width : size.X + padding.X + padding.Z,
        hasHeight ? height : size.Y + padding.Y + padding.W);
    }

    #endregion
  }
}
