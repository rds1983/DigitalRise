using System;
using System.IO;
using DigitalRune.Graphics;
using DigitalRune.Graphics.SceneGraph;

namespace AssetManagementBase
{
	public static class DRGraphicsXNAssetsExt
	{
		private readonly static AssetLoader<ModelNode> _gltfLoader = (manager, assetName, settings, tag) =>
		{
			var loader = new GltfLoader();

			var graphicsService = (IGraphicsService)tag;
			return loader.Load(manager, graphicsService, assetName);
		};

		private readonly static AssetLoader<Material> _drMaterialLoader = (manager, assetName, settings, tag) =>
		{
			var xml = manager.ReadAsString(assetName);

			return Material.FromXml(manager, (IGraphicsService)tag, xml);
		};

		private readonly static AssetLoader<ModelNode> _drModelLoader = (manager, assetName, settings, tag) =>
		{

			var xml = manager.ReadAsString(assetName);
			var modelDescription = ModelDescription.Parse(xml);

			var graphicsService = (IGraphicsService)tag;
			var result = manager.LoadGltf(graphicsService, modelDescription.FileName);

			result.RecursiveProcess(node =>
			{
				var meshNode = node as MeshNode;
				if (meshNode == null)
				{
					return;
				}

				var desc = modelDescription.GetMeshDescription(node.Name);
				if (desc != null)
				{
					for (var i = 0; i < Math.Min(meshNode.Mesh.Submeshes.Count, desc.Submeshes.Count); ++i)
					{
						var subMeshDesc = desc.Submeshes[i];
						if (string.IsNullOrEmpty(subMeshDesc.Material))
						{
							continue;
						}

						var material = manager.LoadDRMaterial(graphicsService, subMeshDesc.Material);
						meshNode.Mesh.Submeshes[i].SetMaterial(material);
					}
				}
			});

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

		public static ModelNode LoadDRModel(this AssetManager assetManager, IGraphicsService graphicsService, string path)
		{
			return assetManager.UseLoader(_drModelLoader, path, tag: graphicsService);
		}
	}
}
