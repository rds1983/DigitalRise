using System;
using DigitalRise.Diagnostics;
using DigitalRise.Geometry;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using AssetManagementBase;


namespace Samples.Particles
{
  [Sample(SampleCategory.Particles,
    @"This sample shows how to create an explosion effect.",
    @"An explosion effect is created by deriving from the ParticleSystem class.
The explosion is triggered periodically.",
    5)]
  public class ExplosionSample : ParticleSample
  {
    private static readonly TimeSpan ExplosionInterval = TimeSpan.FromSeconds(5);

    private readonly Explosion _explosion;
    private readonly ParticleSystemNode _particleSystemNode;
    private readonly SoundEffect _explosionSound;
    private TimeSpan _timeUntilExplosion = TimeSpan.Zero;


    public ExplosionSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      // Create an instance of the Explosion particle system.
      _explosion = new Explosion(Services);
      _explosion.Pose = new Pose(new Vector3F(0, 5, 0));
      ParticleSystemService.ParticleSystems.Add(_explosion);

      _particleSystemNode = new ParticleSystemNode(_explosion);
      GraphicsScreen.Scene.Children.Add(_particleSystemNode);

      _explosionSound = AssetManager.LoadSoundEffect("Particles/Explo1.wav");
    }


    public override void Update(GameTime gameTime)
    {
      // If enough time has passed, trigger the explosion sound and the explosion effect.
      _timeUntilExplosion -= gameTime.ElapsedGameTime;
      if (_timeUntilExplosion <= TimeSpan.Zero)
      {
        _explosion.Explode();
        _explosionSound.Play(0.2f, 0, 0);
        _timeUntilExplosion = ExplosionInterval;
      }

      // Synchronize particles <-> graphics.
      _particleSystemNode.Synchronize(GraphicsService);

      Profiler.AddValue("ParticleCount", ParticleHelper.CountNumberOfParticles(ParticleSystemService.ParticleSystems));
    }
  }
}
