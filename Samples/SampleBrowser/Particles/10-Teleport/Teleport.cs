using System;
using AssetManagementBase;
using CommonServiceLocator;
using DigitalRise.Geometry;
using DigitalRise.Graphics;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Mathematics.Statistics;
using DigitalRise.Particles;
using DigitalRise.Particles.Effectors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Samples.Particles
{
  public class Teleport
  {
    // Count the number of instances.
    private static int _count;


    public ParticleSystemNode ParticleSystemNode { get; private set; }
    public ParticleSystem ParticleSystem
    {
      get { return ParticleSystemNode.ParticleSystem; }
    }
    public Pose Pose
    {
      get { return ParticleSystemNode.PoseWorld; }
      set { ParticleSystemNode.PoseWorld = value; }
    }


    public void Initialize(IServiceLocator services)
    {
      if (ParticleSystemNode == null)
      {
        // This is the first time this instance is used.

        var ps = new ParticleSystem
        {
          Name = "Teleport" + _count,
          ReferenceFrame = ParticleReferenceFrame.Local,
          Children = new ParticleSystemCollection
          {
            CreateSparkles(services),
            CreateFastBeams(services),
            CreateSlowBeams(services),
          }
        };

        // Add a uniform float particle parameter that contains the particle system time.
        ps.Parameters.AddUniform<float>("Time");
        ps.Effectors.Add(new TimeToSingleEffector { Parameter = "Time" });

        // Add a uniform GlobalAlpha parameter. This parameter controls the alpha of all
        // child particle systems. 
        ps.Parameters.AddUniform<float>("GlobalAlpha");
        ps.Effectors.Add(new SingleFadeEffector
        {
          ValueParameter = "GlobalAlpha",
          TimeParameter = "Time",
          FadeInStart = 0,
          FadeInEnd = 2,
          FadeOutStart = 2,
          FadeOutEnd = 3,
        });

        ParticleSystemValidator.Validate(ps);
        ParticleSystemValidator.Validate(ps.Children[0]);
        ParticleSystemValidator.Validate(ps.Children[1]);
        ParticleSystemValidator.Validate(ps.Children[2]);

        ParticleSystemNode = new ParticleSystemNode(ps) { Name = "TeleportNode" + _count };
        _count++;
      }
      else
      {
        ParticleSystem.Reset();
        ParticleSystemNode.PoseWorld = Pose.Identity;
      }
    }


    public bool Update(IGraphicsService graphicsService)
    {
      // The effect lasts for 3 seconds.
      if (ParticleSystem.Time >= TimeSpan.FromSeconds(3))
        return false;

      // Synchronize particles <-> graphics.
      ParticleSystemNode.Synchronize(graphicsService);
      return true;
    }


    private static ParticleSystem CreateSparkles(IServiceLocator services)
    {
			var assetManager = services.GetInstance<AssetManager>();
			var graphicsService = services.GetInstance<IGraphicsService>();

			var ps = new ParticleSystem
      {
        Name = "Sparkles",
        MaxNumberOfParticles = 100,
        ReferenceFrame = ParticleReferenceFrame.Local
      };

      ps.Parameters.AddUniform<float>(ParticleParameterNames.Lifetime).DefaultValue = 1f;

      ps.Effectors.Add(new StreamEmitter
      {
        DefaultEmissionRate = 60,
      });

      ps.Parameters.AddVarying<Vector3>(ParticleParameterNames.Position);
      ps.Effectors.Add(new StartPositionEffector
      {
        Parameter = ParticleParameterNames.Position,
        Distribution = new BoxDistribution
        {
          MinValue = new Vector3(-0.6f, 0.0f, -0.6f),
          MaxValue = new Vector3(0.6f, 3.0f, 0.6f)
        }
      });

      ps.Parameters.AddVarying<Vector3>(ParticleParameterNames.Direction);
      ps.Effectors.Add(new StartDirectionEffector
      {
        Parameter = ParticleParameterNames.Direction,
        DefaultValue = Vector3.Up,
      });

      ps.Parameters.AddVarying<float>(ParticleParameterNames.LinearSpeed);
      ps.Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.LinearSpeed,
        Distribution = new UniformDistributionF(-1, 0.1f),
      });

      ps.Effectors.Add(new LinearVelocityEffector());

      ps.Parameters.AddUniform<Vector3>(ParticleParameterNames.LinearAcceleration).DefaultValue =
        new Vector3(0, 2, 0);

      ps.Effectors.Add(new LinearAccelerationEffector());

      ps.Parameters.AddVarying<float>(ParticleParameterNames.Size);
      ps.Effectors.Add(new SingleLinearSegment3Effector
      {
        OutputParameter = ParticleParameterNames.Size,
        Time0 = 0,
        Value0 = 0.1f,
        Time1 = 0.5f,
        Value1 = 0.3f,
        Time2 = 1,
        Value2 = 0.1f,
      });

      ps.Parameters.AddUniform<Vector3>(ParticleParameterNames.Color).DefaultValue =
        new Vector3(0.0f, 1.0f, 1.0f);

      ps.Parameters.AddVarying<float>(ParticleParameterNames.Alpha);
      ps.Effectors.Add(new SingleLinearSegment3Effector
      {
        InputParameter = "GlobalAlpha",
        OutputParameter = ParticleParameterNames.Alpha,
        Time0 = 0,
        Value0 = 0,
        Time1 = 0.5f,
        Value1 = 1,
        Time2 = 1,
        Value2 = 0,
      });

      ps.Parameters.AddUniform<Texture2D>(ParticleParameterNames.Texture).DefaultValue =
      assetManager.LoadTexture2D(graphicsService.GraphicsDevice, "Particles/LensFlare.dds");

      ps.Parameters.AddUniform<float>(ParticleParameterNames.BlendMode).DefaultValue = 0;

      return ps;
    }


    private static ParticleSystem CreateFastBeams(IServiceLocator services)
    {
			var assetManager = services.GetInstance<AssetManager>();
			var graphicsService = services.GetInstance<IGraphicsService>();

			var ps = new ParticleSystem
      {
        Name = "FastBeams",
        MaxNumberOfParticles = 50,
        ReferenceFrame = ParticleReferenceFrame.Local,
      };

      ps.Parameters.AddUniform<float>(ParticleParameterNames.Lifetime).DefaultValue = 0.5f;

      ps.Effectors.Add(new StreamEmitter
      {
        DefaultEmissionRate = 20,
      });

      ps.Parameters.AddVarying<Vector3>(ParticleParameterNames.Position);
      ps.Effectors.Add(new StartPositionEffector
      {
        Parameter = ParticleParameterNames.Position,
        Distribution = new BoxDistribution
        {
          MinValue = new Vector3(-0.6f, 1.4f, -0.6f),
          MaxValue = new Vector3(0.6f, 2.1f, 0.6f)
        }
      });

      // The particles billboards are stretched in the y-direction.
      ps.Parameters.AddUniform<float>(ParticleParameterNames.SizeX).DefaultValue = 0.4f;
      ps.Parameters.AddUniform<float>(ParticleParameterNames.SizeY).DefaultValue = 4.0f;

      ps.Parameters.AddUniform<Vector3>(ParticleParameterNames.Color).DefaultValue =
        new Vector3(0.0f, 1, 1);

      ps.Parameters.AddVarying<float>(ParticleParameterNames.Alpha);
      ps.Effectors.Add(new SingleLinearSegment3Effector
      {
        InputParameter = "GlobalAlpha",
        OutputParameter = ParticleParameterNames.Alpha,
        Time0 = 0,
        Value0 = 0,
        Time1 = 0.5f,
        Value1 = 0.05f,
        Time2 = 1,
        Value2 = 0,
      });

      ps.Parameters.AddUniform<Texture2D>(ParticleParameterNames.Texture).DefaultValue =
      assetManager.LoadTexture2D(graphicsService.GraphicsDevice, "Particles/Beam.dds");

      // Use cylindrical billboards.
      ps.Parameters.AddUniform<BillboardOrientation>(ParticleParameterNames.BillboardOrientation).DefaultValue =
        BillboardOrientation.AxialViewPlaneAligned;

      ps.Parameters.AddUniform<float>(ParticleParameterNames.BlendMode).DefaultValue = 0;

      return ps;
    }


    private static ParticleSystem CreateSlowBeams(IServiceLocator services)
    {
			var assetManager = services.GetInstance<AssetManager>();
			var graphicsService = services.GetInstance<IGraphicsService>();

			var ps = new ParticleSystem
      {
        Name = "SlowBeams",
        MaxNumberOfParticles = 10,
        ReferenceFrame = ParticleReferenceFrame.Local
      };

      ps.Parameters.AddUniform<float>(ParticleParameterNames.Lifetime).DefaultValue = 1.5f;

      ps.Effectors.Add(new StreamEmitter
      {
        DefaultEmissionRate = 6,
      });

      ps.Parameters.AddVarying<Vector3>(ParticleParameterNames.Position);
      ps.Effectors.Add(new StartPositionEffector
      {
        Parameter = ParticleParameterNames.Position,
        Distribution = new BoxDistribution
        {
          MinValue = new Vector3(-0.6f, 1.5f, -0.6f),
          MaxValue = new Vector3(0.6f, 2.0f, 0.6f)
        }
      });

      ps.Parameters.AddUniform<float>(ParticleParameterNames.SizeX).DefaultValue = 0.5f;

      ps.Parameters.AddVarying<Vector3>(ParticleParameterNames.Color);
      ps.Effectors.Add(new StartValueEffector<Vector3>
      {
        Parameter = ParticleParameterNames.Color,
        Distribution = new LineSegmentDistribution
        {
          Start = new Vector3(0, 0.5f, 0.45f),
          End = new Vector3(0, 0.5f, 0.55f)
        },
      });

      ps.Parameters.AddVarying<float>(ParticleParameterNames.Alpha);
      ps.Effectors.Add(new SingleLinearSegment3Effector
      {
        InputParameter = "GlobalAlpha",
        OutputParameter = ParticleParameterNames.Alpha,
        Time0 = 0,
        Value0 = 0,
        Time1 = 0.5f,
        Value1 = 0.2f,
        Time2 = 1,
        Value2 = 0,
      });

      ps.Parameters.AddUniform<Texture2D>(ParticleParameterNames.Texture).DefaultValue =
      assetManager.LoadTexture2D(graphicsService.GraphicsDevice, "Particles/BeamBlurred.dds");

      // Use cylindrical billboards.
      ps.Parameters.AddUniform<BillboardOrientation>(ParticleParameterNames.BillboardOrientation).DefaultValue =
        BillboardOrientation.AxialViewPlaneAligned;

      ps.Parameters.AddUniform<float>(ParticleParameterNames.BlendMode).DefaultValue = 0;

      return ps;
    }
  }
}