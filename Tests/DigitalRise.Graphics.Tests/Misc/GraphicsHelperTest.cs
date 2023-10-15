using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;


namespace DigitalRise.Graphics.Tests
{
  [TestFixture]
  public class GraphicsHelperTest
  {
    private IGraphicsService _graphicsService0;
    private IGraphicsService _graphicsService1;

    [SetUp]
    public void SetUp()
    {
      var parameters = new PresentationParameters
      {
        BackBufferWidth = 1280,
        BackBufferHeight = 720,
        BackBufferFormat = SurfaceFormat.Color,
        DepthStencilFormat = DepthFormat.Depth24Stencil8,
        PresentationInterval = PresentInterval.Immediate,
        IsFullScreen = false
      };

      _graphicsService0 = new GraphicsManager(TestsEnvironment.GraphicsDevice);
      _graphicsService1 = new GraphicsManager(TestsEnvironment.GraphicsDevice);
    }

    [Test]
    public void DefaultTextures()
    {
      Assert.AreEqual(_graphicsService0.GetDefaultTexture2DBlack(), _graphicsService0.GetDefaultTexture2DBlack());
      Assert.AreNotEqual(_graphicsService0.GetDefaultTexture2DBlack(), _graphicsService1.GetDefaultTexture2DBlack());

      var t = _graphicsService1.GetDefaultTexture2DWhite();
      
      // Note: Since the graphics device is also disposed and re-created when the game is
      // moved between screens - we must not auto-dispose our textures.
      //Assert.IsTrue(t.IsDisposed);
    }
  }
}