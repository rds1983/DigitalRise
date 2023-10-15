using DigitalRise.Input;
using DigitalRise.UI;
using DigitalRise.UI.Controls;
using DigitalRise.UI.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UIEditor
{
	public class EditorGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;
		private InputManager _inputManager;
		private UIManager _uiManager;
		private UIRenderer _uiRenderer;
		private UIScreen _uiScreen;

		public EditorGame(string[] args)
		{
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1600,
				PreferredBackBufferHeight = 900,
			};
			Window.AllowUserResizing = true;

			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			_inputManager = new InputManager(false);
			_uiManager = new UIManager(this, _inputManager);

			var theme = Theme.GetDefault(GraphicsDevice);
			_uiRenderer = new UIRenderer(theme);

			_uiScreen = new UIScreen("Default", _uiRenderer);
			_uiManager.Screens.Add(_uiScreen);

			var grid = new Grid()
			{
				ColumnSpacing = 8,
				RowSpacing = 8,
				SelectionHoverBackground = new SolidBrush(Color.AliceBlue),
				SelectionBackground = new SolidBrush(Color.Cyan)
			};

			grid.GridSelectionMode = GridSelectionMode.Cell;

			grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 1));
			grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 1));
			grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 1));
			grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 1));
			grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 1));

			grid.RowsProportions.Add(new Proportion(ProportionType.Part, 1));
			grid.RowsProportions.Add(new Proportion(ProportionType.Part, 1));
			grid.RowsProportions.Add(new Proportion(ProportionType.Part, 1));
			grid.RowsProportions.Add(new Proportion(ProportionType.Part, 1));

			grid.ShowGridLines = true;

			var textBlock = new TextBlock
			{
				Text = "Test",
				GridRow = 2,
				GridColumn = 3,
			};

			grid.Children.Add(textBlock);

			_uiScreen.Children.Add(grid);
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			_inputManager.Update(gameTime.ElapsedGameTime);
			_uiManager.Update(gameTime.ElapsedGameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			_uiScreen.Draw(gameTime);

			base.Draw(gameTime);
		}
	}
}
