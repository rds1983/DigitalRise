using System.IO;
using DigitalRune.Graphics;
using DigitalRune.Graphics.SceneGraph;

namespace AssetManagementBase
{
	public static class DRGraphicsXNAssetsExt
	{
		private readonly static AssetLoader<Material> _drMaterialLoader = (manager, assetName, settings, tag) =>
		{
			var xml = manager.ReadAsString(assetName);

			return Material.FromXml(manager, (IGraphicsService)tag, xml);
		};

		private readonly static AssetLoader<ModelNode> _gltfLoader = (manager, assetName, settings, tag) =>
		{
			var loader = new GltfLoader();

			var graphicsService = (IGraphicsService)tag;
			var result = loader.Load(manager, graphicsService, assetName);

			var drmlAssetName = Path.ChangeExtension(assetName, "drmdl");
			if (manager.Exists(drmlAssetName))
			{
				var material = manager.LoadDRMaterial(graphicsService, drmlAssetName);


			}

			return result;
		};

		public static ModelNode LoadGltf(this AssetManager assetManager, IGraphicsService graphicsService, string path)
		{
			return assetManager.UseLoader(_gltfLoader, path, tag: graphicsService);
		}

		public static Material LoadDRMaterial(this AssetManager assetManager, IGraphicsService graphicsService, string path)
		{
			return assetManager.UseLoader(_drMaterialLoader, path, tag: graphicsService);
		}
	}
}
