using System.Collections.Generic ;
using System.IO ;

namespace Tools.VisualStudioOrphanedFiles
{
    public static class Disk
    {
        // string getFullPathToProject( )
        // {
        //     return Path.GetFullPath( ProjectPath ) ;
        // }

        public static IEnumerable<string> GetFilesInDirectory(string directory, bool recursive)
        {
            var listOfFiles = new List<string>();

            listOfFiles.AddRange(Directory.GetFiles(directory));

            if (!recursive)
            {
                return listOfFiles;
            }

            foreach (string directoryName in Directory.GetDirectories(directory))
            {
                listOfFiles.AddRange( GetFilesInDirectory(directoryName, true));
            }

            return listOfFiles;
        }        
    }
}