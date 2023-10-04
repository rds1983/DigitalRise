using DigitalRise.Diagnostics;
using DigitalRise.Input;
using DigitalRise.Geometry;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace Samples.Particles
{
  [Sample(SampleCategory.Particles,
    "This sample creates a flame jet.",
    "",
    6)]
  [Controls(@"Sample
  Press the <Left Mouse> or <Right Trigger> to emit fire.")]
  public class FlameJetSample : ParticleSample
  {
    private readonly ParticleSystem _flameJet;
    private readonly ParticleSystemNode _particleSystemNode;


    public FlameJetSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      _flameJet = FlameJet.Create(Services);
      _flameJet.Pose = new Pose(new Vector3(0, 2, 0), Matrix33F.CreateRotationY(ConstantsF.PiOver2));
      ParticleSystemService.ParticleSystems.Add(_flameJet);

      _particleSystemNode = new ParticleSystemNode(_flameJet);
      GraphicsScreen.Scene.Children.Add(_particleSystemNode);
    }


    public override void Update(GameTime gameTime)
    {
      if (InputService.IsDown(MouseButtons.Left) || InputService.IsDown(Buttons.RightTrigger, LogicalPlayerIndex.One))
        _flameJet.AddParticles(6);

      // Synchronize particles <-> graphics.
      _particleSystemNode.Synchronize(GraphicsService);

      Profiler.AddValue("ParticleCount", ParticleHelper.CountNumberOfParticles(ParticleSystemService.ParticleSystems));
    }
  }
}
