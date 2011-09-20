using System ;
using System.Collections.Generic ;
using System.Linq ;
using NUnit.Framework ;

namespace Tools.VisualStudioOrphanedFiles
{
	[TestFixture]
	public class FileFilterTests
	{

		[Test]
		public void TestFilter()
		{
			var items = new List<string>
			        	{
			        		@"c:\items\item1.cs",
			        		@"c:\items\item2.cs",
			        		@"c:\items\item1.resx",
			        		@"c:\items\blah.blah",
			        		@"c:\items\project.project"
			        	};

			var fileFilter = new FileFilter
			{
				Include = @"*.cs;*.resx",
				Input = items
			};

			IEnumerable<string> l2 = fileFilter.Output;
			Assert.AreEqual(3, l2.Count());
			Assert.IsTrue(l2.Contains(@"c:\items\item1.cs"));
			Assert.IsTrue(l2.Contains(@"c:\items\item2.cs"));
			Assert.IsTrue(l2.Contains(@"c:\items\item1.resx"));
		}

		[Test]
		public void TestFilterIncludes()
		{
			var l = new List<string>
			        	{
			        		@"c:\items\item1.cs",
			        		@"c:\items\item2.cs",
			        		@"c:\items\item1.resx",
			        		@"c:\items\blah.blah",
			        		@"c:\items\project.project"
			        	};

			var fileFilter = new FileFilter
			{
				Include = @"*.cs;*.resx",
				Input = l
			};

			IEnumerable<string> l2 = fileFilter.Output;
			Assert.AreEqual(3, l2.Count());
			Assert.IsTrue(l2.Contains(@"c:\items\item1.cs"));
			Assert.IsTrue(l2.Contains(@"c:\items\item2.cs"));
			Assert.IsTrue(l2.Contains(@"c:\items\item1.resx"));
		}

		[Test]
		public void TestFilterExcludes1()
		{
			var items = new List<string>
			            	{
			            		@"c:\items\item1.cs",
			            		@"c:\items\item2.cs",
			            		@"c:\items\item1.resx",
			            		@"c:\items\blah.blah",
			            		@"c:\items\project.project"
			            	};

			var fileFilter = new FileFilter
			{
				Exclude = @"*.blah;*.project;*.resx",
				Input = items
			};

			IEnumerable<string> l2 = fileFilter.Output;
			Assert.AreEqual(2, l2.Count());
			Assert.IsTrue(l2.Contains(@"c:\items\item1.cs"));
			Assert.IsTrue(l2.Contains(@"c:\items\item2.cs"));
		}

		[Test]
		public void TestFilterWithNoIncludesOrExcludes()
		{
			var items = new List<string>
			                     	{
			                     		@"c:\items\item1.cs",
			                     		@"c:\items\item2.cs",
			                     		@"c:\items\item1.resx",
			                     		@"c:\items\blah.blah",
			                     		@"c:\items\project.project"
			                     	};

			var fileFilter = new FileFilter
			{
				Input = items
			};

			IEnumerable<string> l2 = fileFilter.Output;

			Assert.AreEqual(5, l2.Count());

			Assert.IsTrue(l2.Contains(@"c:\items\item1.cs"));
			Assert.IsTrue(l2.Contains(@"c:\items\item2.cs"));
			Assert.IsTrue(l2.Contains(@"c:\items\item1.resx"));
			Assert.IsTrue(l2.Contains(@"c:\items\blah.blah"));
			Assert.IsTrue(l2.Contains(@"c:\items\project.project"));
		}

		[Test]
		public void TestFilterWithSimilarItems()
		{
			var items = new List<string>
			                     	{
			                     		@"c:\items\item.cs1",
			                     		@"c:\items\item.cs2",
			                     		@"c:\items\item3.cs"
			                     	};

			var fileFilter = new FileFilter
			{
				Input = items,
				Exclude = @"*.cs"
			};

			IEnumerable<string> l2 = fileFilter.Output;
			Assert.AreEqual(2, l2.Count());
			Assert.IsTrue(l2.Contains(@"c:\items\item.cs1"));
			Assert.IsTrue(l2.Contains(@"c:\items\item.cs2"));
		}


		/// <summary>
		/// As in Windows filesystem, *.cs* includes *.cs, *.cs1, *.cs2 etc.
		/// </summary>
		[Test]
		public void TestWildcardExpressions1()
		{
			var items = new List<string>
			            	{
			            		@"c:\items\item.cs1",
			            		@"c:\items\item.cs2",
			            		@"c:\items\item3.cs"
			            	};

			var fileFilter = new FileFilter
			{
				Input = items,
				Include = @"*.cs*"
			};

			IEnumerable<string> l2 = fileFilter.Output;

			Assert.AreEqual(3, l2.Count());
			Assert.IsTrue(l2.Contains(@"c:\items\item.cs1"));
			Assert.IsTrue(l2.Contains(@"c:\items\item.cs2"));
			Assert.IsTrue(l2.Contains(@"c:\items\item3.cs"));
		}

		[Test]
		public void TestWildcardExpressions2()
		{
			var items = new List<string>
			                     	{
			                     		@"c:\items\program1.cs",
			                     		@"c:\items\program2.cs",
			                     		@"c:\items\properties.cs"
			                     	};

			var fileFilter = new FileFilter
			{
				Input = items,
				Include = @"prog*.cs*"
			};

			IEnumerable<string> l2 = fileFilter.Output;
			Assert.AreEqual(2, l2.Count());

			Assert.IsTrue(l2.Contains(@"c:\items\program1.cs"));
			Assert.IsTrue(l2.Contains(@"c:\items\program2.cs"));
		}


		[Test]
		public void TestWildcardExpressions3()
		{
			var l = new List<string>
			                 	{
			                 		@"c:\items\file1.txt",
			                 		@"c:\items\file2.txt",
			                 		@"c:\items\file32.txt"
			                 	};

			var fileFilter = new FileFilter
			{
				Input = l,
				Include = @"file?.txt"
			};

			IEnumerable<string> l2 = fileFilter.Output;
			Assert.AreEqual(2, l2.Count());

			Assert.IsTrue(l2.Contains(@"c:\items\file1.txt"));
			Assert.IsTrue(l2.Contains(@"c:\items\file2.txt"));
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void TestFilterWithNoInput()
		{
			var fileFilter = new FileFilter();
			IEnumerable<string> l2 = fileFilter.Output;
			int n = l2.Count();
			++n;
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void TestFilterWithNullInclude()
		{
			new FileFilter
			{
				Include = null
			};
		}

		[Test]
		public void TestFilterWithNullExclude()
		{
			new FileFilter
			{
				Exclude = null
			};
		}

		[Test]
		public void TestFileFilterExclude()
		{
			var fileFilter = new FileFilter
			{
				Exclude = @"*ext1;*.ext2"
			};

			Assert.AreEqual(@"*ext1;*.ext2", fileFilter.Exclude);
		}

		[Test]
		public void TestFileFilterInclude()
		{
			var fileFilter = new FileFilter
			{
				Include = @"*ext1;*.ext2"
			};

			Assert.AreEqual(@"*ext1;*.ext2", fileFilter.Include);
		}

		[Test]
		public void TestFileFilterInput()
		{
			var fileFilter = new FileFilter();
			var al = new List<string>(new[] { @"item1.cs", @"item2.cs" });
			fileFilter.Input = al;

			Assert.AreEqual(2, fileFilter.Input.Count());
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void TestFileFilterInputWithNull()
		{
			new FileFilter
			{
				Input = null
			};
		}		


	}
}