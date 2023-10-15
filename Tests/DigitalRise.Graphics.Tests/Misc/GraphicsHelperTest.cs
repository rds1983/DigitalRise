﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;


namespace DigitalRise.Graphics.Tests
{
  [TestFixture]
  public class GraphicsHelperTest
  {
    private GraphicsDevice _graphicsDevice0;
    private GraphicsDevice _graphicsDevice1;
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

      _graphicsDevice0 = new GraphicsDevice(TestsEnvironment.GraphicsDevice.Adapter, GraphicsProfile.HiDef, parameters);
      _graphicsDevice1 = new GraphicsDevice(TestsEnvironment.GraphicsDevice.Adapter, GraphicsProfile.HiDef, parameters);
      _graphicsService0 = new GraphicsManager(_graphicsDevice0);
      _graphicsService1 = new GraphicsManager(_graphicsDevice1);
    }

    [TearDown]
    public void TearDown()
    {
      _graphicsDevice0.Dispose();
      _graphicsDevice1.Dispose();
    }


    [Test]
    public void DefaultTextures()
    {
      Assert.AreEqual(_graphicsService0.GetDefaultTexture2DBlack(), _graphicsService0.GetDefaultTexture2DBlack());
      Assert.AreNotEqual(_graphicsService0.GetDefaultTexture2DBlack(), _graphicsService1.GetDefaultTexture2DBlack());

      var t = _graphicsService1.GetDefaultTexture2DWhite();
      _graphicsDevice1.Dispose();
      
      // Note: Since the graphics device is also disposed and re-created when the game is
      // moved between screens - we must not auto-dispose our textures.
      //Assert.IsTrue(t.IsDisposed);
    }
  }
}