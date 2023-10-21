using System;
using DigitalRise;
using DigitalRise.Input;
using DigitalRise.Graphics;
using Microsoft.Xna.Framework;
using AssetManagementBase;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.Brushes;

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


		/// <summary>
		/// Gets the user interface that is rendered.
		/// </summary>
		/// <value>The user interface that is rendered.</value>
		public Desktop UIScreen { get; private set; }


		/// <summary>
		/// Gets or sets a value indicating whether graphics screens in the background are hidden.
		/// </summary>
		/// <value>
		/// <see langword="true"/> to hide the background; otherwise, <see langword="false"/>.
		/// The default value is <see langword="false"/>.
		/// </value>
		public bool HideBackground
		{
			get { return UIScreen.Background != null; }
			set { UIScreen.Background = value ? new SolidBrush(new Color(0, 0, 0, 192)) : null; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="GuiGraphicsScreen"/> class.
		/// </summary>
		/// <param name="services">The service locator.</param>
		public GuiGraphicsScreen(IServiceProvider services)
		  : base(services.GetService<IGraphicsService>())
		{
			Name = "GUI"; // Just for debugging.

			// Get required services.
			_inputService = services.GetService<IInputService>();
			var assetManager = services.GetService<AssetManager>();

			// Load a UI theme and create the UI renderer and the UI screen. See the
			// DigitalRise Game UI documentation and samples for more details.
			UIScreen = new Desktop();

			// When the background is hidden, the UIScreen should block all input.
/*			UIScreen.InputProcessed += (s, e) =>
									   {
										   if (HideBackground)
										   {
											   // Set all input devices to 'handled'.
											   _inputService.IsGamePadHandled(LogicalPlayerIndex.Any);
											   _inputService.IsKeyboardHandled = true;
											   _inputService.IsMouseOrTouchHandled = true;
										   }
									   };*/
		}


		public void Dispose()
		{
		}


		protected override void OnUpdate(TimeSpan deltaTime)
		{
		}


		protected override void OnRender(RenderContext context)
		{
			UIScreen.Render();
		}
	}
}
