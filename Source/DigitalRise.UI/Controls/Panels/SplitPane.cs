using DigitalRise.GameBase;
using System;
using System.ComponentModel;

namespace DigitalRise.UI.Controls
{
	public class SplitPane : ContentControl
	{
		private UIControl _first, _second;
		private Thumb _buttonSplitter;
		private bool _dirty = false;

		/// <summary> 
		/// The ID of the <see cref="Orientation"/> game object property.
		/// </summary>
		[Browsable(false)]
		public static readonly int OrientationPropertyId = CreateProperty(
			typeof(SplitPane), "Orientation", GamePropertyCategories.Layout, null, Orientation.Horizontal,
			UIPropertyOptions.AffectsMeasure);

		/// <summary>
		/// Gets or sets the orientation of the split pane. 
		/// This is a game object property.
		/// </summary>
		/// <value>The orientation of the split pane.</value>
		public Orientation Orientation
		{
			get { return GetValue<Orientation>(OrientationPropertyId); }
			set { SetValue(OrientationPropertyId, value); }
		}

		/// <summary> 
		/// The ID of the <see cref="Orientation"/> game object property.
		/// </summary>
		[Browsable(false)]
		public static readonly int HandleSizePropertyId = CreateProperty(
			typeof(SplitPane), "HandleSize", GamePropertyCategories.Layout, null, 8.0f,
			UIPropertyOptions.AffectsMeasure);

		/// <summary>
		/// Gets or sets the handle size. 
		/// This is a game object property.
		/// </summary>
		/// <value>The orientation of the split pane.</value>
		public float HandleSize
		{
			get { return GetValue<float>(HandleSizePropertyId); }
			set { SetValue(HandleSizePropertyId, value); }
		}

		/// <summary>
		/// Gets or sets the splitter position.
		/// </summary>
		/// <value>The splitter position.</value>
		public float SplitterPosition
		{
			get
			{
				Proportion leftProportion, rightProportion;
				GetProportions(out leftProportion, out rightProportion);

				var total = leftProportion.Value + rightProportion.Value;

				return leftProportion.Value / total;
			}
			set
			{
				Proportion leftProportion, rightProportion;
				GetProportions(out leftProportion, out rightProportion);

				var total = leftProportion.Value + rightProportion.Value;

				var fp = value * total;
				var fp2 = total - fp;
				leftProportion.Value = fp;
				rightProportion.Value = fp2;
			}
		}

		/// <summary>
		/// First Control
		/// </summary>
		[Browsable(false)]
		public UIControl First
		{
			get => _first;

			set
			{
				if (value == _first)
				{
					return;
				}

				_first = value;
				_dirty = true;
			}
		}

		/// <summary>
		/// Second Control
		/// </summary>
		[Browsable(false)]
		public UIControl Second
		{
			get => _second;

			set
			{
				if (value == _second)
				{
					return;
				}

				_second = value;
				_dirty = true;
			}
		}

		private Grid Grid => (Grid)Content;

		public event EventHandler ProportionsChanged;

		public SplitPane()
		{
			Width = float.PositiveInfinity;
			Height = float.PositiveInfinity;
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			Style = "SplitPane";

			Content = new Grid();
			_buttonSplitter = new Thumb
			{
				Style = "SplitPaneThumb",
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				Width = float.PositiveInfinity,
				Height = float.PositiveInfinity,
			};
		}

		public float GetProportion(int widgetIndex)
		{
			if (widgetIndex < 0 || widgetIndex >= 3)
			{
				return 0.0f;
			}

			var result = Orientation == Orientation.Horizontal
				? Grid.ColumnsProportions[widgetIndex * 2].Value
				: Grid.RowsProportions[widgetIndex * 2].Value;

			return result;
		}

		protected override void OnHandleInput(InputContext context)
		{
			base.OnHandleInput(context);

			if (_buttonSplitter.IsDragging)
			{
				var grid = Grid;

				Proportion firstProportion, secondProportion;
				float fp;

				if (Orientation == Orientation.Horizontal)
				{
					fp = 2 * ((float)context.MousePosition.X - ActualX) / ActualWidth;

					firstProportion = grid.ColumnsProportions[0];
					secondProportion = grid.ColumnsProportions[2];
				}
				else
				{
					fp = 2 * ((float)context.MousePosition.Y - ActualY) / ActualHeight;

					firstProportion = grid.RowsProportions[0];
					secondProportion = grid.RowsProportions[2];
				}

				if (fp >= 0 && fp <= 2.0f)
				{
					var fp2 = firstProportion.Value + secondProportion.Value - fp;
					firstProportion.Value = fp;
					secondProportion.Value = fp2;
				}
			}
		}

		private void GetProportions(out Proportion leftProportion, out Proportion rightProportion)
		{
			Update();

			var grid = Grid;
			leftProportion = Orientation == Orientation.Horizontal
				? grid.ColumnsProportions[0]
				: grid.RowsProportions[0];
			rightProportion = Orientation == Orientation.Horizontal
				? grid.ColumnsProportions[2]
				: grid.RowsProportions[2];
		}

		private void AddProportion(Proportion proportion)
		{
			var grid = Grid;
			if (Orientation == Orientation.Horizontal)
			{
				grid.ColumnsProportions.Add(proportion);
			}
			else
			{
				grid.RowsProportions.Add(proportion);
			}
		}

		public void Update()
		{
			if (!_dirty)
			{
				return;
			}

			// Clear
			var grid = Grid;
			grid.Children.Clear();

			grid.ColumnsProportions.Clear();
			grid.RowsProportions.Clear();

			// First control
			AddProportion(new Proportion(ProportionType.Part, 1.0f));
			if (_first != null)
			{
				_first.GridRow = 0;
				_first.GridColumn = 0;
				grid.Children.Add(_first);
			}

			// Splitter
			AddProportion(new Proportion(ProportionType.Pixels, HandleSize));
			if (Orientation == Orientation.Horizontal)
			{
				_buttonSplitter.GridRow = 0;
				_buttonSplitter.GridColumn = 1;
			}
			else
			{
				_buttonSplitter.GridRow = 1;
				_buttonSplitter.GridColumn = 0;
			}
			grid.Children.Add(_buttonSplitter);

			// Second control
			AddProportion(Proportion.Fill);
			if (_second != null)
			{
				if (Orientation == Orientation.Horizontal)
				{
					_second.GridRow = 0;
					_second.GridColumn = 2;
				}
				else
				{
					_second.GridRow = 2;
					_second.GridColumn = 0;
				}

				grid.Children.Add(_second);
			}

			_dirty = false;
		}

		protected override void OnUpdate(TimeSpan deltaTime)
		{
			base.OnUpdate(deltaTime);

			Update();
		}
	}
}