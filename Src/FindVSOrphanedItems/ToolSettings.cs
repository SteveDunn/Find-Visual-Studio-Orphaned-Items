using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace Tools.VisualStudioOrphanedFiles
{
	[Serializable]
	public class ToolSettings
	{
		public string IncludeFilter { get; set; }

		public Point WindowLocation { get; set; }

		public Size WindowSize { get; set; }

		public WindowStartupLocation WindowStartupLocation { get; set; }

		[XmlIgnore]
		public bool LoadedFromStorageCorrectly { get; private set; }

		public ToolSettings( )
		{
			IncludeFilter = @"*.cs;*.resx" ;
		}

		public static ToolSettings Load( )
		{
			try
			{
				IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForAssembly();

				using (var stream =
					new IsolatedStorageFileStream(
						@"Settings.xml",
						FileMode.Open,
						FileAccess.Read,
						isolatedStorageFile))
				{
					var serializer = new XmlSerializer(typeof (ToolSettings));
					var settings = serializer.Deserialize(new XmlTextReader(stream)) as ToolSettings;
					settings.LoadedFromStorageCorrectly = true;
					return settings;

				}
			}
			catch
			{
				return new ToolSettings();
			}
		}

		public void Save()
		{
			IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForAssembly();

			using (var fileStream = new IsolatedStorageFileStream(
				@"Settings.xml",
				FileMode.Create,
				FileAccess.Write,
				isolatedStorageFile))
			{
				//var serializer = new XmlSerializer(typeof (Test));
				//var t = new Test();
				//serializer.Serialize(fileStream, t);
				var serializer = new XmlSerializer(typeof(ToolSettings));
				serializer.Serialize(fileStream, this);
			}
		}
	}
}