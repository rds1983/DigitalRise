using DigitalRise.GameBase;
using DigitalRise.Input;
using DigitalRise.UI.Controls.Panels;
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
		private readonly GridLayout _gridLayout = new GridLayout();
		private int? _hoverRowIndex = null;
		private int? _hoverColumnIndex = null;
		private int? _selectedRowIndex = null;
		private int? _selectedColumnIndex = null;

		/// <summary> 
		/// The game object property for <see cref="ShowGridLines"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<bool> ShowGridLinesProperty = CreateProperty(
			typeof(Grid), "ShowGridLines", GamePropertyCategories.Behavior, null, false,
			UIPropertyOptions.None);

		public bool ShowGridLines
		{
			get => ShowGridLinesProperty.GetValue(this);
			set => ShowGridLinesProperty.SetValue(this, value);
		}

		/// <summary> 
		/// The game object property for <see cref="ShowGridLines"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<Color> GridLinesColorProperty = CreateProperty(
			typeof(Grid), "GridLinesColor", GamePropertyCategories.Behavior, null, Color.White,
			UIPropertyOptions.None);

		public Color GridLinesColor
		{
			get => GridLinesColorProperty.GetValue(this);
			set => GridLinesColorProperty.SetValue(this, value);
		}

		/// <summary> 
		/// The game object property for <see cref="ColumnSpacing"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<float> ColumnSpacingProperty = CreateProperty(
			typeof(Grid), "ColumnSpacing", GamePropertyCategories.Behavior, null, 0.0f,
			UIPropertyOptions.AffectsMeasure);
		
		public float ColumnSpacing
		{
			get => ColumnSpacingProperty.GetValue(this);
			set
			{
				ColumnSpacingProperty.SetValue(this, value);
				_gridLayout.ColumnSpacing = value;
			}
		}

		/// <summary> 
		/// The game object property for <see cref="RowSpacing"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<float> RowSpacingProperty = CreateProperty(
			typeof(Grid), "RowSpacing", GamePropertyCategories.Behavior, null, 0.0f,
			UIPropertyOptions.AffectsMeasure);

		public float RowSpacing
		{
			get => RowSpacingProperty.GetValue(this);
			set
			{
				RowSpacingProperty.SetValue(this, value);
				_gridLayout.RowSpacing = value;
			}
		}

		[Browsable(false)]
		public Proportion DefaultColumnProportion
		{
			get => _gridLayout.DefaultColumnProportion;
			set => _gridLayout.DefaultColumnProportion = value;
		}

		[Browsable(false)]
		public Proportion DefaultRowProportion
		{
			get => _gridLayout.DefaultRowProportion;
			set => _gridLayout.DefaultRowProportion = value;
		}

		[Browsable(false)]
		public ObservableCollection<Proportion> ColumnsProportions => _gridLayout.ColumnsProportions;

		[Browsable(false)]
		public ObservableCollection<Proportion> RowsProportions => _gridLayout.RowsProportions;


		[Category("Appearance")]
		public IBrush SelectionBackground { get; set; }

		[Category("Appearance")]
		public IBrush SelectionHoverBackground { get; set; }

		[Category(GamePropertyCategories.Behavior)]
		[DefaultValue(GridSelectionMode.None)]
		public GridSelectionMode GridSelectionMode { get; set; }

		[Category(GamePropertyCategories.Behavior)]
		[DefaultValue(true)]
		public bool HoverIndexCanBeNull { get; set; }

		[Category(GamePropertyCategories.Behavior)]
		[DefaultValue(false)]
		public bool CanSelectNothing { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public List<float> GridLinesX => _gridLayout.GridLinesX;

		[Browsable(false)]
		[XmlIgnore]
		public List<float> GridLinesY => _gridLayout.GridLinesY;

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
			ColumnsProportions.CollectionChanged += OnProportionsChanged;
			RowsProportions.CollectionChanged += OnProportionsChanged;

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			HoverIndexCanBeNull = true;
			CanSelectNothing = false;
			Style = "Grid";
		}

		public float GetColumnWidth(int index) => _gridLayout.GetColumnWidth(index);

		public float GetRowHeight(int index) => _gridLayout.GetRowHeight(index);

		public float GetCellLocationX(int col) => _gridLayout.GetCellLocationX(col);

		public float GetCellLocationY(int row) => _gridLayout.GetCellLocationY(row);

		public RectangleF GetCellRectangle(int col, int row) => _gridLayout.GetCellRectangle(col, row);

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

		public Proportion GetColumnProportion(int col) => _gridLayout.GetColumnProportion(col);

		public Proportion GetRowProportion(int row) => _gridLayout.GetRowProportion(row);


		protected override Vector2 OnMeasure(Vector2 availableSize)
		{
			return _gridLayout.Measure(availableSize, Children);
		}

		protected override void OnArrange(Vector2 position, Vector2 size)
		{
			base.OnArrange(position, size);

			_gridLayout.Arrange(size, ActualBounds, Children, Arrange);

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
				for (var i = 0; i < _gridLayout.CellLocationsX.Count; ++i)
				{
					var cx = _gridLayout.CellLocationsX[i] + bounds.Left - ColumnSpacing / 2;
					if (x >= cx && x < cx + _gridLayout.ColWidths[i] + ColumnSpacing / 2)
					{
						HoverColumnIndex = i;
						break;
					}
				}
			}

			if (GridSelectionMode == GridSelectionMode.Row || GridSelectionMode == GridSelectionMode.Cell)
			{
				var y = pos.Y;
				for (var i = 0; i < _gridLayout.CellLocationsY.Count; ++i)
				{
					var cy = _gridLayout.CellLocationsY[i] + bounds.Top - RowSpacing / 2;
					if (y >= cy && y < cy + _gridLayout.RowHeights[i] + RowSpacing / 2)
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
			}
			else
			{
				UpdateHoverPosition(context.MousePosition);

				var inputService = InputService;
				// Check if button gets pressed down.
				if (!inputService.IsMouseOrTouchHandled
						&& inputService.IsPressed(MouseButtons.Left, false))
				{
					inputService.IsMouseOrTouchHandled = true;
					if (HoverRowIndex != null)
					{
						if (SelectedRowIndex != HoverRowIndex)
						{
							SelectedRowIndex = HoverRowIndex;
						}
						else if (CanSelectNothing)
						{
							SelectedRowIndex = null;
						}
					}

					if (HoverColumnIndex != null)
					{
						if (SelectedColumnIndex != HoverColumnIndex)
						{
							SelectedColumnIndex = HoverColumnIndex;
						}
						else if (CanSelectNothing)
						{
							SelectedColumnIndex = null;
						}
					}
				}
			}
		}
	}
}