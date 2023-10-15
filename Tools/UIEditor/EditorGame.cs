using DigitalRise.Input;
using DigitalRise.UI;
using DigitalRise.UI.Controls;
using DigitalRise.UI.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel.DataAnnotations;

namespace UIEditor
{
	public class EditorGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;
		private InputManager _inputManager;
		private UIManager _uiManager;
		private UIRenderer _uiRenderer;
		private UIScreen _uiScreen;
		private Point? _lastViewPortSize;

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

			var stackPanel = new StackPanel
			{
				Orientation = Orientation.Vertical
			};

			for(var i = 0; i < 20; ++i)
			{
				stackPanel.Children.Add(new TextBlock { Text = "StackPanelItem" + i });
			}

			var scrollViewer = new ScrollViewer
			{
				Content = stackPanel,
				VerticalAlignment = VerticalAlignment.Stretch
			};

			var listBox = new ListBox();
			for(var i = 0; i < 20; ++i)
			{
				listBox.Items.Add("ListBoxItem" + i);
			}

			var splitPane = new SplitPane
			{
				Orientation = Orientation.Horizontal,
				First = grid,
				Second = listBox
			};

			_uiScreen.Children.Add(splitPane);
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			var viewPortSize = new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
			if (_lastViewPortSize == null)
			{
				_lastViewPortSize = viewPortSize;
			} else if (_lastViewPortSize.Value != viewPortSize)
			{
				_uiScreen.InvalidateMeasure();
				_lastViewPortSize = viewPortSize;
			}

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
