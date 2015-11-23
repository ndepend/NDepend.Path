using System;
using NUnit.Framework;
using NDepend.Test;


namespace NDepend.Path {
   [TestFixture]
   public class Test_DirectoryPath {


      [SetUp]
      public void SetUp() {
         TestHelper.SetUpTests();
      }
      /*[Test]
      public void Test_ToString_ans_PlusForbiddenOperator() {
         // Deactivate temporarily Debug.Assert()
         System.Diagnostics.DefaultTraceListener listener = (System.Diagnostics.DefaultTraceListener)System.Diagnostics.Trace.Listeners[0];
         listener.AssertUiEnabled = false;
         IAbsoluteFilePath path = new IAbsoluteFilePath(@"C:\File.txt");
         Assert.IsTrue(path.ToString() == @"C:\File.txt");
         Assert.IsTrue(path.ToString() + "hello" == path.ToString());
         listener.AssertUiEnabled = true;
      }*/


      [TestCase((string)null, false)]
      [TestCase("", false)]
      [TestCase("/", false)]
      [TestCase(@"\", false)]
      [TestCase(@"C:\Lib/", true)]
      [TestCase(@"C:\Lib   ", true)]
      [TestCase(@" C:\Lib", false)]
      public void Test_IsValidAbsoluteDirectoryPath(string str, bool bOk) {
         Assert.IsTrue(str.IsValidAbsoluteDirectoryPath() == bOk);
      }


      [TestCase((string)null, false)]
      [TestCase("", false)]
      [TestCase("/", false)]
      [TestCase(@".\", true)]
      [TestCase(@"C:", true)]
      [TestCase(@"%E%", true)]
      [TestCase(@"$(var)", true)]
      [TestCase(@"C:\Lib   ", true)]
      [TestCase(@" C:\Lib", false)]
      public void Test_IsValidDirectoryPath(string str, bool bOk) {
         Assert.IsTrue(str.IsValidDirectoryPath() == bOk);
      }



      [Test]
      public void Test_IsValidDirectoryPath_KO1() {
         string reason;
         Assert.IsFalse("/".IsValidDirectoryPath(out reason));
         Assert.IsTrue(reason == @"The parameter pathString normalized is empty.");
      }

      [Test]
      public void Test_IsValidDirectoryPath_KO2() {
         const string pathString = @"CC\Dir"; 
         string reason;
         Assert.IsFalse(pathString.IsValidDirectoryPath(out reason));
         Assert.IsTrue(reason == @"The string ""CC\Dir"" is not a valid directory path.");

         bool b = false;
         try {
            pathString.ToDirectoryPath();
         }
         catch (ArgumentException ex) {
            b = true;
            Assert.IsTrue(ex.Message.Contains(reason));
            Assert.IsTrue(ex.Message.StartsWith(@"The parameter pathString is not a valid directory path.
"));
         }
         Assert.IsTrue(b);

      }



      [TestCase(@"C:\Dir1", PathMode.Absolute)]
      [TestCase(@"\\Server\Share\Dir1", PathMode.Absolute)]
      [TestCase(@"..\Dir1", PathMode.Relative)]
      [TestCase(@"$(VAR)\Dir1", PathMode.Variable)]
      [TestCase(@"%ENVVAR%\Dir1", PathMode.EnvVar)]
      public void Test_DoesPathHasThisPathMode(string str, PathMode pathMode) {
         Assert.IsTrue(str.ToDirectoryPath().PathMode == pathMode);
         Assert.IsTrue(str.ToFilePath().PathMode == pathMode);
      }



      [Test, ExpectedException(typeof(ArgumentNullException))]
      public void Test_InvalidInputPathNull() {
         IDirectoryPath directoryPath = PathHelpers.ToAbsoluteDirectoryPath(null);
      }
      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_InvalidInputPathEmpty() {
         Assert.IsFalse((null as string).IsValidAbsoluteDirectoryPath());
         Assert.IsFalse((null as string).IsValidRelativeDirectoryPath());
         Assert.IsFalse(string.Empty.IsValidAbsoluteDirectoryPath());
         Assert.IsFalse(string.Empty.IsValidRelativeDirectoryPath());
         IDirectoryPath directoryPath = string.Empty.ToAbsoluteDirectoryPath();
      }

      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_InvalidInputAbsolutePath1() {
         Assert.IsFalse("C".IsValidAbsoluteDirectoryPath());
         IDirectoryPath directoryPath = "C".ToAbsoluteDirectoryPath();
      }


      // these 2 tests, test the need for \ like c:\a instead of c:a
      public void Test_ValidInputAbsolutePath1_() {
         Assert.IsTrue("C:".IsValidAbsoluteDirectoryPath());
         IDirectoryPath directoryPath = "C:".ToAbsoluteDirectoryPath();
      }

      public void Test_ValidInputAbsolutePath1__() {
         Assert.IsTrue("C:\a".IsValidAbsoluteDirectoryPath());
         IDirectoryPath directoryPath = "C:\a".ToAbsoluteDirectoryPath();
      }

      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_InvalidInputAbsolutePath1_() {
         Assert.IsFalse("C:a".IsValidAbsoluteDirectoryPath());
         IDirectoryPath directoryPath = "C:a".ToAbsoluteDirectoryPath();
      }

      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_InvalidInputAbsolutePath2() {
         Assert.IsFalse(@"C\Dir1".IsValidAbsoluteDirectoryPath());
         IDirectoryPath directoryPath = @"C\Dir1".ToAbsoluteDirectoryPath();
      }

      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_InvalidInputAbsolutePath3() {
         Assert.IsFalse(@"1:\Dir1".IsValidAbsoluteDirectoryPath());
         IDirectoryPath directoryPath = @"1:\Dir1".ToAbsoluteDirectoryPath();
      }

      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_InvalidInputAbsoluteURNPath() {
         Assert.IsFalse(@"http://www.NDepend.com".IsValidAbsoluteDirectoryPath());
         IDirectoryPath directoryPath = @"http://www.NDepend.com".ToAbsoluteDirectoryPath();
      }

      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_IncoherentPathModeException1() {
         Assert.IsFalse(@".".IsValidAbsoluteDirectoryPath());
         IDirectoryPath directoryPath = @".".ToAbsoluteDirectoryPath();
      
      }


      [TestCase(@".", true)]
      [TestCase(@".\", true)]
      [TestCase(@".\a", true)]
      [TestCase(@"./", true)]
      [TestCase(@"./a", true)]
      [TestCase(@"C", false)]
      [TestCase(@"C:\", false)]
      [TestCase(@"..", true)]
      [TestCase(@"..\", true)]
      [TestCase(@"../", true)]
      [TestCase(@"..a", false)]
      [TestCase(@".?\..\bin\Debug", false)]
      [TestCase(@".C\..\bin\Debug", false)]
      [TestCase(@".\Lib   ", true)]
      [TestCase(@" .\Lib", false)]
      public void Test_IsValidRelativeDirectoryPath(string str, bool isValidRelativePath) {
         Assert.IsTrue(str.IsValidRelativeDirectoryPath() == isValidRelativePath);

         if (!isValidRelativePath) {
            bool bThrown = false;
            try {
               IDirectoryPath directoryPath = str.ToRelativeDirectoryPath();
            }
            catch (ArgumentException) {
               bThrown = true;
            }
            Assert.IsTrue(bThrown);
         }
      }


      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_InvalidInputPathBadFormatting1() {
         Assert.IsFalse(@"C:\..".IsValidAbsoluteDirectoryPath());
         IFilePath filePath = @"C:\..".ToAbsoluteFilePath();
      }

      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_InvalidInputPathBadFormatting3() {
         Assert.IsFalse(@"C:\..\Dir1\".IsValidAbsoluteDirectoryPath());
         IFilePath filePath = @"C:\..\Dir1\".ToAbsoluteFilePath();
      }



      [TestCase(@".\aa\..", @".")]
      [TestCase(@".\aa\..\.", @".")]
      [TestCase(@".\aa\..\..", @"..")]
      [TestCase(@"..\.\..", @"..\..")]
      [TestCase(@"..//./..///", @"..\..")]
      [TestCase(@".\..\aa\..\..", @"..\..")]
      [TestCase(@".\aa\..\..\..", @"..\..")]
      [TestCase(@".\aa\..\..\aa\..", @"..")]
      [TestCase(@"..\aa\..\..\..", @"..\..\..")]
      [TestCase(@"..\aa\..\..\..\.", @"..\..\..")]
      public void Test_InnerSpecialDir_IsValidRelativeDirectoryPath(string str, string normalized) {
         Assert.IsTrue(str.IsValidRelativeDirectoryPath());
         Assert.IsTrue(str.ToRelativeDirectoryPath().ToString() == normalized);
         Assert.IsFalse(str.IsValidRelativeFilePath());

         // Test file as well!!
         var strFile = str + @"\File.txt";
         Assert.IsTrue(strFile.IsValidRelativeFilePath());
         var filePath = strFile.ToRelativeFilePath();
         Assert.IsTrue(filePath.ToString() == normalized + @"\File.txt");
         Assert.IsTrue(filePath.FileName == @"File.txt");
      }


      [TestCase(@"C:\aa\..", @"C:")]
      [TestCase(@"C:\aa\bb\..\..", @"C:")]
      [TestCase(@"C:\aa\..\bb\..", @"C:")]
      [TestCase(@"C:\aa\bb\..\..\bb\..", @"C:")]
      [TestCase(@"C:\aa\..\bb\.   ", @"C:\bb")]
      public void Test_InnerSpecialDir_OK(string str, string normalized) {
         Assert.IsTrue(str.IsValidAbsoluteDirectoryPath());
         Assert.IsTrue(str.ToAbsoluteDirectoryPath().ToString() == normalized);

         // Test file as well!!
         var strFile = str.TrimEnd() + @"\File.txt";
         Assert.IsTrue(strFile.IsValidAbsoluteFilePath());
         var filePath = strFile.ToAbsoluteFilePath();
         Assert.IsTrue(filePath.ToString() == normalized + @"\File.txt");
         Assert.IsTrue(filePath.FileName == @"File.txt");


         //
         // Test EnvVar!
         //
         var strA = str.Replace(@"C:", "%ENVVAR%");
         var normalizedA = normalized.Replace(@"C:", "%ENVVAR%");
         Assert.IsTrue(strA.IsValidEnvVarDirectoryPath());
         Assert.IsTrue(strA.ToEnvVarDirectoryPath().ToString() == normalizedA);

         //
         // Test UNC Absolute!
         //
         var strB = str.Replace(@"C:", @"\\Server\Share");
         var normalizedB = normalized.Replace(@"C:", @"\\Server\Share");
         Assert.IsTrue(strB.IsValidAbsoluteDirectoryPath());
         Assert.IsTrue(strB.ToAbsoluteDirectoryPath().ToString() == normalizedB);

         //
         // Test Variable!
         //
         var strC = str.Replace(@"C:", "$(var)");
         var normalizedC = normalized.Replace(@"C:", "$(var)");
         Assert.IsTrue(strC.IsValidVariableDirectoryPath());
         Assert.IsTrue(strC.ToVariableDirectoryPath().ToString() == normalizedC);
      }

      [TestCase(@"C:\..")]
      [TestCase(@"C:\aa\..\..")]
      [TestCase(@"C:\aa\..\..")]
      [TestCase(@"C:\aa\..\bb\..\..")]
      public void Test_InnerSpecialDir_KO(string str) {
         Assert.IsFalse(str.IsValidAbsoluteDirectoryPath());
         bool bThrown = false;
         try {
            str.ToAbsoluteDirectoryPath();
         }
         catch (ArgumentException ex) {
            bThrown = true;
            Assert.IsTrue(ex.Message.StartsWith(@"The pathString {" + str + @"} references the parent dir \..\ of the root dir {C:}, it cannot be resolved.
"));
         }
         Assert.IsTrue(bThrown);

         //
         // Test EnvVar!
         //
         var strA = str.Replace(@"C:", "%ENVVAR%");
         Assert.IsFalse(strA.IsValidEnvVarDirectoryPath());
         bThrown = false;
         try {
            strA.ToEnvVarDirectoryPath();
         } catch (ArgumentException ex) {
            bThrown = true;
            Assert.IsTrue(ex.Message.StartsWith(@"The pathString {" + strA + @"} references the parent dir \..\ of the root dir {%ENVVAR%}, it cannot be resolved.
"));
         }
         Assert.IsTrue(bThrown);

         //
         // Test UNC Absolute!
         //
         var strB = str.Replace(@"C:", @"\\Server\Share");
         Assert.IsFalse(strB.IsValidAbsoluteDirectoryPath());
         bThrown = false;
         try {
            strB.ToAbsoluteDirectoryPath();
         } catch (ArgumentException ex) {
            bThrown = true;
            Assert.IsTrue(ex.Message.StartsWith(@"The pathString {" + strB + @"} references the parent dir \..\ of the root dir {\\Server\Share}, it cannot be resolved.
"));
         }
         Assert.IsTrue(bThrown);

         //
         // Test Variable!
         //
         var strC = str.Replace(@"C:", "$(var)");
         Assert.IsFalse(strC.IsValidVariableDirectoryPath());
         bThrown = false;
         try {
            strC.ToVariableDirectoryPath();
         } catch (ArgumentException ex) {
            bThrown = true;
            Assert.IsTrue(ex.Message.StartsWith(@"The pathString {" + strC + @"} references the parent dir \..\ of the root dir {$(var)}, it cannot be resolved.
"));
         }
         Assert.IsTrue(bThrown);
      }







      [TestCase(@".", PathMode.Relative)]
      [TestCase(@".\dir...1", PathMode.Relative)]
      [TestCase(@"C:\", PathMode.Absolute)]
      [TestCase(@"C:\dir...1", PathMode.Absolute)]
      [TestCase(@"\\server\share\", PathMode.Absolute)]
      [TestCase(@"\\server\share\dir...1", PathMode.Absolute)]
      [TestCase(@"%EV%\", PathMode.EnvVar)]
      [TestCase(@"%EV%\dir...1", PathMode.EnvVar)]
      [TestCase(@"$(var)", PathMode.Variable)]
      [TestCase(@"$(var)\dir...1", PathMode.Variable)]
      public void Test_PathModeOk(string pathString, PathMode pathMode) {
         IDirectoryPath path = pathString.ToDirectoryPath();
         Assert.IsFalse(path.IsFilePath);
         Assert.IsTrue(path.IsDirectoryPath);
         Assert.IsTrue(path.PathMode == pathMode);
         Assert.IsTrue(path.IsAbsolutePath == (pathMode == PathMode.Absolute));
         Assert.IsTrue(path.IsRelativePath == (pathMode == PathMode.Relative));
         Assert.IsTrue(path.IsVariablePath == (pathMode == PathMode.Variable));
         Assert.IsTrue(path.IsEnvVarPath == (pathMode == PathMode.EnvVar));
      }

      [TestCase(@".\  ", ".")]
      [TestCase(@"C:\  ", "C:")]
      [TestCase(@"C:\Dir  ", @"C:\Dir")]
      [TestCase(@"%JJ%\  ", @"%JJ%")]
      public void Test_PathNormalizationTrimEnd(string path, string pathNormalized) {
         Assert.IsTrue(path.IsValidDirectoryPath());
         Assert.IsTrue(path.ToDirectoryPath().ToString() == pathNormalized);
      }

      [Test]
      public void Test_BuildDirectoryPath() {
         IDirectoryPath path = @".\..\Dir1".ToDirectoryPath();
         Assert.IsTrue(path.IsRelativePath);

         path = @"C:\Dir1\Dir2".ToDirectoryPath();
         Assert.IsTrue(path.IsAbsolutePath);
      }

      [Test]
      public void Test_NormalizePath() {
         IDirectoryPath path = @".\".ToRelativeDirectoryPath();
         Assert.IsTrue(path.ToString() == ".");

         path = @".\\\".ToRelativeDirectoryPath();
         Assert.IsTrue(path.ToString() == ".");

         path = @".\\\..\\".ToRelativeDirectoryPath();
         Assert.IsTrue(path.ToString() == @"..");

         path = @".\/dir1\//\dir2\/dir3///".ToRelativeDirectoryPath();
         Assert.IsTrue(path.ToString() == @".\dir1\dir2\dir3");

         path = @"C:/dir1/dir2".ToAbsoluteDirectoryPath();
         Assert.IsTrue(path.ToString() == @"C:\dir1\dir2");
      }


      [Test]
      public void Test_HasParentDir() {
         IDirectoryPath path = @".\".ToRelativeDirectoryPath();
         Assert.IsFalse(path.HasParentDirectory);

         path = @".\Dir1".ToRelativeDirectoryPath();
         Assert.IsTrue(path.HasParentDirectory);

         path = @".\Dir1\Dir".ToRelativeDirectoryPath();
         Assert.IsTrue(path.HasParentDirectory);

         path = @"C:\\".ToAbsoluteDirectoryPath();
         Assert.IsFalse(path.HasParentDirectory);

         path = @"C:\Dir1".ToAbsoluteDirectoryPath();
         Assert.IsTrue(path.HasParentDirectory);
      }


      [Test, ExpectedException(typeof(InvalidOperationException))]
      public void Test_Error1OnParentDirectoryPath() {
         IDirectoryPath path = @".\".ToRelativeDirectoryPath().ParentDirectoryPath;
      }

      [Test, ExpectedException(typeof(InvalidOperationException))]
      public void Test_Error2OnParentDirectoryPath() {
         IDirectoryPath path = @"C:\".ToAbsoluteDirectoryPath().ParentDirectoryPath;
      }


      [Test]
      public void Test_ParentDirectoryPath() {
         IDirectoryPath path = @".\Dir1".ToRelativeFilePath().ParentDirectoryPath;
         Assert.IsTrue(path.ToString() == @".");

         path = @".\Dir1\\Dir2".ToRelativeDirectoryPath().ParentDirectoryPath;
         Assert.IsTrue(path.ToString() == @".\Dir1");

         path = @"C:\Dir1".ToAbsoluteDirectoryPath().ParentDirectoryPath;
         Assert.IsTrue(path.ToString() == @"C:");

         path = @".\\Dir1".ToRelativeDirectoryPath().ParentDirectoryPath;
         Assert.IsTrue(path.ToString() == @".");

         path = @"C:\\\\Dir1\\Dir2".ToAbsoluteDirectoryPath().ParentDirectoryPath;
         Assert.IsTrue(path.ToString() == @"C:\Dir1");

         path = @"C:\dir1\\//dir2\".ToAbsoluteDirectoryPath().ParentDirectoryPath;
         Assert.IsTrue(path.ToString() == @"C:\dir1");
      }

      [Test]
      public void TestDirectoryName() {
         string directoryName = @".\".ToRelativeDirectoryPath().DirectoryName;
         Assert.IsTrue(directoryName == string.Empty);

         directoryName = @"C:\".ToAbsoluteDirectoryPath().DirectoryName;
         Assert.IsTrue(directoryName == string.Empty);

         directoryName = @".\\dir1\\/dir2".ToRelativeDirectoryPath().DirectoryName;
         Assert.IsTrue(directoryName == @"dir2");

         directoryName = @"C:\\\\dir1".ToAbsoluteDirectoryPath().DirectoryName;
         Assert.IsTrue(directoryName == @"dir1");

         directoryName = @"C:\dir1\\//dir2\".ToAbsoluteDirectoryPath().DirectoryName;
         Assert.IsTrue(directoryName == @"dir2");
      }

      //
      //  GetRelativePath
      //
      [Test]
      public void Test_GetRelativePath() {
         IAbsoluteDirectoryPath absoluteDirectoryPathTo,absoluteDirectoryPathFrom;
         absoluteDirectoryPathTo = @"C:\Dir1".ToAbsoluteDirectoryPath();

         absoluteDirectoryPathFrom = @"C:\Dir1".ToAbsoluteDirectoryPath();
         Assert.IsTrue(absoluteDirectoryPathTo.GetRelativePathFrom(absoluteDirectoryPathFrom).ToString() == @".");
         Assert.IsTrue(absoluteDirectoryPathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom));

         absoluteDirectoryPathTo = @"C:\Dir1\Dir2".ToAbsoluteDirectoryPath();
         absoluteDirectoryPathFrom = @"C:\Dir1\Dir3".ToAbsoluteDirectoryPath();
         Assert.IsTrue(absoluteDirectoryPathTo.GetRelativePathFrom(absoluteDirectoryPathFrom).ToString() == @"..\Dir2");
         Assert.IsTrue(absoluteDirectoryPathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom));

         absoluteDirectoryPathTo = @"C:\Dir1".ToAbsoluteDirectoryPath();
         absoluteDirectoryPathFrom = @"C:\Dir2".ToAbsoluteDirectoryPath();
         Assert.IsTrue(absoluteDirectoryPathTo.GetRelativePathFrom(absoluteDirectoryPathFrom).ToString() == @"..\Dir1");
         Assert.IsTrue(absoluteDirectoryPathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom));

         absoluteDirectoryPathTo = @"C:\Dir1\Dir2".ToAbsoluteDirectoryPath();
         absoluteDirectoryPathFrom = @"C:\Dir1".ToAbsoluteDirectoryPath();
         Assert.IsTrue(absoluteDirectoryPathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom));
         Assert.IsTrue(absoluteDirectoryPathTo.GetRelativePathFrom(absoluteDirectoryPathFrom).ToString() == @".\Dir2");
         Assert.IsTrue((absoluteDirectoryPathTo as IAbsolutePath).GetRelativePathFrom(absoluteDirectoryPathFrom).ToString() == @".\Dir2");

         absoluteDirectoryPathTo = @"\\Server\Share\Dir2".ToAbsoluteDirectoryPath();
         absoluteDirectoryPathFrom = @"\\Server\Share".ToAbsoluteDirectoryPath();

         Assert.IsTrue(absoluteDirectoryPathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom));
         string failureReason;
         Assert.IsTrue(absoluteDirectoryPathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom, out failureReason));
         Assert.IsNull(failureReason);

         Assert.IsTrue(absoluteDirectoryPathTo.GetRelativePathFrom(absoluteDirectoryPathFrom).ToString() == @".\Dir2");
         Assert.IsTrue((absoluteDirectoryPathTo as IAbsolutePath).GetRelativePathFrom(absoluteDirectoryPathFrom).ToString() == @".\Dir2");
      }


      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_GetRelativePathWithError3() {
         IAbsoluteDirectoryPath absoluteDirectoryPathTo = @"C:\Dir1".ToAbsoluteDirectoryPath();
         IAbsoluteDirectoryPath absoluteDirectoryPathFrom = @"D:\Dir1".ToAbsoluteDirectoryPath();
         Assert.IsFalse(absoluteDirectoryPathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom));
         string failureReason;
         Assert.IsFalse(absoluteDirectoryPathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom, out failureReason));
         Assert.IsTrue(failureReason == @"Cannot compute relative path from 2 paths that are not on the same volume 
   PathFrom = ""D:\Dir1""
   PathTo   = ""C:\Dir1""");
         absoluteDirectoryPathTo.GetRelativePathFrom(absoluteDirectoryPathFrom);
      }

      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_GetRelativePathWithError4() {
         IAbsoluteDirectoryPath absoluteDirectoryPathTo = @"C:\Dir1".ToAbsoluteDirectoryPath();
         IAbsoluteDirectoryPath absoluteDirectoryPathFrom = @"\\Server\Share".ToAbsoluteDirectoryPath();
         Assert.IsFalse(absoluteDirectoryPathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom));
         absoluteDirectoryPathTo.GetRelativePathFrom(absoluteDirectoryPathFrom);
      }

      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_GetRelativePathWithError5() {
         IAbsoluteDirectoryPath absoluteDirectoryPathTo = @"\\Server1\Share".ToAbsoluteDirectoryPath();
         IAbsoluteDirectoryPath absoluteDirectoryPathFrom = @"\\Server2\Share".ToAbsoluteDirectoryPath();
         Assert.IsFalse(absoluteDirectoryPathTo.CanGetRelativePathFrom(absoluteDirectoryPathFrom));
         absoluteDirectoryPathTo.GetRelativePathFrom(absoluteDirectoryPathFrom);
      }

      //
      //  GetAbsolutePath
      //
      [Test]
      public void Test_GetAbsolutePath() {
         IRelativeDirectoryPath relativeDirectoryPathTo;
         IAbsoluteDirectoryPath absoluteDirectoryPathFrom;

         relativeDirectoryPathTo = @"..".ToRelativeDirectoryPath();
         absoluteDirectoryPathFrom = @"C:\Dir1".ToAbsoluteDirectoryPath();
         Assert.IsTrue(relativeDirectoryPathTo.GetAbsolutePathFrom(absoluteDirectoryPathFrom).ToString() == @"C:");
         Assert.IsTrue(relativeDirectoryPathTo.CanGetAbsolutePathFrom(absoluteDirectoryPathFrom));

         relativeDirectoryPathTo = @".".ToRelativeDirectoryPath();
         absoluteDirectoryPathFrom = @"C:\Dir1".ToAbsoluteDirectoryPath();
         Assert.IsTrue(relativeDirectoryPathTo.GetAbsolutePathFrom(absoluteDirectoryPathFrom).ToString() == @"C:\Dir1");
         Assert.IsTrue(relativeDirectoryPathTo.CanGetAbsolutePathFrom(absoluteDirectoryPathFrom));

         relativeDirectoryPathTo = @"..\Dir2".ToRelativeDirectoryPath();
         absoluteDirectoryPathFrom = @"C:\Dir1".ToAbsoluteDirectoryPath();
         Assert.IsTrue(relativeDirectoryPathTo.GetAbsolutePathFrom(absoluteDirectoryPathFrom).ToString() == @"C:\Dir2");
         Assert.IsTrue(relativeDirectoryPathTo.CanGetAbsolutePathFrom(absoluteDirectoryPathFrom));

         relativeDirectoryPathTo = @"..\..\Dir4\Dir5".ToRelativeDirectoryPath();
         absoluteDirectoryPathFrom = @"C:\Dir1\Dir2\Dir3".ToAbsoluteDirectoryPath();
         Assert.IsTrue(relativeDirectoryPathTo.GetAbsolutePathFrom(absoluteDirectoryPathFrom).ToString() == @"C:\Dir1\Dir4\Dir5");
         Assert.IsTrue(relativeDirectoryPathTo.CanGetAbsolutePathFrom(absoluteDirectoryPathFrom));

         relativeDirectoryPathTo = @".\..\Dir4\Dir5".ToRelativeDirectoryPath();
         absoluteDirectoryPathFrom = @"C:\Dir1\Dir2\Dir3".ToAbsoluteDirectoryPath();
         Assert.IsTrue(relativeDirectoryPathTo.CanGetAbsolutePathFrom(absoluteDirectoryPathFrom));

         string failureReason;
         Assert.IsTrue(relativeDirectoryPathTo.CanGetAbsolutePathFrom(absoluteDirectoryPathFrom, out failureReason));
         Assert.IsNull(failureReason);


         Assert.IsTrue(relativeDirectoryPathTo.GetAbsolutePathFrom(absoluteDirectoryPathFrom).ToString() == @"C:\Dir1\Dir2\Dir4\Dir5");
         Assert.IsTrue((relativeDirectoryPathTo as IRelativePath).GetAbsolutePathFrom(absoluteDirectoryPathFrom).ToString() == @"C:\Dir1\Dir2\Dir4\Dir5");
      }

      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_GetAbsolutePathPathWithError3() {
         IRelativeDirectoryPath relativeDirectoryPathTo = @"..\..\Dir1".ToRelativeDirectoryPath();
         IAbsoluteDirectoryPath absoluteDirectoryPathFrom = @"C:\Dir1".ToAbsoluteDirectoryPath();
         Assert.IsFalse(relativeDirectoryPathTo.CanGetAbsolutePathFrom(absoluteDirectoryPathFrom));

         string failureReason;
         Assert.IsFalse(relativeDirectoryPathTo.CanGetAbsolutePathFrom(absoluteDirectoryPathFrom, out failureReason));
         Assert.IsTrue(failureReason == @"Cannot resolve pathTo.TryGetAbsolutePath(pathFrom) because there are too many parent dirs in pathTo:
   PathFrom = ""C:\Dir1""
   PathTo   = ""..\..\Dir1""");

         relativeDirectoryPathTo.GetAbsolutePathFrom(absoluteDirectoryPathFrom);
      }


      //
      //  PathComparison
      //
      [Test]
      public void Test_PathEquality() {

         //
         // RelativeDirectoryPath 
         //
         IRelativeDirectoryPath relativeRelativeDirectoryPath1 = @"..\Dir1\Dir2".ToRelativeDirectoryPath();
         IRelativeDirectoryPath relativeRelativeDirectoryPath2 = @"..\\dir1//DIR2/".ToRelativeDirectoryPath();
         Assert.IsTrue(relativeRelativeDirectoryPath1.Equals(relativeRelativeDirectoryPath2));

         relativeRelativeDirectoryPath1 = @"..\Dir1\Dir2".ToRelativeDirectoryPath();
         relativeRelativeDirectoryPath2 = @".\Dir1\Dir2".ToRelativeDirectoryPath();
         Assert.IsFalse(relativeRelativeDirectoryPath1.Equals(relativeRelativeDirectoryPath2));

         relativeRelativeDirectoryPath1 = @"..\Dir1\Dir2".ToRelativeDirectoryPath();
         relativeRelativeDirectoryPath2 = @"..\Dir1\Dir2\Dir3".ToRelativeDirectoryPath();
         Assert.IsFalse(relativeRelativeDirectoryPath1.Equals(relativeRelativeDirectoryPath2));

         relativeRelativeDirectoryPath1 = @"..\Dir1\Dir2".ToRelativeDirectoryPath();
         relativeRelativeDirectoryPath2 = @"..\Dir1\Dir".ToRelativeDirectoryPath();
         Assert.IsFalse(relativeRelativeDirectoryPath1.Equals(relativeRelativeDirectoryPath2));

         //
         // AbsoluteDirectoryPath 
         //
         IAbsoluteDirectoryPath absoluteAbsoluteDirectoryPath1 = @"C:\Dir1\Dir2".ToAbsoluteDirectoryPath();
         IAbsoluteDirectoryPath absoluteAbsoluteDirectoryPath2 = @"C:\\dir1//Dir2\\".ToAbsoluteDirectoryPath();
         Assert.IsTrue(absoluteAbsoluteDirectoryPath1.Equals(absoluteAbsoluteDirectoryPath2));

         absoluteAbsoluteDirectoryPath1 = @"C:\Dir1\Dir2".ToAbsoluteDirectoryPath();
         absoluteAbsoluteDirectoryPath2 = @"D:\Dir1\Dir2".ToAbsoluteDirectoryPath();
         Assert.IsFalse(absoluteAbsoluteDirectoryPath1.Equals(absoluteAbsoluteDirectoryPath2));

         absoluteAbsoluteDirectoryPath1 = @"C:\Dir1\Dir2".ToAbsoluteDirectoryPath();
         absoluteAbsoluteDirectoryPath2 = @"C:\Dir1\Dir2\Dir2".ToAbsoluteDirectoryPath();
         Assert.IsFalse(absoluteAbsoluteDirectoryPath1.Equals(absoluteAbsoluteDirectoryPath2));

         absoluteAbsoluteDirectoryPath1 = @"C:\Dir1\Dir2".ToAbsoluteDirectoryPath();
         absoluteAbsoluteDirectoryPath2 = @"C:\Dir1\Dir".ToAbsoluteDirectoryPath();
         Assert.IsFalse(absoluteAbsoluteDirectoryPath1.Equals(absoluteAbsoluteDirectoryPath2));

         //
         // Mix between AbsoluteDirectoryPath and RelativeDirectoryPath
         //
         relativeRelativeDirectoryPath1 = @"..\Dir1\Dir2".ToRelativeDirectoryPath();
         absoluteAbsoluteDirectoryPath1 = @"C:\Dir1\Dir2".ToAbsoluteDirectoryPath();
         Assert.IsFalse(absoluteAbsoluteDirectoryPath1.Equals(relativeRelativeDirectoryPath1));
      }



      //
      //  Get Child With Name
      //
      [Test]
      public void Test_GetChildWithName() {
         IAbsoluteDirectoryPath absoluteDirectoryPath = @"C:\Dir1\Dir2".ToAbsoluteDirectoryPath();
         Assert.IsTrue(absoluteDirectoryPath.GetChildFileWithName("File.txt").ToString() == @"C:\Dir1\Dir2\File.txt");
         Assert.IsTrue(absoluteDirectoryPath.GetChildDirectoryWithName("Dir3").ToString() == @"C:\Dir1\Dir2\Dir3");

         IRelativeDirectoryPath relativeDirectoryPath = @"..\..\Dir1\Dir2".ToRelativeDirectoryPath();
         Assert.IsTrue(relativeDirectoryPath.GetChildFileWithName("File.txt").ToString() == @"..\..\Dir1\Dir2\File.txt");
         Assert.IsTrue(relativeDirectoryPath.GetChildDirectoryWithName("Dir3").ToString() == @"..\..\Dir1\Dir2\Dir3");
      }




      [Test]
      public void Test_GetChildWithName_Error6() {
         IRelativeDirectoryPath relativeDirectoryPath = @"..\Dir1\Dir2".ToRelativeDirectoryPath();
         //RelativeDirectoryPath.GetChildFileWithName(null);
      }

      [Test]
      public void Test_GetChildWithName_Error8() {
         IRelativeDirectoryPath relativeDirectoryPath = @"..\Dir1\Dir2".ToRelativeDirectoryPath();
         //RelativeDirectoryPath.GetChildDirectoryWithName(null);
      }




      //
      //  IsChildOf
      //
      [TestCase(@"C:\Dir1\Dir2\Dir3", @"C:\dir1\dir2\", true)]
      [TestCase(@"C:\Dir1\Dir2\Dir3", @"C:\Dir1\", true)]
      [TestCase(@"C:\Dir1\Dir2\Dir3", @"c:", true)]
      [TestCase(@"\\server\SHARE\Dir2\Dir3", @"\\SERVER\\share\dir2\", true)]
      [TestCase(@".\Dir2\Dir3", @".\dir2\", true)]
      [TestCase(@"D:/Foo/bar", @"D:/Foo", true)]


      [TestCase(@"C:\Dir1\Dir2\Dir3", @"E:", false)]
      [TestCase(@"C:\Dir1\Dir2\Dir3", @"C:\Dir2", false)]
      [TestCase(@"C:\Dir2\", @"C:\Dir2", false)]
      [TestCase(@"\\serverA\SHARE\Dir2\Dir3", @"\\SERVERB\\share\dir2\", false)]
      [TestCase(@"\\server\SHARE\Dir2\Dir3", @"\\SERVER\\share\dir2\dir3", false)]
      [TestCase(@"D:/Foo bar", @"D:/Foo", false)]
      [TestCase(@"D:/Foobar", @"D:/Foo", false)]

      public void Test_IsChildDirectoryOf(string pathChildString, string pathParentString, bool isChildOf) {
         var pathChild = pathChildString.ToDirectoryPath();
         var pathParent = pathParentString.ToDirectoryPath();
         Assert.IsTrue(pathChild.IsChildOf(pathParent) == isChildOf);

         if (pathChildString.IsValidFilePath()) {
            var pathFileChild = pathChildString.ToFilePath();
            Assert.IsTrue(pathFileChild.IsChildOf(pathParent) == isChildOf);
         }
      }


      //
      //  GetBrother
      //


      [Test]
      public void Test_GetBrotherWithName1() {
         Assert.IsTrue(@"C:\Dir1\Dir2".ToAbsoluteDirectoryPath().GetBrotherDirectoryWithName("Dir3").Equals(
            @"C:\Dir1\Dir3".ToAbsoluteDirectoryPath()));
      }
      [Test]
      public void Test_GetBrotherWithName2() {
         Assert.IsTrue(@"C:\Dir1\Dir2".ToDirectoryPath().GetBrotherDirectoryWithName("Dir3").Equals(
            @"C:\Dir1\Dir3".ToDirectoryPath()));
      }
      [Test]
      public void Test_GetBrotherWithName3() {
         Assert.IsTrue((@"C:\Dir1\Dir2".ToAbsoluteDirectoryPath() as IDirectoryPath).GetBrotherDirectoryWithName("Dir3").Equals(
            @"C:\Dir1\Dir3".ToAbsoluteDirectoryPath()));
      }
      [Test]
      public void Test_GetBrotherWithName4() { 
         Assert.IsTrue((@"C:\Dir1\Dir2".ToDirectoryPath() as IDirectoryPath).GetBrotherDirectoryWithName("Dir3").Equals(
            @"C:\Dir1\Dir3".ToDirectoryPath()));
      }
      [Test]
      public void Test_GetBrotherWithName5() {
         Assert.IsTrue(@"..\Dir1\Dir2".ToRelativeDirectoryPath().GetBrotherDirectoryWithName("Dir3").Equals(
            @"..\Dir1\Dir3".ToRelativeDirectoryPath()));
      }
      [Test]
      public void Test_GetBrotherWithName6() {      
         Assert.IsTrue(@"..\Dir1\Dir2".ToDirectoryPath().GetBrotherDirectoryWithName("Dir3").Equals(
            @"..\Dir1\Dir3".ToDirectoryPath()));
      }
      [Test]
      public void Test_GetBrotherWithName7() {
         Assert.IsTrue(@"C:\Dir1\Dir2".ToAbsoluteDirectoryPath().GetBrotherFileWithName("File.txt").Equals(
            @"C:\Dir1\File.txt".ToAbsoluteFilePath()));
      }
      [Test]
      public void Test_GetBrotherWithName8() {
         Assert.IsTrue(@"C:\Dir1\Dir2".ToDirectoryPath().GetBrotherFileWithName("File.txt").Equals(
            @"C:\Dir1\File.txt".ToFilePath()));
      }

      [Test]
      public void Test_GetBrotherWithName9() {
         Assert.IsTrue(@"..\Dir1\Dir2".ToRelativeDirectoryPath().GetBrotherFileWithName("File.txt").Equals(
            @"..\Dir1\File.txt".ToRelativeFilePath()));
      }
      [Test]
      public void Test_GetBrotherWithName10() {
         Assert.IsTrue(@"..\Dir1\Dir2".ToDirectoryPath().GetBrotherFileWithName("File.txt").Equals(
            @"..\Dir1\File.txt".ToFilePath()));
      }

      [Test]
      public void Test_GetBrotherWithName11() {
         Assert.IsTrue(@"%E%\Dir1\Dir2".ToEnvVarDirectoryPath().GetBrotherDirectoryWithName("Dir3").Equals(
            @"%E%\Dir1\Dir3".ToEnvVarDirectoryPath()));
      }
      [Test]
      public void Test_GetBrotherWithName12() {
         Assert.IsTrue(@"%E%\Dir1\Dir2".ToDirectoryPath().GetBrotherDirectoryWithName("Dir3").Equals(
            @"%E%\Dir1\Dir3".ToDirectoryPath()));
      }


      [Test]
      public void Test_GetBrotherWithName13() {
         Assert.IsTrue(@"%E%\Dir1\Dir2".ToEnvVarDirectoryPath().GetBrotherFileWithName("File.txt").Equals(
            @"%E%\Dir1\File.txt".ToEnvVarFilePath()));
      }
      [Test]
      public void Test_GetBrotherWithName14() {
         Assert.IsTrue(@"%E%\Dir1\Dir2".ToDirectoryPath().GetBrotherFileWithName("File.txt").Equals(
            @"%E%\Dir1\File.txt".ToFilePath()));
      }

      [Test]
      public void Test_GetBrotherWithName15() {
         Assert.IsTrue(@"%E%\Dir1\Dir2".ToEnvVarDirectoryPath().GetChildDirectoryWithName("Dir3").Equals(
            @"%E%\Dir1\Dir2\Dir3".ToEnvVarDirectoryPath()));
      }
      [Test]
      public void Test_GetBrotherWithName16() {
         Assert.IsTrue(@"%E%\Dir1\Dir2".ToDirectoryPath().GetChildDirectoryWithName("Dir3").Equals(
            @"%E%\Dir1\Dir2\Dir3".ToDirectoryPath()));
      }


      [Test]
      public void Test_GetBrotherWithName17() {
         Assert.IsTrue(@"%E%\Dir1\Dir2".ToEnvVarDirectoryPath().GetChildFileWithName("File.txt").Equals(
            @"%E%\Dir1\Dir2\File.txt".ToEnvVarFilePath()));
      }
      [Test]
      public void Test_GetBrotherWithName18() {
         Assert.IsTrue(@"%E%\Dir1\Dir2".ToDirectoryPath().GetChildFileWithName("File.txt").Equals(
            @"%E%\Dir1\Dir2\File.txt".ToFilePath()));
      }


      [Test]
      public void Test_GetBrotherWithName19() {
         Assert.IsTrue(@"%E%\Dir1\Dir2\File.txt".ToEnvVarFilePath().GetBrotherDirectoryWithName("Dir3").Equals(
            @"%E%\Dir1\Dir2\Dir3".ToEnvVarDirectoryPath()));
      }
      [Test]
      public void Test_GetBrotherWithName20() {
         Assert.IsTrue(@"%E%\Dir1\Dir2\File.txt".ToFilePath().GetBrotherDirectoryWithName("Dir3").Equals(
            @"%E%\Dir1\Dir2\Dir3".ToDirectoryPath()));
      }


      [Test]
      public void Test_GetBrotherWithName21() {
         Assert.IsTrue(@"%E%\Dir1\Dir2\File.txt".ToEnvVarFilePath().GetBrotherFileWithName("FILE2").Equals(
            @"%E%\Dir1\Dir2\FILE2".ToEnvVarFilePath()));
      }
      [Test]
      public void Test_GetBrotherWithName22() {
         Assert.IsTrue(@"%E%\Dir1\Dir2\File.txt".ToFilePath().GetBrotherFileWithName("FILE2").Equals(
            @"%E%\Dir1\Dir2\FILE2".ToFilePath()));
      }



      [TestCase(@"cc\")]
      [TestCase(@"v.\")]
      public void Test_IsValidDirectory_KO(string pathString) {
         string failureReason;
         Assert.IsFalse(pathString.IsValidDirectoryPath(out failureReason));
         Assert.IsTrue(failureReason == @"The string """ + pathString + @""" is not a valid directory path.");

         bool b = false;
         try {
            pathString.ToDirectoryPath();
         } catch (ArgumentException ex) {
            b = true;
            Assert.IsTrue(ex.Message.Contains(failureReason));
            Assert.IsTrue(ex.Message.StartsWith(@"The parameter pathString is not a valid directory path.
" + failureReason));
         }
         Assert.IsTrue(b);
      }
   }
}
