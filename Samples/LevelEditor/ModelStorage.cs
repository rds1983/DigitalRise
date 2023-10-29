using System.Collections.Generic;
using System.IO;
using AssetManagementBase;
using DigitalRise.Graphics.SceneGraph;

namespace DigitalRise.LevelEditor
{
	public static class ModelStorage
	{
		public static Dictionary<string, ModelNode> Models { get; } = new Dictionary<string, ModelNode>();

		public static void Load(string path)
		{
			Models.Clear();

/*			var assetManager = AssetManager.CreateFileAssetManager(path);

			var files = Directory.EnumerateFiles(path);
			foreach (var file in files)
			{
				if (!file.EndsWith(".glb") && !file.EndsWith(".gltf"))
				{
					continue;
				}

				var name = Path.GetFileName(file);
				var model = assetManager.LoadGltf(name);
				Models[Path.GetFileNameWithoutExtension(name)] = model;
			}*/
		}
	}
}
