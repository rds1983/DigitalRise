#if !WP7
using AssetManagementBase;
using DigitalRise;
using DigitalRise.Graphics;
using DigitalRise.Graphics.PostProcessing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Samples.Graphics
{
  // A simple PostProcessor that inverts colors to create a negative image.
  public class NegativeFilter : PostProcessor
  {
    private readonly Effect _effect;
    private readonly EffectParameter _strengthParameter;
    private readonly EffectParameter _textureParameter;
    private readonly EffectParameter _viewportSizeParameter;


    // The strength of the effect in the range [0, 1].
    public float Strength { get; set; }


    public NegativeFilter(IGraphicsService graphicsService, AssetManager assetManager)
      : base(graphicsService)
    {
      _effect = assetManager.LoadEffect(graphicsService.GraphicsDevice, Utility.EffectsPrefix + "PostProcessing/NegativeFilter.efb");
      _strengthParameter = _effect.Parameters["Strength"];
      _textureParameter = _effect.Parameters["SourceTexture"];
      _viewportSizeParameter = _effect.Parameters["ViewportSize"];
      Strength = 1;
    }


    protected override void OnProcess(RenderContext context)
    {
      var graphicsDevice = GraphicsService.GraphicsDevice;

      // Set the render target and also apply the current viewport. 
      graphicsDevice.SetRenderTarget(context.RenderTarget);
      graphicsDevice.Viewport = context.Viewport;

      // Choose the effect technique. 
      // (Since the current NegativeFilter has only 1 technique we don't need to do anything.)
      // _effect.CurrentTechnique = _effect.Techniques[0];

      // Update the required effect parameters.
      _strengthParameter.SetValue(Strength);
      _textureParameter.SetValue(context.SourceTexture);
      _viewportSizeParameter.SetValue(new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height));

      // Apply the effect and do the post-processing.
      _effect.CurrentTechnique.Passes[0].Apply();
      graphicsDevice.DrawFullScreenQuad();
    }
  }
}
#endif