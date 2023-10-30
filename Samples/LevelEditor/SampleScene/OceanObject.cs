﻿using System.Linq;
using DigitalRise;
using DigitalRise.GameBase;
using DigitalRise.Geometry;
using DigitalRise.Graphics;
using DigitalRise.Graphics.SceneGraph;
using AssetManagementBase;
using Microsoft.Xna.Framework;
using System;

namespace DigitalRise.LevelEditor
{
	/// <summary>
	/// Adds an infinite ocean to the scene.
	/// </summary>
	public class OceanObject : GameObject
	{
		private readonly IServiceProvider _services;
		private WaterNode _waterNode;


		public OceanObject(IServiceProvider services)
		{
			_services = services;
			Name = "Ocean";
		}


		protected override void OnLoad()
		{
			var scene = (Scene)_services.GetService<IScene>();
			var graphicsService = _services.GetService<IGraphicsService>();
			var assetManager = _services.GetService<AssetManager>();

			// Define the appearance of the water.
			var waterOcean = new Water
			{
				SpecularColor = new Vector3(20f),
				SpecularPower = 500,

				NormalMap0 = null,
				NormalMap1 = null,

				RefractionDistortion = 0.1f,
				ReflectionColor = new Vector3(0.2f),
				RefractionColor = new Vector3(0.6f),

				//UnderwaterFogDensity = new Vector3(1, 0.8f, 0.6f),
				//WaterColor = new Vector3(0.2f, 0.4f, 0.5f),
				UnderwaterFogDensity = new Vector3(12, 8, 8) * 0.02f,
				WaterColor = new Vector3(60, 30, 19) * 0.002f,

				// Water is scattered in high waves and this makes the wave crests brighter.
				// ScatterColor defines the intensity of this effect.
				ScatterColor = new Vector3(0.05f, 0.1f, 0.1f),

				// Foam is automatically rendered where the water intersects geometry and
				// where wave are high.
				FoamMap = assetManager.LoadTexture2D(graphicsService.GraphicsDevice, "Water/Foam.png"),
				FoamMapScale = 5,
				FoamColor = new Vector3(1),
				FoamCrestMin = 0.3f,
				FoamCrestMax = 0.8f,

				// Approximate underwater caustics are computed in real-time from the waves.
				CausticsSampleCount = 0,
				CausticsIntensity = 3,
				CausticsPower = 100,
			};

			// If we do not specify a shape in the WaterNode constructor, we get an infinite
			// water plane.
			_waterNode = new WaterNode(waterOcean, null)
			{
				PoseWorld = new Pose(new Vector3(0, -100, 0)),
				SkyboxReflection = scene.GetDescendants().OfType<SkyboxNode>().First(),

				// ExtraHeight must be set to a value greater than the max. wave height. 
				ExtraHeight = 2,
			};
			scene.Children.Add(_waterNode);

			// OceanWaves can be set to displace water surface using a displacement map.
			// The displacement map is computed by the WaterWaveRenderer (see DeferredGraphicsScreen)
			// using FFT and a statistical ocean model.
			_waterNode.Waves = new OceanWaves
			{
				TextureSize = 256,
				HeightScale = 0.008f,
				Wind = new Vector3(10, 0, 10),
				Directionality = 1,
				Choppiness = 1,
				TileSize = 20,
				EnableCpuQueries = false,
			};
		}

		// OnUnload() is called when the GameObject is removed from the IGameObjectService.
		protected override void OnUnload()
		{
			// Remove model and rigid body.
			_waterNode.Parent.Children.Remove(_waterNode);
			_waterNode.Dispose(false);
			_waterNode = null;
		}
	}
}
