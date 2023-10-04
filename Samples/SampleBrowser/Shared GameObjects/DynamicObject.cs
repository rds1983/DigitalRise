using System;
using System.Linq;
using DigitalRise.GameBase;
using DigitalRise.Geometry;
using DigitalRise.Geometry.Shapes;
using DigitalRise.Graphics;
using DigitalRise.Graphics.Effects;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Mathematics.Statistics;
using DigitalRise.Physics;
using CommonServiceLocator;
using Microsoft.Xna.Framework.Graphics;
using AssetManagementBase;
using Microsoft.Xna.Framework;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace Samples
{
	// Creates and controls a dynamic object (model + rigid body). 
	// Please note, the shining sphere, which drops when <4> is pressed, casts an 
	// omnidirectional shadow. Dropping multiple spheres can quickly reduce performance
	// due to the amount of shadow maps that need to be rendered. Shadows can start to 
	// flicker, if more than 8 shadow-casting light sources overlap on screen.
  public class DynamicObject : GameObject
  {
    private readonly IServiceLocator _services;
    private readonly int _type;


    public ModelNode ModelNode { get; private set; }
    public RigidBody RigidBody { get; private set; }


    public DynamicObject(IServiceLocator services, int type)
    {
      if (type < 1 || type > 7)
        throw new ArgumentOutOfRangeException("type");

      _services = services;
      _type = type;
    }


    // OnLoad() is called when the GameObject is added to the IGameObjectService.
    protected override void OnLoad()
    {
			var assetManager = _services.GetInstance<AssetManager>();
			var graphicsService = _services.GetInstance<IGraphicsService>();

			if (_type == 1)
      {
        // A simple cube.
        RigidBody = new RigidBody(new BoxShape(1, 1, 1));
        ModelNode = assetManager.LoadDRModel(graphicsService, "RustyCube/RustyCube.drmdl").Clone();
      }
      else if (_type == 2)
      {
        // Another simple cube.
        RigidBody = new RigidBody(new BoxShape(1, 1, 1));
        ModelNode = assetManager.LoadDRModel(graphicsService, "MetalGrateBox/MetalGrateBox.drmdl").Clone();
      }
      else if (_type == 3)
      {
        // A TV-like box.
        RigidBody = new RigidBody(new BoxShape(1, 0.6f, 0.8f)) { UserData = "TV" };
        ModelNode = assetManager.LoadDRModel(graphicsService, "TVBox/TVBox.drmdl");

        if (ModelNode.Children.OfType<LightNode>().Count() == 0)
        {
          // This is the first time the "TVBox" is loaded.

          // Add a projector light to the model that projects the TV screen. The
          // TV screen is the emissive part of the TV mesh.
          var meshNode = ModelNode.FindFirstMeshNode();
          var material = meshNode.Mesh.Materials.First(m => m.Name == "TestCard");

          // Get texture from material.
          // Note: In XNA the effect parameter type is Texture. In MonoGame it is Texture2D.
          Texture2D texture;
          EffectParameterBinding parameterBinding = material["Material"].ParameterBindings["EmissiveTexture"];
          if (parameterBinding is EffectParameterBinding<Texture>)
            texture = (Texture2D)((EffectParameterBinding<Texture>)parameterBinding).Value;
          else
            texture = ((EffectParameterBinding<Texture2D>)parameterBinding).Value;

          var projection = new PerspectiveProjection();
          projection.Near = 0.55f;
          projection.Far = 3.0f;
          projection.SetFieldOfView(MathHelper.ToRadians(60), 0.76f / 0.56f);

          var projectorLight = new ProjectorLight(texture, projection);
          projectorLight.Attenuation = 4;
          var projectorLightNode = new LightNode(projectorLight);
          projectorLightNode.LookAt(new Vector3(0, 0.2f, 0), Vector3.Zero, Vector3.UnitZ);

          // Attach the projector light to the model.
          ModelNode.Children.Add(projectorLightNode);
        }

        ModelNode = ModelNode.Clone();
      }
      else if (_type == 4)
      {
        // A "magic" sphere with a colored point light.
        RigidBody = new RigidBody(new SphereShape(0.25f));
        ModelNode = assetManager.LoadDRModel(graphicsService, "MagicSphere/MagicSphere.drmdl");

        if (ModelNode.Children.OfType<LightNode>().Count() == 0)
        {
          // This is the first time the "MagicSphere" is loaded.

          // Change the size of the sphere.
          var meshNode = ModelNode.FindFirstMeshNode();
          meshNode.ScaleLocal = new Vector3(0.5f);

          // Disable shadows. (The sphere acts as a light source.)
          meshNode.CastsShadows = false;

          // Add a point light.
          var pointLight = new PointLight
          {
            Color = new Vector3(1, 1, 1),
            DiffuseIntensity = 4,
            SpecularIntensity = 4,
            Range = 3,
            Attenuation = 1,
            Texture = assetManager.LoadTextureCube(graphicsService.GraphicsDevice, "MagicSphere/ColorCube.dds"),
          };
          var pointLightNode = new LightNode(pointLight)
          {
            // The point light uses shadow mapping to cast an omnidirectional shadow.
            Shadow = new CubeMapShadow
            {
              PreferredSize = 64,
            }
          };

          ModelNode.Children.Add(pointLightNode);
        }

        ModelNode = ModelNode.Clone();
      }
      else if (_type == 5)
      {
        // A sphere of glass (or "bubble").
        RigidBody = new RigidBody(new SphereShape(0.3f));
        ModelNode = assetManager.LoadDRModel(graphicsService, "Bubble/Bubble.drmdl").Clone();
        ModelNode.FindFirstMeshNode().ScaleLocal *= new Vector3(0.5f);
      }
      else if (_type == 6)
      {
        // A rusty barrel with multiple levels of detail (LODs).
        RigidBody = new RigidBody(new CylinderShape(0.35f, 1));
        ModelNode = assetManager.LoadDRModel(graphicsService, "Barrel/Barrel.drmdl").Clone();
      }
      else
      {
        // A cube consisting of a frame and transparent sides.
        RigidBody = new RigidBody(new BoxShape(1, 1, 1));
        ModelNode = assetManager.LoadDRModel(graphicsService, "GlassBox/GlassBox.drmdl").Clone();
      }

      SampleHelper.EnablePerPixelLighting(ModelNode);

      // Set a random pose.
      var randomPosition = new Vector3(
        RandomHelper.Random.NextFloat(-10, 10),
        RandomHelper.Random.NextFloat(2, 5),
        RandomHelper.Random.NextFloat(-20, 0));
      RigidBody.Pose = new Pose(randomPosition, RandomHelper.Random.NextQuaternionF());
      ModelNode.PoseWorld = RigidBody.Pose;

      // Add rigid body to physics simulation and model to scene.
      var simulation = _services.GetInstance<Simulation>();
      simulation.RigidBodies.Add(RigidBody);

      var scene = _services.GetInstance<IScene>();
      scene.Children.Add(ModelNode);
    }


    // OnUnload() is called when the GameObject is removed from the IGameObjectService.
    protected override void OnUnload()
    {
      // Remove model and rigid body.
      ModelNode.Parent.Children.Remove(ModelNode);
      ModelNode.Dispose(false);
      ModelNode = null;

      RigidBody.Simulation.RigidBodies.Remove(RigidBody);
      RigidBody = null;
    }


    // OnUpdate() is called once per frame.
    protected override void OnUpdate(TimeSpan deltaTime)
    {
      // Synchronize graphics <--> physics.
      if (ModelNode != null)
      {
        // Update SceneNode.LastPoseWorld - this is required for some effects 
        // like object motion blur. 
        ModelNode.SetLastPose(true);

        ModelNode.PoseWorld = RigidBody.Pose;
      }
    }
  }
}
