using DigitalRise.Mathematics.Statistics;
using DigitalRise.Particles;
using DigitalRise.Particles.Effectors;
using Microsoft.Xna.Framework;

namespace Samples.Particles
{
  // A particle system that simulates rocket paths. A custom RocketEffector is used
  // to create child particle systems for explosions and rocket trails.
  public class Rockets : ParticleSystem
  {
    public Rockets()
    {
      Name = "Rockets";
      MaxNumberOfParticles = 10;

      // The RocketEffector will add child particle systems.
      Children = new ParticleSystemCollection();

      Parameters.AddUniform<float>(ParticleParameterNames.Lifetime).DefaultValue = 2;

      Effectors.Add(new StreamEmitter
      {
        DefaultEmissionRate = 2,
      });

      Parameters.AddVarying<Vector3>(ParticleParameterNames.Position);
      Effectors.Add(new StartPositionEffector
      {
        Parameter = ParticleParameterNames.Position,
        Distribution = new BoxDistribution { MinValue = new Vector3(-5, 0, -5), MaxValue = new Vector3(5, 0, 0) },
      });

      Parameters.AddVarying<Vector3>(ParticleParameterNames.Direction);
      Effectors.Add(new StartDirectionEffector
      {
        Parameter = ParticleParameterNames.Direction,
        Distribution = new DirectionDistribution { Deviation = 0.5f, Direction = Vector3.UnitY },
      });

      Parameters.AddVarying<float>(ParticleParameterNames.LinearSpeed);
      Effectors.Add(new StartValueEffector<float>
      {
        Parameter = ParticleParameterNames.LinearSpeed,
        Distribution = new UniformDistributionF(2, 6),
      });

      Effectors.Add(new LinearVelocityEffector());

      Parameters.AddUniform<Vector3>(ParticleParameterNames.LinearAcceleration).DefaultValue = new Vector3(0, -2f, 0);
      Effectors.Add(new LinearAccelerationEffector());

      Parameters.AddUniform<float>(ParticleParameterNames.Alpha).DefaultValue = 0;

      // The RocketEffector creates and controls nested particle systems for the rocket trails
      // and explosions.
      Effectors.Add(new RocketEffector());

      ParticleSystemValidator.Validate(this);
    }
  }
}
