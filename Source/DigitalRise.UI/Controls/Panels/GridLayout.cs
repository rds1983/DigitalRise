using DigitalRise.Mathematics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DigitalRise.UI.Controls.Panels
{
	internal class GridLayout
	{
		private readonly List<float> _measureColWidths = new List<float>();
		private readonly List<float> _measureRowHeights = new List<float>();
		private readonly List<UIControl> _visibleUIControls = new List<UIControl>();
		private List<UIControl>[,] _UIControlsByGridPosition;
		private Vector2 _actualSize;

		public float ColumnSpacing { get; set; }

		public float RowSpacing { get; set; }

		public Proportion DefaultColumnProportion { get; set; } = Proportion.GridDefault;

		public Proportion DefaultRowProportion { get; set; } = Proportion.GridDefault;

		public ObservableCollection<Proportion> ColumnsProportions { get; } = new ObservableCollection<Proportion>();

		public ObservableCollection<Proportion> RowsProportions { get; } = new ObservableCollection<Proportion>();
		public List<float> GridLinesX { get; } = new List<float>();
		public List<float> GridLinesY { get; } = new List<float>();
		public List<float> ColWidths { get; } = new List<float>();
		public List<float> RowHeights { get; } = new List<float>();
		public List<float> CellLocationsX { get; } = new List<float>();
		public List<float> CellLocationsY { get; } = new List<float>();


		public GridLayout()
		{
		}

		public float GetColumnWidth(int index)
		{
			if (ColWidths == null || index < 0 || index >= ColWidths.Count)
			{
				return 0;
			}

			return ColWidths[index];
		}

		public float GetRowHeight(int index)
		{
			if (RowHeights == null || index < 0 || index >= RowHeights.Count)
			{
				return 0;
			}

			return RowHeights[index];
		}

		public float GetCellLocationX(int col)
		{
			if (col < 0 || col >= CellLocationsX.Count)
			{
				return 0;
			}

			return CellLocationsX[col];
		}

		public float GetCellLocationY(int row)
		{
			if (row < 0 || row >= CellLocationsY.Count)
			{
				return 0;
			}

			return CellLocationsY[row];
		}

		public RectangleF GetCellRectangle(int col, int row)
		{
			if (col < 0 || col >= CellLocationsX.Count ||
				row < 0 || row >= CellLocationsY.Count)
			{
				return RectangleF.Empty;
			}

			return new RectangleF(CellLocationsX[col], CellLocationsY[row],
				ColWidths[col], RowHeights[row]);
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

		private Vector2 LayoutProcessFixed(Vector2 availableSize, IEnumerable<UIControl> controls)
		{
			var rows = 0;
			var columns = 0;

			_visibleUIControls.Clear();
			foreach (var child in controls)
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

			availableSize.X -= (_measureColWidths.Count - 1) * ColumnSpacing;
			availableSize.Y -= (_measureRowHeights.Count - 1) * RowSpacing;

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
							measuredSize = new Vector2(UIControl.DesiredWidth, UIControl.DesiredHeight);
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
					result.X += ColumnSpacing;
				}
			}

			for (i = 0; i < _measureRowHeights.Count; ++i)
			{
				var h = _measureRowHeights[i];
				result.Y += h;

				if (i < _measureRowHeights.Count - 1)
				{
					result.Y += RowSpacing;
				}
			}

			return result;
		}

		public Vector2 Measure(Vector2 availableSize, IEnumerable<UIControl> controls)
		{
			return LayoutProcessFixed(availableSize, controls);
		}

		public void Arrange(Vector2 size, RectangleF bounds, 
			IEnumerable<UIControl> controls, Action<UIControl, Vector2, Vector2> arrangeCallback)
		{
			LayoutProcessFixed(size, controls);

			ColWidths.Clear();
			for (var i = 0; i < _measureColWidths.Count; ++i)
			{
				ColWidths.Add(_measureColWidths[i]);
			}

			RowHeights.Clear();
			for (var i = 0; i < _measureRowHeights.Count; ++i)
			{
				RowHeights.Add(_measureRowHeights[i]);
			}

			// Partition available space
			int row, col;

			// Dynamic widths
			// First run: calculate available width
			var availableWidth = (float)bounds.Width;
			availableWidth -= (ColWidths.Count - 1) * ColumnSpacing;

			var totalPart = 0.0f;
			for (col = 0; col < ColWidths.Count; ++col)
			{
				var colWidth = ColWidths[col];
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
				for (col = 0; col < ColWidths.Count; ++col)
				{
					var prop = GetColumnProportion(col);
					if (prop.Type == ProportionType.Part)
					{
						ColWidths[col] = (int)(prop.Value * availableWidth / totalPart);
						tookSpace += ColWidths[col];
					}
				}

				availableWidth -= tookSpace;
			}

			// Update part fill widths
			for (col = 0; col < ColWidths.Count; ++col)
			{
				var prop = GetColumnProportion(col);
				if (prop.Type == ProportionType.Fill)
				{
					ColWidths[col] = (int)availableWidth;
					break;
				}
			}

			// Same with row heights
			var availableHeight = (float)bounds.Height;
			availableHeight -= (RowHeights.Count - 1) * RowSpacing;

			totalPart = 0.0f;
			for (col = 0; col < RowHeights.Count; ++col)
			{
				var colHeight = RowHeights[col];
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
				for (row = 0; row < RowHeights.Count; ++row)
				{
					var prop = GetRowProportion(row);
					if (prop.Type != ProportionType.Part) continue;

					RowHeights[row] = (int)(prop.Value * availableHeight / totalPart);
					tookSpace += RowHeights[row];
				}

				availableHeight -= tookSpace;
			}

			// Update part fill heights
			for (row = 0; row < RowHeights.Count; ++row)
			{
				var prop = GetRowProportion(row);
				if (prop.Type == ProportionType.Fill)
				{
					RowHeights[row] = (int)availableHeight;
					break;
				}
			}

			_actualSize = Vector2.Zero;
			GridLinesX.Clear();
			CellLocationsX.Clear();

			var p = Vector2.Zero;

			for (var i = 0; i < ColWidths.Count; ++i)
			{
				CellLocationsX.Add(p.X);
				var w = ColWidths[i];
				p.X += w;

				if (i < ColWidths.Count - 1)
				{
					GridLinesX.Add(p.X + ColumnSpacing / 2);
				}

				p.X += ColumnSpacing;

				_actualSize.X += ColWidths[i];
			}

			GridLinesY.Clear();
			CellLocationsY.Clear();

			for (var i = 0; i < RowHeights.Count; ++i)
			{
				CellLocationsY.Add(p.Y);
				var h = RowHeights[i];
				p.Y += h;

				if (i < RowHeights.Count - 1)
				{
					GridLinesY.Add(p.Y + RowSpacing / 2);
				}

				p.Y += RowSpacing;

				_actualSize.Y += RowHeights[i];
			}

			foreach (var control in _visibleUIControls)
			{
				LayoutControl(control, bounds, arrangeCallback);
			}
		}

		private void LayoutControl(UIControl control, RectangleF bounds, Action<UIControl, Vector2, Vector2> arrangeCallback)
		{
			var gridPosition = GetActualGridPosition(control);
			var col = gridPosition.X;
			var row = gridPosition.Y;

			var cellSize = Vector2.Zero;

			for (var i = col; i < col + control.GridColumnSpan; ++i)
			{
				cellSize.X += ColWidths[i];

				if (i < col + control.GridColumnSpan - 1)
				{
					cellSize.X += ColumnSpacing;
				}
			}

			for (var i = row; i < row + control.GridRowSpan; ++i)
			{
				cellSize.Y += RowHeights[i];

				if (i < row + control.GridRowSpan - 1)
				{
					cellSize.Y += RowSpacing;
				}
			}

			var rect = new RectangleF(bounds.Left + CellLocationsX[col], bounds.Top + CellLocationsY[row], cellSize.X, cellSize.Y);

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

			arrangeCallback(control, new Vector2(rect.X, rect.Y), new Vector2(rect.Width, rect.Height));
		}
	}
}
