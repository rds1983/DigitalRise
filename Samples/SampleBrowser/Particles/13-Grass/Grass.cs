using AssetManagementBase;
using DigitalRise;
using DigitalRise.Graphics;
using DigitalRise.Mathematics.Statistics;
using DigitalRise.Particles;
using DigitalRise.Particles.Effectors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Samples.Particles
{
  public static class Grass
  {
    public static ParticleSystem Create(IServiceProvider services)
    {
			var assetManager = services.GetService<AssetManager>();
			var graphicsService = services.GetService<IGraphicsService>();

			var ps = new ParticleSystem
      {
        Name = "Grass",
        MaxNumberOfParticles = 400,
      };

      // The grass particles do not die.
      ps.Parameters.AddUniform<float>(ParticleParameterNames.Lifetime).DefaultValue = float.PositiveInfinity;

      // We create all particles instantly. Up to 400 particles. Then the emission stops.
      ps.Effectors.Add(new StreamEmitter
      {
        DefaultEmissionRate = 400 * 60,
        EmissionLimit = 400,
      });

      ps.Parameters.AddVarying<Vector3>(ParticleParameterNames.Position);
      ps.Effectors.Add(new StartPositionEffector
      {
        Parameter = ParticleParameterNames.Position,
        Distribution = new BoxDistribution { MinValue = new Vector3(-10, 0.4f, -10), MaxValue = new Vector3(10, 0.4f, 10) }
      });

      ps.Parameters.AddVarying<float>(ParticleParameterNames.SizeX);
      ps.Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.SizeX,
        Distribution = new UniformDistributionF(0.6f, 1),
      });
      ps.Parameters.AddVarying<float>(ParticleParameterNames.SizeY);
      ps.Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.SizeY,
        Distribution = new UniformDistributionF(0.6f, 1),
      });

      ps.Parameters.AddVarying<Vector3>(ParticleParameterNames.Color);
      ps.Effectors.Add(new StartValueEffector<Vector3>
      {
        Parameter = ParticleParameterNames.Color,
        Distribution = new LineSegmentDistribution { Start = new Vector3(0.82f, 0.92f, 1) * 0.9f, End = new Vector3(1, 1, 1) }
      });

      ps.Parameters.AddUniform<Texture2D>(ParticleParameterNames.Texture).DefaultValue =
      assetManager.LoadTexture2D(graphicsService.GraphicsDevice, "Particles/Grass.png");

      ps.Parameters.AddUniform<BillboardOrientation>(ParticleParameterNames.BillboardOrientation).DefaultValue =
        BillboardOrientation.AxialViewPlaneAligned;

      ps.Parameters.AddUniform<bool>(ParticleParameterNames.IsDepthSorted).DefaultValue = true;

      ParticleSystemValidator.Validate(ps);

      return ps;
    }
  }
}