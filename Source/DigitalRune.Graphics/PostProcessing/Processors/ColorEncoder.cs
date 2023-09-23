// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace DigitalRune.Graphics.PostProcessing
{
  /// <summary>
  /// Changes the <see cref="ColorEncoding"/> of a texture.
  /// </summary>
  public class ColorEncoder : PostProcessor
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------

    private readonly Effect _effect;
    private readonly EffectParameter _viewportSizeParameter;
    private readonly EffectParameter _sourceTextureParameter;
    private readonly EffectParameter _sourceTypeParameter;
		private readonly EffectParameter _sourceParamParameter;
		private readonly EffectParameter _targetTypeParameter;
		private readonly EffectParameter _targetParamParameter;
		#endregion


		//--------------------------------------------------------------
		#region Properties & Events
		//--------------------------------------------------------------

		/// <summary>
		/// Gets or sets the <see cref="ColorEncoding"/> of the source texture.
		/// </summary>
		/// <value>
		/// The <see cref="ColorEncoding"/> of the source texture. The default encoding is 
		/// <see cref="ColorEncoding.Rgb"/>.
		/// </value>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="value"/> is <see langword="null"/>.
		/// </exception>
		public ColorEncoding SourceEncoding
    {
      get { return _sourceEncoding; }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");

        VerifyEncoding(value);
        _sourceEncoding = value;
      }
    }

    private ColorEncoding _sourceEncoding;


    /// <summary>
    /// Gets or sets the <see cref="ColorEncoding"/> of the render target.
    /// </summary>
    /// <value>
    /// The <see cref="ColorEncoding"/> of the render target. The default encoding is 
    /// <see cref="ColorEncoding.Rgb"/>.
    /// </value>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    public ColorEncoding TargetEncoding
    {
      get { return _targetEncoding; }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");

        VerifyEncoding(value);
        _targetEncoding = value;
      }
    }

    private ColorEncoding _targetEncoding;
    #endregion


    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorEncoder"/> class.
    /// </summary>
    /// <param name="graphicsService">The graphics service.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="graphicsService"/> is <see langword="null"/>.
    /// </exception>
    public ColorEncoder(IGraphicsService graphicsService)
      : base(graphicsService)
    {
      _effect = GraphicsService.GetStockEffect("DigitalRune/PostProcessing/ColorEncoder");
      _viewportSizeParameter = _effect.Parameters["ViewportSize"];
      _sourceTextureParameter = _effect.Parameters["SourceTexture"];
      _sourceTypeParameter = _effect.Parameters["SourceEncodingType"];
			_sourceParamParameter = _effect.Parameters["SourceEncodingParam"];
			_targetTypeParameter = _effect.Parameters["TargetEncodingType"];
			_targetParamParameter = _effect.Parameters["TargetEncodingParam"];

			_sourceEncoding = ColorEncoding.Rgb;
      _targetEncoding = ColorEncoding.Rgb;
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <summary>
    /// Throws exception if the encoding is not supported.
    /// </summary>
    /// <param name="encoding">The encoding.</param>
    /// <exception cref="NotSupportedException">
    /// The given color encoding is not supported by the <see cref="ColorEncoder"/>.
    /// </exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly")]
    private static void VerifyEncoding(ColorEncoding encoding)
    {
      if (encoding is RgbEncoding
          || encoding is SRgbEncoding
          || encoding is RgbmEncoding
          || encoding is RgbeEncoding
          || encoding is LogLuvEncoding)
      {
        return;
      }

      throw new NotSupportedException("The given color encoding is not supported by the ColorEncoder.");
    }


    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
    protected override void OnProcess(RenderContext context)
    {
      var graphicsDevice = GraphicsService.GraphicsDevice;

      if (TextureHelper.IsFloatingPointFormat(context.SourceTexture.Format))
        graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
      else
        graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

      graphicsDevice.SetRenderTarget(context.RenderTarget);
      graphicsDevice.Viewport = context.Viewport;

      _viewportSizeParameter.SetValue(new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height));
      _sourceTextureParameter.SetValue(context.SourceTexture);

      SetEncoding(_sourceTypeParameter, _sourceParamParameter, SourceEncoding);
      SetEncoding(_targetTypeParameter, _targetParamParameter, TargetEncoding);

      _effect.CurrentTechnique.Passes[0].Apply();
      graphicsDevice.DrawFullScreenQuad();

      _sourceTextureParameter.SetValue((Texture2D)null);
    }


    private static void SetEncoding(EffectParameter parameterType, EffectParameter parameterParam, ColorEncoding encoding)
    {
      // Constants need to be kept in sync with ColorEncoder.fx.
      const int rgbEncoding = 0;
      const int sRgbEncoding = 1;
      const int rgbmEncoding = 2;
      const int rgbeEncoding = 3;
      const int logLuvEncoding = 4;

      if (encoding is RgbEncoding)
      {
        parameterType.SetValue(rgbEncoding);
      }
      else if (encoding is SRgbEncoding)
      {
        parameterType.SetValue(sRgbEncoding);
      }
      else if (encoding is RgbmEncoding)
      {
        float max = GraphicsHelper.ToGamma(((RgbmEncoding)encoding).Max);
        parameterType.SetValue(rgbmEncoding);
        parameterParam.SetValue(max);
      }
      else if (encoding is RgbeEncoding)
      {
        parameterType.SetValue(rgbeEncoding);
      }
      else if (encoding is LogLuvEncoding)
      {
        parameterType.SetValue(logLuvEncoding);
      }
    }
  }
  #endregion
}
