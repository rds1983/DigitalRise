using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DigitalRise.Graphics.Tests
{
	class TestGame: Game
	{
		private readonly GraphicsDeviceManager _graphics;

		public TestGame()
		{
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1200,
				PreferredBackBufferHeight = 800,
				PreferredBackBufferFormat = SurfaceFormat.Color,
				PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8,
				IsFullScreen = false
			};

			((IGraphicsDeviceManager)Services.GetService(typeof(IGraphicsDeviceManager))).CreateDevice();
		}
	}
}
