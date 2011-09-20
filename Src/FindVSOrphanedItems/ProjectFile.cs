using System;
using System.IO;
using System.Xml.XPath;

namespace Tools.VisualStudioOrphanedFiles
{
    /// <summary>
    /// Represents a file item in a project.
    /// </summary>
    public class ProjectFile
    {
        readonly string _relativePath;
        readonly BuildAction _buildAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectFile"/> class.
        /// </summary>
        /// <param name="nodeFromProject">The x path navigator.</param>
        public ProjectFile(XPathNavigator nodeFromProject)
        {
            if (nodeFromProject == null)
            {
                throw new ArgumentNullException("nodeFromProject", @"Cannot create project file as the XML passed was null.");
            }

            _relativePath = nodeFromProject.GetAttribute(@"RelPath", string.Empty);

            string buildAction = nodeFromProject.GetAttribute(@"BuildAction", string.Empty);
            _buildAction = (BuildAction)Enum.Parse(typeof(BuildAction), buildAction);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectFile"/> class.
        /// </summary>
        /// <param name="buildAction">The build action.</param>
        /// <param name="relativePath">The relative path.</param>
        public ProjectFile(BuildAction buildAction, string relativePath)
        {
            _buildAction = buildAction;

            if (string.IsNullOrEmpty( relativePath ))
            {
                throw new ArgumentNullException(@"relativePath");
            }

            if (Path.IsPathRooted(relativePath))
            {
                throw new ArgumentException(@"Cannot set path as it is not a relative path.", @"relativePath");
            }

            _relativePath = relativePath;
        }

        /// <summary>
        /// Gets the build action.
        /// </summary>
        /// <value>The build action.</value>
        public BuildAction BuildAction
        {
            get { return _buildAction; }
        }

        /// <summary>
        /// Gets the relative path.
        /// </summary>
        /// <value>The relative path.</value>
        public string RelativePath
        {
            get { return _relativePath; }
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <param name="project">The parser.</param>
        /// <param name="projectFile">The project file.</param>
        /// <returns></returns>
        public static string GetFullPath(Project project, ProjectFile projectFile)
        {
            return Path.GetFullPath(Path.Combine(project.Directory, projectFile.RelativePath));
        }
    }
}