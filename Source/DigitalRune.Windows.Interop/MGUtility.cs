using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D11;
using System.Reflection;

namespace DigitalRune.Windows.Interop
{
	internal static class MGUtility
	{
		private static FieldInfo _sharpDXTextureField = typeof(Texture).GetField("_texture", BindingFlags.NonPublic | BindingFlags.Instance);

		public static Resource SharpDXTexture(this Texture texture)
		{
			return (Resource)_sharpDXTextureField.GetValue(texture);
		}
	}
}
