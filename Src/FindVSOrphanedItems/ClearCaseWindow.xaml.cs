using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Tools.VisualStudioOrphanedFiles
{
	/// <summary>
	/// Interaction logic for ClearCaseWindow.xaml
	/// </summary>
	public partial class ClearCaseWindow : Window
	{
		private readonly IEnumerable<string> _files;

		public ClearCaseWindow(IEnumerable<string> files)
		{
			_files = files;
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var stringBuilder = new StringBuilder();

			stringBuilder.Append(@"REM THE CODE THAT PRODUCED THIS SCRIPT IS INCOMPLETE AND HAS NOT BEEN TESTED!!!")
				.Append(Environment.NewLine)
				.Append(Environment.NewLine)
				.Append(getDirectoryCheckoutCommands())
				.Append(Environment.NewLine)
				.Append(Environment.NewLine)
				.Append(getFileDeleteCommands());

			txtBoxCommands.Text = stringBuilder.ToString();
		}

		string getDirectoryCheckoutCommands()
		{
			var uniqueDirectories = new List<string>();
			foreach (string path in _files)
			{
				string directory = System.IO.Path.GetDirectoryName(path).ToUpper();
				
				if (!uniqueDirectories.Contains(directory))
				{
					uniqueDirectories.Add(directory);
				}
			}

			var stringBuilder = new StringBuilder();
			foreach (string directory in uniqueDirectories)
			{
				stringBuilder.AppendFormat(
					@"cleartool checkout -comment:""Checking out directory " +
					@"before removing file(s)"" {0}{1}", directory, Environment.NewLine);
			}
			
			return stringBuilder.ToString();
		}

		string getFileDeleteCommands()
		{
			var stringBuilder = new StringBuilder();
			foreach (string path in _files)
			{
				stringBuilder.AppendFormat(@"cleartool rm {0}{1}", path, Environment.NewLine);
			}

			return stringBuilder.ToString();
		}

		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			Close( );
		}

		private void btnCopyToClipboard_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(txtBoxCommands.Text);
		}
	}
}
