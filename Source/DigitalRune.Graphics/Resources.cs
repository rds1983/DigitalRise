using AssetManagementBase;
using Microsoft.Xna.Framework.Graphics;

namespace DigitalRune
{
	internal static class Resources
	{
		private static Texture2D _normalsFittingTexture;
		private static AssetManager _assetManagerEffects = AssetManager.CreateResourceAssetManager(typeof(Resources).Assembly, "EffectsSource.FNA");
		private static AssetManager _assetManagerResources = AssetManager.CreateResourceAssetManager(typeof(Resources).Assembly, "Resources");

		public static Effect GetDREffect(GraphicsDevice graphicsDevice, string path)
		{
			if (path.StartsWith("DigitalRune/"))
			{
				path = path.Substring(12);
			}

			if (!path.EndsWith(".efb"))
			{
				path += ".efb";
			}

			return _assetManagerEffects.LoadEffect(graphicsDevice, path);
		}

		public static Texture2D NormalsFittingTexture(GraphicsDevice graphicsDevice)
		{
			if (_normalsFittingTexture == null)
			{
				_normalsFittingTexture = _assetManagerResources.LoadTexture2D(graphicsDevice, "NormalsFittingTexture.dds");
			}

			return _normalsFittingTexture;
		}
	}
}
