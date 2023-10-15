using DigitalRise.UI.Controls;
using Microsoft.Xna.Framework;

namespace DigitalRise.UI.Rendering
{
	partial class UIRenderer
	{
		private void RenderSelection(Grid grid, UIRenderContext context)
		{
			var bounds = GetContentBoundsRounded(grid);

			switch (grid.GridSelectionMode)
			{
				case GridSelectionMode.None:
					break;
				case GridSelectionMode.Row:
					{
						if (grid.HoverRowIndex != null && grid.HoverRowIndex != grid.SelectedRowIndex && grid.SelectionHoverBackground != null)
						{
							var rect = new RectangleF(bounds.Left,
								grid.GetCellLocationY(grid.HoverRowIndex.Value) + bounds.Top - grid.RowSpacing / 2,
								bounds.Width,
								grid.GetRowHeight(grid.HoverRowIndex.Value) + grid.RowSpacing);

							grid.SelectionHoverBackground.Draw(context, rect);
						}

						if (grid.SelectedRowIndex != null && grid.SelectionBackground != null)
						{
							var rect = new RectangleF(bounds.Left,
								grid.GetCellLocationY(grid.SelectedRowIndex.Value) + bounds.Top - grid.RowSpacing / 2,
								bounds.Width,
								grid.GetRowHeight(grid.SelectedRowIndex.Value) + grid.RowSpacing);

							grid.SelectionBackground.Draw(context, rect);
						}
					}
					break;
				case GridSelectionMode.Column:
					{
						if (grid.HoverColumnIndex != null && grid.HoverColumnIndex != grid.SelectedColumnIndex && grid.SelectionHoverBackground != null)
						{
							var rect = new RectangleF(grid.GetCellLocationX(grid.HoverColumnIndex.Value) + bounds.Left - grid.ColumnSpacing / 2,
								bounds.Top,
								grid.GetColumnWidth(grid.HoverColumnIndex.Value) + grid.ColumnSpacing,
								bounds.Height);

							grid.SelectionHoverBackground.Draw(context, rect);
						}

						if (grid.SelectedColumnIndex != null && grid.SelectionBackground != null)
						{
							var rect = new RectangleF(grid.GetCellLocationX(grid.SelectedColumnIndex.Value) + bounds.Left - grid.ColumnSpacing / 2,
								bounds.Top,
								grid.GetColumnWidth(grid.SelectedColumnIndex.Value) + grid.ColumnSpacing,
								bounds.Height);

							grid.SelectionBackground.Draw(context, rect);
						}
					}
					break;
				case GridSelectionMode.Cell:
					{
						if (grid.HoverRowIndex != null && grid.HoverColumnIndex != null &&
							(grid.HoverRowIndex != grid.SelectedRowIndex || grid.HoverColumnIndex != grid.SelectedColumnIndex) &&
							grid.SelectionHoverBackground != null)
						{
							var rect = new RectangleF(grid.GetCellLocationX(grid.HoverColumnIndex.Value) + bounds.Left - grid.ColumnSpacing / 2,
								grid.GetCellLocationY(grid.HoverRowIndex.Value) + bounds.Top - grid.RowSpacing / 2,
								grid.GetColumnWidth(grid.HoverColumnIndex.Value) + grid.ColumnSpacing,
								grid.GetRowHeight(grid.HoverRowIndex.Value) + grid.RowSpacing);

							grid.SelectionHoverBackground.Draw(context, rect);
						}

						if (grid.SelectedRowIndex != null && grid.SelectedColumnIndex != null && grid.SelectionBackground != null)
						{
							var rect = new RectangleF(grid.GetCellLocationX(grid.SelectedColumnIndex.Value) + bounds.Left - grid.ColumnSpacing / 2,
								grid.GetCellLocationY(grid.SelectedRowIndex.Value) + bounds.Top - grid.RowSpacing / 2,
								grid.GetColumnWidth(grid.SelectedColumnIndex.Value) + grid.ColumnSpacing,
								grid.GetRowHeight(grid.SelectedRowIndex.Value) + grid.RowSpacing);

							grid.SelectionBackground.Draw(context, rect);
						}
					}
					break;
			}
		}

		private void RenderGrid(UIControl control, UIRenderContext context)
		{
			RenderUIControl(control, context);

			var grid = (Grid)control;
			var bounds = GetContentBoundsRounded(grid);

			RenderSelection(grid, context);

			if (!grid.ShowGridLines)
			{
				return;
			}

			int i;
			for (i = 0; i < grid.GridLinesX.Count; ++i)
			{
				var x = grid.GridLinesX[i] + bounds.Left;
				context.FillRectangle(new RectangleF(x, bounds.Top, 1, bounds.Height), grid.GridLinesColor);
			}

			for (i = 0; i < grid.GridLinesY.Count; ++i)
			{
				var y = grid.GridLinesY[i] + bounds.Top;

				context.FillRectangle(new RectangleF(bounds.Left, y, bounds.Width, 1), grid.GridLinesColor);
			}
		}
	}
}
