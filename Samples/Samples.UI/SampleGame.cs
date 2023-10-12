using System;
using System.IO;
using System.Threading.Tasks;
using AssetManagementBase;
using DigitalRise.Animation;
using DigitalRise.Diagnostics;
using DigitalRise.Input;
using DigitalRise.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Samples.UI;
using DigitalRise.GameBase;

namespace Samples
{
  // The main game class, which creates and updates the game services (physics, 
  // graphics, animation, etc.). It also adds the MenuComponent which will 
  // load/switch the samples and end the application.
  public class SampleGame : Game
  {
    // The XNA GraphicsDeviceManager.
    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    // Services of the game:
    private InputManager _inputManager;                   // Input
    private UIManager _uiManager;                         // GUI
		private AnimationManager _animationManager;           // Animation
    private GameObjectManager _gameObjectManager;         // Game logic
    private HierarchicalProfiler _profiler;               // Profiler for game loop

    // Animation, physics simulation, and particle simulation may run in threads
    // parallel to input and graphics. For this we need some delegates and tasks.
    private Action _updateAnimation;
    private Task _updateAnimationTask;

    // The size of the current time step.
    private TimeSpan _deltaTime;
		private SpriteBatch _spriteBatch;
    private Sample _currentSample;


		// Enables/Disables the multi-threaded game loop. If enabled, certain game 
		// services will be updated in parallel.
		public bool EnableParallelGameLoop { get; set; }

    public static SampleGame Instance { get; private set; }


    public SampleGame()
    {
      Instance = this;

#if MONOGAME && DEBUG
      // Optional: Enable Direct3D debug layer to get additional debug information during development.
      //GraphicsAdapter.UseDebugDevice = true;
#endif

      _graphicsDeviceManager = new GraphicsDeviceManager(this)
      {
#if WINDOWS_PHONE || IOS
        PreferredBackBufferWidth = 800,
        PreferredBackBufferHeight = 480,
        IsFullScreen = true,   // Set fullscreen to hide the Windows Phone status bar.
        SupportedOrientations = DisplayOrientation.LandscapeLeft,
#elif XBOX
        // Following resolution is used in several AAA games on Xbox 360.
        PreferredBackBufferWidth = 1024,
        PreferredBackBufferHeight = 600,
#elif ANDROID
        // Using PreferredBackBufferWidth/Height does not work in Android. 
        // We would have to manually render into a low resolution render target and upscale the result.
        // This sample simply uses the default device resolution.
        IsFullScreen = true,
        SupportedOrientations = DisplayOrientation.LandscapeLeft,
#else
        PreferredBackBufferWidth =1600,
        PreferredBackBufferHeight = 900,
#endif
        PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8,
        PreferMultiSampling = false,
        SynchronizeWithVerticalRetrace = true,
        GraphicsProfile = GraphicsProfile.HiDef
      };

      IsMouseVisible = false;
      IsFixedTimeStep = false;
    }


    // Initializes services and adds game components.
    protected override void Initialize()
    {
#if WINDOWS || WINDOWS_UWP || XBOX
      if (GraphicsDevice.GraphicsProfile == GraphicsProfile.Reach)
      {
        throw new NotSupportedException(
          "The DigitalRise Samples and Content for Windows, Universal Windows Apps and Xbox 360 are " +
          "designed for the HiDef graphics profile. A graphics cards supporting DirectX 10.0 or better " +
          "is required.");
      }
#endif

      // The GraphicsDeviceManager needs to be registered in the service container.
      // (This is required by the XNA content managers.)
      Services.AddService(typeof(GraphicsDeviceManager), _graphicsDeviceManager);

      // ----- Initialize Services
      // Register the game class.
      Services.AddService(typeof(Game), this);
      Services.AddService(typeof(SampleGame), this);

#if XBOX
      // On Xbox, we use the XNA gamer services (e.g. for text input).
      Components.Add(new GamerServicesComponent(this));
#endif

      // Input
#if XBOX
      const bool useGamerServices = true;
#else
      const bool useGamerServices = false;
#endif
			Services.AddService(typeof(GraphicsDevice), GraphicsDevice);

			_inputManager = new InputManager(useGamerServices);
      Services.AddService(typeof(IInputService), _inputManager);

      // GUI
      _uiManager = new UIManager(this, _inputManager);
      Services.AddService(typeof(IUIService), _uiManager);

      // Animation
      _animationManager = new AnimationManager();
      Services.AddService(typeof(IAnimationService), _animationManager);

      // Game logic
      _gameObjectManager = new GameObjectManager();
      Services.AddService(typeof(IGameObjectService), _gameObjectManager);

      // Profiler
      _profiler = new HierarchicalProfiler("Main");
      Services.AddService(typeof(HierarchicalProfiler), _profiler);

      // Initialize delegates for running tasks in parallel.
      // (Creating delegates allocates memory, therefore we do this only once and
      // cache the delegates.)
      _updateAnimation = () => _animationManager.Update(_deltaTime);

			// SampleFramework
			// The SampleFramework automatically discovers all samples using reflection, provides 
			// controls for switching samples and starts the initial sample.
			var initialSample = typeof(ControlsSample);
			var assetManager = AssetManager.CreateFileAssetManager(Path.Combine(Utility.ExecutingAssemblyDirectory, "../../../../../Assets"));
      DefaultAssets.DefaultFont = assetManager.LoadFontSystem("Fonts/DroidSans.ttf").GetFont(16);

			Services.AddService(typeof(AssetManager), assetManager);

      IsMouseVisible = true;

      _spriteBatch = new SpriteBatch(GraphicsDevice);
      _currentSample = new ControlsSample();

      base.Initialize();
    }


