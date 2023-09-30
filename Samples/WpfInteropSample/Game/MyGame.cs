using System;
using System.IO;
using System.Windows.Media;
using AssetManagementBase;
using CommonServiceLocator;
using DigitalRune;
using DigitalRune.Game.Timing;
using DigitalRune.Graphics;
using DigitalRune.ServiceLocation;
using DigitalRune.Storages;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace WpfInteropSample2
{
  // This class implements a simple game loop. The 3D graphics are rendered into the 
  // MyGamePresentationTargets.
  // The MyGame class renders only graphics, but other services (e.g. physics, animations,
  // game objects, etc.) could be added as well.
  internal class MyGame
  {
    private readonly ServiceContainer _serviceContainer;
    private readonly HighPrecisionClock _clock;
    private readonly IGameTimer _timer;
    private readonly GraphicsManager _graphicsManager;


    public MyGame()
    {
      // ----- Service Container
      // The MyGame uses a ServiceContainer, which is a simple service locator 
      // and Inversion of Control (IoC) container. (The ServiceContainer can be 
      // replaced by any other container that implements System.IServiceProvider.)
      _serviceContainer = new ServiceContainer();
      ServiceLocator.SetLocatorProvider(() => _serviceContainer);

			var assetManager = AssetManager.CreateFileAssetManager(Path.Combine(Utility.ExecutingAssemblyDirectory, "../../../../../Assets"));
			_serviceContainer.Register(typeof(AssetManager), null, assetManager);

      // ----- Graphics
      // Create Direct3D 11 device.
      var presentationParameters = new PresentationParameters
      {
        BackBufferWidth = 1,
        BackBufferHeight = 1,
        // Do not associate graphics device with any window.
        DeviceWindowHandle = IntPtr.Zero,
      };
      var graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, presentationParameters);

      // An IGraphicsDeviceService is required by the MonoGame/XNA content manager.
      _serviceContainer.Register(typeof(IGraphicsDeviceService), null, new DummyGraphicsDeviceManager(graphicsDevice));

      // Create and register the graphics manager.
      _graphicsManager = new GraphicsManager(graphicsDevice);
      _serviceContainer.Register(typeof(IGraphicsService), null, _graphicsManager);

      // ----- Timing
      // We can use the CompositionTarget.Rendering event to trigger our game loop.
      // The CompositionTarget.Rendering event is raised once per frame by WPF.

      // To measure the time that has passed, we use a HighPrecisionClock.
      _clock = new HighPrecisionClock();
      _clock.Start();
      CompositionTarget.Rendering += (s, e) => _clock.Update();

      // The FixedStepTimer reads the clock and triggers the game loop at 60 Hz.
      //_timer = new FixedStepTimer(_clock)
      //{
      //  StepSize = new TimeSpan(166667), // ~60 Hz
      //  AccumulateTimeSteps = false,
      //};
      // The VariableStepTimer reads the clock and triggers the game loop as often
      // as possible.
      _timer = new VariableStepTimer(_clock);
      _timer.TimeChanged += (s, e) => GameLoop(e.DeltaTime);
      _timer.Start();
    }


    private void GameLoop(TimeSpan deltaTime)
    {
      // Update graphics service and graphics screens.
      _graphicsManager.Update(deltaTime);

      // Render the current graphics screens into the presentation targets.
      foreach (var presentationTarget in _graphicsManager.PresentationTargets)
        _graphicsManager.Render(presentationTarget);
    }
  }
}