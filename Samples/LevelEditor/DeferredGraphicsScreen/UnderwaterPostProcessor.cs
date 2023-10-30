using AssetManagementBase;
using DigitalRise;
using DigitalRise.Graphics;
using DigitalRise.Graphics.PostProcessing;
using Microsoft.Xna.Framework.Graphics;


namespace DigitalRise.LevelEditor
{
  // This post-processor adds a simple distortion effect. See Water/Underwater.fx.
  // Underwater.fx uses only effect parameters for which the DigitalRise Engine already
  // provides effect parameter bindings:
  //   DefaultEffectParameterSemantics.ViewportSize
  //   DefaultEffectParameterSemantics.Time
  //   DefaultEffectParameterSemantics.SourceTexture
  // Therefore, we can simply derive from the EffectPostProcessor and all parameters will
  // be set automatically.
  public class UnderwaterPostProcessor : EffectPostProcessor
  {
    public UnderwaterPostProcessor(IGraphicsService graphicsService, AssetManager content)
      : base(graphicsService, content.LoadEffect(graphicsService.GraphicsDevice, Utility.EffectsPrefix + "Water/Underwater.efb"))
    {
    }
  }
}
