using DigitalRise.UI.Controls;
using Microsoft.Xna.Framework;

namespace DigitalRise.UI.Rendering
{
	public interface IBrush
	{
		void Draw(UIRenderContext context, RectangleF dest, Color color);
	}

	public static class IBrushExtensions
	{
		public static void Draw(this IBrush brush, UIRenderContext context, RectangleF dest)
		{
			brush.Draw(context, dest, Color.White);
		}
	}
}
