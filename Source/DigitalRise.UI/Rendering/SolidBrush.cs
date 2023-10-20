using FontStashSharp.RichText;
using System;
using Microsoft.Xna.Framework;
using DigitalRise.UI.Controls;

namespace DigitalRise.UI.Rendering
{
	public class SolidBrush : IBrush
	{
		private Color _color = Color.White;

		public Color Color
		{
			get
			{
				return _color;
			}

			set
			{
				_color = value;
			}
		}

		public SolidBrush(Color color)
		{
			Color = color;
		}

		public SolidBrush(string color)
		{
			var c = ColorStorage.FromName(color);
			if (c == null)
			{
				throw new ArgumentException(string.Format("Could not recognize color '{0}'", color));
			}

			Color = c.Value;
		}

		public void Draw(UIRenderContext context, RectangleF dest, Color color)
		{
			if (color == Color.White)
			{
				context.FillRectangle(dest, Color);
			}
			else
			{
				var c = new Color((int)(Color.R * color.R / 255.0f),
					(int)(Color.G * color.G / 255.0f),
					(int)(Color.B * color.B / 255.0f),
					(int)(Color.A * color.A / 255.0f));

				context.FillRectangle(dest, c);
			}
		}
	}
}