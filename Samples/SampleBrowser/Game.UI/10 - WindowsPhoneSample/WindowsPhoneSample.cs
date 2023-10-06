using DigitalRise.UI.Controls;
using DigitalRise.UI.Rendering;
using DigitalRise.Graphics;
using Microsoft.Xna.Framework;
using AssetManagementBase;

namespace Samples.Game.UI
{
  [Sample(SampleCategory.GameUI,
    "This sample shows how to use the DigitalRise Game UI on the Windows Phone 7.",
    "Note: This sample was created before we could mix Silverlight and XNA on WP7.",
    10)]
  public class WindowsPhoneSample : Sample
  {
    private readonly UIScreen _uiScreen;


    public WindowsPhoneSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      // Add a DelegateGraphicsScreen as the first graphics screen to the graphics
      // service. This lets us do the rendering in the Render method of this class.
      var graphicsScreen = new DelegateGraphicsScreen(GraphicsService)
      {
        RenderCallback = Render,
      };
      GraphicsService.Screens.Insert(0, graphicsScreen);

      // Load a UI theme, which defines the appearance and default values of UI controls.
      Theme theme = AssetManager.LoadTheme("UI Themes/WindowsPhone7/ThemeDark.xml");

      // Create a UI renderer, which uses the theme info to renderer UI controls.
      UIRenderer renderer = new UIRenderer(theme);

      // Create a UIScreen and add it to the UI service. The screen is the root of the 
      // tree of UI controls. Each screen can have its own renderer.
      _uiScreen = new UIScreen("SampleUIScreen", renderer)
      {
        // Make the screen transparent.
        Background = new Color(0, 0, 0, 0),
      };
      UIService.Screens.Add(_uiScreen);

      // Open a window.
      var window = new WpWindow();
      window.Show(_uiScreen);
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Remove UIScreen from UI service.
        UIService.Screens.Remove(_uiScreen);
      }

      base.Dispose(disposing);
    }


    private void Render(RenderContext context)
    {
      _uiScreen.Draw(context.DeltaTime);
    }
  }
}
