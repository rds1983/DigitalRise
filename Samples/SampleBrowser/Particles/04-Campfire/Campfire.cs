﻿using AssetManagementBase;
using CommonServiceLocator;
using DigitalRise.Graphics;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Mathematics.Statistics;
using DigitalRise.Particles;
using DigitalRise.Particles.Effectors;


namespace Samples.Particles
{
  // Creates a fire effect for a campfire.
  public class Campfire
  {
    public static ParticleSystem CreateCampfire(IServiceLocator services)
    {
			var assetManager = services.GetInstance<AssetManager>();
			var graphicsService = services.GetInstance<IGraphicsService>();

			ParticleSystem ps = new ParticleSystem
      {
        Name = "Campfire",
        MaxNumberOfParticles = 50
      };

      // Each particle lives for a random time span.
      ps.Parameters.AddVarying<float>(ParticleParameterNames.Lifetime);
      ps.Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.Lifetime,
        Distribution = new UniformDistributionF(0.8f, 1.2f),
      });

      // Add an effector that emits particles at a constant rate.
      ps.Effectors.Add(new StreamEmitter
      {
        DefaultEmissionRate = 30,
      });

      // Particle positions start on a circular area (in the xy-plane).
      ps.Parameters.AddVarying<Vector3F>(ParticleParameterNames.Position);
      ps.Effectors.Add(new StartPositionEffector
      {
        Parameter = ParticleParameterNames.Position,
        Distribution = new CircleDistribution { OuterRadius = 0.4f, InnerRadius = 0 }
      });

      // Particles move in forward direction with a random speed.
      ps.Parameters.AddUniform<Vector3F>(ParticleParameterNames.Direction).DefaultValue = Vector3F.Forward;
      ps.Parameters.AddVarying<float>(ParticleParameterNames.LinearSpeed);
      ps.Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.LinearSpeed,
        Distribution = new UniformDistributionF(0, 1),
      });

      // The LinearVelocityEffector uses the Direction and LinearSpeed to update the Position
      // of particles.
      ps.Effectors.Add(new LinearVelocityEffector());

      // Lets apply a damping (= exponential decay) to the LinearSpeed using the SingleDampingEffector.
      ps.Parameters.AddUniform<float>(ParticleParameterNames.Damping).DefaultValue = 1.0f;
      ps.Effectors.Add(new SingleDampingEffector
      {
        // Following parameters are equal to the default values. No need to set them.
        //ValueParameter = ParticleParameterNames.LinearSpeed,
        //DampingParameter = ParticleParameterNames.Damping,
      });

      // To create a wind effect, we apply an acceleration to all particles.
      ps.Parameters.AddUniform<Vector3F>("Wind").DefaultValue = new Vector3F(-1, 3, -0.5f);
      ps.Effectors.Add(new LinearAccelerationEffector { AccelerationParameter = "Wind" });

      // Each particle starts with a random rotation angle and a random angular speed.
      ps.Parameters.AddVarying<float>(ParticleParameterNames.Angle);
      ps.Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.Angle,
        Distribution = new UniformDistributionF(-ConstantsF.Pi, ConstantsF.Pi),
      });
      ps.Parameters.AddVarying<float>(ParticleParameterNames.AngularSpeed);
      ps.Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.AngularSpeed,
        Distribution = new UniformDistributionF(-2f, 2f),
      });

      // The AngularVelocityEffector uses the AngularSpeed to update the particle Angle.
      ps.Effectors.Add(new AngularVelocityEffector());

      // All particle have the same size.
      ps.Parameters.AddUniform<float>(ParticleParameterNames.Size).DefaultValue = 0.8f;

      // Particle alpha fades in to 1 and then back out to 0.
      ps.Parameters.AddVarying<float>(ParticleParameterNames.Alpha);
      ps.Effectors.Add(new SingleFadeEffector
      {
        ValueParameter = ParticleParameterNames.Alpha,
        FadeInStart = 0.0f,
        FadeInEnd = 0.2f,
        FadeOutStart = 0.8f,
        FadeOutEnd = 1.0f,
      });

      // DigitalRise Graphics supports "texture atlases": The class PackedTexture 
      // describes a single texture or tile set packed into a texture atlas. The 
      // fire texture in this example consists of 4 tiles.
      ps.Parameters.AddUniform<PackedTexture>(ParticleParameterNames.Texture).DefaultValue =
        new PackedTexture(
          "FireParticles",
          assetManager.LoadTexture2D(graphicsService.GraphicsDevice, "Campfire/FireParticles.dds"),
          Vector2F.Zero, Vector2F.One,
          4, 1);

      // The particle parameter "AnimationTime" determines which tile is used,
      // where 0 = first tile, 1 = last tile.
      // --> Chooses a random tile for each particle when it is created.
      ps.Parameters.AddVarying<float>(ParticleParameterNames.AnimationTime);
      ps.Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.AnimationTime,
        Distribution = new UniformDistributionF(0, 1),
      });

      // Fire needs additive blending.
      ps.Parameters.AddUniform<float>(ParticleParameterNames.BlendMode).DefaultValue = 0;

      ParticleSystemValidator.Validate(ps);

      return ps;
    }
  }
}