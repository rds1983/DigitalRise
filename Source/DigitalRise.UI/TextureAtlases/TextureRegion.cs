using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DigitalRise.UI.TextureAtlases
{
	public class TextureRegion
	{
		public Texture2D Texture { get; private set; }
		public Rectangle Rectangle { get; private set; }

		public TextureRegion(Texture2D texture, Rectangle rectangle)
		{
			Texture = texture ?? throw new ArgumentNullException(nameof(texture));
			Rectangle = rectangle;
		}
	}
}
