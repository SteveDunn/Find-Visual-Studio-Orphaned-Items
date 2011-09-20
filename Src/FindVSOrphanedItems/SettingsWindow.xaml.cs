using System.Windows;

namespace Tools.VisualStudioOrphanedFiles
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		readonly ToolSettings _settings;

		public SettingsWindow(ToolSettings settings)
		{
			_settings = settings;
		
			InitializeComponent();
			textBoxIncludes.DataContext = settings;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void buttonOKClicked(object sender, RoutedEventArgs e)
		{
			_settings.IncludeFilter = textBoxIncludes.Text;
			
			DialogResult = true;
			
			Close();
		}

		private void buttonCancelClicked(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			
			Close();
		}
	}
}
