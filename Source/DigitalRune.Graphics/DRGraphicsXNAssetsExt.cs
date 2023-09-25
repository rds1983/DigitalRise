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

			var result = Material.FromXml(manager, (IGraphicsService)tag, xml);

			if (string.IsNullOrEmpty(result.Name))
			{
				result.Name = Path.GetFileNameWithoutExtension(assetName);
			}

			return result;
		};

		private readonly static AssetLoader<ModelNode> _drModelLoader = (manager, assetName, settings, tag) =>
		{

			var xml = manager.ReadAsString(assetName);
			var modelDescription = ModelDescription.Parse(xml);

			var graphicsService = (IGraphicsService)tag;

			var source = manager.LoadGltf(graphicsService, modelDescription.FileName);
			var result = source.Clone();

			foreach(var meshNode in result.MeshNodes())
			{
				var desc = modelDescription.GetMeshDescription(meshNode.Name);
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
				} else
				{
					// If material isn't set explicitly, determine it from the texture file name
					for (var i = 0; i < meshNode.Mesh.Submeshes.Count; ++i)
					{
						var subMesh = meshNode.Mesh.Submeshes[i];
						var defaultMaterial = subMesh.GetMaterial();
						if (defaultMaterial == null || string.IsNullOrEmpty(defaultMaterial.Name))
						{
							continue;
						}

						var materialName = Path.ChangeExtension(defaultMaterial.Name, "drmat");
						if (manager.Exists(materialName))
						{
							var material = manager.LoadDRMaterial(graphicsService, materialName);
							subMesh.SetMaterial(material);
						}
					}
				}

				if (modelDescription.Scale != 1.0f)
				{
					meshNode.ScaleLocal *= modelDescription.Scale;
				}
			};

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
