using System;
using NDepend.Test;
using NUnit.Framework;

namespace NDepend.Path {

   [TestFixture]
   public class Test_UNCPath {

      [SetUp]
      public void SetUp() {
         TestHelper.SetUpTests();
      }



      [TestCase(@"\\server\Share", "server", "Share", @"\\server\Share")]
      [TestCase(@"\\server\Share\filepath", "server", "Share", @"\\server\Share\filepath")]

      [TestCase(@"\\a\b", "a", "b", @"\\a\b")]
      [TestCase(@"\\a\b\c", "a", "b", @"\\a\b\c")]

      [TestCase(@"\\\a\b", "a", "b", @"\\a\b")]
      [TestCase(@"\\\a\b\c", "a", "b", @"\\a\b\c")]

      [TestCase(@"\\\a\\\b", "a", "b", @"\\a\b")]
      [TestCase(@"\//a\b//\c", "a", "b", @"\\a\b\c")]
      public void TestSyntaxUNC_DirOK(string path, string server, string share, string pathNormalized) {
         Assert.IsTrue(path.IsValidAbsoluteDirectoryPath());
         var dirPath = path.ToAbsoluteDirectoryPath();
         Assert.IsTrue(dirPath.Kind == AbsolutePathKind.UNC);
         Assert.IsTrue(dirPath.ToString() == pathNormalized);

         Assert.IsTrue(dirPath.UNCServer == server);
         Assert.IsTrue(dirPath.UNCShare == share);
      }

