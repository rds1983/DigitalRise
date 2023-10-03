using AssetManagementBase;
using DigitalRise.UI;
using DigitalRise.UI.Controls;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Samples.Game.UI
{
  // A resizable window with a ScrollViewer.
  public class ResizableWindow : Window
  {
    public ResizableWindow(AssetManager assetManager, GraphicsDevice graphicsDevice)
    {
      Title = "Resizable Window";
      Width = 320;
      Height = 240;
      CanResize = true;

      var image = new Image
      {
        Texture = assetManager.LoadTexture2D(graphicsDevice, "Sky.png"),
      };

      var scrollViewer = new ScrollViewer
      {
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Stretch,
        Content = image
      };

      Content = scrollViewer;
    }
  }
}
