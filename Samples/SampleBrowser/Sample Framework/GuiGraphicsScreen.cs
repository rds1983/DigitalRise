using System;
using DigitalRise;
using DigitalRise.Input;
using DigitalRise.UI;
using DigitalRise.UI.Controls;
using DigitalRise.UI.Rendering;
using DigitalRise.Graphics;
using CommonServiceLocator;
using Microsoft.Xna.Framework;
using AssetManagementBase;

namespace Samples
{
  /// <summary>
  /// Renders the user interface.
  /// </summary>
  /// <remarks>
  /// This class implements a <see cref="GraphicsScreen"/> (DigitalRise Graphics), which renders a
  /// user interface using a <see cref="UIScreen"/> (DigitalRise Game UI).
  /// </remarks>
  public sealed class GuiGraphicsScreen : GraphicsScreen, IDisposable
  {
    private readonly IInputService _inputService;
    private readonly IUIService _uiService;


    /// <summary>
    /// Gets the user interface that is rendered.
    /// </summary>
    /// <value>The user interface that is rendered.</value>
    public UIScreen UIScreen { get; private set; }


    /// <summary>
    /// Gets or sets a value indicating whether graphics screens in the background are hidden.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to hide the background; otherwise, <see langword="false"/>.
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool HideBackground
    {
      get { return UIScreen.Background.A != 0; }
      set { UIScreen.Background = value ? new Color(0, 0, 0, 192) : Color.Transparent; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="GuiGraphicsScreen"/> class.
    /// </summary>
    /// <param name="services">The service locator.</param>
    public GuiGraphicsScreen(IServiceLocator services)
      : base(services.GetInstance<IGraphicsService>())
    {
      Name = "GUI"; // Just for debugging.

      // Get required services.
      _inputService = services.GetInstance<IInputService>();
      _uiService = services.GetInstance<IUIService>();
      var assetManager = services.GetInstance<AssetManager>();

      // Load a UI theme and create the UI renderer and the UI screen. See the
      // DigitalRise Game UI documentation and samples for more details.
      var renderer = new UIRenderer(GraphicsService.GraphicsDevice);
      UIScreen = new UIScreen("Default", renderer)
      {
        Background = Color.Transparent,
        ZIndex = int.MaxValue,
      };
      _uiService.Screens.Add(UIScreen);

      // When the background is hidden, the UIScreen should block all input.
      UIScreen.InputProcessed += (s, e) =>
                                 {
                                   if (HideBackground)
                                   {
                                     // Set all input devices to 'handled'.
                                     _inputService.IsGamePadHandled(LogicalPlayerIndex.Any);
                                     _inputService.IsKeyboardHandled = true;
                                     _inputService.IsMouseOrTouchHandled = true;
                                   }
                                 };
    }


    public void Dispose()
    {
      _uiService.Screens.Remove(UIScreen);
      UIScreen.Renderer.SafeDispose();
    }


    protected override void OnUpdate(TimeSpan deltaTime)
    {
    }


    protected override void OnRender(RenderContext context)
    {
      UIScreen.Draw(context.DeltaTime);
    }
  }
}
