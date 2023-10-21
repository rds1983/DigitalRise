using System.Linq;
using DigitalRise;
using DigitalRise.Animation;
using DigitalRise.GameBase;
using DigitalRise.Input;
using DigitalRise.Graphics;
using DigitalRise.Particles;
using DigitalRise.Physics;
using Microsoft.Xna.Framework;
using AssetManagementBase;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel.Design;

namespace Samples
{
	// Samples in this solution are derived from the XNA class GameComponent. The
	// abstract class Sample can be used as the base class of samples. It provides
	// access to the game services (input, graphics, physics, etc.).
	// In addition, it creates a new ServiceContainer which can be used in samples.
	//
	// ----- Clean up:
	// When this class is disposed, it performs common clean-up operations to restore 
	// a clean state for the next sample instance. Things that are automatically removed:
	// - GameObjects
	// - GraphicsScreens
	// - Physics objects
	// - ParticleSystems
	// Other objects have to be cleaned up manually (e.g. UIScreens, Animations, etc.)!
	public abstract class Sample : GameComponent
	{
		// Services which can be used in derived classes.
		protected readonly ServiceContainer Services;
		protected readonly AssetManager AssetManager;
		protected readonly IInputService InputService;
		protected readonly IAnimationService AnimationService;
		protected readonly Simulation Simulation;
		protected readonly IParticleSystemService ParticleSystemService;
		protected readonly IGraphicsService GraphicsService;
		protected readonly IGameObjectService GameObjectService;
		protected readonly SampleFramework SampleFramework;

		private readonly GraphicsScreen[] _originalGraphicsScreens;

		public GraphicsDevice GraphicsDevice => GraphicsService.GraphicsDevice;


		protected Sample(Microsoft.Xna.Framework.Game game)
		  : base(game)
		{
			// Get services from the global service container.
			var services = game.Services;
			SampleFramework = services.GetService<SampleFramework>();
			AssetManager = services.GetService<AssetManager>();
			InputService = services.GetService<IInputService>();
			AnimationService = services.GetService<IAnimationService>();
			Simulation = services.GetService<Simulation>();
			ParticleSystemService = services.GetService<IParticleSystemService>();
			GraphicsService = services.GetService<IGraphicsService>();
			GameObjectService = services.GetService<IGameObjectService>();

			// Create a local service container which can be modified in samples:
			// The local service container is a child container, i.e. it inherits the
			// services of the global service container. Samples can add new services
			// or override existing entries without affecting the global services container
			// or other samples.
			Services = new ServiceContainer(services);

			// Store a copy of the original graphics screens.
			_originalGraphicsScreens = GraphicsService.Screens.ToArray();

			// Mouse is visible by default.
			SampleFramework.IsMouseVisible = true;
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				// ----- Clean up
				// Remove all game objects.
				GameObjectService.Objects.Clear();

				// Dispose all graphics screens which were added by the sample. We
				// must not remove graphics screens from the menu or help component.
				foreach (var screen in GraphicsService.Screens.ToArray())
				{
					if (!_originalGraphicsScreens.Contains(screen))
					{
						GraphicsService.Screens.Remove(screen);
						screen.SafeDispose();
					}
				}

			  // Remove all rigid bodies, constraints, force-effects.
			  // Restore original simulation settings.
			  ((SampleGame)Game).ResetPhysicsSimulation();

				// Remove all particle systems.
				ParticleSystemService.ParticleSystems.Clear();

				// Dispose the local service container.
				Services.Dispose();
			}

			base.Dispose(disposing);
		}
	}
}
