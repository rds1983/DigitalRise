using DigitalRune.Graphics;
using DigitalRune.Graphics.SceneGraph;

namespace AssetManagementBase
{
	public static class DRGraphicsXNAssetsExt
	{
		private readonly static AssetLoader<ModelNode> _gltfLoader = (manager, assetName, settings, tag) =>
		{
			var loader = new GltfLoader();

			return loader.Load(manager, (IGraphicsService)tag, assetName);
		};

		public static ModelNode LoadGltf(this AssetManager assetManager, IGraphicsService graphicsService, string path)
		{
			return assetManager.UseLoader(_gltfLoader, path, tag: graphicsService);
		}
	}
}
