using DigitalRise.Collections;
using DigitalRise.GameBase;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace DigitalRise.UI.Controls
{
	public class ListBox: ContentControl
	{
		// Very similar to ContextMenu, which is also a kind of "popup". See comments in 
		// ContextMenu code.
		// On Windows Phone the DropDown fills the entire screen and displays a title.

		//--------------------------------------------------------------
		#region Fields
		//--------------------------------------------------------------

		internal StackPanel _itemsPanel;
		private ScrollViewer _scrollViewer;
		#endregion

		/// <summary> 
		/// The game object property for <see cref="SelectedIndex"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<int> SelectedIndexProperty = CreateProperty(
			typeof(DropDownButton), "SelectedIndex", GamePropertyCategories.Default, null, -1,
			UIPropertyOptions.AffectsMeasure);

		/// <summary>
		/// Gets or sets the index of the selected item. 
		/// This is a game object property.
		/// </summary>
		/// <value>
		/// The index of the selected item; or -1 if no item is selected. 
		/// </value>
		public int SelectedIndex
		{
			get => SelectedIndexProperty.GetValue(this);
			set => SelectedIndexProperty.SetValue(this, value);
		}

		/// <summary>
		/// Gets the items.
		/// </summary>
		/// <value>The items.</value>
		public NotifyingCollection<object> Items { get; private set; }

		/// <summary>
		/// Gets or sets the method that creates <see cref="UIControl"/>s for the <see cref="Items"/>.
		/// </summary>
		/// <value>
		/// The method that creates <see cref="UIControl"/>s for the <see cref="Items"/>. If this 
		/// property is <see langword="null"/>, <see cref="TextBlock"/>s are used to display the 
		/// <see cref="Items"/>. The default is <see langword="null"/>.
		/// </value>
		public Func<object, UIControl> CreateControlForItem { get; set; }

		//--------------------------------------------------------------
		#region Creation & Cleanup
		//--------------------------------------------------------------

		/// <summary>
		/// Initializes static members of the <see cref="DropDown"/> class.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static ListBox()
		{
			// TODO: Is this needed?
			OverrideDefaultValue(typeof(ListBox), IsFocusScopeProperty.Id, true);
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="DropDown"/> class.
		/// </summary>
		public ListBox()
		{
			Style = "ListBox";

			Items = new NotifyingCollection<object>(false, true);
			Items.CollectionChanged += OnItemsChanged;

			_itemsPanel = new StackPanel
			{
				Orientation = Orientation.Vertical,
			};

			_scrollViewer = new ScrollViewer
			{
				Content = _itemsPanel,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch
			};

			Content = _scrollViewer;
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
		}
		#endregion


		//--------------------------------------------------------------
		#region Methods
		//--------------------------------------------------------------

		private void OnItemsChanged(object sender, CollectionChangedEventArgs<object> eventArgs)
		{
			_scrollViewer.HorizontalOffset = 0;
			_scrollViewer.VerticalOffset = 0;

			_itemsPanel.Children.Clear();

			foreach (var item in Items)
			{
				_itemsPanel.Children.Add(CreateControl(item));
			}
		}


		/// <summary>
		/// Creates a new control for the item. 
		/// </summary>
		internal UIControl CreateControl(object item)
		{
			if (CreateControlForItem != null)
			{
				return CreateControlForItem(item);
			}

			var control = new ListBoxItem(this)
			{
				Content = new TextBlock
				{
					Style = "ListBoxItemTextBlock",
					Text = item.ToString()
				}
			};

			return control;
		}


		internal void SetSelectedItem(ListBoxItem listBoxItem)
		{
			int index = _itemsPanel.Children.IndexOf(listBoxItem);
			SelectedIndex = index;

			Debug.WriteLine(SelectedIndex);
		}

		#endregion
	}
}
