using DigitalRise.Collections;
using DigitalRise.GameBase;
using DigitalRise.UI.Controls.Panels;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DigitalRise.UI.Controls
{
	public class StackPanel : Panel
	{
		private readonly GridLayout _gridLayout = new GridLayout();
		private bool _dirty = true;

		/// <summary> 
		/// The game object property for <see cref="Orientation"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<Orientation> OrientationProperty = CreateProperty(
			typeof(StackPanel), "Orientation", GamePropertyCategories.Layout, null, Orientation.Vertical,
			UIPropertyOptions.AffectsMeasure);

		/// <summary>
		/// Gets or sets the orientation of the stack panel. 
		/// This is a game object property.
		/// </summary>
		/// <value>The orientation of the stack panel.</value>
		public Orientation Orientation
		{
			get => OrientationProperty.GetValue(this);
			set => OrientationProperty.SetValue(this, value);
		}

		/// <summary> 
		/// The game object property for <see cref="ShowGridLines"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<bool> ShowGridLinesProperty = CreateProperty(
			typeof(StackPanel), "ShowGridLines", GamePropertyCategories.Behavior, null, false,
			UIPropertyOptions.None);

		public bool ShowGridLines
		{
			get => ShowGridLinesProperty.GetValue(this);
			set => ShowGridLinesProperty.SetValue(this, value);
		}

		/// <summary> 
		/// The game object property for <see cref="GridLinesColor"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<Color> GridLinesColorProperty = CreateProperty(
			typeof(StackPanel), "GridLinesColor", GamePropertyCategories.Behavior, null, Color.White,
			UIPropertyOptions.None);

		public Color GridLinesColor
		{
			get => GridLinesColorProperty.GetValue(this);
			set => GridLinesColorProperty.SetValue(this, value);
		}

		/// <summary> 
		/// The game object property for <see cref="Spacing"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<float> SpacingProperty = CreateProperty(
			typeof(StackPanel), "Spacing", GamePropertyCategories.Behavior, null, 0.0f,
			UIPropertyOptions.None);

		public float Spacing
		{
			get => SpacingProperty.GetValue(this);
			set
			{
				SpacingProperty.SetValue(this, value);
				if (Orientation == Orientation.Horizontal)
				{
					_gridLayout.ColumnSpacing = value;
				}
				else
				{
					_gridLayout.RowSpacing = value;
				}
			}
		}

		[Browsable(false)]
		public Proportion DefaultProportion
		{
			get => Orientation == Orientation.Horizontal ? _gridLayout.DefaultColumnProportion : _gridLayout.DefaultRowProportion;
			set
			{
				if (Orientation == Orientation.Horizontal)
				{
					_gridLayout.DefaultColumnProportion = value;
				}
				else
				{
					_gridLayout.DefaultRowProportion = value;
				}
			}
		}

		[Browsable(false)]
		public ObservableCollection<Proportion> Proportions
		{
			get => Orientation == Orientation.Horizontal ? _gridLayout.ColumnsProportions : _gridLayout.RowsProportions;
		}

		[Browsable(false)]
		[XmlIgnore]
		public List<float> GridLinesX => _gridLayout.GridLinesX;

		[Browsable(false)]
		[XmlIgnore]
		public List<float> GridLinesY => _gridLayout.GridLinesY;


		public StackPanel()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			Style = "StackPanel";

			DefaultProportion = Proportion.StackPanelDefault;
			Children.CollectionChanged += Children_CollectionChanged;
		}

		private void UpdateGrid()
		{
			if (!_dirty)
			{
				return;
			}

			var index = 0;
			foreach (var widget in Children)
			{
				if (Orientation == Orientation.Horizontal)
				{
					widget.GridColumn = index;
				}
				else
				{
					widget.GridRow = index;
				}

				++index;
			}

			_dirty = false;
		}

		private void Children_CollectionChanged(object sender, CollectionChangedEventArgs<UIControl> e)
		{
			_dirty = true;
		}

		protected override Vector2 OnMeasure(Vector2 availableSize)
		{
			UpdateGrid();

			return _gridLayout.Measure(availableSize, Children);
		}

		protected override void OnArrange(Vector2 position, Vector2 size)
		{
			UpdateGrid();

			_gridLayout.Arrange(size, ActualBounds, Children, Arrange);
		}
	}
}