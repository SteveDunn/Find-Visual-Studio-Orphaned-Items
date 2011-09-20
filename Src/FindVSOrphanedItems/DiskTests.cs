using System.Collections.Generic ;
using System.Linq ;
using NUnit.Framework ;

namespace Tools.VisualStudioOrphanedFiles
{
    [TestFixture]
    public class DiskTests
    {
        [Test]
        public void TestGetFilesInDirectoryRecursively()
        {
            IEnumerable<string> files = Disk.GetFilesInDirectory(@"..\..\..\TestVS71Project", true);
            Assert.AreEqual(12, files.Count());
        }

        [Test]
        public void TestGetFilesInDirectoryNonRecursively()
        {
            IEnumerable<string> files = Disk.GetFilesInDirectory(@"..\..\..\TestVS71Project", false);
            Assert.AreEqual(11, files.Count());
        }
    }
}