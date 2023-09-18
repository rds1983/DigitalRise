using DigitalRune.Graphics.SceneGraph;
using Microsoft.Xna.Framework.Graphics;

namespace AssetManagementBase
{
	public static class DRGraphicsXNAssetsExt
	{
		private readonly static AssetLoader<ModelNode> _gltfLoader = (manager, assetName, settings, tag) =>
		{
			var loader = new GltfLoader();

			return loader.Load(manager, (GraphicsDevice)tag, assetName);
		};

		public static ModelNode LoadGltf(this AssetManager assetManager, GraphicsDevice graphicsDevice, string path)
		{
			return assetManager.UseLoader(_gltfLoader, path, tag: graphicsDevice);
		}
	}
}