    // Updates the different sub-systems (input, physics, game logic, ...).
    protected override void Update(GameTime gameTime)
    {
      _deltaTime = gameTime.ElapsedGameTime;

      // Tell the profiler that a new frame has begun.
      _profiler.NewFrame();
      _profiler.Start("Update");

      // Update input manager. The input manager gets the device states and performs other work.
      // (Note: XNA requires that the input service is run on the main thread!)
      _profiler.Start("InputManager.Update             ");
      _inputManager.Update(_deltaTime);
      _profiler.Stop();

      if (EnableParallelGameLoop)
      {
        // In a parallel game loop animation, physics and particles are started at
        // the end of the Update method. The services are now running in parallel. 
        // --> Wait for services to finish.
        _updateAnimationTask.Wait();

        // Now, nothing is running in parallel anymore and we can apply the animations.
        // (This means the animation values are written to the objects and properties
        // that are being animated.)
        _animationManager.ApplyAnimations();
      }
      else
      {
        // Update animations.
        // (The animation results are stored internally but not yet applied).
        _profiler.Start("AnimationManger.Update          ");
        _animationManager.Update(_deltaTime);
        _profiler.Stop();

        // Apply animations.
        // (The animation results are written to the objects and properties that 
        // are being animated. ApplyAnimations() must be called at a point where 
        // it is thread-safe to change the animated objects and properties.)
        _profiler.Start("AnimationManager.ApplyAnimations");
        _animationManager.ApplyAnimations();
        _profiler.Stop();
      }

      // Run any task completion callbacks that have been scheduled.
      _profiler.Start("Parallel.RunCallbacks           ");
      // TODO:
      // Parallel.RunCallbacks();
      _profiler.Stop();

      // Update XNA GameComponents.
      _profiler.Start("base.Update                     ");
      base.Update(gameTime);
      _profiler.Stop();

      // Update UI manager. The UI manager updates all registered UIScreens.
      _profiler.Start("UIManager.Update                ");
      _uiManager.Update(_deltaTime);
      _profiler.Stop();

      // Update DigitalRise GameObjects.
      _profiler.Start("GameObjectManager.Update        ");
      _gameObjectManager.Update(_deltaTime);
      _profiler.Stop();

      if (EnableParallelGameLoop)
      {
        // Start animation, physics and particle simulation. They will be executed 
        // parallel to the graphics rendering in Draw().
        _updateAnimationTask = Task.Run(_updateAnimation);
      }

      _profiler.Stop();
    }


    // Draws the game content.
    protected override void Draw(GameTime gameTime)
    {
      // Manually clear background. 
      // (This is not really necessary because the individual samples are 
      // responsible for rendering. However, if we skip this Clear() on Android 
      // then we can see trash in the back buffer when switching between samples.)
      GraphicsDevice.Clear(Color.Black);

      _profiler.Start("Draw");

      // Render all DrawableGameComponents registered in Components.
      _profiler.Start("base.Draw                       ");
      base.Draw(gameTime);
      _profiler.Stop();

      // Update the graphics (including graphics screens).
      // Important, if symbol EnableParallelGameLoop is true: Currently 
      // animation, physics and particles are running in parallel. Therefore, 
      // the GraphicsScreen.OnUpdate() methods must not influence the animation,
      // physics or particle state!
      _profiler.Start("Sample.Update          ");
      _currentSample.Update(gameTime);
      _profiler.Stop();

      // Render graphics screens to the back buffer.
      _profiler.Start("Sample.Render          ");
			_currentSample.Render(gameTime);
			_profiler.Stop();

      _profiler.Stop();
		}

		private void DebugDraw(Texture2D texture, int gridX, int gridY)
		{
			if (texture == null || texture.IsDisposed)
			{
				return;
			}

			_spriteBatch.Draw(texture, new Rectangle(gridX * 200, gridY * 200, 200, 200), Color.White);
		}

    public void LoadNextSample()
    {
    }
	}
}
