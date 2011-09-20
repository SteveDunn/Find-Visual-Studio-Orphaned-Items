using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Win32;

namespace Tools.VisualStudioOrphanedFiles
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		private readonly ToolSettings _settings = ToolSettings.Load();
		
		public Window1()
		{
			InitializeComponent();
		}

		private void openFolderClicked(object sender, RoutedEventArgs e)
		{
			var openFolderDialog = new OpenFileDialog
			                       	{
			                       		Filter = @"Visual Studio Projects(*.csproj)|*.csproj|All Files(*.*)|*.*"
			                       	};
			if (openFolderDialog.ShowDialog(this) == false)
			{
				return;
			}

			textBoxPathToProject.Text = openFolderDialog.FileName;
			if( File.Exists(textBoxPathToProject.Text))
			{
				updateUI(getOrphanedFiles(textBoxPathToProject.Text));
			}
		}

		private void buttonGoClicked(object sender, RoutedEventArgs e)
		{
			updateUI(getOrphanedFiles(textBoxPathToProject.Text));
		}
		
		private void buttonRemove_Click(object sender, EventArgs e)
		{
			var selectedItems = new List<string>();
			
			foreach (string s in listBoxOrphanedItems.SelectedItems)
			{
				selectedItems.Add(s);
			}

			var removeFilesWindow = new RemoveFilesWindow(selectedItems);

			if (removeFilesWindow.ShowDialog() == false)
			{
				return;
			}

			updateUI(getOrphanedFiles(textBoxPathToProject.Text));
		}

		private void updateUI(ICollection<string> pathToItems)
		{
			listBoxOrphanedItems.ItemsSource = pathToItems;

			lblStatus.Foreground = Brushes.LightGreen;

			lblStatus.Content = string.Format(
				@"Found {0} orphaned item(s) for: {1}",
				pathToItems.Count,
				_settings.IncludeFilter);
		}

		List<string> getOrphanedFiles(string pathToProjectFile)
		{
			Project project = Project.LoadFromFile(pathToProjectFile);
			// pp.OnNotify += new ProjectParserNotification(parserUpdated);

			IEnumerable<string> orphanedFiles = project.OrphanedFiles;
			
			var fileFilter = new FileFilter
			                 	{
			                 		Input = orphanedFiles, 
									Include = _settings.IncludeFilter
			                 	};
			
			var pathToItems = new List<string>(fileFilter.Output);

			return pathToItems;
		}

		private void closeClicked(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void settingsClicked(object sender, RoutedEventArgs e)
		{
			var settingsWindow = new SettingsWindow(_settings);
			bool? result = settingsWindow.ShowDialog();
			if (result != true)
			{
				return;
			}
			
			lblStatus.Content = @"Settings have changed - results may be out of date";
			
			lblStatus.Foreground = Brushes.Red;
		}

		private void pathChanged(object sender, TextChangedEventArgs e)
		{
			if(goButton==null)
			{
				return;
			}
			
			goButton.IsEnabled = false;

			string path = textBoxPathToProject.Text;
			string errors = getErrorsConcerningPath(path);

			bool hasErrors = !string.IsNullOrEmpty(errors);

			goButton.IsEnabled = !hasErrors;
			animatePathBox(hasErrors);
		}

		private void animatePathBox(bool hasErrors)
		{
			var startColor = Colors.White;
			var endColor = Colors.LightPink;
			if( !hasErrors)
			{
				endColor = startColor;
				startColor = Colors.LightPink;
			}
			
			var brush = new SolidColorBrush(startColor);
			textBoxPathToProject.Background = brush;
			var colorAnimation = new ColorAnimation(startColor, endColor, new Duration(TimeSpan.FromSeconds(.25)))
			                     	{
			                     		AutoReverse = false
			                     	};
			
			brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
		}

		static string getErrorsConcerningPath(string path)
		{
			if (path.Length == 0)
			{
				return @"Enter the path to the project or drag multiple projects here";
			}

			if (!File.Exists(path))
			{
				return @"No such file.";
			}

			return string.Empty;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_settings.WindowLocation = new Point(Left, Top);
			_settings.WindowSize = new Size {Width = Width, Height = Height};
			_settings.WindowStartupLocation = WindowStartupLocation;

			_settings.Save();

		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if( ! _settings.LoadedFromStorageCorrectly )
			{
				return;
			}
			Left = _settings.WindowLocation.X;
			Top = _settings.WindowLocation.Y;
			Width = _settings.WindowSize.Width;
			Height = _settings.WindowSize.Height;
			WindowStartupLocation = _settings.WindowStartupLocation;

			string[] args = Environment.GetCommandLineArgs();

			if (args.Length > 1)
			{
				string path = args[1];
				textBoxPathToProject.Text = path;
				updateUI(getOrphanedFiles(path));
			}
		}

		private void listBoxOrphanedItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			buttonRemove.IsEnabled = listBoxOrphanedItems.SelectedItems.Count > 0;
		}

		private void Window_Drop(object sender, DragEventArgs e)
		{
			// Can only drop files, so check
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				return;
			}

			var files = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (files.Length == 0)
			{
				return;
			}

			if (files.Length == 1)
			{
				textBoxPathToProject.Text = files[0];

				updateUI(getOrphanedFiles(files[0]));
				return;
			}

			updateUI(getOrphanedFiles(files));
		}

		private List<string> getOrphanedFiles(IEnumerable<string> paths)
		{
			var orphanedItems = new List<string>();

			foreach (string path in paths)
			{
				orphanedItems.AddRange(getOrphanedFiles(path));
			}

			return orphanedItems;
		}

		private void Window_DragOver(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.None;
				return;
			}

			e.Effects = DragDropEffects.Copy;
		}

		private void Window_DragEnter(object sender, DragEventArgs e)
		{
			e.Effects = DragDropEffects.Copy;
		}


	}
}
