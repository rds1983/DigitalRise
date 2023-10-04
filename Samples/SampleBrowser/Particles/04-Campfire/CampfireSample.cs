using DigitalRise.Diagnostics;
using DigitalRise.Geometry;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Particles;
using Microsoft.Xna.Framework;


namespace Samples.Particles
{
  [Sample(SampleCategory.Particles,
    @"This sample shows how to create a campfire effect that uses different particle textures
from texture atlases.",
    @"",
    4)]
  public class CampfireSample : ParticleSample
  {
    private readonly ParticleSystemNode _particleSystemNode;


    public CampfireSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      var particleSystem = Campfire.CreateCampfire(Services);

      // Add a smoke effect as a child to the campfire.
      particleSystem.Children = new ParticleSystemCollection();
      particleSystem.Children.Add(CampfireSmoke.CreateCampfireSmoke(Services));

      // Position the campfire (including its child) in the level.
      // (The fire effect lies in the xy plane and shoots into the forward direction (= -z axis).
      // Therefore, we rotate the particle system to shoot upwards.)
      particleSystem.Pose = new Pose(new Vector3(0, 0.2f, 0), Matrix33F.CreateRotationX(ConstantsF.PiOver2));

      ParticleSystemService.ParticleSystems.Add(particleSystem);

      _particleSystemNode = new ParticleSystemNode(particleSystem);
      GraphicsScreen.Scene.Children.Add(_particleSystemNode);
    }


    public override void Update(GameTime gameTime)
    {
      // Synchronize particles <-> graphics.
      _particleSystemNode.Synchronize(GraphicsService);

      Profiler.AddValue("ParticleCount", ParticleHelper.CountNumberOfParticles(ParticleSystemService.ParticleSystems));
    }
  }
}
