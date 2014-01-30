using System;
using NDepend.Test;
using NUnit.Framework;

namespace NDepend.Path {

   [TestFixture]
   public class Test_EnvVarPath_Resolving {

      [SetUp]
      public void SetUp() {
         TestHelper.SetUpTests();
      }


      private static bool TryExpandEnvironmentVariables(string envVarWith2PercentsChar, out string envVarValue) {
         envVarValue = Environment.ExpandEnvironmentVariables(envVarWith2PercentsChar);
         return envVarValue != envVarWith2PercentsChar; // Replacement only occurs for environment variables that are set. 
      }



      //
      // Test     EnvVarPathResolvingStatus.Success
      //
      [TestCase(@"%TEMP%\Dir", @"\Dir")]
      [TestCase(@"%TEMP%", @"")]
      public void Test_DirOK_CoverAllOverloads(string pathEnvVarString, string extraString) {
         string envVarValue;
         Assert.IsTrue(TryExpandEnvironmentVariables(@"%TEMP%", out envVarValue));
         Assert.IsTrue(envVarValue.IsValidAbsoluteDirectoryPath());

         var pathEnvVar = pathEnvVarString.ToEnvVarDirectoryPath();

         //
         // Cover IAbsoluteDirectoryPath
         //
         IAbsoluteDirectoryPath pathDirAbsolute;
         var result = pathEnvVar.TryResolve(out pathDirAbsolute);
         Assert.IsTrue(result == EnvVarPathResolvingStatus.Success);
         Assert.IsTrue(pathDirAbsolute.ToString() == envVarValue + extraString);

         string failureReason;
         Assert.IsTrue(pathEnvVar.TryResolve(out pathDirAbsolute, out failureReason));
         Assert.IsTrue(pathDirAbsolute.ToString() == envVarValue + extraString);


         //
         // Cover IAbsolutPath
         //
         IAbsolutePath pathAbsolute;
         result = pathEnvVar.TryResolve(out pathAbsolute);
         Assert.IsTrue(result == EnvVarPathResolvingStatus.Success);
         Assert.IsTrue(pathAbsolute.ToString() == envVarValue + extraString);
         Assert.IsTrue(pathAbsolute.IsDirectoryPath);

         Assert.IsTrue(pathEnvVar.TryResolve(out pathAbsolute, out failureReason));
         Assert.IsTrue(pathAbsolute.ToString() == envVarValue + extraString);
         Assert.IsTrue(pathAbsolute.IsDirectoryPath);
      }


      [TestCase(@"%TEMP%\Dir\File.txt", @"\Dir\File.txt", @"File.txt")]
      [TestCase(@"%TEMP%\File.txt", @"\File.txt", @"File.txt")]
      public void Test_FileOK_CoverAllOverloads(string pathEnvVarString, string extraString, string fileName) {
         string envVarValue;
         Assert.IsTrue(TryExpandEnvironmentVariables(@"%TEMP%", out envVarValue));
         Assert.IsTrue(envVarValue.IsValidAbsoluteDirectoryPath());

         var pathEnvVar = pathEnvVarString.ToEnvVarFilePath();

         //
         // Cover IAbsoluteFilePath
         //
         IAbsoluteFilePath pathFileAbsolute;
         var result = pathEnvVar.TryResolve(out pathFileAbsolute);
         Assert.IsTrue(result == EnvVarPathResolvingStatus.Success);
         Assert.IsTrue(pathFileAbsolute.ToString() == envVarValue + extraString);
         Assert.IsTrue(pathFileAbsolute.FileName == fileName);

         string failureReason;
         Assert.IsTrue(pathEnvVar.TryResolve(out pathFileAbsolute, out failureReason));
         Assert.IsTrue(pathFileAbsolute.ToString() == envVarValue + extraString);
         Assert.IsTrue(pathFileAbsolute.FileName == fileName);


         //
         // Cover IAbsolutPath
         //
         IAbsolutePath pathAbsolute;
         result = pathEnvVar.TryResolve(out pathAbsolute);
         Assert.IsTrue(result == EnvVarPathResolvingStatus.Success);
         Assert.IsTrue(pathAbsolute.ToString() == envVarValue + extraString);
         Assert.IsTrue(pathAbsolute.IsFilePath);

         Assert.IsTrue(pathEnvVar.TryResolve(out pathAbsolute, out failureReason));
         Assert.IsTrue(pathAbsolute.ToString() == envVarValue + extraString);
         Assert.IsTrue(pathAbsolute.IsFilePath);
      }




      //
      // Test     EnvVarPathResolvingStatus.ErrorUnresolvedEnvVar
      //
      [TestCase(@"%UNEXISTING_ENVVAR%\Dir")]
      [TestCase(@"%UNEXISTING_ENVVAR%")]
      public void Test_DirKO_UnresolvedEnvVar_CoverAllOverloads(string pathEnvVarString) {
         string envVarValue;
         Assert.IsFalse(TryExpandEnvironmentVariables(@"%UNEXISTING_ENVVAR%", out envVarValue));

         var pathEnvVar = pathEnvVarString.ToEnvVarDirectoryPath();

         //
         // Cover IAbsoluteDirectoryPath
         //
         IAbsoluteDirectoryPath pathDirAbsolute;
         var result = pathEnvVar.TryResolve(out pathDirAbsolute);
         Assert.IsTrue(result == EnvVarPathResolvingStatus.ErrorUnresolvedEnvVar);

         string failureReason, failureReason1;
         Assert.IsFalse(pathEnvVar.TryResolve(out pathDirAbsolute, out failureReason));
         Assert.IsTrue(failureReason == "Can't resolve the environment variable %UNEXISTING_ENVVAR%.");

         //
         // Cover IAbsolutPath
         //
         IAbsolutePath pathAbsolute;
         result = pathEnvVar.TryResolve(out pathAbsolute);
         Assert.IsTrue(result == EnvVarPathResolvingStatus.ErrorUnresolvedEnvVar);

         Assert.IsFalse(pathEnvVar.TryResolve(out pathAbsolute, out failureReason1));
         Assert.IsTrue(failureReason == failureReason1);
      }


