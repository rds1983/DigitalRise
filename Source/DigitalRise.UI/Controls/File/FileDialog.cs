using DigitalRise.Input;
using DigitalRise.UI.TextureAtlases;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace DigitalRise.UI.Controls
{
	public partial class FileDialog
	{
		class ListItem
		{
			public string Name { get; private set; }
			public string Path { get; private set; }
			public TextureRegion Image { get; private set; }

			public ListItem(string name, string path, TextureRegion image)
			{
				Name = name;
				Path = path;
				Image = image;
			}
		}

		private const int ImageTextSpacing = 4;

		private static readonly string[] Folders =
		{
			"Desktop", "Downloads"
		};

		private readonly List<string> _history = new List<string>();
		private int _historyPosition;
		private readonly FileDialogMode _mode;

		public string Folder
		{
			get { return _textFieldPath.Text; }

			set
			{
				SetFolder(value, true);
			}
		}

		/// <summary>
		/// File filter that is used as 2nd parameter for Directory.EnumerateFiles call
		/// </summary>
		public string Filter
		{
			get; set;
		}

		internal string FileName
		{
			get { return _textFieldFileName.Text; }

			set
			{
				_textFieldFileName.Text = value;
			}
		}

		public string FilePath
		{
			get
			{
				if (_mode == FileDialogMode.ChooseFolder)
				{
					return Folder;
				}

				if (string.IsNullOrEmpty(Folder))
				{
					return FileName;
				}

				if (string.IsNullOrEmpty(FileName))
				{
					return Folder;
				}

				return Path.Combine(Folder, FileName);
			}

			set
			{
				Folder = Path.GetDirectoryName(value);
				FileName = Path.GetFileName(value);

				if (!string.IsNullOrEmpty(FileName))
				{
					for(var i = 0; i < _listBoxFiles.Items.Count; i++)
					{
						var listItem = (ListItem)_listBoxFiles.Items[i];
						if (listItem.Path == FileName)
						{
							_listBoxFiles.SelectedIndex = i;
							break;
						}
					}
				}
			}
		}

		public bool AutoAddFilterExtension { get; set; }

		public FileDialog(FileDialogMode mode)
		{
			_mode = mode;
			Style = "FileDialog";
			IsModal = true;

			BuildUI();

			_listBoxPlaces.CreateControlForItem = CreateControlPlaces;
			_listBoxFiles.CreateControlForItem = CreateControlFiles;

			switch (mode)
			{
				case FileDialogMode.OpenFile:
					Title = "Open File...";
					break;
				case FileDialogMode.SaveFile:
					Title = "Save File...";
					break;
				case FileDialogMode.ChooseFolder:
					Title = "Choose Folder...";
					break;
			}

			AutoAddFilterExtension = true;

			if (mode == FileDialogMode.ChooseFolder)
			{
				_textBlockFileName.IsVisible = false;
				_textFieldFileName.IsVisible = false;
			}

			_splitPane.SplitterPosition = 0.3f;

			var homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
							Environment.OSVersion.Platform == PlatformID.MacOSX)
				? Environment.GetEnvironmentVariable("HOME")
				: Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

			var places = new List<string>
			{
				homePath
			};

			foreach (var f in Folders)
			{
				places.Add(Path.Combine(homePath, f));
			}

			TextureRegion iconFolder = null;
			TextureRegion iconDrive = null;

			foreach (var p in places)
			{
				if (!Directory.Exists(p))
				{
					continue;
				}

				_listBoxPlaces.Items.Add(new ListItem(Path.GetFileName(p), p, iconFolder));
			}

			if (_listBoxPlaces.Items.Count > 0)
			{
				SetFolder(((ListItem)_listBoxPlaces.Items[0]).Path, false);
			}

			var drives = DriveInfo.GetDrives();
//			var iconDrive = DefaultAssets.UITextureRegionAtlas["icon-drive"];
			foreach (var d in drives)
			{
				if (d.DriveType == DriveType.Ram || d.DriveType == DriveType.Unknown)
				{
					continue;
				}

				try
				{
					var s = d.RootDirectory.FullName;

					if (!string.IsNullOrEmpty(d.VolumeLabel) && d.VolumeLabel != d.RootDirectory.FullName)
					{
						s += " (" + d.VolumeLabel + ")";
					}

					_listBoxPlaces.Items.Add(new ListItem(s, d.RootDirectory.FullName, iconDrive));
				}
				catch (Exception)
				{
				}
			}

			_listBoxPlaces.Properties.Get<int>("SelectedIndex").Changed += OnPlacesSelectedIndexChanged;
			_listBoxFiles.Properties.Get<int>("SelectedIndex").Changed += OnFilesSelectedIndexChanged;
			_listBoxFiles.InputProcessed += _listBoxFiles_InputProcessed;

			_buttonParent.Click += OnButtonParent;

//			_textFieldFileName.TextChanged += (s, a) => UpdateEnabled();

			_buttonBack.Click += OnButtonBack;
			_buttonForward.Click += OnButtonForward;

			UpdateEnabled();
		}

		private void _listBoxFiles_InputProcessed(object sender, InputEventArgs e)
		{
			if (!InputService.IsDoubleClick(MouseButtons.Left) ||
				_listBoxFiles.SelectedIndex == -1)
			{
				return;
			}

			var path = ((ListItem)_listBoxFiles.Items[_listBoxFiles.SelectedIndex]).Path;

			if (Directory.Exists(path))
			{
				_listBoxPlaces.SelectedIndex = -1;
				Folder = path;
			}
			else
			{
				//				OnOk();
			}
		}

		private UIControl CreateControlPlaces(object obj)
		{
			ListItem item = (ListItem)obj;

			var result = new ListBoxItem(_listBoxPlaces)
			{
				Content = new TextBlock
				{
					Style = "ListBoxItemTextBlock",
					Text = item.Name
				}
			};

			return result;
		}

		private UIControl CreateControlFiles(object obj)
		{
			ListItem item = (ListItem)obj;

			var result = new ListBoxItem(_listBoxFiles)
			{
				Content = new TextBlock
				{
					Style = "ListBoxItemTextBlock",
					Text = item.Name
				}
			};

			return result;
		}

		protected override void OnLoad()
		{
			base.OnLoad();

		}

		private void UpdateEnabled()
		{
			var enabled = false;
			switch (_mode)
			{
				case FileDialogMode.OpenFile:
					enabled = !string.IsNullOrEmpty(FileName) && System.IO.File.Exists(FilePath);
					break;
				case FileDialogMode.SaveFile:
					enabled = !string.IsNullOrEmpty(FileName);
					break;
				case FileDialogMode.ChooseFolder:
					enabled = !string.IsNullOrEmpty(Folder);
					break;
			}

			// ButtonOk.Enabled = enabled;
		}

		private void OnButtonParent(object sender, EventArgs args)
		{
			if (string.IsNullOrEmpty(Folder))
			{
				return;
			}

			var parentFolder = Path.GetDirectoryName(Folder);

			Folder = parentFolder;
		}

		private void OnButtonBack(object sender, EventArgs args)
		{
			if (_historyPosition <= 0)
			{
				return;
			}

			--_historyPosition;
			if (_historyPosition >= 0 && _historyPosition < _history.Count)
			{
				SetFolder(_history[_historyPosition], false);
			}
		}

		private void OnButtonForward(object sender, EventArgs args)
		{
			if (_historyPosition >= _history.Count - 1)
			{
				return;
			}

			++_historyPosition;
			if (_historyPosition >= 0 && _historyPosition < _history.Count)
			{
				SetFolder(_history[_historyPosition], false);
			}
		}

		private void SetFolder(string value, bool storeInHistory)
		{
			if (!Directory.Exists(value))
			{
				return;
			}

			_textFieldPath.Text = value;
			UpdateFolder();
			UpdateEnabled();

			if (!storeInHistory)
			{
				return;
			}

			while (_history.Count > 0 && _historyPosition < _history.Count - 1)
			{
				_history.RemoveAt(_history.Count - 1);
			}

			_history.Add(Folder);

			_historyPosition = _history.Count - 1;
		}

		private void OnFilesSelectedIndexChanged(object sender, EventArgs args)
		{
			if (_listBoxFiles.SelectedIndex == -1)
			{
				return;
			}

			_listBoxPlaces.SelectedIndex = -1;

			var path = ((ListItem)_listBoxFiles.Items[_listBoxFiles.SelectedIndex]).Path;
			var fi = new FileInfo(path);
			if (fi.Attributes.HasFlag(FileAttributes.Directory) && _mode == FileDialogMode.ChooseFolder)
			{
				_textFieldPath.Text = path;
			}
			else if (!fi.Attributes.HasFlag(FileAttributes.Directory) && _mode != FileDialogMode.ChooseFolder)
			{
				FileName = Path.GetFileName(path);
			}
		}

		private void OnPlacesSelectedIndexChanged(object sender, EventArgs args)
		{
			if (_listBoxPlaces.SelectedIndex == -1)
			{
				return;
			}

			var selectedItem = (ListItem)_listBoxPlaces.Items[_listBoxPlaces.SelectedIndex];
			var path = (string)selectedItem.Path;
			Folder = path;
		}

		private void UpdateFolder()
		{
			_listBoxFiles.Items.Clear();

			var path = _textFieldPath.Text;
			var folders = Directory.EnumerateDirectories(path);

			TextureRegion iconFolder = null;

			foreach (var f in folders)
			{
				var fileInfo = new FileInfo(f);
				if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
				{
					continue;
				}

				var image = new Image
				{
					// Renderable = iconFolder,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				};

				_listBoxFiles.Items.Add(new ListItem(Path.GetFileName(f), f, null));
			}

			if (_mode == FileDialogMode.ChooseFolder)
			{
				return;
			}

			IEnumerable<string> files;

			if (string.IsNullOrEmpty(Filter))
			{
				files = Directory.EnumerateFiles(path);
			}
			else
			{
				var parts = Filter.Split('|');
				var result = new List<string>();

				foreach (var part in parts)
				{
					result.AddRange(Directory.EnumerateFiles(path, part));
				}

				files = result;
			}

			foreach (var f in files)
			{
				var fileInfo = new FileInfo(f);
				if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
				{
					continue;
				}

				_listBoxFiles.Items.Add(new ListItem(Path.GetFileName(f), f, null));
			}
		}

/*		protected internal override bool CanCloseByOk()
		{
			if (_mode != FileDialogMode.SaveFile)
			{
				return true;
			}

			var fileName = FileName;

			if (AutoAddFilterExtension && !string.IsNullOrEmpty(Filter))
			{
				var idx = Filter.LastIndexOf('.');
				if (idx != -1)
				{
					var ext = Filter.Substring(idx);

					if (!fileName.EndsWith(ext))
					{
						fileName += ext;
					}
				}
			}

			if (System.IO.File.Exists(Path.Combine(Folder, fileName)))
			{
				var dlg = CreateMessageBox("Confirm Replace",
					string.Format("File named '{0}' already exists. Do you want to replace it?", fileName));

				dlg.Closed += (s, a) =>
				{
					if (!dlg.Result)
					{
						return;
					}

					FileName = fileName;

					Result = true;
					Close();
				};

				dlg.ShowModal(Desktop);
			}
			else
			{
				FileName = fileName;

				Result = true;
				Close();
			}

			return false;
		}*/
	}
}