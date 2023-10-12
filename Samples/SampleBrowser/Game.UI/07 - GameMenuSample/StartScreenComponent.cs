using System;
using DigitalRise;
using DigitalRise.Input;
using DigitalRise.UI;
using DigitalRise.UI.Controls;
using DigitalRise.UI.Rendering;
using DigitalRise.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Threading.Tasks;

namespace Samples.Game.UI
{
  // Display the start screen.
  // This component loads content in the background.
  // When background loading is finished, the component listens to START buttons 
  // presses. This is necessary to find out which gamepad the user is using.
  // After that, this component is replaced by the MainMenuComponent.
  public class StartScreenComponent : GameComponent
  {
    private readonly IServiceProvider _services;
    private readonly IInputService _inputService;
    private readonly IGraphicsService _graphicsService;
    private readonly IUIService _uiService;

    private readonly SampleGraphicsScreen _graphicsScreen;
    private Task _loadStuffTask;
    private UIScreen _uiScreen;


    public StartScreenComponent(Microsoft.Xna.Framework.Game game, IServiceProvider services)
      : base(game)
    {
      _services = services;
      _inputService = services.GetInstance<IInputService>();
      _graphicsService = services.GetInstance<IGraphicsService>();
      _uiService = _services.GetInstance<IUIService>();

      // Add a GraphicsScreen to draw some text. In a real game we would draw
      // a spectacular start screen image instead.
      _graphicsScreen = new SampleGraphicsScreen(services);
      _graphicsScreen.ClearBackground = true;
      _graphicsService.Screens.Insert(0, _graphicsScreen);

      // Load stuff in a parallel task.
      _loadStuffTask = Task.Run(LoadStuff);
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        _graphicsService.Screens.Remove(_graphicsScreen);
      }

      base.Dispose(disposing);
    }


    // Loads stuff. This method is executed in parallel, therefore we can only do 
    // thread-safe things.
    public void LoadStuff()
    {
      // Create a UI renderer
      UIRenderer renderer = new UIRenderer(_graphicsService.GraphicsDevice);

      // Create a UIScreen and add it to the UI service. The screen is the root of the 
      // tree of UI controls. Each screen can have its own renderer. 
      _uiScreen = new UIScreen("SampleUI", renderer)
      {
        // Make the screen transparent.
        Background = new Color(0, 0, 0, 0),
      };

      // Simulate more loading time.
#if NETFX_CORE
      System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(2)).Wait();
#else
      System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
#endif
    }


    public override void Update(GameTime gameTime)
    {
      var debugRenderer = _graphicsScreen.DebugRenderer2D;
      debugRenderer.Clear();
      debugRenderer.DrawText("MY GAME", new Vector2(575, 100), Color.Black);
      debugRenderer.DrawText(
        "This is the start screen.\n\nThis sample shows how to create menus for Xbox games.\nIt must be controlled with a gamepad.",
        new Vector2(375, 200),
        Color.Black);

      if (!_loadStuffTask.IsCompleted)
      {
        debugRenderer.DrawText("Loading...", new Vector2(575, 400), Color.Black);
      }
      else
      {
        if (_uiScreen.UIService == null)
        {
          // This is the first frame where the LoadStuff() was completed.
          // Add the UIScreen to the UI service. 
          _uiService.Screens.Add(_uiScreen);
        }

        debugRenderer.DrawText("Press START to continue...", new Vector2(475, 400), Color.Black);

        // Check if the user presses START on any connected gamepad.
        for (var controller = PlayerIndex.One; controller <= PlayerIndex.Four; controller++)
        {
          if (_inputService.IsDown(Buttons.Start, controller))
          {
            // A or START was pressed. Assign this controller to the first "logical player".
            // If no logical player is assigned, the UI controls will not react to the gamepad.
            _inputService.SetLogicalPlayer(LogicalPlayerIndex.One, controller);

            // Remove this StartScreenComponent. And load the next components.
            Game.Components.Remove(this);
            Dispose();
            Game.Components.Add(new MainMenuComponent(Game, _services));
          }
        }
      }

      base.Update(gameTime);
    }
  }
}
