using DigitalRise.GameBase;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DigitalRise.UI.Controls
{
	public class SplitPane : ContentControl
	{
		private StackPanel _stackPanel;
		private Button _buttonHandle = new Button();
		private int? _mouseCoord;
		private int _handlesSize;

		/// <summary>
		/// Gets or sets the orientation of the stack panel. 
		/// This is a game object property.
		/// </summary>
		/// <value>The orientation of the stack panel.</value>
		public Orientation Orientation
		{
			get => _stackPanel.Orientation;
			set => _stackPanel.Orientation = value;
		}

		/// <summary>
		/// Gets or sets the first control
		/// </summary>
		/// <value>The first control.</value>
		public UIControl First
		{
			get { return _stackPanel.Children[0]; }
			set
			{
				_stackPanel.Children[0] = value;
			}
		}

		/// <summary>
		/// Gets or sets the first control
		/// </summary>
		/// <value>The first control.</value>
		public UIControl Second
		{
			get { return _stackPanel.Children[2]; }
			set
			{
				_stackPanel.Children[2] = value;
			}
		}

		public event EventHandler ProportionsChanged;

		public SplitPane()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			_stackPanel = new StackPanel();
			_stackPanel.Children.Add(null);
			_stackPanel.Children.Add(new Button() { Content = new TextBlock { Text = "test" } });
			_stackPanel.Children.Add(null);

			Content = _stackPanel;
		}

		public float GetProportion() => 0.0f;
	}
}