      [TestCase(@"\\server\"), ExpectedException(typeof (ArgumentException))]
      [TestCase(@"\server\share")]
      [TestCase(@"\\server")]
      [TestCase(@"\\server\\")]
      [TestCase(@"\\server\..")]
      [TestCase(@"\\server\..\aa")]
      [TestCase(@"\\server\.")]
      [TestCase(@"\\server\.\aa")]
      [TestCase(@"\\..\share")]
      [TestCase(@"\\.\share")]
      [TestCase(@"\\..\share\aa")]
      [TestCase(@"\\.\share\aa")]
      [TestCase(@"\")]
      [TestCase(@"\\")]
      [TestCase(@"\\\")]
      [TestCase(@"\\\\")]
      public void TestSyntaxUNC_DirKO(string path) {
         Assert.IsFalse(path.IsValidAbsoluteDirectoryPath());
         Assert.IsFalse(path.IsValidAbsoluteFilePath());
         var dirPath = path.ToAbsoluteDirectoryPath();
      }


      [TestCase(@"\\server\Share\filePath", "server", "Share", "filePath", @"\\server\Share\filePath")]
      [TestCase(@"\\a\b\File.txt", "a", "b", "File.txt", @"\\a\b\File.txt")]
      [TestCase(@"\\\\a\\\b\\\File.txt", "a", "b", "File.txt", @"\\a\b\File.txt")]
      [TestCase(@"\\a\b\c\File.txt", "a", "b", "File.txt", @"\\a\b\c\File.txt")]
      [TestCase(@"\\a\b\c\d\File.txt", "a", "b", "File.txt", @"\\a\b\c\d\File.txt")]
      public void TestSyntaxUNC_FileOK(string path, string server, string share, string fileName, string pathNormalized) {
         Assert.IsTrue(path.IsValidAbsoluteFilePath());
         var filePath = path.ToAbsoluteFilePath();
         Assert.IsTrue(filePath.Kind == AbsolutePathKind.UNC);
         Assert.IsTrue(filePath.ToString() == pathNormalized);
         Assert.IsTrue(filePath.FileName == fileName);

         Assert.IsTrue(filePath.UNCServer == server);
         Assert.IsTrue(filePath.UNCShare == share);
      }

      [TestCase(@"\\server\share"), ExpectedException(typeof (ArgumentException))]
      [TestCase(@"\\a\b")]
      [TestCase(@"\\a\b\c\..")]
      public void TestSyntaxUNC_DirOK_FileKO(string path) {
         Assert.IsTrue(path.IsValidAbsoluteDirectoryPath());
         Assert.IsFalse(path.IsValidAbsoluteFilePath());
         var dirPath = path.ToAbsoluteFilePath();
      }

      //UNC -UNC
      [TestCase(@"\\server\Share\", @"\\server\Share\fileName", true)]
      [TestCase(@"\\server\SHARE\DIR", @"\\SERVER\Share\fileName", true)]
      [TestCase(@"\\server1\Share\", @"\\server\Share\fileName", false)]
      [TestCase(@"\\server\Share\", @"\\server\Share1\fileName", false)]

      // DriveLetter - DriveLetter
      [TestCase(@"C:", @"C:\fileName", true)]
      [TestCase(@"C:\dir", @"C:\d\fileName", true)]
      [TestCase(@"d:", @"C:\fileName", false)]

      //UNC -DriveLetter
      [TestCase(@"C:", @"\\server\Share\fileName", false)]
      [TestCase(@"\\server\Share\", @"C:\fileName", false)]
      public void Test_OnSameVolumeThan(string uncDirPathString, string uncFilePathString, bool bOnSameVolume) {
         var uncDirPath = uncDirPathString.ToAbsoluteDirectoryPath();
         var uncFilePath = uncFilePathString.ToAbsoluteDirectoryPath();
         Assert.IsTrue(uncDirPath.OnSameVolumeThan(uncFilePath) == bOnSameVolume);
      }





      [Test]
      public void Test_DriveLetterPath_HasNoUNCServer() {
         bool b = false;
         try {
            var uncServer = @"C:\Dir".ToAbsoluteFilePath().UNCServer;
         }
         catch (InvalidOperationException ex) {
            b = true;
            Assert.IsTrue(ex.Message == @"The property getter UNCServer must be called on a pathString of kind UNC.");
         }
         Assert.IsTrue(b);
      }



      [Test]
      public void Test_DriveLetterPath_HasNoUNCShare() {
         bool b = false;
         try {
            var uncShare = @"C:\Dir".ToAbsoluteFilePath().UNCShare;
         }
         catch (InvalidOperationException ex) {
            b = true;
            Assert.IsTrue(ex.Message == @"The property getter UNCShare must be called on a pathString of kind UNC.");
         }
         Assert.IsTrue(b);
      }


      [TestCase(@"..\bin", @"\\Server\Share")]
      [TestCase(@"..", @"\\Server\Share")]
      [TestCase(@".\..", @"\\Server\Share")]
      [TestCase(@"..\..", @"\\Server\Share")]
      [TestCase(@"..\..", @"\\Server\Share\Dir1")]
      [TestCase(@"..\..\..", @"\\Server\Share\Dir1\Dir2")]
      public void Test_RelativeAndUNC_KO(string relativePathStr, string absolutePathStr) {
         var relativePath = relativePathStr.ToRelativeDirectoryPath();
         var absolutePath = absolutePathStr.ToAbsoluteDirectoryPath();
         Assert.IsFalse(relativePath.CanGetAbsolutePathFrom(absolutePath));
         bool bThrow = false;
         try {
            relativePath.GetAbsolutePathFrom(absolutePath);
         }
         catch (ArgumentException ex) {
            Assert.IsTrue(ex.Message ==
                          @"Cannot resolve pathTo.TryGetAbsolutePath(pathFrom) because there are too many parent dirs in pathTo:
   PathFrom = """ + absolutePath.ToString() + @"""
   PathTo   = """ + relativePath.ToString() + @"""");
            bThrow = true;
         }
         Assert.IsTrue(bThrow);
      }

      [TestCase(@"..\bin", @"\\Server\Share\Dir", @"\\Server\Share\bin")]
      [TestCase(@"..", @"\\Server\Share\Dir", @"\\Server\Share")]
      [TestCase(@"..\..", @"\\Server\Share\Dir1\Dir2", @"\\Server\Share")]
      [TestCase(@".", @"\\Server\Share", @"\\Server\Share")]
      [TestCase(@"..\..\Dir3\Dir4", @"\\Server\Share\Dir1\Dir2", @"\\Server\Share\Dir3\Dir4")]
      public void Test_RelativeAndUNC_OK(string relativePathStr, string absolutePathStr, string resultPathStr) {
         var relativePath = relativePathStr.ToRelativeDirectoryPath();
         var absolutePath = absolutePathStr.ToAbsoluteDirectoryPath();
         var resultPath = resultPathStr.ToAbsoluteDirectoryPath();

         Assert.IsTrue(relativePath.CanGetAbsolutePathFrom(absolutePath));
         var resultPathTmp = relativePath.GetAbsolutePathFrom(absolutePath);
         Assert.IsTrue(resultPath.Equals(resultPathTmp));

         // Test the iverse operation!
         Assert.IsTrue(resultPath.CanGetRelativePathFrom(absolutePath));
         var relativePathTmp = resultPath.GetRelativePathFrom(absolutePath);
         Assert.IsTrue(relativePathTmp.Equals(relativePath));
      }


      [TestCase(@"\\A\B")]
      [TestCase(@"\\Server\Share")]
      [TestCase(@"\\Server\S")]
      public void Test_UNC_HasParentDirectory(string pathString) {
         var path = pathString.ToAbsoluteDirectoryPath();
         Assert.IsTrue(path.Kind == AbsolutePathKind.UNC);
         Assert.IsFalse(path.HasParentDirectory);

         var bThrown = false;
         try {
            var parent = path.ParentDirectoryPath;
         }
         catch (ArgumentException) {
            bThrown = true;
         }
         Assert.IsTrue(bThrown);
      }
   }
}
