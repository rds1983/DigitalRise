using DigitalRise;
using DigitalRise.UI.Controls;
using DigitalRise.UI.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Samples.Game.UI
{
  // This UIRenderer adds a render callback for TreeViewItems.
  internal class MyUIRenderer : UIRenderer
  {
    public MyUIRenderer(Theme theme): base(theme)
    {
      // Add a new render method for the style "TreeViewItem".
      RenderCallbacks.Add("TreeViewItem", RenderTreeViewItem);
    }

    public MyUIRenderer(GraphicsDevice graphicsDevice): this(Theme.GetDefault(graphicsDevice))
    {
    }


    public void RenderTreeViewItem(UIControl control, UIRenderContext context)
    {
      var treeViewItem = control as TreeViewItem;
      if (treeViewItem != null && treeViewItem.IsSelected && treeViewItem.Header != null)
      {
        // Draw a blue rectangle behind selected tree view items.
        context.RenderTransform.Draw(
          SpriteBatch,
          WhiteTexture,
          treeViewItem.Header.ActualBounds,
          null,
          Color.CornflowerBlue);
      }

      // Call the default render callback to draw all the rest.
      RenderCallbacks["UIControl"](control, context);
    }
  }
}
