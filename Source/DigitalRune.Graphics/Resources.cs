using System.Collections.Generic;
using AssetManagementBase;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DigitalRune
{
	public static class Resources
	{
		private static Texture2D _normalsFittingTexture;
		private static AssetManager _assetManagerEffects = AssetManager.CreateResourceAssetManager(typeof(Resources).Assembly, "EffectsSource.FNA.bin");
		private static AssetManager _assetManagerResources = AssetManager.CreateResourceAssetManager(typeof(Resources).Assembly, "Resources");

		public static Effect GetDREffect(GraphicsDevice graphicsDevice, string path, Dictionary<string, string> defs = null)
		{
			path = path.Replace('\\', '/');
			if (path.StartsWith("DigitalRune/"))
			{
				path = path.Substring(12);
			}

			if (!path.EndsWith(".efb"))
			{
				path += ".efb";
			}

			return _assetManagerEffects.LoadEffect(graphicsDevice, path, defs);
		}

		public static Game game;

		public static Texture2D NormalsFittingTexture(GraphicsDevice graphicsDevice)
		{
			if (_normalsFittingTexture == null)
			{
				// _normalsFittingTexture = _assetManagerResources.LoadTexture2D(graphicsDevice, "NormalsFittingTexture.dds");
				_normalsFittingTexture = game.Content.Load<Texture2D>(@"D:\Projects\DigitalRune2\Source\DigitalRune.Graphics.Content\bin\Windows\DigitalRune\NormalsFittingTexture.xnb");
			}

			return _normalsFittingTexture;
		}
	}
}
