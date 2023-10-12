using System;
using DigitalRise;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Statistics;
using DigitalRise.Particles;
using DigitalRise.Particles.Effectors;
using Microsoft.Xna.Framework.Graphics;
using AssetManagementBase;
using DigitalRise.Graphics;
using Microsoft.Xna.Framework;

namespace Samples.Particles
{
	// A rocket smoke trail that supports resource pooling.
	// The particle parameter "EmitterVelocity" can be set to modify initial movement of the 
	// explosion particles.
  public class RocketTrail : ParticleSystem
  {
    private static readonly ResourcePool<ParticleSystem> Pool = new ResourcePool<ParticleSystem>(
      () => new RocketTrail(SampleGame.Instance.Services),
      null,
      null);


    public static ParticleSystem Obtain()
    {
      return Pool.Obtain();
    }


    private RocketTrail(IServiceProvider services)
    {
			var assetManager = services.GetInstance<AssetManager>();
			var graphicsService = services.GetInstance<IGraphicsService>();

			MaxNumberOfParticles = 200;

      Parameters.AddVarying<float>(ParticleParameterNames.Lifetime);
      Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.Lifetime,
        Distribution = new UniformDistributionF(1, 2),
      });

      Parameters.AddUniform<float>(ParticleParameterNames.EmissionRate);
      Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.EmissionRate,
        DefaultValue = 60,
      });
      Effectors.Add(new StreamEmitter
      {
        EmissionRateParameter = ParticleParameterNames.EmissionRate,
      });

      Parameters.AddVarying<Vector3>(ParticleParameterNames.Position);
      Effectors.Add(new StartPositionEffector
      {
        Parameter = ParticleParameterNames.Position,
      });

      Parameters.AddVarying<Vector3>(ParticleParameterNames.Direction);
      Effectors.Add(new StartDirectionEffector
      {
        Parameter = ParticleParameterNames.Direction,
        Distribution = new DirectionDistribution { Deviation = ConstantsF.Pi, Direction = Vector3.UnitY },
      });

      Parameters.AddVarying<float>(ParticleParameterNames.LinearSpeed);
      Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.LinearSpeed,
        Distribution = new UniformDistributionF(0.1f, 0.3f),
      });

      // The StartVelocityBiasEffector adds a velocity to new particles. We use this to add
      // the rocket velocity (stored in the parameter "EmitterVelocity") to the start velocities
      // of the particles.
      Parameters.AddUniform<Vector3>(ParticleParameterNames.EmitterVelocity);
      Effectors.Add(new StartVelocityBiasEffector { Strength = 0.1f });

      Effectors.Add(new LinearVelocityEffector());

      Parameters.AddVarying<float>(ParticleParameterNames.Angle);
      Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.Angle,
        Distribution = new UniformDistributionF(-ConstantsF.Pi, ConstantsF.Pi),
      });

      Parameters.AddVarying<float>(ParticleParameterNames.AngularSpeed);
      Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.AngularSpeed,
        Distribution = new UniformDistributionF(-1, 1),
      });
      Effectors.Add(new AngularVelocityEffector());

      Parameters.AddVarying<float>("StartSize");
      Effectors.Add(new StartValueEffector<float>
      {
        Parameter = "StartSize",
        Distribution = new UniformDistributionF(0.2f, 0.5f),
      });

      Parameters.AddVarying<float>("EndSize");
      Effectors.Add(new StartValueEffector<float>
      {
        Parameter = "EndSize",
        Distribution = new UniformDistributionF(0.5f, 1f),
      });

      Parameters.AddVarying<float>(ParticleParameterNames.Size);
      Effectors.Add(new SingleLerpEffector
      {
        ValueParameter = ParticleParameterNames.Size,
        StartParameter = "StartSize",
        EndParameter = "EndSize",
      });

      Parameters.AddVarying<Vector3>(ParticleParameterNames.Color);
      Effectors.Add(new StartValueEffector<Vector3>
      {
        Parameter = ParticleParameterNames.Color,
        Distribution = new LineSegmentDistribution { Start = new Vector3(0.5f, 0.4f, 0.25f), End = new Vector3(0.7f, 0.6f, 0.5f) },
      });

      Parameters.AddVarying<float>(ParticleParameterNames.Alpha);
      Effectors.Add(new FuncEffector<float, float>
      {
        InputParameter = ParticleParameterNames.NormalizedAge,
        OutputParameter = ParticleParameterNames.Alpha,
        Func = age => 6.7f * age * (1 - age) * (1 - age),
      });


      Parameters.AddUniform<Texture2D>(ParticleParameterNames.Texture).DefaultValue =
      assetManager.LoadTexture2D(graphicsService.GraphicsDevice, "Particles/Smoke.png");

      // Draw behind explosion.
      Parameters.AddUniform<int>(ParticleParameterNames.DrawOrder).DefaultValue = -100;

      // The ParticleSystemRecycler recycles this instance into the specified resource 
      // pool when all particles are dead.
      Effectors.Add(new ParticleSystemRecycler
      {
        ResourcePool = Pool,

        // Set a minimum life-time to avoid that the particle system is recycled too early.
        // (The rocket trail might need a few frames before particles are created.)
        MinRuntime = TimeSpan.FromSeconds(0.05f),
      });

      ParticleSystemValidator.Validate(this);
    }
  }
}