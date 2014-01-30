using System;
using NUnit.Framework;
using NDepend.Test;

namespace NDepend.Path {
   [TestFixture]  
   public class Test_FilePath {
      [SetUp]
      public void SetUp() {
         TestHelper.SetUpTests();
      }


      [TestCase(null, false)]
      [TestCase(@"C:", false)]
      [TestCase(@"C:\Dir1", true)]
      [TestCase(@"C:\Dir1  ", true)]
      [TestCase(@" C:\Dir1", false)]
      [TestCase(@"C:\File.txt", true)]
      [TestCase(@"C:\Dir1\File.txt", true)]
      [TestCase(@"\\", false)]
      [TestCase(@"\\server", false)]
      [TestCase(@"\\server\share\", false)]
      [TestCase(@"\\server\share\Dir1", true)]
      [TestCase(@"\\server\share\Dir1\File.txt", true)]
      [TestCase(@"\\server\share\.", false)]
      [TestCase(@"\\server\share\..", false)]
      [TestCase(@"\\server\share\aa\..", false)]
      [TestCase(@"\\server\share\bb\aa\..", false)]
      [TestCase(@"C:\aa\.", false)]
      [TestCase(@"C:\bb\aa\.", false)]
      [TestCase(@"\\server\share\aa\.", false)]
      [TestCase(@"\\server\share\bb\aa\.", false)]
      [TestCase(@"%E%\dir\file", false)]
      [TestCase(@"%%E%\dir\file", false)]
      [TestCase(@"$(var)\file", false)]
      [TestCase(@"$$(var)\file", false)]
      public void Test_IsValidAbsoluteFilePath(string pathString, bool isValid) {
         Assert.IsTrue(pathString.IsValidAbsoluteFilePath() == isValid);
      }

      [TestCase(null, false)]
      [TestCase(@".", false)]
      [TestCase(@"..", false)]
      [TestCase(@"..\Dir1", true)]
      [TestCase(@"..\File.txt", true)]
      [TestCase(@"..\Dir1\File.txt", true)]
      [TestCase(@".\.", false)]
      [TestCase(@".\..\Dir1\..\..", false)]
      [TestCase(@".\..\..\.", false)]
      [TestCase(@".\..\.", false)]
      [TestCase(@"..\.\..", false)]
      [TestCase(@"%%E%\dir\file", false)]
      [TestCase(@"$$(var)\file", false)]
      public void Test_IsValidRelativeFilePath(string pathString, bool isValid) {
         Assert.IsTrue(pathString.IsValidFilePath() == isValid);
         Assert.IsTrue(pathString.IsValidRelativeFilePath() == isValid);
      }


      [TestCase(null, false)]
      [TestCase(@"cC:\file", false)]
      [TestCase(@"o\\server\share\file", false)]
      [TestCase(@"%E%\dir\file", true)]
      [TestCase(@"%E%\file", true)]
      [TestCase(@"$$(var)\file", false)]
      public void Test_IsValidEnvVarFilePath(string pathString, bool isValid) {
         Assert.IsTrue(pathString.IsValidFilePath() == isValid);
         Assert.IsTrue(pathString.IsValidEnvVarFilePath() == isValid);
      }

      [TestCase(null, false)]
      [TestCase(@"cC:\file", false)]
      [TestCase(@"o\\server\share\file", false)]
      [TestCase(@"%%E%\dir\file", false)]
      [TestCase(@"%%E%\file", false)]
      [TestCase(@"$$(var)\file", false)]
      [TestCase(@"$(var)\file", true)]
      [TestCase(@"$(var)\$(var)\file", true)]
      public void Test_IsValidVariableFilePath(string pathString, bool isValid) {
         Assert.IsTrue(pathString.IsValidFilePath() == isValid);
         Assert.IsTrue(pathString.IsValidVariableFilePath() == isValid);
      }


      [Test,ExpectedException(typeof(ArgumentNullException))]
      public void Test_InvalidInputPathNull() {
         Assert.IsFalse(PathHelpers.IsValidAbsoluteFilePath(null));
         IFilePath filePath = PathHelpers.ToAbsoluteFilePath(null);
      }


      [TestCase(""), ExpectedException(typeof(ArgumentException))]
      [TestCase("C")]
      [TestCase(@"C\File.txt")]
      [TestCase(@"1:\File.txt")]
      [TestCase(@"http://www.NDepend.com/File.txt")]
      [TestCase(@"C:File.txt")]
      [TestCase(@"D:\")]
      [TestCase(@"File.txt")]
      [TestCase(@"C:\..\File.txt")]
      [TestCase(@"C:\..\Dir1\File.txt")]
      [TestCase(@"File.txt")]
      [TestCase(@"File.txt")]
      public void Test_Invalid_IsValidAbsoluteFilePath(string pathString) {
         Assert.IsFalse(pathString.IsValidAbsoluteFilePath());
         IFilePath filePath = pathString.ToAbsoluteFilePath();
      }


      [Test, ExpectedException(typeof(ArgumentNullException))]
      public void Test_InvalidInputRelativePathNull() {
         Assert.IsFalse(PathHelpers.IsValidRelativeFilePath(null));
         IFilePath filePath = PathHelpers.ToRelativeFilePath(null);
      }








      [TestCase(""), ExpectedException(typeof(ArgumentException))]
      [TestCase(@"\Dir1\..\..\File.txt")]
      [TestCase(@"v.\Dir1\..\..\Dir1\File.txt")]
      public void Test_Invalid_IsValidFile(string pathString) {
         Assert.IsFalse(pathString.IsValidFilePath());
         IFilePath filePath = pathString.ToRelativeFilePath();
      }


      [TestCase(@".\File.txt")]
      [TestCase(@".\dir...1\File.txt")]
      [TestCase(@".\Dir1\..\Dir1\File.txt")]
      [TestCase(@"..\Dir1\..\Dir1\File.txt")]
      public void Test_ValidRelativeFilePath(string pathString) {
         Assert.IsTrue(pathString.IsValidFilePath());
         Assert.IsTrue(pathString.IsValidRelativeFilePath());
         IFilePath path = pathString.ToRelativeFilePath();
         Assert.IsTrue(path.IsRelativePath);
      }

      [TestCase(@"C:\File.txt")]
      [TestCase(@"c:\File.txt")]
      [TestCase(@"C:\dir...1\File.txt")]
      public void Test_ValidAbsoluteFilePath(string pathString) {
         Assert.IsTrue(pathString.IsValidFilePath());
         Assert.IsTrue(pathString.IsValidAbsoluteFilePath());
         var path = pathString.ToAbsoluteFilePath();
         Assert.IsTrue(path.IsAbsolutePath);
      }

      [TestCase(@".\Dir1\File.txt")]
      [TestCase(@"C:\Dir1\File.txt")]
      public void Test_ValidFilePath(string pathString) {
         Assert.IsTrue(pathString.IsValidFilePath());
         IFilePath path = pathString.ToFilePath();
      }

      [Test]  
      public void Test_IsValidFile_EmptyKO() {
         string failureReason;
         Assert.IsFalse("".IsValidFilePath(out failureReason));
         Assert.IsTrue(failureReason == @"The parameter pathString is empty.");
      }


      [TestCase(@"cc\")]
      [TestCase(@"v.\File.txt")]
      [TestCase(@"C:\")]
      [TestCase(@".\")]
      [TestCase(@"..\")]
      [TestCase(@"%ENVVAR%\")]
      [TestCase(@"\\machine\share\")]
      public void Test_IsValidFile_KO(string pathString) {
         string failureReason;
         Assert.IsFalse(pathString.IsValidFilePath(out failureReason));
         Assert.IsTrue(failureReason == @"The string """ + pathString + @""" is not a valid file path.");

         bool b = false;
         try {
            pathString.ToFilePath();
         } catch (ArgumentException ex) {
            b = true;
            Assert.IsTrue(ex.Message.Contains(failureReason));
            Assert.IsTrue(ex.Message.StartsWith(@"The parameter pathString is not a valid file path.
" + failureReason));
         }
         Assert.IsTrue(b);
      }


      [TestCase(@".\File.txt", @".\File.txt")]
      [TestCase(@".\\File.txt\\", @".\File.txt")]
      [TestCase(@".\/dir1\//\dir2\/dir3///\File.txt/", @".\dir1\dir2\dir3\File.txt")]
      [TestCase(@"C:/dir1/dir2/\File.txt", @"C:\dir1\dir2\File.txt")]
      public void Test_NormalizePath(string pathIn, string pathNormalized) {
         //Assert.IsTrue(pathIn.IsValidFilePath());
         Assert.IsTrue(pathNormalized.IsValidFilePath());
         IFilePath path = pathIn.ToFilePath();
         Assert.IsTrue(path.ToString() == pathNormalized);
      }

      [Test]
      public void Test_ParentDirectoryPath() {
         IDirectoryPath path = @".\File.txt".ToRelativeFilePath().ParentDirectoryPath;
         Assert.IsTrue(path.ToString() == @".");

         path = @"C:\File.txt".ToAbsoluteFilePath().ParentDirectoryPath;
         Assert.IsTrue(path.ToString() == @"C:");

         path = @".\\File.txt".ToRelativeFilePath().ParentDirectoryPath;
         Assert.IsTrue(path.ToString() == @".");

         path = @"C:\\\\File.txt".ToAbsoluteFilePath().ParentDirectoryPath;
         Assert.IsTrue(path.ToString() == @"C:");

         path = @"C:\dir1\\//dir2\File.txt".ToAbsoluteFilePath().ParentDirectoryPath;
         Assert.IsTrue(path.ToString() == @"C:\dir1\dir2");
      }


      [TestCase(@".\File.txt  ", @".\File.txt")]
      [TestCase(@"C:\File.txt     ", @"C:\File.txt")]
      [TestCase(@"C:\Dir\File.txt    ", @"C:\Dir\File.txt")]
      [TestCase(@"%JJ%\File.txt  ", @"%JJ%\File.txt")]
      public void Test_PathNormalizationTrimEnd(string path, string pathNormalized) {
         Assert.IsTrue(path.IsValidFilePath());
         Assert.IsTrue(path.ToFilePath().ToString() == pathNormalized);
         Assert.IsTrue(path.ToFilePath().FileName == "File.txt");
      }

      [Test]
      public void Test_FileName() {
         string fileName = @".\File.txt".ToRelativeFilePath().FileName;
         Assert.IsTrue(fileName == @"File.txt");

         fileName = @"C:\File.txt".ToAbsoluteFilePath().FileName;
         Assert.IsTrue(fileName == @"File.txt");

         fileName = @".\\File.txt".ToRelativeFilePath().FileName;
         Assert.IsTrue(fileName == @"File.txt");

         fileName = @"C:\\\\File.txt".ToAbsoluteFilePath().FileName;
         Assert.IsTrue(fileName == @"File.txt");

         fileName = @"C:\dir1\\//dir2\File.txt".ToAbsoluteFilePath().FileName;
         Assert.IsTrue(fileName == @"File.txt");
      }


      [Test]
      public void Test_FileExtension() {
         IFilePath filePath = @".\File.txt".ToRelativeFilePath();
         Assert.IsTrue(filePath.FileExtension == @".txt");
         Assert.IsTrue(filePath.HasExtension(@".txt"));
         Assert.IsTrue(filePath.HasExtension(@".TxT"));
         Assert.IsTrue(filePath.HasExtension(@".TXT"));


         filePath = @"%E%\File.txt".ToEnvVarFilePath();
         Assert.IsTrue(filePath.FileExtension == @".txt");
         Assert.IsTrue(filePath.HasExtension(@".txt"));
         Assert.IsTrue(filePath.HasExtension(@".TxT"));
         Assert.IsTrue(filePath.HasExtension(@".TXT"));

         filePath = @".\File".ToRelativeFilePath();
         Assert.IsTrue(filePath.FileExtension == string.Empty);

         filePath = @".\File.".ToRelativeFilePath();
         Assert.IsTrue(filePath.FileExtension == string.Empty);

         filePath = @"C:\dir1\\//dir2\File.txt.Exe".ToAbsoluteFilePath();
         Assert.IsTrue(filePath.FileExtension == @".Exe");
         Assert.IsTrue(filePath.HasExtension(@".exe"));
      }

      [Test]
      public void Test_FileNameWithoutExtension() {
         IFilePath filePath = @".\File.txt".ToRelativeFilePath();
         Assert.IsTrue(filePath.FileNameWithoutExtension == "File" );

         filePath = @".\File".ToRelativeFilePath();
         Assert.IsTrue(filePath.FileNameWithoutExtension == "File");

         filePath = @".\File.tmp.exe".ToRelativeFilePath();
         Assert.IsTrue(filePath.FileNameWithoutExtension == "File.tmp");
      }


      //
      // PathMode
      //
      [TestCase(@".\File", PathMode.Relative)]
      [TestCase(@".\dir...1\File", PathMode.Relative)]
      [TestCase(@"C:\File", PathMode.Absolute)]
      [TestCase(@"C:\dir...1\File", PathMode.Absolute)]
      [TestCase(@"\\server\share\File", PathMode.Absolute)]
      [TestCase(@"\\server\share\dir...1\File", PathMode.Absolute)]
      [TestCase(@"%EV%\File", PathMode.EnvVar)]
      [TestCase(@"%EV%\dir...1\File", PathMode.EnvVar)]
      [TestCase(@"$(var)\File", PathMode.Variable)]
      [TestCase(@"$(var)\dir...1\File", PathMode.Variable)]
      public void Test_PathModeOk(string pathString, PathMode pathMode) {
         IFilePath path = pathString.ToFilePath();
         Assert.IsTrue(path.IsFilePath);
         Assert.IsFalse(path.IsDirectoryPath);
         Assert.IsTrue(path.PathMode == pathMode);
         Assert.IsTrue(path.IsAbsolutePath == (pathMode == PathMode.Absolute));
         Assert.IsTrue(path.IsRelativePath == (pathMode == PathMode.Relative));
         Assert.IsTrue(path.IsVariablePath == (pathMode == PathMode.Variable));
         Assert.IsTrue(path.IsEnvVarPath == (pathMode == PathMode.EnvVar));
      }


      //
      //  TestInnerSpecialDir
      //
      [Test]
      public void Test_InnerSpecialDir1() {
         Assert.IsTrue(@"C:\Dir1\..\File.txt".ToAbsoluteFilePath().ToString() == @"C:\File.txt");
         Assert.IsTrue(@"C:\Dir1\..\Dir2\..\File.txt".ToAbsoluteFilePath().ToString() == @"C:\File.txt");
         Assert.IsTrue(@"C:\Dir1\..\Dir2\..\.\File.txt".ToAbsoluteFilePath().ToString() == @"C:\File.txt");
         Assert.IsTrue(@"C:\.\Dir1\..\File.txt".ToAbsoluteFilePath().ToString() == @"C:\File.txt");
         Assert.IsTrue(@"C:\.\Dir1\Dir2\Dir3\..\..\..\File.txt".ToAbsoluteFilePath().ToString() == @"C:\File.txt");

         Assert.IsTrue(@".\Dir1\..\File.txt".ToRelativeFilePath().ToString() == @".\File.txt");
         Assert.IsTrue(@".\Dir1\..\Dir2\..\File.txt".ToRelativeFilePath().ToString() == @".\File.txt");
         Assert.IsTrue(@".\Dir1\..\Dir2\..\.\File.txt".ToRelativeFilePath().ToString() == @".\File.txt");
         Assert.IsTrue(@".\.\Dir1\..\File.txt".ToRelativeFilePath().ToString() == @".\File.txt");
         Assert.IsTrue(@".\.\Dir1\Dir2\Dir3\..\..\..\File.txt".ToRelativeFilePath().ToString() == @".\File.txt");

         Assert.IsTrue(@"C:\Dir1\..\".ToAbsoluteDirectoryPath().ToString() == @"C:");
         Assert.IsTrue(@"C:\Dir1\..\Dir2\..\".ToAbsoluteDirectoryPath().ToString() == @"C:");
         Assert.IsTrue(@"C:\Dir1\..\Dir2\..\.\".ToAbsoluteDirectoryPath().ToString() == @"C:");
         Assert.IsTrue(@"C:\.\Dir1\..\".ToAbsoluteDirectoryPath().ToString() == @"C:");
         Assert.IsTrue(@"C:\.\Dir1\Dir2\Dir3\..\..\..\".ToAbsoluteDirectoryPath().ToString() == @"C:");

         Assert.IsTrue(@".\Dir1\..\File.txt".ToRelativeDirectoryPath().ToString() == @".\File.txt");
         Assert.IsTrue(@".\Dir1\..\Dir2\..\File.txt".ToRelativeDirectoryPath().ToString() == @".\File.txt");
         Assert.IsTrue(@".\Dir1\..\Dir2\..\.\File.txt".ToRelativeDirectoryPath().ToString() == @".\File.txt");
         Assert.IsTrue(@".\.\Dir1\..\File.txt".ToRelativeDirectoryPath().ToString() == @".\File.txt");
         Assert.IsTrue(@".\.\Dir1\Dir2\Dir3\..\..\..\File.txt".ToRelativeFilePath().ToString() == @".\File.txt");

         Assert.IsTrue(@".\.".ToRelativeDirectoryPath().ToString() == @".");
         Assert.IsTrue(@"./.\./.\".ToRelativeDirectoryPath().ToString() == @".");

         Assert.IsTrue(@".\..\Dir1\..".ToRelativeDirectoryPath().ToString() == @"..");
         Assert.IsTrue(@".\Dir1\Dir2\..\..".ToRelativeDirectoryPath().ToString() == @".");
         Assert.IsTrue(@".\.\.\Dir1\Dir2\..\..".ToRelativeDirectoryPath().ToString() == @".");
         Assert.IsTrue(@".\.\.\..\..".ToRelativeDirectoryPath().ToString() == @".\..\..");
         Assert.IsTrue(@"..\.\..\Dir1".ToRelativeDirectoryPath().ToString() == @"..\..\Dir1");
         Assert.IsTrue(@"..\Dir1\.\..\Dir2\".ToRelativeDirectoryPath().ToString() == @"..\Dir2");
         

      }



      // @"The path {" + path + @"} references the parent dir \..\ of the root dir {" + pathDirs[0] + "}, it cannot be resolved.";
      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_InnerSpecialDir_Error20() {
         var path = @"C:\..".ToAbsoluteDirectoryPath();
      }
      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_InnerSpecialDir_Error21() {
         var path = @"C:\Dir1\..\..".ToAbsoluteDirectoryPath();
      }
      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_InnerSpecialDir_Error22() {
         var path = @"C:\Dir1\..\..".ToAbsoluteDirectoryPath();
      }





      //
      //  GetRelativePath
      //
      [Test]
      public void Test_GetRelativePath() {
         IAbsoluteFilePath filePathTo;
         IAbsoluteDirectoryPath absoluteDirectoryPathFrom;
;

         filePathTo = @"C:\Dir1\File.txt".ToAbsoluteFilePath();
         absoluteDirectoryPathFrom = @"C:\Dir1".ToAbsoluteDirectoryPath();
         Assert.IsTrue(filePathTo.GetRelativePathFrom(absoluteDirectoryPathFrom).ToString() == @".\File.txt");
         Assert.IsTrue(filePathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom));

         filePathTo = @"C:\Dir1\Dir2\File.txt".ToAbsoluteFilePath();
         absoluteDirectoryPathFrom = @"C:\Dir1\Dir3".ToAbsoluteDirectoryPath();
         Assert.IsTrue(filePathTo.GetRelativePathFrom(absoluteDirectoryPathFrom).ToString() == @"..\Dir2\File.txt");
         Assert.IsTrue(filePathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom));

         filePathTo = @"C:\Dir1\File.txt".ToAbsoluteFilePath();
         absoluteDirectoryPathFrom = @"C:\Dir2".ToAbsoluteDirectoryPath();
         Assert.IsTrue(filePathTo.GetRelativePathFrom(absoluteDirectoryPathFrom).ToString() == @"..\Dir1\File.txt");
         Assert.IsTrue(filePathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom));

         filePathTo = @"C:\Dir1\Dir2\File.txt".ToAbsoluteFilePath();
         absoluteDirectoryPathFrom = @"C:\Dir1".ToAbsoluteDirectoryPath();

         Assert.IsTrue(filePathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom));
         string failureReason;
         Assert.IsTrue(filePathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom, out failureReason));
         Assert.IsNull(failureReason);

         Assert.IsTrue(filePathTo.GetRelativePathFrom(absoluteDirectoryPathFrom).ToString() == @".\Dir2\File.txt");
         Assert.IsTrue((filePathTo as IAbsolutePath).GetRelativePathFrom(absoluteDirectoryPathFrom).ToString() == @".\Dir2\File.txt");
      }


      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_GetRelativePathWithError3() {
         IAbsoluteFilePath filePathTo = @"C:\Dir1\File.txt".ToAbsoluteFilePath();
         IAbsoluteDirectoryPath absoluteDirectoryPathFrom = @"D:\Dir1".ToAbsoluteDirectoryPath();
         Assert.IsFalse(filePathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom));
         string failureReason;
         Assert.IsFalse(filePathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom, out failureReason));
         Assert.IsTrue(failureReason == @"Cannot compute relative path from 2 paths that are not on the same volume 
   PathFrom = ""D:\Dir1""
   PathTo   = ""C:\Dir1\File.txt""");
         filePathTo.GetRelativePathFrom(absoluteDirectoryPathFrom);
      }




      //
      //  GetAbsolutePath
      //
      [Test, ExpectedException(typeof(ArgumentNullException))]
      public void Test_GetAbsolutePath_ArgumentNullException() {
         IRelativeFilePath p = @"..\File.txt".ToRelativeFilePath();
         var x = p.GetAbsolutePathFrom((null as string).ToAbsoluteDirectoryPath());
      }


      [Test]
      public void Test_GetAbsolutePath() {
         IRelativeFilePath filePathTo;
         IAbsoluteDirectoryPath absoluteDirectoryPathFrom;

         filePathTo = @"..\File.txt".ToRelativeFilePath();
         absoluteDirectoryPathFrom = @"C:\Dir1".ToAbsoluteDirectoryPath();
         Assert.IsTrue(filePathTo.GetAbsolutePathFrom(absoluteDirectoryPathFrom).ToString() == @"C:\File.txt");
         Assert.IsTrue(filePathTo.CanGetAbsolutePathFrom(absoluteDirectoryPathFrom));

         filePathTo = @".\File.txt".ToRelativeFilePath();
         absoluteDirectoryPathFrom = @"C:\Dir1".ToAbsoluteDirectoryPath();
         Assert.IsTrue(filePathTo.GetAbsolutePathFrom(absoluteDirectoryPathFrom).ToString() == @"C:\Dir1\File.txt");
         Assert.IsTrue(filePathTo.CanGetAbsolutePathFrom(absoluteDirectoryPathFrom));

         filePathTo = @"..\Dir2\File.txt".ToRelativeFilePath();
         absoluteDirectoryPathFrom = @"C:\Dir1".ToAbsoluteDirectoryPath();
         Assert.IsTrue(filePathTo.GetAbsolutePathFrom(absoluteDirectoryPathFrom).ToString() == @"C:\Dir2\File.txt");
         Assert.IsTrue(filePathTo.CanGetAbsolutePathFrom(absoluteDirectoryPathFrom));

         filePathTo = @"..\..\Dir4\Dir5\File.txt".ToRelativeFilePath();
         absoluteDirectoryPathFrom = @"C:\Dir1\Dir2\Dir3".ToAbsoluteDirectoryPath();
         Assert.IsTrue(filePathTo.GetAbsolutePathFrom(absoluteDirectoryPathFrom).ToString() == @"C:\Dir1\Dir4\Dir5\File.txt");
         Assert.IsTrue(filePathTo.CanGetAbsolutePathFrom(absoluteDirectoryPathFrom));

         filePathTo = @".\..\Dir4\Dir5\File.txt".ToRelativeFilePath();
         absoluteDirectoryPathFrom = @"C:\Dir1\Dir2\Dir3".ToAbsoluteDirectoryPath();
         
         Assert.IsTrue(filePathTo.CanGetAbsolutePathFrom(absoluteDirectoryPathFrom));
         string failureReason;
         Assert.IsTrue(filePathTo.CanGetAbsolutePathFrom(absoluteDirectoryPathFrom, out failureReason));
         Assert.IsNull(failureReason);

         Assert.IsTrue(filePathTo.GetAbsolutePathFrom(absoluteDirectoryPathFrom).ToString() == @"C:\Dir1\Dir2\Dir4\Dir5\File.txt");
         Assert.IsTrue((filePathTo as IRelativePath).GetAbsolutePathFrom(absoluteDirectoryPathFrom).ToString() == @"C:\Dir1\Dir2\Dir4\Dir5\File.txt");
         
      }


      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_GetAbsolutePathPathWithError3() {
         IRelativeFilePath directoryPathTo = @"..\..\Dir1\File.txt".ToRelativeFilePath();
         IAbsoluteDirectoryPath absoluteDirectoryPathFrom = @"C:\Dir1".ToAbsoluteDirectoryPath();
         Assert.IsFalse(directoryPathTo.CanGetAbsolutePathFrom(absoluteDirectoryPathFrom));
         string failureReason;
         Assert.IsFalse(directoryPathTo.CanGetAbsolutePathFrom(absoluteDirectoryPathFrom, out failureReason));
         Assert.IsTrue(failureReason == @"Cannot resolve pathTo.TryGetAbsolutePath(pathFrom) because there are too many parent dirs in pathTo:
   PathFrom = ""C:\Dir1""
   PathTo   = ""..\..\Dir1""");
         directoryPathTo.GetAbsolutePathFrom(absoluteDirectoryPathFrom);
      }




      //
      //  PathComparison
      //
      [Test]
      public void Test_PathEquality() {
         //
         // RelativeFilePath 
         //
         IRelativeFilePath relativeFilePath1 = @"..\Dir1\File.txt".ToRelativeFilePath();
         IRelativeFilePath relativeFilePath2 = @"..\\dir1//file.TXT".ToRelativeFilePath();
         Assert.IsTrue(relativeFilePath1.Equals(relativeFilePath2));

         relativeFilePath1 = @"..\Dir1\File.txt".ToRelativeFilePath();
         relativeFilePath2 = @".\Dir1\File.txt".ToRelativeFilePath();
         Assert.IsFalse(relativeFilePath1.Equals(relativeFilePath2));

         relativeFilePath1 = @"..\Dir1\File.txt".ToRelativeFilePath();
         relativeFilePath2 = @"..\Dir1\Dir2\File.txt".ToRelativeFilePath();
         Assert.IsFalse(relativeFilePath1.Equals(relativeFilePath2));

         relativeFilePath1 = @"..\Dir1\File.txt".ToRelativeFilePath();
         relativeFilePath2 = @"..\Dir1\File.tx".ToRelativeFilePath();
         Assert.IsFalse(relativeFilePath1.Equals(relativeFilePath2));

         //
         // AbsoluteFilePath 
         //
         IAbsoluteFilePath absoluteAbsoluteFilePath1 = @"C:\Dir1\File.txt".ToAbsoluteFilePath();
         IAbsoluteFilePath absoluteAbsoluteFilePath2 = @"C:\\dir1//file.TXT".ToAbsoluteFilePath();
         Assert.IsTrue(absoluteAbsoluteFilePath1.Equals(absoluteAbsoluteFilePath2));

         absoluteAbsoluteFilePath1 = @"C:\Dir1\File.txt".ToAbsoluteFilePath();
         absoluteAbsoluteFilePath2 = @"D:\Dir1\File.txt".ToAbsoluteFilePath();
         Assert.IsFalse(absoluteAbsoluteFilePath1.Equals(absoluteAbsoluteFilePath2));

         absoluteAbsoluteFilePath1 = @"C:\Dir1\File.txt".ToAbsoluteFilePath();
         absoluteAbsoluteFilePath2 = @"C:\Dir1\Dir2\File.txt".ToAbsoluteFilePath();
         Assert.IsFalse(absoluteAbsoluteFilePath1.Equals(absoluteAbsoluteFilePath2));

         absoluteAbsoluteFilePath1 = @"C:\Dir1\File.txt".ToAbsoluteFilePath();
         absoluteAbsoluteFilePath2 = @"C:\Dir1\File.tx".ToAbsoluteFilePath();
         Assert.IsFalse(absoluteAbsoluteFilePath1.Equals(absoluteAbsoluteFilePath2));

         //
         // Mix between AbsoluteFilePath and RelativeFilePath
         //
         relativeFilePath1 = @"..\Dir1\File.txt".ToRelativeFilePath();
         absoluteAbsoluteFilePath1 = @"C:\Dir1\File.txt".ToAbsoluteFilePath();
         Assert.IsFalse(absoluteAbsoluteFilePath1.Equals(relativeFilePath1));

         //
         // Mix between absoluteDirectoryPath and filePath
         //
         IAbsoluteDirectoryPath absoluteAbsoluteDirectoryPath1 = @"C:\Dir1\File".ToAbsoluteDirectoryPath();
         absoluteAbsoluteFilePath1 = @"C:\Dir1\File".ToAbsoluteFilePath();
         Assert.IsFalse(absoluteAbsoluteDirectoryPath1.Equals(absoluteAbsoluteFilePath1));
         Assert.IsFalse(absoluteAbsoluteFilePath1.Equals(absoluteAbsoluteDirectoryPath1));

         IRelativeDirectoryPath relativeRelativeDirectoryPath1 = @"..\Dir1\File".ToRelativeDirectoryPath();
         relativeFilePath1 = @"..\Dir1\File".ToRelativeFilePath();
         Assert.IsFalse(relativeRelativeDirectoryPath1.Equals(relativeFilePath1));
         Assert.IsFalse(relativeFilePath1.Equals(relativeRelativeDirectoryPath1));
      }





      //
      //  GetBrother
      //


      [Test]
      public void Test_GetBrotherWithName1() {
         Assert.IsTrue(@"C:\Dir1\File.txt".ToAbsoluteFilePath().GetBrotherDirectoryWithName("Dir3").Equals(
            @"C:\Dir1\Dir3".ToAbsoluteDirectoryPath()));
      }
      [Test]
      public void Test_GetBrotherWithName2() {
         Assert.IsTrue(@"C:\Dir1\File.txt".ToFilePath().GetBrotherDirectoryWithName("Dir3").Equals(
            @"C:\Dir1\Dir3".ToDirectoryPath()));
      }
      [Test]
      public void Test_GetBrotherWithName3() {
         Assert.IsTrue(@"\\server\share\Dir1\File.txt".ToAbsoluteFilePath().GetBrotherDirectoryWithName("Dir3").Equals(
            @"\\server\share\Dir1\Dir3".ToAbsoluteDirectoryPath()));
      }
      [Test]
      public void Test_GetBrotherWithName4() {
         Assert.IsTrue(@"\\server\share\Dir1\File.txt".ToFilePath().GetBrotherDirectoryWithName("Dir3").Equals(
         @"\\server\share\Dir1\Dir3".ToDirectoryPath()));
      }
      [Test]
      public void Test_GetBrotherWithName5() {
         Assert.IsTrue(@"..\Dir1\File.txt".ToRelativeFilePath().GetBrotherDirectoryWithName("Dir3").Equals(
            @"..\Dir1\Dir3".ToRelativeDirectoryPath()));
      }
      [Test]
      public void Test_GetBrotherWithName6() {
         Assert.IsTrue(@"..\Dir1\File.txt".ToFilePath().GetBrotherDirectoryWithName("Dir3").Equals(
            @"..\Dir1\Dir3".ToDirectoryPath()));
      }
      [Test]
      public void Test_GetBrotherWithName7() {
         Assert.IsTrue(@"C:\Dir1\File.txt".ToAbsoluteFilePath().GetBrotherFileWithName("File.exe").Equals(
            @"C:\Dir1\File.exe".ToAbsoluteFilePath()));
      }
      [Test]
      public void Test_GetBrotherWithName8() {
         Assert.IsTrue(@"C:\Dir1\File.txt".ToFilePath().GetBrotherFileWithName("File.exe").Equals(
            @"C:\Dir1\File.exe".ToFilePath()));
      }
      [Test]
      public void Test_GetBrotherWithName9() {
         Assert.IsTrue(@"..\Dir1\File.txt".ToRelativeFilePath().GetBrotherFileWithName("File.exe").Equals(
            @"..\Dir1\File.exe".ToRelativeFilePath()));
      }
      [Test]
      public void Test_GetBrotherWithName10() {
         Assert.IsTrue(@"..\Dir1\File.txt".ToFilePath().GetBrotherFileWithName("File.exe").Equals(
            @"..\Dir1\File.exe".ToFilePath()));
      }



      //
      //  Update Extension
      //
      [Test]
      public void Test_UpdateExtension1() {
         Assert.IsTrue(@"..\Dir1\File.txt".ToRelativeFilePath().UpdateExtension(".exe").Equals(
            @"..\Dir1\File.exe".ToRelativeFilePath()));
      }
      [Test]
      public void Test_UpdateExtension2() { 
         Assert.IsTrue(@"..\Dir1\File.txt".ToFilePath().UpdateExtension(".exe").Equals(
            @"..\Dir1\File.exe".ToFilePath()));
      }
      [Test]
      public void Test_UpdateExtension3() {
         Assert.IsTrue(@"..\Dir1\File".ToRelativeFilePath().UpdateExtension(".exe").Equals(
            @"..\Dir1\File.exe".ToRelativeFilePath()));
      }
      [Test]
      public void Test_UpdateExtension4() {
         Assert.IsTrue(@"..\Dir1\File".ToFilePath().UpdateExtension(".exe").Equals(
            @"..\Dir1\File.exe".ToFilePath()));
      }
      [Test]
      public void Test_UpdateExtension5() {
         Assert.IsTrue(@"..\Dir1\File.txt.bmp".ToRelativeFilePath().UpdateExtension(".exe").Equals(
            @"..\Dir1\File.txt.exe".ToRelativeFilePath()));
      }
      [Test]
      public void Test_UpdateExtension6() {
         Assert.IsTrue(@"..\Dir1\File.txt.bmp".ToFilePath().UpdateExtension(".exe").Equals(
            @"..\Dir1\File.txt.exe".ToFilePath()));
      }
      [Test]
      public void Test_UpdateExtension7() {         
         Assert.IsTrue(@"..\Dir1\File".ToRelativeFilePath().UpdateExtension(".exe").Equals(
            @"..\Dir1\File.exe".ToRelativeFilePath()));
      }
      [Test]
      public void Test_UpdateExtension8() {
         Assert.IsTrue(@"..\Dir1\File".ToFilePath().UpdateExtension(".exe").Equals(
            @"..\Dir1\File.exe".ToFilePath()));
      }
      [Test]
      public void Test_UpdateExtension9() {
         Assert.IsTrue(@"C:\Dir1\File.txt".ToAbsoluteFilePath().UpdateExtension(".exe").Equals(
            @"C:\Dir1\File.exe".ToAbsoluteFilePath()));
      }
      [Test]
      public void Test_UpdateExtension10() {
         Assert.IsTrue(@"C:\Dir1\File.txt".ToFilePath().UpdateExtension(".exe").Equals(
            @"C:\Dir1\File.exe".ToFilePath()));
      }
      [Test]
      public void Test_UpdateExtension11() {
         Assert.IsTrue(@"\\server\share\Dir1\File.txt".ToAbsoluteFilePath().UpdateExtension(".exe").Equals(
            @"\\server\share\Dir1\File.exe".ToAbsoluteFilePath()));
      }
      [Test]
      public void Test_UpdateExtension12() {
         Assert.IsTrue(@"\\server\share\Dir1\File.txt".ToFilePath().UpdateExtension(".exe").Equals(
            @"\\server\share\Dir1\File.exe".ToFilePath()));
      }
      [Test]
      public void Test_UpdateExtension13() {        
         Assert.IsTrue(@"C:\Dir1\File".ToAbsoluteFilePath().UpdateExtension(".exe").Equals(
            @"C:\Dir1\File.exe".ToAbsoluteFilePath()));
      }
      [Test]
      public void Test_UpdateExtension14() {
         Assert.IsTrue(@"C:\Dir1\File".ToFilePath().UpdateExtension(".exe").Equals(
            @"C:\Dir1\File.exe".ToFilePath()));
      }
      [Test]
      public void Test_UpdateExtension15() {         
         Assert.IsTrue(@"C:\Dir1\File.txt.bmp".ToAbsoluteFilePath().UpdateExtension(".exe").Equals(
            @"C:\Dir1\File.txt.exe".ToAbsoluteFilePath()));
      }
      [Test]
      public void Test_UpdateExtension16() {
         Assert.IsTrue(@"C:\Dir1\File.txt.bmp".ToFilePath().UpdateExtension(".exe").Equals(
            @"C:\Dir1\File.txt.exe".ToFilePath()));
      }
      [Test]
      public void Test_UpdateExtension17() {
         Assert.IsTrue(@"C:\Dir1\File".ToAbsoluteFilePath().UpdateExtension(".exe").Equals(
            @"C:\Dir1\File.exe".ToAbsoluteFilePath()));
      }
      [Test]
      public void Test_UpdateExtension18() {
         Assert.IsTrue(@"C:\Dir1\File".ToFilePath().UpdateExtension(".exe").Equals(
            @"C:\Dir1\File.exe".ToFilePath()));
      }

      [Test]
      public void Test_UpdateExtension19() {
         Assert.IsTrue(@"%E%\Dir1\File".ToEnvVarFilePath().UpdateExtension(".exe").Equals(
            @"%E%\Dir1\File.exe".ToEnvVarFilePath()));
      }
      [Test]
      public void Test_UpdateExtension20() {
         Assert.IsTrue(@"%E%\Dir1\File".ToFilePath().UpdateExtension(".exe").Equals(
           @"%E%\Dir1\File.exe".ToFilePath()));
      }

      [Test]
      public void Test_ExplicitConversionFromPathToString() {
         Assert.IsTrue(@"..\Dir1\File.txt".ToRelativeFilePath().ToString() == @"..\Dir1\File.txt");
      }

   }
}
