using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DigitalRise.UI.TextureAtlases
{
	public struct Padding
	{
		public int Left, Top, Right, Bottom;
	}

	public class NinePatchRegion: TextureRegion
	{
		public Padding Border { get; private set; }

		public NinePatchRegion(Texture2D texture, Rectangle rectangle, Padding border):
			base(texture, rectangle)
		{
			Border = border;
		}
	}
}
