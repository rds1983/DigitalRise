// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace DigitalRise.Graphics.PostProcessing
{
	/// <summary>
	/// Changes the <see cref="ColorEncoding"/> of a texture.
	/// </summary>
	public class ColorEncoder : PostProcessor
  {
		private enum ColorEncodingType
		{
			RGB,
			SRGB,
			RGBM,
			RGBE,
			LOGLUV
		}

		private class EffectData
    {
      public readonly Effect Effect;
			public readonly EffectParameter ViewportSizeParameter;
			public readonly EffectParameter SourceTextureParameter;
			public readonly EffectParameter SourceParamParameter;
			public readonly EffectParameter TargetParamParameter;

      public EffectData(Effect effect, 
				EffectParameter viewportSizeParameter, EffectParameter sourceTextureParameter,
				EffectParameter sourceParamParameter, EffectParameter targetParamParameter)
      {
				Effect = effect;
				ViewportSizeParameter = viewportSizeParameter;
				SourceTextureParameter = sourceTextureParameter;
				SourceParamParameter = sourceParamParameter;
				TargetParamParameter = targetParamParameter;
			}
		}

    //--------------------------------------------------------------
    #region Fields
    private static readonly EffectData[] _effectsCache = new EffectData[25];
		private readonly EffectData _effect;

		//--------------------------------------------------------------
		#endregion


		//--------------------------------------------------------------
		#region Properties & Events
		//--------------------------------------------------------------

		/// <summary>
		/// Gets or sets the <see cref="ColorEncoding"/> of the source texture.
		/// </summary>
		/// <value>
		/// The <see cref="ColorEncoding"/> of the source texture. The default encoding is 
		/// <see cref="ColorEncodingType.RGB"/>.
		/// </value>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="value"/> is <see langword="null"/>.
		/// </exception>
		public readonly ColorEncoding SourceEncoding;

		/// <summary>
		/// Gets or sets the <see cref="ColorEncoding"/> of the render target.
		/// </summary>
		/// <value>
		/// The <see cref="ColorEncoding"/> of the render target. The default encoding is 
		/// <see cref="ColorEncodingType.RGB"/>.
		/// </value>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="value"/> is <see langword="null"/>.
		/// </exception>
		public readonly ColorEncoding TargetEncoding;
		
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
		public ColorEncoder(IGraphicsService graphicsService, ColorEncoding source, ColorEncoding target):
			base(graphicsService)
    {
			SourceEncoding = source ?? throw new ArgumentNullException(nameof(source));
			TargetEncoding = target ?? throw new ArgumentNullException(nameof(target));
			_effect = GetEffect(GraphicsService, ToType(SourceEncoding), ToType(TargetEncoding));
		}

		#endregion

		//--------------------------------------------------------------
		#region Methods
		//--------------------------------------------------------------

		private static ColorEncodingType ToType(ColorEncoding enc)
		{
			switch(enc)
			{
				case RgbEncoding _:
					return ColorEncodingType.RGB;
				case SRgbEncoding _:
					return ColorEncodingType.SRGB;
				case RgbmEncoding _:
					return ColorEncodingType.RGBM;
				case RgbeEncoding _:
					return ColorEncodingType.RGBE;
				case LogLuvEncoding _:
					return ColorEncodingType.LOGLUV;
			}

			throw new NotSupportedException("The given color encoding is not supported by the ColorEncoder.");
		}

		private static EffectData GetEffect(IGraphicsService service, ColorEncodingType source, ColorEncodingType target)
    {
			var key = ((int)source) * 5 + (int)target;

			if (_effectsCache[key] != null)
			{
				return _effectsCache[key];
			}

			var defs = new Dictionary<string, string>
				{
					{ "SOURCE_" + source.ToString(), "1" },
					{ "TARGET_" + target.ToString(), "1" }
				};

			var effect = service.GetStockEffect("PostProcessing/ColorEncoder", defs);

			var result = new EffectData(effect,
						effect.Parameters["ViewportSize"], effect.Parameters["SourceTexture"],
						effect.Parameters["SourceEncodingParam"], effect.Parameters["TargetEncodingParam"]);

			_effectsCache[key] = result;
			return result;
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

      _effect.ViewportSizeParameter.SetValue(new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height));
      _effect.SourceTextureParameter.SetValue(context.SourceTexture);

			var asRgbm = SourceEncoding as RgbmEncoding;
			if (asRgbm != null)
			{
				float max = GraphicsHelper.ToGamma(asRgbm.Max);
				_effect.SourceParamParameter.SetValue(max);
			}

			asRgbm = TargetEncoding as RgbmEncoding;
			if (asRgbm != null)
			{
				float max = GraphicsHelper.ToGamma(asRgbm.Max);
				_effect.TargetParamParameter.SetValue(max);
			}


			_effect.Effect.CurrentTechnique.Passes[0].Apply();
      graphicsDevice.DrawFullScreenQuad();

			_effect.SourceTextureParameter.SetValue((Texture2D)null);
    }
  }
  #endregion
}
