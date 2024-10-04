﻿using DigitalRise.Modelling;
using DigitalRise.Rendering;

namespace AssetManagementBase
{
	public static class DigitalRiseAssetsExt
	{
		private readonly static AssetLoader<ModelInstance> _gltfLoader = (manager, assetName, settings, tag) =>
		{
			var loader = new GltfLoader();

			return loader.Load(manager, assetName);
		};

		private readonly static AssetLoader<Scene> _sceneLoader = (manager, assetName, settings, tag) =>
		{
			var data = manager.LoadString(assetName);
			return Scene.ReadFromString(data, manager);
		};

		public static ModelInstance LoadGltf(this AssetManager assetManager,
			string path) => assetManager.UseLoader(_gltfLoader, path);

		public static Scene LoadScene(this AssetManager assetManager,
			string path) => assetManager.UseLoader(_sceneLoader, path);
	}
}
