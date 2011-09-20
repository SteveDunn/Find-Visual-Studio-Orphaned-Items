using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Tools.VisualStudioOrphanedFiles
{
	/// <summary>
	/// Interaction logic for RemoveFilesWindow.xaml
	/// </summary>
	public partial class RemoveFilesWindow : Window
	{
		private readonly IEnumerable<string> _pathsToFiles;

		public RemoveFilesWindow(IEnumerable<string> pathsToFiles)
		{
			_pathsToFiles = pathsToFiles;
			InitializeComponent();
			listBoxFiles.ItemsSource = _pathsToFiles;
			DataContext = this;
		}

		public bool AnythingChecked
		{
			get { return (bool)checkBoxClearCase.IsChecked || (bool)checkBoxDisk.IsChecked; }
		}

		private void okClicked(object sender, RoutedEventArgs e)
		{
			DialogResult = true;

			IList selected = listBoxFiles.SelectedItems;

			if ((bool) checkBoxDisk.IsChecked)
			{
				foreach (string path in selected)
				{
					Helper.DeleteFileIntoRecycleBin(path);
				}
			}

			if ((bool) checkBoxClearCase.IsChecked)
			{
				new ClearCaseWindow(new List<string>(selected.Cast<string>())).ShowDialog();
			}


			Close();

		}

		private void cancelClicked(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			listBoxFiles.SelectAll();
			listBoxFiles.SelectionMode = SelectionMode.Multiple;
			listBoxFiles.IsEnabled = false; 
		}


		private void deleteFromChecked(object sender, RoutedEventArgs e)
		{
			updateOkButton();
		}

		private void updateOkButton()
		{
			buttonOk.IsEnabled = AnythingChecked && listBoxFiles.SelectedItems.Count > 0;
		}
	}
}
