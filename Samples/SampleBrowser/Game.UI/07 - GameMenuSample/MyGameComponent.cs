using DigitalRise;
using DigitalRise.GameBase;
using DigitalRise.Input;
using DigitalRise.UI;
using DigitalRise.UI.Controls;
using DigitalRise.Graphics;
using DigitalRise.Graphics.Rendering;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Physics;
using DigitalRise.Physics.ForceEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.ComponentModel.Design;

namespace Samples.Game.UI
{
  // This game component implements the actual game.
  // The user can press BACK to pause the game and display the game menu window.
  public class MyGameComponent : GameComponent
  {
    private readonly ServiceContainer _services;
    private readonly IInputService _inputService;
    private readonly Simulation _simulation;
    private readonly IGraphicsService _graphicsService;
    private readonly IGameObjectService _gameObjectService;
    private readonly IUIService _uiService;
    private readonly DeferredGraphicsScreen _deferredGraphicsScreen;
    private readonly DelegateGraphicsScreen _delegateGraphicsScreen;

    private readonly UIScreen _uiScreen;
    private readonly GameMenuWindow _gameMenuWindow;
    private readonly CameraObject _cameraGameObject;


    public MyGameComponent(Microsoft.Xna.Framework.Game game, IServiceProvider services)
      : base(game)
    {
      // Get the services that this component needs regularly.
      _services = new ServiceContainer(services);
      _inputService = services.GetService<IInputService>();
      _simulation = services.GetService<Simulation>();
      _graphicsService = services.GetService<IGraphicsService>();
      _gameObjectService = services.GetService<IGameObjectService>();
      _uiService = services.GetService<IUIService>();

      // Add gravity and damping to the physics simulation.
      _simulation.ForceEffects.Add(new Gravity());
      _simulation.ForceEffects.Add(new Damping());

      // Create the DeferredGraphicsScreen and some 3D objects.
      _deferredGraphicsScreen = new DeferredGraphicsScreen(services);
      _deferredGraphicsScreen.DrawReticle = true;
      _graphicsService.Screens.Insert(0, _deferredGraphicsScreen);

      // The GameObjects below expect try to retrieve DebugRenderer and Scene via
      // service container.
      _services.AddService(typeof(DebugRenderer), _deferredGraphicsScreen.DebugRenderer);
			_services.AddService(typeof(IScene), _deferredGraphicsScreen.Scene);

      _cameraGameObject = new CameraObject(services);
      _gameObjectService.Objects.Add(_cameraGameObject);
      _deferredGraphicsScreen.ActiveCameraNode = _cameraGameObject.CameraNode;
      _gameObjectService.Objects.Add(new GrabObject(services));
      _gameObjectService.Objects.Add(new StaticSkyObject(services));
      _gameObjectService.Objects.Add(new GroundObject(services));
      for (int i = 0; i < 10; i++)
        _gameObjectService.Objects.Add(new DynamicObject(services, 1));

      // Get the "SampleUI" screen that was created by the StartScreenComponent.
      _uiScreen = _uiService.Screens["SampleUI"];

      // Add a second GraphicsScreen. This time it is a DelegateGraphicsScreen that
      // draws the UI over DeferredGraphicsScreen.
      _delegateGraphicsScreen = new DelegateGraphicsScreen(_graphicsService)
      {
        RenderCallback = context => _uiScreen.Draw(context.DeltaTime)
      };
      _graphicsService.Screens.Insert(1, _delegateGraphicsScreen);

      // Create the game menu window. But do not display it yet.
      _gameMenuWindow = new GameMenuWindow
      {
        // If the menu is opened and closed a lot, it is more efficient when _gameMenuWindow.Close()
        // makes the window invisible but does not remove it from the screen.
        HideOnClose = true,
      };
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        _gameObjectService.Objects.Clear();

        _simulation.RigidBodies.Clear();
        _simulation.ForceEffects.Clear();

        _graphicsService.Screens.Remove(_deferredGraphicsScreen);
        _graphicsService.Screens.Remove(_delegateGraphicsScreen);
      }
      base.Dispose(disposing);
    }


    public override void Update(GameTime gameTime)
    {
      // This sample is written for the gamepad and the mouse cursor is hidden.
      // --> Ignore mouse input, otherwise it could conflict with the UI.
      _inputService.IsMouseOrTouchHandled = true;

      // The game is paused when the menu is visible or the controller is disconnected.
      if (_gameMenuWindow.ActualIsVisible || !_inputService.GetGamePadState(LogicalPlayerIndex.One).IsConnected)
      {
        // Pause game...
        _cameraGameObject.IsEnabled = false;
        // TODO: Pause other game objects, physics simulation, particle system, etc.
      }
      else
      {
        // Update game...
        _cameraGameObject.IsEnabled = true;

        if (!_inputService.IsGamePadHandled(LogicalPlayerIndex.One))
        {
          // Nobody has handled the gamepad input yet, so we can handle it.
          // Display the menu when BACK or START button is pressed..
          if (_inputService.IsPressed(Buttons.Back, false, LogicalPlayerIndex.One)
              || _inputService.IsPressed(Buttons.Start, false, LogicalPlayerIndex.One))
          {
            _inputService.SetGamePadHandled(LogicalPlayerIndex.One, true);
            _gameMenuWindow.Show(_uiScreen);
          }
        }
      }

      // The game menu window sets its DialogResult to false if the user wants to quit.
      if (_gameMenuWindow.DialogResult == false)
      {
        // Close the game menu window. 
        _gameMenuWindow.HideOnClose = false;
        _gameMenuWindow.Close();

        // Remove this game component and load the main menu.
        Game.Components.Remove(this);
        Dispose();
        Game.Components.Add(new MainMenuComponent(Game, _services));
      }

      _deferredGraphicsScreen.DebugRenderer.Clear();
      _deferredGraphicsScreen.DebugRenderer.DrawText(
        "\n\n\n\nThis is the game.\nPress <Back> or <Start> to pause the game." +
        "\nPress <Tab> or <ChatPadGreen> to display debug console.",
        new Vector2(300, 100),
        Color.Black);

      base.Update(gameTime);
    }
  }
}