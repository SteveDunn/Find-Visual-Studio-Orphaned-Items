using System.IO ;

namespace Tools.VisualStudioOrphanedFiles
{
	public static class Helper
	{
		public static void DeleteFileIntoRecycleBin(string path)
		{
			RemoveReadOnlyFlag(path);

			var fileop = new NativeMethods.SHFILEOPSTRUCT
			             	{
			             		wFunc = NativeMethods.SHFileOperationType.FO_DELETE,
			             		pFrom = (path + '\0' + '\0'),
			             		fFlags =
			             			(NativeMethods.ShFileOperationFlags.FOF_ALLOWUNDO |
			             			 NativeMethods.ShFileOperationFlags.FOF_NOCONFIRMATION)
			             	};

			NativeMethods.SHFileOperation(fileop);
		}

		public static void RemoveReadOnlyFlag(string path)
		{
			var fileInfo = new FileInfo(path);

			if ((fileInfo.Attributes & FileAttributes.ReadOnly) != 0)
			{
				fileInfo.Attributes -= FileAttributes.ReadOnly;
			}
		}
	}
}
