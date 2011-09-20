using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Tools.VisualStudioOrphanedFiles
{
	[TestFixture]
	public class VS80Tests
	{
        int _notifications = 0;
        
        [Test]
		public void ShouldParseVS80Project()
		{
			Project p = Project.LoadFromFile(@"..\..\..\TestVS80Project\TestVS80Project.csproj");
		    IEnumerable< string > orphanedFiles = p.OrphanedFiles ;
		    Assert.AreEqual(6, orphanedFiles.Count());
		}

		[Test]
		public void TestIsOrphaned()
		{
			Project p = Project.LoadFromFile(@"..\..\..\TestVS80Project\TestVS80Project.csproj");
			Assert.IsTrue(p.IsFileOrphaned(Path.GetFullPath(@"..\..\..\TestVS80Project\Class2.cs")));
			Assert.IsFalse(p.IsFileOrphaned(Path.GetFullPath(@"..\..\..\TestVS80Project\Class1.cs")));
		}

		[Test]
		public void TestIsOrphanedWithDifferingTextCase()
		{
			Project p = Project.LoadFromFile(@"..\..\..\TestVS80Project\TestVS80Project.cSProj");
			Assert.IsTrue(p.IsFileOrphaned(Path.GetFullPath(@"..\..\..\TestVS80Project\CLaSs2.cs")));
			Assert.IsFalse(p.IsFileOrphaned(Path.GetFullPath(@"..\..\..\TestVS80Project\CLASS1.cS")));
		}

		[Test, ExpectedException(typeof(FileNotFoundException))]
		public void TestIsOrphanedWithMissingFile()
		{
			Project p = Project.LoadFromFile(@"..\..\..\TestVS80Project\TestVS80Project.csproj");
			Assert.IsTrue(p.IsFileOrphaned(Path.GetFullPath(@"..\..\..\TestVS80Project\Imaginary.cs")));
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void TestIsOrphanedWithRelativePath()
		{
			Project p = Project.LoadFromFile(@"..\..\..\TestVS80Project\TestVS80Project.csproj");
			Assert.IsTrue(p.IsFileOrphaned(@"..\..\InvalidRelativePath\Imaginary.cs"));
		}

		[Test]
		public void TestProjectFiles()
		{
			Project project = Project.LoadFromFile(
				@"..\..\..\TestVS80Project\TestVS80Project.csproj");
			IEnumerable<ProjectFile> projectFiles = project.ProjectFiles;
			Assert.AreEqual(6, projectFiles.Count( ));

			var projectFile = projectFiles.ElementAt(0);
			Assert.AreEqual(@"App.ico", projectFile.RelativePath);
			Assert.AreEqual(BuildAction.Content, projectFile.BuildAction);

			projectFile = projectFiles.ElementAt(1);
			Assert.AreEqual(@"AssemblyInfo.cs", projectFile.RelativePath);
			Assert.AreEqual(BuildAction.Compile, projectFile.BuildAction);

			projectFile = projectFiles.ElementAt(2);
			Assert.AreEqual(@"Class1.cs", projectFile.RelativePath);
			Assert.AreEqual(BuildAction.Compile, projectFile.BuildAction);

			projectFile = projectFiles.ElementAt(3);
			Assert.AreEqual(@"Class3.cs", projectFile.RelativePath);
			Assert.AreEqual(BuildAction.Compile, projectFile.BuildAction);

			projectFile = projectFiles.ElementAt(4);
			Assert.AreEqual(@"Form1.cs", projectFile.RelativePath);
			Assert.AreEqual(BuildAction.Compile, projectFile.BuildAction);

			projectFile = projectFiles.ElementAt(5);
			Assert.AreEqual(@"Form1.resx", projectFile.RelativePath);
			Assert.AreEqual(BuildAction.EmbeddedResource, projectFile.BuildAction);
		}


        [Test]
        public void ShouldHaveTheRightAmountOfFilesPresentInFolderAndSubfolders()
        {
            IEnumerable<string> files = Disk.GetFilesInDirectory(@"..\..\..\TestVS71Project", true);
            Assert.AreEqual(12, files.Count());
        }

		[Test]
		public void TestOrphanedFilesWithNoFilter()
		{
			Project p = Project.LoadFromFile(@"..\..\..\TestVS80Project\TestVS80Project.csproj");
			IEnumerable< string > orphanedFiles = p.OrphanedFiles;
			Assert.AreEqual(6, orphanedFiles.Count( ));
			Assert.IsTrue(orphanedFiles.Contains(@"..\..\..\TestVS80Project\Class2.cs"));
			Assert.IsTrue(orphanedFiles.Contains(@"..\..\..\TestVS80Project\Class4.cs"));
			Assert.IsTrue(orphanedFiles.Contains(@"..\..\..\TestVS80Project\Class5.cs"));
		}
	}
}
