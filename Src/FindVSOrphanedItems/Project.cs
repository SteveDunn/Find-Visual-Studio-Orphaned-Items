using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Tools.VisualStudioOrphanedFiles
{

    /// <summary>
    /// Notification from the project parser.
    /// </summary>
    public delegate void ProjectParserNotification(string message);

    /// <summary>
    /// Represents an instance of a project parser.
    /// </summary>
    public class Project
    {
        readonly XPathDocument _xpathDocument;
        readonly List<ProjectFile> _projectFiles;

        /// <summary>
        /// Occurs when there is a notification.
        /// </summary>
        public event ProjectParserNotification OnNotify;

        public static Project LoadFromFile(string pathToProject)
        {
            return new Project(pathToProject);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        Project(string path)
            : this(File.OpenText(path))
        {
            ProjectPath = path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="streamReader">The stream reader.</param>
        Project(TextReader streamReader)
        {
            _projectFiles = new List<ProjectFile>();

            _xpathDocument = new XPathDocument(streamReader);
            XPathNavigator root = _xpathDocument.CreateNavigator();

            if (!containsNamespace(@"http://schemas.microsoft.com/developer/msbuild/2003"))
            {
                XPathNodeIterator it = root.Select(@"VisualStudioProject/CSHARP/Files/Include//File");
                while (it.MoveNext())
                {
                    _projectFiles.Add(new ProjectFile(it.Current));
                }
            }
            else
            {
                readVS80Files();
            }
        }

        void readVS80Files()
        {
            XPathNavigator root = _xpathDocument.CreateNavigator();

            var xmlNamespaceManager = new XmlNamespaceManager(root.NameTable);
            xmlNamespaceManager.AddNamespace(@"local", @"http://schemas.microsoft.com/developer/msbuild/2003");

            XPathExpression expr = root.Compile(@"//local:ItemGroup/child::node()");
            expr.SetContext(xmlNamespaceManager);

            XPathNodeIterator nodes = root.Select(expr);

            while (nodes.MoveNext())
            {
                string buildActionName = nodes.Current.Name;
                BuildAction buildAction;
                try
                {
                    buildAction = (BuildAction)Enum.Parse(typeof(BuildAction), buildActionName);
                }
                catch (ArgumentException)
                {
                    continue;
                }

                string relativePath = nodes.Current.GetAttribute(@"Include", string.Empty);

                _projectFiles.Add(new ProjectFile(buildAction, relativePath));
            }
        }

        bool containsNamespace(string @namespace)
        {
            XPathNavigator root = _xpathDocument.CreateNavigator();

            XPathNodeIterator nodeIterator =
                root.Select(@"//namespace::*[not(. = ../../namespace::*)]");
            while (nodeIterator.MoveNext())
            {
                if (nodeIterator.Current.Value == @namespace)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets or sets the project path.
        /// </summary>
        /// <value>The project path.</value>
        public string ProjectPath { get; set; }

        /// <summary>
        /// Gets the project directory from the project path that was set with the ProjectPath property.
        /// </summary>
        /// <value>The project directory.</value>
        public string Directory
        {
            get { return Path.GetDirectoryName(ProjectPath); }
        }

        /// <summary>
        /// Determines whether the the specified file is orphaned.
        /// </summary>
        /// <param name="pathToFile">The path to file.</param>
        /// <returns>
        /// 	<c>true</c> if the file is orphaned; otherwise, <c>false</c>.
        /// </returns>
        public bool IsFileOrphaned(string pathToFile)
        {
            if (!Path.IsPathRooted(pathToFile))
            {
                throw new ArgumentException(@"Please pass an absolute pathToFile.", "pathToFile");
            }

            if (!File.Exists(pathToFile))
            {
                throw new FileNotFoundException(@"The file supplied does not exist.", pathToFile);
            }

            IEnumerable<ProjectFile> projectFiles = ProjectFiles;
            foreach (ProjectFile projectFile in projectFiles)
            {
                string fullProjectFilePath = ProjectFile.GetFullPath(this, projectFile);
                if (string.Compare(fullProjectFilePath, pathToFile, true, CultureInfo.InvariantCulture) == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the files that belong to the project.
        /// </summary>
        /// <value>The project files.</value>
        public IEnumerable<ProjectFile> ProjectFiles
        {
            get
            {
                return _projectFiles;
            }
        }

        /// <summary>
        /// Gets the orphaned files.
        /// </summary>
        /// <value>The orphaned files.</value>
        public IEnumerable<string> OrphanedFiles
        {
            get
            {
                IEnumerable<string> allFilesInDirectory = getFilesInProjectDirectoryRecursively();

                foreach (string filePath in allFilesInDirectory)
                {
                    notify(string.Format(CultureInfo.InvariantCulture, @"Processing {0}", filePath));

                    if (IsFileOrphaned(Path.GetFullPath(filePath)))
                    {
                        yield return filePath;
                    }
                }

                notify(@"Finished");
            }
        }

        void notify(string message)
        {
            if (OnNotify != null)
            {
                OnNotify(message);
            }
        }

        //		public IList BeginGetOrphanedFiles( AsyncCallback callback )
        //		{
        //			return GetOrphanedFiles( ) ;
        //		}
        IEnumerable<string> getFilesInProjectDirectoryRecursively()
        {
            return getFilesInProjectDirectory(true);
        }

        IEnumerable<string> getFilesInProjectDirectory(bool recursive)
        {
            return Disk.GetFilesInDirectory(Directory, recursive);
        }

    }
}