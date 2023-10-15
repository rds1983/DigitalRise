using DigitalRise.Mathematics;
using DigitalRise.UI.Rendering;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DigitalRise.UI.Controls
{
	public enum GridSelectionMode
	{
		None,
		Row,
		Column,
		Cell
	}

	public class Grid : Panel
	{
		private float _columnSpacing;
		private float _rowSpacing;
		private readonly ObservableCollection<Proportion> _columnsProportions = new ObservableCollection<Proportion>();
		private readonly ObservableCollection<Proportion> _rowsProportions = new ObservableCollection<Proportion>();
		private readonly List<float> _cellLocationsX = new List<float>();
		private readonly List<float> _cellLocationsY = new List<float>();
		private readonly List<float> _gridLinesX = new List<float>();
		private readonly List<float> _gridLinesY = new List<float>();
		private Vector2 _actualSize;

		private readonly List<float> _measureColWidths = new List<float>();
		private readonly List<float> _measureRowHeights = new List<float>();
		private readonly List<UIControl> _visibleUIControls = new List<UIControl>();
		private readonly List<float> _colWidths = new List<float>();
		private readonly List<float> _rowHeights = new List<float>();
		private int? _hoverRowIndex = null;
		private int? _hoverColumnIndex = null;
		private int? _selectedRowIndex = null;
		private int? _selectedColumnIndex = null;
		private List<UIControl>[,] _UIControlsByGridPosition;

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool ShowGridLines { get; set; }

		[Category("Behavior")]
		[DefaultValue("White")]
		public Color GridLinesColor { get; set; }

		[Category("Grid")]
		[DefaultValue(0)]
		public float ColumnSpacing
		{
			get { return _columnSpacing; }
			set
			{
				if (value == _columnSpacing)
				{
					return;
				}

				_columnSpacing = value;
				InvalidateMeasure();
			}
		}

		[Category("Grid")]
		[DefaultValue(0)]
		public float RowSpacing
		{
			get { return _rowSpacing; }
			set
			{
				if (value == _rowSpacing)
				{
					return;
				}

				_rowSpacing = value;
				InvalidateMeasure();
			}
		}

		[Browsable(false)]
		public Proportion DefaultColumnProportion { get; set; } = Proportion.GridDefault;

		[Browsable(false)]
		public Proportion DefaultRowProportion { get; set; } = Proportion.GridDefault;

		[Browsable(false)]
		public ObservableCollection<Proportion> ColumnsProportions
		{
			get { return _columnsProportions; }
		}

		[Browsable(false)]
		public ObservableCollection<Proportion> RowsProportions
		{
			get { return _rowsProportions; }
		}

		[Category("Appearance")]
		public IBrush SelectionBackground { get; set; }

		[Category("Appearance")]
		public IBrush SelectionHoverBackground { get; set; }

		[Category("Behavior")]
		[DefaultValue(GridSelectionMode.None)]
		public GridSelectionMode GridSelectionMode { get; set; }

		[Category("Behavior")]
		[DefaultValue(true)]
		public bool HoverIndexCanBeNull { get; set; }

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool CanSelectNothing { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public List<float> GridLinesX
		{
			get
			{
				return _gridLinesX;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public List<float> GridLinesY
		{
			get
			{
				return _gridLinesY;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public int? HoverRowIndex
		{
			get
			{
				return _hoverRowIndex;
			}

			set
			{
				if (value == _hoverRowIndex)
				{
					return;
				}

				_hoverRowIndex = value;

				var ev = HoverIndexChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public int? HoverColumnIndex
		{
			get
			{
				return _hoverColumnIndex;
			}

			set
			{
				if (value == _hoverColumnIndex)
				{
					return;
				}

				_hoverColumnIndex = value;

				var ev = HoverIndexChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public int? SelectedRowIndex
		{
			get { return _selectedRowIndex; }

			set
			{
				if (value == _selectedRowIndex)
				{
					return;
				}

				_selectedRowIndex = value;

				var ev = SelectedIndexChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public int? SelectedColumnIndex
		{
			get { return _selectedColumnIndex; }

			set
			{
				if (value == _selectedColumnIndex)
				{
					return;
				}

				_selectedColumnIndex = value;

				var ev = SelectedIndexChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler SelectedIndexChanged = null;
		public event EventHandler HoverIndexChanged = null;

		public Grid()
		{
			_columnsProportions.CollectionChanged += OnProportionsChanged;
			_rowsProportions.CollectionChanged += OnProportionsChanged;

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			ShowGridLines = false;
			GridLinesColor = Color.White;
			HoverIndexCanBeNull = true;
			CanSelectNothing = false;
			Style = "Grid";
		}

		public float GetColumnWidth(int index)
		{
			if (_colWidths == null || index < 0 || index >= _colWidths.Count)
			{
				return 0;
			}

			return _colWidths[index];
		}

		public float GetRowHeight(int index)
		{
			if (_rowHeights == null || index < 0 || index >= _rowHeights.Count)
			{
				return 0;
			}

			return _rowHeights[index];
		}

		public float GetCellLocationX(int col)
		{
			if (col < 0 || col >= _cellLocationsX.Count)
			{
				return 0;
			}

			return _cellLocationsX[col];
		}

		public float GetCellLocationY(int row)
		{
			if (row < 0 || row >= _cellLocationsY.Count)
			{
				return 0;
			}

			return _cellLocationsY[row];
		}

		public RectangleF GetCellRectangle(int col, int row)
		{
			if (col < 0 || col >= _cellLocationsX.Count ||
				row < 0 || row >= _cellLocationsY.Count)
			{
				return RectangleF.Empty;
			}

			return new RectangleF(_cellLocationsX[col], _cellLocationsY[row],
				_colWidths[col], _rowHeights[row]);
		}

		private void OnProportionsChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (var i in args.NewItems)
				{
					((Proportion)i).Changed += OnProportionsChanged;
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (var i in args.OldItems)
				{
					((Proportion)i).Changed -= OnProportionsChanged;
				}
			}

			HoverRowIndex = null;
			SelectedRowIndex = null;

			InvalidateMeasure();
		}

		private void OnProportionsChanged(object sender, EventArgs args)
		{
			InvalidateMeasure();
		}

		public Proportion GetColumnProportion(int col)
		{
			if (col < 0 || col >= ColumnsProportions.Count)
			{
				return DefaultColumnProportion;
			}

			return ColumnsProportions[col];
		}

		public Proportion GetRowProportion(int row)
		{
			if (row < 0 || row >= RowsProportions.Count)
			{
				return DefaultRowProportion;
			}

			return RowsProportions[row];
		}

		private Point GetActualGridPosition(UIControl child)
		{
			return new Point(child.GridColumn, child.GridRow);
		}

		private void LayoutProcessFixedPart()
		{
			int i = 0;
			var size = 0.0f;

			// First run - find maximum size
			for (i = 0; i < _measureColWidths.Count; ++i)
			{
				var prop = GetColumnProportion(i);
				if (prop.Type != ProportionType.Part)
				{
					continue;
				}

				if (_measureColWidths[i] > size)
				{
					size = _measureColWidths[i];
				}
			}

			// Second run - update
			for (i = 0; i < _measureColWidths.Count; ++i)
			{
				var prop = GetColumnProportion(i);
				if (prop.Type != ProportionType.Part)
				{
					continue;
				}

				_measureColWidths[i] = (int)(size * prop.Value);
			}

			size = 0;

			// First run - find maximum size
			for (i = 0; i < _measureRowHeights.Count; ++i)
			{
				var prop = GetRowProportion(i);
				if (prop.Type != ProportionType.Part)
				{
					continue;
				}

				if (_measureRowHeights[i] > size)
				{
					size = _measureRowHeights[i];
				}
			}

			// Second run - update
			for (i = 0; i < _measureRowHeights.Count; ++i)
			{
				var prop = GetRowProportion(i);
				if (prop.Type != ProportionType.Part)
				{
					continue;
				}

				_measureRowHeights[i] = (int)(size * prop.Value);
			}
		}

		private Vector2 LayoutProcessFixed(Vector2 availableSize)
		{
			var rows = 0;
			var columns = 0;

			_visibleUIControls.Clear();
			foreach (var child in VisualChildren)
			{
				if (!child.IsVisible)
				{
					continue;
				}

				_visibleUIControls.Add(child);

				var gridPosition = GetActualGridPosition(child);
				var c = gridPosition.X + Math.Max(child.GridColumnSpan, 1);
				if (c > columns)
				{
					columns = c;
				}

				var r = gridPosition.Y + Math.Max(child.GridRowSpan, 1);
				if (r > rows)
				{
					rows = r;
				}
			}

			if (ColumnsProportions.Count > columns)
			{
				columns = ColumnsProportions.Count;
			}

			if (RowsProportions.Count > rows)
			{
				rows = RowsProportions.Count;
			}

			_measureColWidths.Clear();
			int i;
			for (i = 0; i < columns; ++i)
			{
				_measureColWidths.Add(0);
			}

			_measureRowHeights.Clear();
			for (i = 0; i < rows; ++i)
			{
				_measureRowHeights.Add(0);
			}

			// Put all visible UIControl into 2d array
			if (_UIControlsByGridPosition == null ||
				_UIControlsByGridPosition.GetLength(0) < rows ||
				_UIControlsByGridPosition.GetLength(1) < columns)
			{
				_UIControlsByGridPosition = new List<UIControl>[rows, columns];
			}

			for (var row = 0; row < rows; ++row)
			{
				for (var col = 0; col < columns; ++col)
				{
					if (_UIControlsByGridPosition[row, col] == null)
					{
						_UIControlsByGridPosition[row, col] = new List<UIControl>();
					}

					_UIControlsByGridPosition[row, col].Clear();
				}
			}

			foreach (var UIControl in _visibleUIControls)
			{
				_UIControlsByGridPosition[UIControl.GridRow, UIControl.GridColumn].Add(UIControl);
			}

			availableSize.X -= (_measureColWidths.Count - 1) * _columnSpacing;
			availableSize.Y -= (_measureRowHeights.Count - 1) * _rowSpacing;

			for (var row = 0; row < rows; ++row)
			{
				for (var col = 0; col < columns; ++col)
				{
					var rowProportion = GetRowProportion(row);
					var colProportion = GetColumnProportion(col);

					if (colProportion.Type == ProportionType.Pixels)
					{
						_measureColWidths[col] = (int)colProportion.Value;
					}

					if (rowProportion.Type == ProportionType.Pixels)
					{
						_measureRowHeights[row] = (int)rowProportion.Value;
					}

					var UIControls = _UIControlsByGridPosition[row, col];
					foreach (var UIControl in UIControls)
					{
						var gridPosition = GetActualGridPosition(UIControl);

						var measuredSize = Vector2.Zero;
						if (rowProportion.Type != ProportionType.Pixels ||
							colProportion.Type != ProportionType.Pixels)
						{
							UIControl.Measure(availableSize);
							measuredSize = new Vector2 (UIControl.DesiredWidth, UIControl.DesiredHeight);
						}

						if (UIControl.GridColumnSpan != 1)
						{
							measuredSize.X = 0;
						}

						if (UIControl.GridRowSpan != 1)
						{
							measuredSize.Y = 0;
						}

						if (measuredSize.X > _measureColWidths[col] && colProportion.Type != ProportionType.Pixels)
						{
							_measureColWidths[col] = measuredSize.X;
						}

						if (measuredSize.Y > _measureRowHeights[row] && rowProportion.Type != ProportionType.Pixels)
						{
							_measureRowHeights[row] = measuredSize.Y;
						}
					}
				}
			}

			// #181: All Part proportions must have maximum size
			LayoutProcessFixedPart();

			var result = Vector2.Zero;
			for (i = 0; i < _measureColWidths.Count; ++i)
			{
				var w = _measureColWidths[i];

				result.X += w;
				if (i < _measureColWidths.Count - 1)
				{
					result.X += _columnSpacing;
				}
			}

			for (i = 0; i < _measureRowHeights.Count; ++i)
			{
				var h = _measureRowHeights[i];
				result.Y += h;

				if (i < _measureRowHeights.Count - 1)
				{
					result.Y += _rowSpacing;
				}
			}

			return result;
		}

		protected override void OnArrange(Vector2 position, Vector2 size)
		{
			base.OnArrange(position, size);

			var bounds = ActualBounds;
			LayoutProcessFixed(size);

			_colWidths.Clear();
			for (var i = 0; i < _measureColWidths.Count; ++i)
			{
				_colWidths.Add(_measureColWidths[i]);
			}

			_rowHeights.Clear();
			for (var i = 0; i < _measureRowHeights.Count; ++i)
			{
				_rowHeights.Add(_measureRowHeights[i]);
			}

			// Partition available space
			int row, col;

			// Dynamic widths
			// First run: calculate available width
			var availableWidth = (float)bounds.Width;
			availableWidth -= (_colWidths.Count - 1) * _columnSpacing;

			var totalPart = 0.0f;
			for (col = 0; col < _colWidths.Count; ++col)
			{
				var colWidth = _colWidths[col];
				var prop = GetColumnProportion(col);
				if (prop.Type == ProportionType.Auto || prop.Type == ProportionType.Pixels)
				{
					// Fixed width
					availableWidth -= colWidth;
				}
				else
				{
					totalPart += prop.Value;
				}
			}

			if (!Numeric.IsZero(totalPart))
			{
				// Second run update dynamic widths
				var tookSpace = 0.0f;
				for (col = 0; col < _colWidths.Count; ++col)
				{
					var prop = GetColumnProportion(col);
					if (prop.Type == ProportionType.Part)
					{
						_colWidths[col] = (int)(prop.Value * availableWidth / totalPart);
						tookSpace += _colWidths[col];
					}
				}

				availableWidth -= tookSpace;
			}

			// Update part fill widths
			for (col = 0; col < _colWidths.Count; ++col)
			{
				var prop = GetColumnProportion(col);
				if (prop.Type == ProportionType.Fill)
				{
					_colWidths[col] = (int)availableWidth;
					break;
				}
			}

			// Same with row heights
			var availableHeight = (float)bounds.Height;
			availableHeight -= (_rowHeights.Count - 1) * _rowSpacing;

			totalPart = 0.0f;
			for (col = 0; col < _rowHeights.Count; ++col)
			{
				var colHeight = _rowHeights[col];
				var prop = GetRowProportion(col);
				if (prop.Type == ProportionType.Auto || prop.Type == ProportionType.Pixels)
				{
					// Fixed height
					availableHeight -= colHeight;
				}
				else
				{
					totalPart += prop.Value;
				}
			}

			if (!Numeric.IsZero(totalPart))
			{
				var tookSpace = 0.0f;
				for (row = 0; row < _rowHeights.Count; ++row)
				{
					var prop = GetRowProportion(row);
					if (prop.Type != ProportionType.Part) continue;

					_rowHeights[row] = (int)(prop.Value * availableHeight / totalPart);
					tookSpace += _rowHeights[row];
				}

				availableHeight -= tookSpace;
			}

			// Update part fill heights
			for (row = 0; row < _rowHeights.Count; ++row)
			{
				var prop = GetRowProportion(row);
				if (prop.Type == ProportionType.Fill)
				{
					_rowHeights[row] = (int)availableHeight;
					break;
				}
			}

			_actualSize = Vector2.Zero;
			_gridLinesX.Clear();
			_cellLocationsX.Clear();

			var p = Vector2.Zero;

			for (var i = 0; i < _colWidths.Count; ++i)
			{
				_cellLocationsX.Add(p.X);
				var w = _colWidths[i];
				p.X += w;

				if (i < _colWidths.Count - 1)
				{
					_gridLinesX.Add(p.X + _columnSpacing / 2);
				}

				p.X += _columnSpacing;

				_actualSize.X += _colWidths[i];
			}

			_gridLinesY.Clear();
			_cellLocationsY.Clear();

			for (var i = 0; i < _rowHeights.Count; ++i)
			{
				_cellLocationsY.Add(p.Y);
				var h = _rowHeights[i];
				p.Y += h;

				if (i < _rowHeights.Count - 1)
				{
					_gridLinesY.Add(p.Y + _rowSpacing / 2);
				}

				p.Y += _rowSpacing;

				_actualSize.Y += _rowHeights[i];
			}

			foreach (var control in _visibleUIControls)
			{
				LayoutControl(control);
			}
		}

		private void LayoutControl(UIControl control)
		{
			var gridPosition = GetActualGridPosition(control);
			var col = gridPosition.X;
			var row = gridPosition.Y;

			var cellSize = Vector2.Zero;

			for (var i = col; i < col + control.GridColumnSpan; ++i)
			{
				cellSize.X += _colWidths[i];

				if (i < col + control.GridColumnSpan - 1)
				{
					cellSize.X += _columnSpacing;
				}
			}

			for (var i = row; i < row + control.GridRowSpan; ++i)
			{
				cellSize.Y += _rowHeights[i];

				if (i < row + control.GridRowSpan - 1)
				{
					cellSize.Y += _rowSpacing;
				}
			}

			var bounds = ActualBounds;
			var rect = new RectangleF(bounds.Left + _cellLocationsX[col], bounds.Top + _cellLocationsY[row], cellSize.X, cellSize.Y);

			if (rect.Right > bounds.Right)
			{
				rect.Width = bounds.Right - rect.X;
			}

			if (rect.Width < 0)
			{
				rect.Width = 0;
			}

			if (rect.Bottom > bounds.Bottom)
			{
				rect.Height = bounds.Bottom - rect.Y;
			}

			if (rect.Height < 0)
			{
				rect.Height = 0;
			}

			Arrange(control, new Vector2(rect.X, rect.Y), new Vector2(rect.Width, rect.Height));
		}

		protected override Vector2 OnMeasure(Vector2 availableSize)
		{
			return LayoutProcessFixed(availableSize);
		}

		private void UpdateHoverPosition(Vector2? position)
		{
			if (GridSelectionMode == GridSelectionMode.None)
			{
				return;
			}

			if (position == null)
			{
				if (HoverIndexCanBeNull)
				{
					HoverRowIndex = null;
					HoverColumnIndex = null;
				}
				return;
			}

			var pos = position.Value;
			var bounds = ActualBounds;
			if (GridSelectionMode == GridSelectionMode.Column || GridSelectionMode == GridSelectionMode.Cell)
			{
				var x = pos.X;
				for (var i = 0; i < _cellLocationsX.Count; ++i)
				{
					var cx = _cellLocationsX[i] + bounds.Left - ColumnSpacing / 2;
					if (x >= cx && x < cx + _colWidths[i] + ColumnSpacing / 2)
					{
						HoverColumnIndex = i;
						break;
					}
				}
			}

			if (GridSelectionMode == GridSelectionMode.Row || GridSelectionMode == GridSelectionMode.Cell)
			{
				var y = pos.Y;
				for (var i = 0; i < _cellLocationsY.Count; ++i)
				{
					var cy = _cellLocationsY[i] + bounds.Top - RowSpacing / 2;
					if (y >= cy && y < cy + _rowHeights[i] + RowSpacing / 2)
					{
						HoverRowIndex = i;
						break;
					}
				}
			}
		}

		protected override void OnHandleInput(InputContext context)
		{
			base.OnHandleInput(context);

			if (!IsMouseOver)
			{
				UpdateHoverPosition(null);
			} else
			{
				UpdateHoverPosition(context.MousePosition);
			}
		}

/*		public override bool OnTouchDown()
		{
			base.OnTouchDown();

			if (Desktop == null)
			{
				return false;
			}

			UpdateHoverPosition(Desktop.TouchPosition);

			if (HoverRowIndex != null)
			{
				if (SelectedRowIndex != HoverRowIndex)
				{
					SelectedRowIndex = HoverRowIndex;
				} else if (CanSelectNothing)
				{
					SelectedRowIndex = null;
				}
			}

			if (HoverColumnIndex != null)
			{
				if (SelectedColumnIndex != HoverColumnIndex)
				{
					SelectedColumnIndex = HoverColumnIndex;
				} else if (CanSelectNothing)
				{
					SelectedColumnIndex = null;
				}
			}

			return (SelectedRowIndex != null && SelectedColumnIndex != null);
		}*/
	}
}