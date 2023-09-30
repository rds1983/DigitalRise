using System;
using System.Collections.Generic;
using DigitalRise.Game;
using DigitalRise.Geometry;
using DigitalRise.Graphics;
using DigitalRise.Graphics.Rendering;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using CommonServiceLocator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using DirectionalLight = DigitalRise.Graphics.DirectionalLight;
using AssetManagementBase;


namespace Samples
{
  // Adds an array of different lights to the scene for testing.
  public class TestLightsObject : GameObject
  {
    private readonly IServiceLocator _services;
    private DebugRenderer _debugRenderer;
    private readonly List<LightNode> _lights = new List<LightNode>();


    public TestLightsObject(IServiceLocator services)
    {
      _services = services;
      Name = "TestLights";
    }


    protected override void OnLoad()
    {
      var assetManager = _services.GetInstance<AssetManager>();
      var graphicsService = _services.GetInstance<IGraphicsService>();

      _lights.Add(new LightNode(new AmbientLight
      {
        Color = new Vector3F(0.9f, 0.9f, 1f),
        Intensity = 0.05f,
        HemisphericAttenuation = 1,
      })
      {
        Name = "AmbientLight",

        // This ambient light is "infinite", the pose is irrelevant for the lighting. It is only
        // used for the debug rendering below.
        PoseWorld = new Pose(new Vector3F(0, 4, 0)),
      });

      _lights.Add(new LightNode(new DirectionalLight
      {
        Color = new Vector3F(0.6f, 0.8f, 1f),
        DiffuseIntensity = 0.1f,
        SpecularIntensity = 0.1f,
      })
      {
        Name = "DirectionalLightWithShadow",
        Priority = 10,   // This is the most important light.
        PoseWorld = new Pose(new Vector3F(0, 5, 0), Matrix33F.CreateRotationY(-1.4f) * Matrix33F.CreateRotationX(-0.6f)),
        Shadow = new CascadedShadow
        {
          PreferredSize = 1024,
        }
      });

      _lights.Add(new LightNode(new DirectionalLight
      {
        Color = new Vector3F(0.8f, 0.4f, 0.0f),
        DiffuseIntensity = 0.1f,
        SpecularIntensity = 0.0f,
      })
      {
        Name = "DirectionalLight",
        PoseWorld = new Pose(new Vector3F(0, 6, 0), Matrix33F.CreateRotationY(-1.4f) * Matrix33F.CreateRotationX(-0.6f) * Matrix33F.CreateRotationX(ConstantsF.Pi)),
      });

      _lights.Add(new LightNode(new PointLight
      {
        Color = new Vector3F(0, 1, 0),
        DiffuseIntensity = 2,
        SpecularIntensity = 2,
        Range = 3,
        Attenuation = 1f,
      })
      {
        Name = "PointLight",
        PoseWorld = new Pose(new Vector3F(-9, 1, 0))
      });

      _lights.Add(new LightNode(new PointLight
      {
        DiffuseIntensity = 4,
        SpecularIntensity = 4,
        Range = 3,
        Attenuation = 1f,
        Texture = assetManager.LoadTextureCube(graphicsService.GraphicsDevice, "LavaBall/LavaCubeMap.dds"),
      })
      {
        Name = "PointLightWithTexture",
        PoseWorld = new Pose(new Vector3F(-3, 1, 0))
      });

      _lights.Add(new LightNode(new PointLight
      {
        Color = new Vector3F(1, 1, 1),
        DiffuseIntensity = 2,
        SpecularIntensity = 2,
        Range = 3,
        Attenuation = 1f,
      })
      {
        Name = "PointLightWithShadow",
        PoseWorld = new Pose(new Vector3F(3, 1, 0)),
        Shadow = new CubeMapShadow
        {
          PreferredSize = 128,
        }
      });

      _lights.Add(new LightNode(new PointLight
      {
        Color = new Vector3F(1, 1, 1),
        DiffuseIntensity = 4,
        SpecularIntensity = 4,
        Range = 3,
        Attenuation = 1f,
        Texture = assetManager.LoadTextureCube(graphicsService.GraphicsDevice, "MagicSphere/ColorCube.dds"),
      })
      {
        Name = "PointLightWithTextureAndShadow",
        PoseWorld = new Pose(new Vector3F(9, 1, 0)),
        Shadow = new CubeMapShadow
        {
          PreferredSize = 128,
        }
      });

			_lights.Add(new LightNode(new ProjectorLight
			{
        Texture = assetManager.LoadTexture2D(graphicsService.GraphicsDevice, "TVBox/TestCard.png"),
      })
      {
        Name = "ProjectorLight",
        PoseWorld = Pose.FromMatrix(Matrix44F.CreateLookAt(new Vector3F(-1, 1, -7), new Vector3F(-5, 0, -7), new Vector3F(0, 1, 0))).Inverse,
      });

			_lights.Add(new LightNode(new ProjectorLight
			{
        Texture = assetManager.LoadTexture2D(graphicsService.GraphicsDevice, "TVBox/TestCard.png"),
      })
      {
        Name = "ProjectorLightWithShadow",
        PoseWorld = Pose.FromMatrix(Matrix44F.CreateLookAt(new Vector3F(5, 1, -7), new Vector3F(1, 0, -7), new Vector3F(0, 1, 0))).Inverse,
        Shadow = new StandardShadow
				{
          PreferredSize = 128,
        }
      });

      _lights.Add(new LightNode(new Spotlight
      {
        Color = new Vector3F(0, 1, 0),
        DiffuseIntensity = 2,
        SpecularIntensity = 2,
      })
      {
        Name = "Spotlight",
        PoseWorld = Pose.FromMatrix(Matrix44F.CreateLookAt(new Vector3F(-7, 1, -14), new Vector3F(-10, 0, -14), new Vector3F(0, 1, 0))).Inverse,
      });

			_lights.Add(new LightNode(new Spotlight
			{
        DiffuseIntensity = 2,
        SpecularIntensity = 2,
        Texture = assetManager.LoadTexture2D(graphicsService.GraphicsDevice, "TVBox/TestCard.png"),
      })
      {
        Name = "SpotlightWithTexture",
        PoseWorld = Pose.FromMatrix(Matrix44F.CreateLookAt(new Vector3F(-1, 1, -14), new Vector3F(-5, 0, -14), new Vector3F(0, 1, 0))).Inverse,
      });

      _lights.Add(new LightNode(new Spotlight
      {
        DiffuseIntensity = 2,
        SpecularIntensity = 2,
      })
      {
        Name = "SpotlightWithShadow",
        PoseWorld = Pose.FromMatrix(Matrix44F.CreateLookAt(new Vector3F(5, 1, -14), new Vector3F(1, 0, -14), new Vector3F(0, 1, 0))).Inverse,
        Shadow = new StandardShadow
        {
          PreferredSize = 128,
        }
      });

			_lights.Add(new LightNode(new Spotlight
			{
        Color = new Vector3F(1, 1, 0),
        DiffuseIntensity = 2,
        SpecularIntensity = 2,
        Texture = assetManager.LoadTexture2D(graphicsService.GraphicsDevice, "TVBox/TestCard.png"),
      })
      {
        Name = "SpotlightWithTextureAndShadow",
        PoseWorld = Pose.FromMatrix(Matrix44F.CreateLookAt(new Vector3F(11, 1, -14), new Vector3F(5, 0, -14), new Vector3F(0, 1, 0))).Inverse,
        Shadow = new StandardShadow
				{
          PreferredSize = 128,
        }
      });

      var scene = _services.GetInstance<IScene>();
      _debugRenderer = _services.GetInstance<DebugRenderer>();

      foreach (var lightNode in _lights)
        scene.Children.Add(lightNode);
    }


    protected override void OnUnload()
    {
      _debugRenderer = null;

      foreach (var lightNode in _lights)
      {
        lightNode.Parent.Children.Remove(lightNode);
        lightNode.Dispose(false);
      }
      _lights.Clear();
    }


    protected override void OnUpdate(TimeSpan deltaTime)
    {
      // Render wireframe and name of the lights.
      // (Note: This code expects that DebugRenderer.Clear is called every frame.)
      foreach (var lightNode in _lights)
      {
        _debugRenderer.DrawObject(lightNode, Color.Yellow, true, false);
        _debugRenderer.DrawText(lightNode.Name, lightNode.PoseWorld.Position, Color.Yellow, true);
      }
    }
  }
}