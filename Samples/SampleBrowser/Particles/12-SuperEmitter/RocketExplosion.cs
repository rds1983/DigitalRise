using DigitalRise;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Particles;
using DigitalRise.Particles.Effectors;
using CommonServiceLocator;
using Microsoft.Xna.Framework.Content;


namespace Samples.Particles
{
  // An explosion smoke effect that supports resource pooling.
  // The particle parameter "EmitterVelocity" can be set to modify initial movement of the 
  // explosion particles.
  public class RocketExplosion : ParticleSystem
  {
    private static readonly ResourcePool<ParticleSystem> Pool = new ResourcePool<ParticleSystem>(
      () => new RocketExplosion(ServiceLocator.Current),
      null,
      null);


    public static ParticleSystem Obtain()
    {
      return Pool.Obtain();
    }


    private RocketExplosion(IServiceLocator services)
    {
      Children = new ParticleSystemCollection
      {
        new RocketExplosionSmoke(services),
        new RocketExplosionCore(services),
      };

      // This EmitterVelocity parameter can be used by all child particle systems.
      Parameters.AddUniform<Vector3F>(ParticleParameterNames.EmitterVelocity);

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