      [TestCase(@"%UNEXISTING_ENVVAR%\Dir\File.txt")]
      [TestCase(@"%UNEXISTING_ENVVAR%\File.txt")]
      public void Test_FileKO_UnresolvedEnvVar_CoverAllOverloads(string pathEnvVarString) {
         string envVarValue;
         Assert.IsFalse(TryExpandEnvironmentVariables(@"%UNEXISTING_ENVVAR%", out envVarValue));

         var pathEnvVar = pathEnvVarString.ToEnvVarFilePath();

         //
         // Cover IAbsoluteFilePath
         //
         IAbsoluteFilePath pathFileAbsolute;
         var result = pathEnvVar.TryResolve(out pathFileAbsolute);
         Assert.IsTrue(result == EnvVarPathResolvingStatus.ErrorUnresolvedEnvVar);

         string failureReason, failureReason1;
         Assert.IsFalse(pathEnvVar.TryResolve(out pathFileAbsolute, out failureReason));
         Assert.IsTrue(failureReason == "Can't resolve the environment variable %UNEXISTING_ENVVAR%.");

         //
         // Cover IAbsolutPath
         //
         IAbsolutePath pathAbsolute;
         result = pathEnvVar.TryResolve(out pathAbsolute);
         Assert.IsTrue(result == EnvVarPathResolvingStatus.ErrorUnresolvedEnvVar);

         Assert.IsFalse(pathEnvVar.TryResolve(out pathAbsolute, out failureReason1));
         Assert.IsTrue(failureReason == failureReason1);
      }





      //
      // Test     EnvVarPathResolvingStatus.ErrorEnvVarResolvedButCannotConvertToAbsolutePath
      //

      [TestCase(@"%NUMBER_OF_PROCESSORS%\Dir")]
      [TestCase(@"%NUMBER_OF_PROCESSORS%")]
      public void Test_DirKO_NoAbsoluteEnvVarValue_CoverAllOverloads(string pathEnvVarString) {
         string envVarValue;
         Assert.IsTrue(TryExpandEnvironmentVariables(@"%NUMBER_OF_PROCESSORS%", out envVarValue));

         var pathEnvVar = pathEnvVarString.ToEnvVarDirectoryPath();

         //
         // Cover IAbsoluteDirectoryPath
         //
         IAbsoluteDirectoryPath pathDirAbsolute;
         var result = pathEnvVar.TryResolve(out pathDirAbsolute);
         Assert.IsTrue(result == EnvVarPathResolvingStatus.ErrorEnvVarResolvedButCannotConvertToAbsolutePath);

         string failureReason, failureReason1;
         Assert.IsFalse(pathEnvVar.TryResolve(out pathDirAbsolute, out failureReason));
         Assert.IsTrue(failureReason == "The environment variable %NUMBER_OF_PROCESSORS% is resolved into the value {4} but this value cannot be the prefix of an absolute path.");

         //
         // Cover IAbsolutPath
         //
         IAbsolutePath pathAbsolute;
         result = pathEnvVar.TryResolve(out pathAbsolute);
         Assert.IsTrue(result == EnvVarPathResolvingStatus.ErrorEnvVarResolvedButCannotConvertToAbsolutePath);

         Assert.IsFalse(pathEnvVar.TryResolve(out pathAbsolute, out failureReason1));
         Assert.IsTrue(failureReason == failureReason1);
      }


      [TestCase(@"%NUMBER_OF_PROCESSORS%\Dir\File.txt")]
      [TestCase(@"%NUMBER_OF_PROCESSORS%\File.txt")]
      public void Test_FileKO_NoAbsoluteEnvVarValue_CoverAllOverloads(string pathEnvVarString) {
         string envVarValue;
         Assert.IsTrue(TryExpandEnvironmentVariables(@"%NUMBER_OF_PROCESSORS%", out envVarValue));

         var pathEnvVar = pathEnvVarString.ToEnvVarFilePath();

         //
         // Cover IAbsoluteFilePath
         //
         IAbsoluteFilePath pathFileAbsolute;
         var result = pathEnvVar.TryResolve(out pathFileAbsolute);
         Assert.IsTrue(result == EnvVarPathResolvingStatus.ErrorEnvVarResolvedButCannotConvertToAbsolutePath);

         string failureReason, failureReason1;
         Assert.IsFalse(pathEnvVar.TryResolve(out pathFileAbsolute, out failureReason));
         Assert.IsTrue(failureReason == "The environment variable %NUMBER_OF_PROCESSORS% is resolved into the value {4} but this value cannot be the prefix of an absolute path.");

         //
         // Cover IAbsolutPath
         //
         IAbsolutePath pathAbsolute;
         result = pathEnvVar.TryResolve(out pathAbsolute);
         Assert.IsTrue(result == EnvVarPathResolvingStatus.ErrorEnvVarResolvedButCannotConvertToAbsolutePath);

         Assert.IsFalse(pathEnvVar.TryResolve(out pathAbsolute, out failureReason1));
         Assert.IsTrue(failureReason == failureReason1);
      }

   }
}
