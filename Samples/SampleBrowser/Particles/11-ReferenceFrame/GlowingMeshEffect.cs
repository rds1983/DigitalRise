using AssetManagementBase;
using DigitalRise.Geometry.Meshes;
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
  public static class GlowingMeshEffect
  {
    public static ParticleSystem Create(ITriangleMesh mesh, IServiceProvider services)
    {
			var assetManager = services.GetInstance<AssetManager>();
			var graphicsService = services.GetInstance<IGraphicsService>();

			var ps = new ParticleSystem
      {
        Name = "GlowingMeshEffect",
        MaxNumberOfParticles = 100
      };

      ps.Parameters.AddUniform<float>(ParticleParameterNames.Lifetime).DefaultValue = 1.0f;

      ps.Effectors.Add(new StreamEmitter
      {
        DefaultEmissionRate = 100,
      });

      // The particles start on random positions on the surface of the given triangle mesh.
      ps.Parameters.AddVarying<Vector3>(ParticleParameterNames.Position);
      ps.Effectors.Add(new StartOnMeshEffector
      {
        Parameter = ParticleParameterNames.Position,
        Mesh = mesh
      });

      // Just to demonstrate a new custom effector:
      // The size follows a user-defined curve using the FuncEffector.
      ps.Parameters.AddVarying<float>(ParticleParameterNames.Size);
      ps.Effectors.Add(new FuncEffector<float, float>
      {
        InputParameter = ParticleParameterNames.NormalizedAge,
        OutputParameter = ParticleParameterNames.Size,
        Func = age => 6.7f * age * (1 - age) * (1 - age) * 0.4f,
      });

      ps.Parameters.AddVarying<Vector3>(ParticleParameterNames.Color);
      ps.Effectors.Add(new StartValueEffector<Vector3>
      {
        Parameter = ParticleParameterNames.Color,
        Distribution = new BoxDistribution
        {
          MinValue = new Vector3(0.5f, 0.5f, 0.5f),
          MaxValue = new Vector3(1, 1, 1)
        }
      });

      ps.Parameters.AddVarying<float>(ParticleParameterNames.Alpha);
      ps.Effectors.Add(new FuncEffector<float, float>
      {
        InputParameter = ParticleParameterNames.NormalizedAge,
        OutputParameter = ParticleParameterNames.Alpha,
        Func = age => 6.7f * age * (1 - age) * (1 - age),
      });

      ps.Parameters.AddUniform<Texture2D>(ParticleParameterNames.Texture).DefaultValue =
      assetManager.LoadTexture2D(graphicsService.GraphicsDevice, "Particles/Star.dds");

      ps.Parameters.AddUniform<float>(ParticleParameterNames.BlendMode).DefaultValue = 0;


      ps.Parameters.AddUniform<BillboardOrientation>(ParticleParameterNames.BillboardOrientation).DefaultValue =
        BillboardOrientation.ScreenAligned;

      ParticleSystemValidator.Validate(ps);

      return ps;
    }
  }
}
