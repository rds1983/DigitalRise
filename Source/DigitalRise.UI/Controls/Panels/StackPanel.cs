using DigitalRise.Collections;
using DigitalRise.GameBase;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DigitalRise.UI.Controls
{
	public class StackPanel : ContentControl
	{
		private bool _dirty = true;

		private Grid Grid => (Grid)Content;

		/// <summary> 
		/// The game object property for <see cref="Orientation"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<Orientation> OrientationProperty = CreateProperty(
			typeof(SplitPane), "Orientation", GamePropertyCategories.Layout, null, Orientation.Vertical,
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

		[Category("StackPanel")]
		[DefaultValue(false)]
		public bool ShowGridLines
		{
			get => Grid.ShowGridLines;
			set => Grid.ShowGridLines = value;
		}

		[Category("StackPanel")]
		[DefaultValue(0)]
		public float Spacing
		{
			get => Orientation == Orientation.Horizontal ? Grid.ColumnSpacing : Grid.RowSpacing;
			set
			{
				if (Orientation == Orientation.Horizontal)
				{
					Grid.ColumnSpacing = value;
				}
				else
				{
					Grid.RowSpacing = value;
				}
			}
		}

		[Browsable(false)]
		public Proportion DefaultProportion
		{
			get => Orientation == Orientation.Horizontal ? Grid.DefaultColumnProportion : Grid.DefaultRowProportion;
			set
			{
				if (Orientation == Orientation.Horizontal)
				{
					Grid.DefaultColumnProportion = value;
				}
				else
				{
					Grid.DefaultRowProportion = value;
				}
			}
		}

		[Browsable(false)]
		public ObservableCollection<Proportion> Proportions
		{
			get => Orientation == Orientation.Horizontal ? Grid.ColumnsProportions : Grid.RowsProportions;
		}

		[Browsable(false)]
		public NotifyingCollection<UIControl> Children => Grid.Children;

		public StackPanel()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			Style = "StackPanel";

			Content = new Grid();
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

			return base.OnMeasure(availableSize);
		}

		protected override void OnArrange(Vector2 position, Vector2 size)
		{
			UpdateGrid();

			base.OnArrange(position, size);
		}
	}
}