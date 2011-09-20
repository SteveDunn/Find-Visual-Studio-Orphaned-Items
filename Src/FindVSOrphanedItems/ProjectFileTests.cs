using System ;
using NUnit.Framework ;

namespace Tools.VisualStudioOrphanedFiles
{
	[TestFixture]
	public class ProjectFileTests
	{
		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void TestCreatingProjectFileWithNullRelativePath()
		{
			new ProjectFile(BuildAction.None, null);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void TestCreatingProjectFileWithEmptyRelativePath()
		{
			new ProjectFile(BuildAction.None, string.Empty);
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void TestCreatingProjectFileWithAnAbsolutePath()
		{
			new ProjectFile(BuildAction.None, @"c:\");
		}

		[Test]
		public void TestCreatingProjectFile()
		{
			var projectFile = new ProjectFile(BuildAction.None, @"..\..\imaginary.txt");
			Assert.AreEqual(@"..\..\imaginary.txt", projectFile.RelativePath);
		}
	}
}