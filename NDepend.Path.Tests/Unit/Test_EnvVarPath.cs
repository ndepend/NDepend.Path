using System;
using NDepend.Test;
using NUnit.Framework;

namespace NDepend.Path {

   [TestFixture]
   public class Test_EnvVarPath {

      [SetUp]
      public void SetUp() {
         TestHelper.SetUpTests();
      }


      [TestCase(@"%E%", "%E%", @"%E%")]
      [TestCase(@"%E%/", "%E%", @"%E%")]
      [TestCase(@"%E%\", "%E%", @"%E%")]
      [TestCase(@"%E% ", "%E%", @"%E%")]
      [TestCase(@"%E%\%F%", "%E%", @"%E%\%F%")]
      [TestCase(@"%ENV_VAR1%", "%ENV_VAR1%", @"%ENV_VAR1%")]
      [TestCase(@"%ENV_VAR1%\", "%ENV_VAR1%", @"%ENV_VAR1%")]
      [TestCase(@"%ENV_VAR1%\Dir\", "%ENV_VAR1%", @"%ENV_VAR1%\Dir")]
      [TestCase(@"%ENV_VAR1%\\Dir\\Dir2", "%ENV_VAR1%", @"%ENV_VAR1%\Dir\Dir2")]
      [TestCase(@"%ENV_VAR1%\\Dir\\..", "%ENV_VAR1%", @"%ENV_VAR1%")]
      [TestCase(@"%ENV_VAR1%\\Dir1\..\Dir2", "%ENV_VAR1%", @"%ENV_VAR1%\Dir2")]
      [TestCase(@"%ENV_VAR1%\\Dir1\.\Dir2", "%ENV_VAR1%", @"%ENV_VAR1%\Dir1\Dir2")]
      public void TestSyntaxEnvVar_DirOK(string path, string envVar, string pathNormalized) {
         Assert.IsTrue(path.IsValidEnvVarDirectoryPath());
         var dirPath = path.ToEnvVarDirectoryPath();
         Assert.IsTrue(dirPath.EnvVar == envVar);
         Assert.IsTrue(dirPath.ToString() == pathNormalized);
      }

      [TestCase(@"%"), ExpectedException(typeof(ArgumentException))]
      [TestCase(@"%%")]
      [TestCase(@"%%\Dir")]
      [TestCase(@"%\%\Dir")]
      [TestCase(@"%E%\Dir\..\..")]
      [TestCase(@"%E%\..\Dir")]
      [TestCase(@"%E%\..")]
      [TestCase(@"%a\b%\Dir")]
      [TestCase(@"%a\Dir")]
      [TestCase(@"%a\b%")]
      [TestCase(@"%AAA\BBB%")]
      [TestCase(@"%AAA BBB%")]
      [TestCase(@"%AAA%BBB%")]
      [TestCase(@"%AAA%a")]
      [TestCase(@"%AAA%.")]
      [TestCase(@"%AAA%_")]
      [TestCase(@"C:\Dir\%ENVVAR%")]
      [TestCase(@"C:\Dir")]
      [TestCase(@"\\server\share")]
      [TestCase(@"")]
      public void TestSyntaxEnvVar_KO(string path) {
         Assert.IsFalse(path.IsValidEnvVarFilePath());
         Assert.IsFalse(path.IsValidEnvVarDirectoryPath());
         var dirPath = path.ToEnvVarDirectoryPath();
      }


      [TestCase((string)null), ExpectedException(typeof(ArgumentNullException))]
      public void TestSyntaxEnvVarDir_KO_NUll(string path) {
         Assert.IsFalse(path.IsValidEnvVarDirectoryPath());
         var dirPath = path.ToEnvVarDirectoryPath();
      }

      [TestCase((string)null), ExpectedException(typeof(ArgumentNullException))]
      public void TestSyntaxEnvVarFile_KO_NUll(string path) {
         Assert.IsFalse(path.IsValidEnvVarFilePath());
         var dirPath = path.ToEnvVarFilePath();
      }


      [TestCase(@"%ENVVAR%\fileName", "fileName", @"%ENVVAR%\fileName")]
      [TestCase(@"%ENVVAR%\dir\..\fileName", "fileName", @"%ENVVAR%\fileName")]
      [TestCase(@"%ENVVAR%\dir\fileName", "fileName", @"%ENVVAR%\dir\fileName")]
      [TestCase(@"%ENVVAR%\/dir%\/fileName%\\", "fileName%", @"%ENVVAR%\dir%\fileName%")]
      public void TestSyntaxEnvVar_FileOK(string path, string fileName, string pathNormalized) {
         Assert.IsTrue(path.IsValidEnvVarFilePath());
         var filePath = path.ToEnvVarFilePath();
         Assert.IsTrue(filePath.ToString() == pathNormalized);
         Assert.IsTrue(filePath.FileName == fileName);
         Assert.IsTrue(filePath.EnvVar == @"%ENVVAR%");
      }

      [TestCase(@"%ENVVAR%"), ExpectedException(typeof(ArgumentException))]
      [TestCase(@"%ENVVAR%\")]
      [TestCase(@"%ENVVAR%\FileName\..")]
      public void TestSyntaxEnvVar_DirOK_FileKO(string path) {
         Assert.IsTrue(path.IsValidEnvVarDirectoryPath());
         Assert.IsFalse(path.IsValidEnvVarFilePath());
         var dirPath = path.ToEnvVarFilePath();
      }

      [TestCase(@"%ENVVAR%\Dir", "Dir", @"%ENVVAR%")]
      [TestCase(@"%ENVVAR%\Dir\Dir1\..", "Dir", @"%ENVVAR%")]
      [TestCase(@"%ENVVAR%\Dir\Dir1\/DIR2//", "DIR2", @"%ENVVAR%\Dir\Dir1")]
      public void TestSyntaxEnvVar_Dir_ParentDirectoryOk(string path, string dirName, string parentDirPathString) {
         var dirPath = path.ToEnvVarDirectoryPath();
         Assert.IsTrue(dirPath.DirectoryName == dirName);
         var parentDirPath1 = dirPath.ParentDirectoryPath;
         Assert.IsTrue(parentDirPath1.ToString() == parentDirPathString);
         var parentDirPath2 = (dirPath as IDirectoryPath).ParentDirectoryPath;
         Assert.IsTrue(parentDirPath2.ToString() == parentDirPathString);
      }

      [TestCase(@"%ENVVAR%\FileName.txt", @"%ENVVAR%")]
      [TestCase(@"%ENVVAR%\Dir\FileName.txt", @"%ENVVAR%\Dir")]
      [TestCase(@"%ENVVAR%\Dir\Dir1\..\FileName.txt\", @"%ENVVAR%\Dir")]
      [TestCase(@"%ENVVAR%\Dir\Dir1\/DIR2//\FileName.txt//", @"%ENVVAR%\Dir\Dir1\DIR2")]
      public void TestSyntaxEnvVar_File_ParentDirectoryOk(string path, string parentDirPathString) {
         var filePath = path.ToEnvVarFilePath();
         Assert.IsTrue(filePath.FileName == "FileName.txt");
         var parentDirPath1 = filePath.ParentDirectoryPath;
         Assert.IsTrue(parentDirPath1.ToString() == parentDirPathString);
         var parentDirPath2 = (filePath as IFilePath).ParentDirectoryPath;
         Assert.IsTrue(parentDirPath2.ToString() == parentDirPathString);
      }

   }
}
