#if !WP7 && !WP8
using AssetManagementBase;
using DigitalRise.Geometry;
using DigitalRise.Geometry.Shapes;
using DigitalRise.Graphics;
using DigitalRise.Graphics.Rendering;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Mathematics.Statistics;
using DigitalRise.Particles;
using DigitalRise.Particles.Effectors;
using DigitalRise.Physics.ForceEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Samples.Graphics
{
  [Sample(SampleCategory.Graphics,
    @"This sample shows how to create a very simple volumetric cloud using a particle system.",
    @"",
    128)]
  public class VolumetricCloudSample : Sample
  {
    private readonly DeferredGraphicsScreen _graphicsScreen;
    private readonly ParticleSystemNode _particleCloud0;
    private readonly ParticleSystemNode _particleCloud1;
    private readonly ParticleSystemNode _particleCloud2;
    private DynamicSkyObject _dynamicSkyObject;


    public VolumetricCloudSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      SampleFramework.IsMouseVisible = false;

      _graphicsScreen = new DeferredGraphicsScreen(Services);
      _graphicsScreen.DrawReticle = true;
      GraphicsService.Screens.Insert(0, _graphicsScreen);
      GameObjectService.Objects.Add(new DeferredGraphicsOptionsObject(Services));

      Services.Register(typeof(DebugRenderer), null, _graphicsScreen.DebugRenderer);
      Services.Register(typeof(IScene), null, _graphicsScreen.Scene);

      // Add gravity and damping to the physics simulation.
      Simulation.ForceEffects.Add(new Gravity());
      Simulation.ForceEffects.Add(new Damping());

      // Add a custom game object which controls the camera.
      var cameraGameObject = new CameraObject(Services);
      GameObjectService.Objects.Add(cameraGameObject);
      _graphicsScreen.ActiveCameraNode = cameraGameObject.CameraNode;

      GameObjectService.Objects.Add(new GrabObject(Services));
      _dynamicSkyObject = new DynamicSkyObject(Services, false, false, true);
      GameObjectService.Objects.Add(_dynamicSkyObject);
      GameObjectService.Objects.Add(new GroundObject(Services));
      GameObjectService.Objects.Add(new ObjectCreatorObject(Services));
      GameObjectService.Objects.Add(new LavaBallsObject(Services));

      _particleCloud0 = new ParticleSystemNode(CreateParticleCloud(AssetManager))
      {
        PoseLocal = new Pose(new Vector3(-0, 100, -400)),
      };
      ParticleSystemService.ParticleSystems.Add(_particleCloud0.ParticleSystem);
      _graphicsScreen.Scene.Children.Add(_particleCloud0);

      _particleCloud1 = new ParticleSystemNode(CreateParticleCloud(AssetManager))
      {
        PoseLocal = new Pose(new Vector3(-200, 100, -200)),
      };
      ParticleSystemService.ParticleSystems.Add(_particleCloud1.ParticleSystem);
      _graphicsScreen.Scene.Children.Add(_particleCloud1);

      _particleCloud2 = new ParticleSystemNode(CreateParticleCloud(AssetManager))
      {
        PoseLocal = new Pose(new Vector3(400, 400, -400)),
      };
      ParticleSystemService.ParticleSystems.Add(_particleCloud2.ParticleSystem);
      _graphicsScreen.Scene.Children.Add(_particleCloud2);
    }



    private ParticleSystem CreateParticleCloud(AssetManager assetManager)
    {
      var ps = new ParticleSystem
      {
        Name = "Cloud",
        MaxNumberOfParticles = 10,
        Shape = new BoxShape(400, 300, 400),  // This bounding shape must be big enough to include all particles!
        ReferenceFrame = ParticleReferenceFrame.Local,
      };

      ps.Parameters.AddUniform<float>(ParticleParameterNames.Lifetime).DefaultValue = float.PositiveInfinity;

      ps.Parameters.AddVarying<Vector3>(ParticleParameterNames.Position);
      ps.Effectors.Add(new StartPositionEffector
      {
        Parameter = ParticleParameterNames.Position,
        Distribution = new SphereDistribution
        {
          Center = new Vector3(0),
          InnerRadius = 0,
          OuterRadius = 1,
          Scale = new Vector3(100, 50, 100),
        },
      });

      ps.Parameters.AddVarying<float>(ParticleParameterNames.Angle);
      ps.Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.Angle,
        Distribution = new UniformDistributionF(-0.5f, 0.5f),
      });

      ps.Parameters.AddVarying<float>(ParticleParameterNames.Size);
      ps.Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.Size,
        Distribution = new UniformDistributionF(100, 200),
      });

      ps.Parameters.AddUniform<Vector3>(ParticleParameterNames.Color);
      ps.Effectors.Add(new StartValueEffector<Vector3>
      {
        Parameter = ParticleParameterNames.Color,
        DefaultValue = new Vector3(0.6f),
      });

      ps.Parameters.AddUniform<float>(ParticleParameterNames.Alpha).DefaultValue = 0.5f;

      // DigitalRise Graphics supports "texture atlases": The class PackedTexture 
      // describes a single texture or tile set packed into a texture atlas. The 
      // clouds texture in this example consists of 2 tiles.
      ps.Parameters.AddUniform<PackedTexture>(ParticleParameterNames.Texture).DefaultValue =
        new PackedTexture(
          "Clouds",
          assetManager.LoadTexture2D(GraphicsService.GraphicsDevice, "Particles/Clouds.png"),
          Vector2.Zero, Vector2.One,
          2, 1);

      // The particle parameter "AnimationTime" determines which tile is used,
      // where 0 = first tile, 1 = last tile. 
      // --> Chooses a random tile for each particle when it is created.
      ps.Parameters.AddVarying<float>(ParticleParameterNames.AnimationTime);
      ps.Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.AnimationTime,
        Distribution = new UniformDistributionF(0, 1),
      });

      // It looks better with back-to-front sorting, but for better performance we skip sorting.
      ps.Parameters.AddUniform<bool>(ParticleParameterNames.IsDepthSorted).DefaultValue = false;

      ps.Parameters.AddUniform<BillboardOrientation>(ParticleParameterNames.BillboardOrientation).DefaultValue = BillboardOrientation.ViewpointOriented;

      // Create some particles.
      ps.AddParticles(ps.MaxNumberOfParticles);

      return ps;
    }


    public override void Update(GameTime gameTime)
    {
      _graphicsScreen.DebugRenderer.Clear();

      // Synchronize particle data and graphics data.
      _particleCloud0.Synchronize(GraphicsService);
      _particleCloud1.Synchronize(GraphicsService);
      _particleCloud2.Synchronize(GraphicsService);

      // Update color of clouds.
      var cloudColor = (_dynamicSkyObject.AmbientLight + _dynamicSkyObject.SunLight) * 2;
      _particleCloud0.ParticleSystem.Parameters.Get<Vector3>(ParticleParameterNames.Color).DefaultValue = cloudColor;
      _particleCloud1.ParticleSystem.Parameters.Get<Vector3>(ParticleParameterNames.Color).DefaultValue = cloudColor;
      _particleCloud2.ParticleSystem.Parameters.Get<Vector3>(ParticleParameterNames.Color).DefaultValue = cloudColor;
    }
  }
}
#endif