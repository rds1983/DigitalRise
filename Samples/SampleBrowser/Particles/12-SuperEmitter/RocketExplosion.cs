using DigitalRise;
using DigitalRise.Particles;
using DigitalRise.Particles.Effectors;
using Microsoft.Xna.Framework;
using System;

namespace Samples.Particles
{
  // An explosion smoke effect that supports resource pooling.
  // The particle parameter "EmitterVelocity" can be set to modify initial movement of the 
  // explosion particles.
  public class RocketExplosion : ParticleSystem
  {
    private static readonly ResourcePool<ParticleSystem> Pool = new ResourcePool<ParticleSystem>(
      () => new RocketExplosion(SampleGame.Instance.Services),
      null,
      null);


    public static ParticleSystem Obtain()
    {
      return Pool.Obtain();
    }


    private RocketExplosion(IServiceProvider services)
    {
      Children = new ParticleSystemCollection
      {
        new RocketExplosionSmoke(services),
        new RocketExplosionCore(services),
      };

      // This EmitterVelocity parameter can be used by all child particle systems.
      Parameters.AddUniform<Vector3>(ParticleParameterNames.EmitterVelocity);

      // The ParticleSystemRecycler recycles this instance into the resource pool when all 
      // particles are dead.
      Effectors.Add(new ParticleSystemRecycler
      {
        ResourcePool = Pool,
      });

      ParticleSystemValidator.Validate(this);
      ParticleSystemValidator.Validate(Children[0]);
      ParticleSystemValidator.Validate(Children[1]);
    }
  }
}
