using DigitalRune.Graphics;
using DigitalRune.Graphics.Effects;
using DigitalRune.Graphics.Rendering;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics.ForceEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AssetManagementBase;
using System.Collections.Generic;

namespace Samples.Graphics
{
  [Sample(SampleCategory.Graphics,
    @"This sample shows how to change the terrain material to enable parallax occlusion mapping.",
    @"",
    138)]
  public class ParallaxTerrainSample : Sample
  {
    private readonly DeferredGraphicsScreen _graphicsScreen;
    private readonly TerrainObject _terrainObject;


    public ParallaxTerrainSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      SampleFramework.IsMouseVisible = false;

      _graphicsScreen = new DeferredGraphicsScreen(Services);
      _graphicsScreen.DrawReticle = true;
      GraphicsService.Screens.Insert(0, _graphicsScreen);
      GameObjectService.Objects.Add(new DeferredGraphicsOptionsObject(Services));

      Services.Register(typeof(DebugRenderer), null, _graphicsScreen.DebugRenderer);

      var scene = _graphicsScreen.Scene;
      Services.Register(typeof(IScene), null, scene);

      // Add standard game objects.
      var cameraGameObject = new CameraObject(Services, 5000);
      cameraGameObject.ResetPose(new Vector3F(0, 2, 0), 0, 0);
      GameObjectService.Objects.Add(cameraGameObject);
      _graphicsScreen.ActiveCameraNode = cameraGameObject.CameraNode;

      Simulation.ForceEffects.Add(new Gravity());
      Simulation.ForceEffects.Add(new Damping());
      GameObjectService.Objects.Add(new GrabObject(Services));
      GameObjectService.Objects.Add(new DynamicSkyObject(Services, true, false, true)
      {
        EnableCloudShadows = false,
      });
      GameObjectService.Objects.Add(new ObjectCreatorObject(Services));

      // Create terrain.
      _terrainObject = new TerrainObject(Services);
      GameObjectService.Objects.Add(_terrainObject);

      // To see parallax occlusion mapping, we need a detail texture with a height map.
      // In this sample we reuse the pavement texture of the ParallaxMappingSample and 
      // add it to the terrain.
      for (int row = 0; row < 2; row++)
      {
        for (int column = 0; column < 2; column++)
        {
          var terrainTile = _terrainObject.TerrainNode.Terrain.Tiles[row * 2 + column];
          string tilePostfix = "-" + row + "-" + column; // e.g. "-0-1"

          var materialPavement = new TerrainMaterialLayer(GraphicsService)
          {
            DiffuseColor = new Vector3F(1),
            SpecularColor = new Vector3F(5),
            SpecularPower = 20,
            DiffuseTexture = AssetManager.LoadTexture2D(GraphicsService.GraphicsDevice, "Parallax/AgedPavement_diffuse.dds"),
            NormalTexture = AssetManager.LoadTexture2D(GraphicsService.GraphicsDevice, "Parallax/AgedPavement_normal.dds"),
            SpecularTexture = AssetManager.LoadTexture2D(GraphicsService.GraphicsDevice, "Parallax/AgedPavement_specular.dds"),
            HeightTexture = AssetManager.LoadTexture2D(GraphicsService.GraphicsDevice, "Parallax/AgedPavement_height.dds"),
            TileSize = 0.005f * 512,
            BlendTexture = AssetManager.LoadTexture2D(GraphicsService.GraphicsDevice, "Terrain/Terrain001-Blend-Grass" + tilePostfix + ".png"),
            BlendTextureChannel = 0,
            BlendHeightInfluence = 0.5f,
            BlendThreshold = 0.5f,
            BlendRange = 0.5f,
          };

          terrainTile.Layers.Add(materialPavement);
        }
      }

      // Replace the terrain effects with effects which support parallax occlusion mapping.
      UpdateTerrainMaterial(true);

      CreateGuiControls();
    }


    private void UpdateTerrainMaterial(bool enableParallaxOcclusionMapping)
    {
      _terrainObject.UpdateMaterial(HolesType.None, enableParallaxOcclusionMapping ? MappingType.Parallax : MappingType.Simple);
    }


    private void CreateGuiControls()
    {
      var panel = SampleFramework.AddOptions("Terrain");

      SampleHelper.AddCheckBox(
        panel,
        "Enable parallax occlusion mapping",
        true,
        isChecked => UpdateTerrainMaterial(isChecked));

      SampleFramework.ShowOptionsWindow("Terrain");
    }


    public override void Update(GameTime gameTime)
    {
      _graphicsScreen.DebugRenderer.Clear();
    }
  }
}
