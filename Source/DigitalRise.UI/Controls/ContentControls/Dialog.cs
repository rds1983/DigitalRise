using Microsoft.Xna.Framework;

namespace DigitalRise.UI.Controls
{
	public class Dialog: Window
	{
		private StackPanel _buttonsPanel;
		private Button _okButton;
		private Button _cancelButton;

		public Button OkButton => _okButton;
		public Button CancelButton => _cancelButton;


		public Dialog()
		{
			Style = "Dialog";
		}

		protected override void OnLoad()
		{
			base.OnLoad();

			_buttonsPanel = new StackPanel
			{
				HorizontalAlignment = HorizontalAlignment.Right,
				VerticalAlignment = VerticalAlignment.Bottom,
				Margin = new Vector4(10),
				Orientation = Orientation.Horizontal,
			};

			_okButton = new Button
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top,
				Name = "OkButton",
				Focusable = true,
				Content = new TextBlock
				{
					Text = "Ok",
				}
			};

			_okButton.Click += (s, e) =>
			{
				DialogResult = true;
				Close();
			};

			_buttonsPanel.Children.Add(_okButton);

			_cancelButton = new Button
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top,
				Name = "CancelButton",
				Focusable = true,
				Content = new TextBlock
				{
					Text = "Cancel",
				}
			};

			_cancelButton.Click += (s, e) =>
			{
				DialogResult = false;
				Close();
			};

			_buttonsPanel.Children.Add(_cancelButton);

			VisualChildren.Add(_buttonsPanel);
		}
	}
}
