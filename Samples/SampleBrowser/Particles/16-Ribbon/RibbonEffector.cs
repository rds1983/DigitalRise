using System;
using DigitalRise.Particles;
using Microsoft.Xna.Framework;

namespace Samples.Particles
{
  // This effector creates particles when the particle system has moved a certain 
  // distance.
  public class RibbonEffector : ParticleEffector
  {
    // The index and the position of the last created particle.
    private Vector3 _lastPosition;


    protected override ParticleEffector CreateInstanceCore()
    {
      return new RibbonEffector();
    }


    protected override void OnBeginUpdate(TimeSpan deltaTime)
    {
      // If the particle system has moved a minimum distance, then we emit the 
      // next particle of the ribbon.
      Vector3 newPosition = ParticleSystem.Pose.Position;
      if ((newPosition - _lastPosition).LengthSquared() >= 0.3f)
      {
        ParticleSystem.AddParticles(1, this);
        _lastPosition = ParticleSystem.Pose.Position;
      }
    }
  }
}
