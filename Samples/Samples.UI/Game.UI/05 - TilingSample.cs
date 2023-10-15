﻿using DigitalRise.UI.Controls;
using DigitalRise.UI.Rendering;
using AssetManagementBase;
using Microsoft.Xna.Framework;

namespace Samples.UI
{
  [Sample(@"This sample shows how to create UI controls using image tiling.",
    @"Image tiling is new feature supported by DigitalRise Game UI: UI controls are usually 
rendered using images from a texture atlas. The images can be automatically repeated to 
fill up the required space.

This sample creates two resizable windows with custom styles:
- The first window consists of images which are stretched to fill the space of the window.
- The second windows consists of images which are tiled automatically depending on the size 
  of the window.

Open the file Content/UI Themes/TilingSample/Theme.xml to see how the styles are defined.",
    5)]
  public class TilingSample : Sample
  {
    private readonly UIScreen _uiScreen;


    public TilingSample()
    {
      // Load a UI theme, which defines the appearance and default values of UI controls.
      Theme theme = AssetManager.LoadTheme(GraphicsDevice, "UI Themes/TilingSample/Theme.xml");

      // Create a UI renderer, which uses the theme info to renderer UI controls.
      UIRenderer renderer = new UIRenderer(theme);

      // Create a UIScreen and add it to the UI service. The screen is the root of the 
      // tree of UI controls. Each screen can have its own renderer.
      _uiScreen = new UIScreen("SampleUIScreen", renderer);
      UIService.Screens.Add(_uiScreen);

      // Create a window using the default style "Window".
      var stretchedWindow = new Window
      {
        X = 100,
        Y = 100,
        Width = 480,
        Height = 320,
        CanResize = true,
      };
      _uiScreen.Children.Add(stretchedWindow);

      // Create a window using the style "TiledWindow".
      var tiledWindow = new Window
      {
        X = 200,
        Y = 200,
        Width = 480,
        Height = 320,
        CanResize = true,
        Style = "TiledWindow",
      };
      _uiScreen.Children.Add(tiledWindow);

      // Check file TilingSampleContent/Theme.xml to see how the styles are defined.
    }

		public override void Render(GameTime gameTime)
		{
			base.Render(gameTime);

      _uiScreen.Draw(gameTime);
    }
  }
}