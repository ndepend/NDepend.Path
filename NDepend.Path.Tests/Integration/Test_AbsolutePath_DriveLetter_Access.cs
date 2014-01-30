using NDepend.Test.Unit;
using NUnit.Framework;
using System.IO;
using NDepend.Test;

namespace NDepend.Path {
   [TestFixture]
   public class Test_AbsolutePath_DriveLetter_Access {


      [SetUp]
      public void SetUp() {
         TestHelper.SetUpTests();
      }

      [Test]
      public void Test_Drive() {
         IAbsoluteDirectoryPath absoluteDirectoryPath = @"C:\Dir1".ToAbsoluteDirectoryPath();
         Assert.IsTrue(absoluteDirectoryPath.DriveLetter.ToString() == "C");
         absoluteDirectoryPath = @"c:\Dir1".ToAbsoluteDirectoryPath();
         Assert.IsTrue(absoluteDirectoryPath.DriveLetter.ToString() == "c");

         IAbsoluteFilePath absoluteFilePath = @"C:\Dir1\File.txt".ToAbsoluteFilePath();
         Assert.IsTrue(absoluteFilePath.DriveLetter.ToString() == "C");
         absoluteFilePath = @"c:\Dir1\File.txt".ToAbsoluteFilePath();
         Assert.IsTrue(absoluteFilePath.DriveLetter.ToString() == "c");
      }



      [Test]
      public void Test_Exist() {
         DirForTest.Delete();
         string dirForTestPath = DirForTest.Dir;
         IAbsoluteDirectoryPath absoluteDirectoryPath = dirForTestPath.ToAbsoluteDirectoryPath();
         Assert.IsFalse(absoluteDirectoryPath.Exists);
         Assert.IsFalse(absoluteDirectoryPath.IsNotNullAndExists());

         DirForTest.EnsureDirForTestExistAndEmpty();
         Assert.IsTrue(absoluteDirectoryPath.Exists);
         Assert.IsTrue(absoluteDirectoryPath.IsNotNullAndExists());

         string dirForTestWithExecutingAssemblyFilePath = DirForTest.ExecutingAssemblyFilePathInDirForTest;
         IAbsoluteFilePath absoluteFilePath = dirForTestWithExecutingAssemblyFilePath.ToAbsoluteFilePath();
         Assert.IsFalse(absoluteFilePath.Exists);

         DirForTest.CopyExecutingAssemblyFileInDirForTest();
         Assert.IsTrue(absoluteFilePath.Exists);
      }


      [Test, ExpectedException(typeof(DirectoryNotFoundException))]
      public void Test_DirDontExist() {
         DirForTest.Delete();
         Assert.IsFalse(DirForTest.DirAbsolute.Exists);
         IAbsoluteDirectoryPath absoluteDirectoryPath = DirForTest.Dir.ToAbsoluteDirectoryPath();
         DirectoryInfo directoryInfo = absoluteDirectoryPath.DirectoryInfo;
      }

      [Test, ExpectedException(typeof(FileNotFoundException))]
      public void Test_FileDontExist() {
         DirForTest.Delete();
         string dirForTestWithExecutingAssemblyFilePath = DirForTest.ExecutingAssemblyFilePathInDirForTest;
         IAbsoluteFilePath absoluteFilePath = dirForTestWithExecutingAssemblyFilePath.ToAbsoluteFilePath();
         FileInfo fileInfo = absoluteFilePath.FileInfo;
      }

      [Test]
      public void Test_DirInfo_FileInfo_ChildrenOfDir() {
         DirForTest.EnsureDirForTestExistAndEmpty();
         string dirForTestPath = DirForTest.Dir;
         IAbsoluteDirectoryPath absoluteDirectoryPath = dirForTestPath.ToAbsoluteDirectoryPath();
         DirectoryInfo directoryInfo = absoluteDirectoryPath.DirectoryInfo;
         Assert.IsTrue(directoryInfo != null);

         DirForTest.CopyExecutingAssemblyFileInDirForTest();
         string dirForTestWithExecutingAssemblyFilePath = DirForTest.ExecutingAssemblyFilePathInDirForTest;
         IAbsoluteFilePath absoluteFilePath = dirForTestWithExecutingAssemblyFilePath.ToAbsoluteFilePath();
         FileInfo fileInfo = absoluteFilePath.FileInfo;
         Assert.IsTrue(fileInfo != null);
      }

      [Test]
      public void Test_ChildrenOfDir() {
         DirForTest.EnsureDirForTestExistAndEmpty();
         string dirForTestPath = DirForTest.Dir;
         var directoryAbsolutePath = dirForTestPath.ToAbsoluteDirectoryPath();
         Assert.IsTrue(directoryAbsolutePath.ChildrenDirectoriesPath.Count == 0);
         Assert.IsTrue(directoryAbsolutePath.ChildrenFilesPath.Count == 0);

         Directory.CreateDirectory( dirForTestPath + System.IO.Path.DirectorySeparatorChar + "Dir1" );
         Directory.CreateDirectory( dirForTestPath + System.IO.Path.DirectorySeparatorChar + "Dir2");
         Assert.IsTrue(directoryAbsolutePath.ChildrenDirectoriesPath.Count == 2);

         DirForTest.CopyExecutingAssemblyFileInDirForTest();
         Assert.IsTrue(directoryAbsolutePath.ChildrenFilesPath.Count == 1);
      }


      [Test]
      public void Test_ChildrenOfDirOfC() {
         var cDrive = DirForTest.Dir.Substring(0, 2);
         Assert.IsTrue(cDrive[1] == ':');
         var cPath = cDrive.ToAbsoluteDirectoryPath();
         Assert.IsTrue(cPath.Exists);
         var children = cPath.ChildrenDirectoriesPath;
         Assert.IsTrue(children.Count > 0);
      }

   }
}
