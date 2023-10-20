namespace DigitalRise.UI.Controls
{
	partial class FileDialog
	{
		private void BuildUI()
		{
			_buttonBack = new Button();
			_buttonBack.HorizontalAlignment = HorizontalAlignment.Center;
			_buttonBack.VerticalAlignment = VerticalAlignment.Center;
			_buttonBack.Name = "_buttonBack";

			_buttonForward = new Button();
			_buttonForward.HorizontalAlignment = HorizontalAlignment.Center;
			_buttonForward.VerticalAlignment = VerticalAlignment.Center;
			_buttonForward.GridColumn = 1;
			_buttonForward.Name = "_buttonForward";

			_textFieldPath = new TextBox();
			_textFieldPath.IsReadOnly = true;
			_textFieldPath.HorizontalAlignment = HorizontalAlignment.Stretch;
			_textFieldPath.VerticalAlignment = VerticalAlignment.Center;
			_textFieldPath.GridColumn = 2;
			_textFieldPath.Name = "_textFieldPath";

			_buttonParent = new Button();
			_buttonParent.HorizontalAlignment = HorizontalAlignment.Center;
			_buttonParent.VerticalAlignment = VerticalAlignment.Center;
			_buttonParent.GridColumn = 3;
			_buttonParent.Name = "_buttonParent";

			var grid1 = new Grid();
			grid1.ColumnSpacing = 4;
			grid1.ColumnsProportions.Add(new Proportion
			{
				Type = ProportionType.Auto,
			});
			grid1.ColumnsProportions.Add(new Proportion
			{
				Type = ProportionType.Auto,
			});
			grid1.ColumnsProportions.Add(new Proportion
			{
				Type = ProportionType.Fill,
			});
			grid1.ColumnsProportions.Add(new Proportion
			{
				Type = ProportionType.Auto,
			});
			grid1.Children.Add(_buttonBack);
			grid1.Children.Add(_buttonForward);
			grid1.Children.Add(_textFieldPath);
			grid1.Children.Add(_buttonParent);

			_listBoxPlaces = new ListBox();
			_listBoxPlaces.HorizontalAlignment = HorizontalAlignment.Stretch;
			_listBoxPlaces.Name = "_listBoxPlaces";

			_listBoxFiles = new ListBox();
			_listBoxFiles.Name = "_listBoxFiles";

			_splitPane = new SplitPane();
			_splitPane.Orientation = Orientation.Horizontal;
			_splitPane.Name = "_splitPane";
			_splitPane.First = _listBoxPlaces;
			_splitPane.Second = _listBoxFiles;

			_textBlockFileName = new TextBlock();
			_textBlockFileName.Text = "File name";
			_textBlockFileName.VerticalAlignment = VerticalAlignment.Center;
			_textBlockFileName.Name = "_textBlockFileName";

			_textFieldFileName = new TextBox();
			_textFieldFileName.IsReadOnly = true;
			_textFieldFileName.GridColumn = 1;
			_textFieldFileName.HorizontalAlignment = HorizontalAlignment.Stretch;
			_textFieldFileName.Name = "_textFieldFileName";

			var grid2 = new Grid();
			grid2.ColumnSpacing = 4;
			grid2.ColumnsProportions.Add(new Proportion
			{
				Type = ProportionType.Auto,
			});
			grid2.ColumnsProportions.Add(new Proportion
			{
				Type = ProportionType.Fill,
			});
			grid2.Children.Add(_textBlockFileName);
			grid2.Children.Add(_textFieldFileName);

			var verticalStackPanel1 = new StackPanel();
			verticalStackPanel1.Orientation = Orientation.Vertical;
			verticalStackPanel1.Spacing = 4;
			verticalStackPanel1.Proportions.Add(new Proportion
			{
				Type = ProportionType.Auto,
			});
			verticalStackPanel1.Proportions.Add(new Proportion
			{
				Type = ProportionType.Fill,
			});
			verticalStackPanel1.Children.Add(grid1);
			verticalStackPanel1.Children.Add(_splitPane);
			verticalStackPanel1.Children.Add(grid2);

			Title = "Open File...";
			X = 176;
			Y = 18;
			Width = 600;
			Height = 400;
			Content = verticalStackPanel1;
		}


		public Button _buttonBack;
		public Button _buttonForward;
		public TextBox _textFieldPath;
		public Button _buttonParent;
		public ListBox _listBoxPlaces;
		public ListBox _listBoxFiles;
		public SplitPane _splitPane;
		public TextBlock _textBlockFileName;
		public TextBox _textFieldFileName;
	}
}