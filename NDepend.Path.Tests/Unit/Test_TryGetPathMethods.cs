using NDepend.Test;
using NUnit.Framework;

namespace NDepend.Path {

   [TestFixture]
   public class Test_TryGetPathMethods {

      [SetUp]
      public void SetUp() {
         TestHelper.SetUpTests();
      }

  


      [TestCase(@"C:", PathMode.Absolute)]
      [TestCase(@"\\Server\Share", PathMode.Absolute)]
      [TestCase(@"..", PathMode.Relative)]
      [TestCase(@"%ENVAR%", PathMode.EnvVar)]
      [TestCase(@"$(Var)", PathMode.Variable)]
      public void Test_TryGetAbsolute_OK(string pathString, PathMode pathMode) {
         string failureReason;

         var isAbsolute = pathMode == PathMode.Absolute;
         IAbsoluteDirectoryPath absoluteDirectoryPath;
         Assert.IsTrue(pathString.TryGetAbsoluteDirectoryPath(out absoluteDirectoryPath) == isAbsolute);
         Assert.IsTrue(pathString.TryGetAbsoluteDirectoryPath(out absoluteDirectoryPath, out failureReason) == isAbsolute);

         var isRelative = pathMode == PathMode.Relative;
         IRelativeDirectoryPath relativeDirectoryPath;
         Assert.IsTrue(pathString.TryGetRelativeDirectoryPath(out relativeDirectoryPath) == isRelative);
         Assert.IsTrue(pathString.TryGetRelativeDirectoryPath(out relativeDirectoryPath, out failureReason) == isRelative);

         var isEnvVar = pathMode == PathMode.EnvVar;
         IEnvVarDirectoryPath envVarDirectoryPath;
         Assert.IsTrue(pathString.TryGetEnvVarDirectoryPath(out envVarDirectoryPath) == isEnvVar);
         Assert.IsTrue(pathString.TryGetEnvVarDirectoryPath(out envVarDirectoryPath, out failureReason) == isEnvVar);

         var isVariable = pathMode == PathMode.Variable;
         IVariableDirectoryPath variableDirectoryPath;
         Assert.IsTrue(pathString.TryGetVariableDirectoryPath(out variableDirectoryPath) == isVariable);
         Assert.IsTrue(pathString.TryGetVariableDirectoryPath(out variableDirectoryPath, out failureReason) == isVariable);

         IDirectoryPath directoryPath;
         Assert.IsTrue(pathString.TryGetDirectoryPath(out directoryPath));
         Assert.IsTrue(pathString.TryGetDirectoryPath(out directoryPath, out failureReason));


         var pathStringFile = pathString + @"\File.txt";

         IAbsoluteFilePath absoluteFilePath;
         Assert.IsTrue(pathStringFile.TryGetAbsoluteFilePath(out absoluteFilePath) == isAbsolute);
         Assert.IsTrue(pathStringFile.TryGetAbsoluteFilePath(out absoluteFilePath, out failureReason) == isAbsolute);
         Assert.IsFalse(pathString.TryGetAbsoluteFilePath(out absoluteFilePath));
         Assert.IsFalse(pathString.TryGetAbsoluteFilePath(out absoluteFilePath, out failureReason));

         IRelativeFilePath relativeFilePath;
         Assert.IsTrue(pathStringFile.TryGetRelativeFilePath(out relativeFilePath) == isRelative);
         Assert.IsTrue(pathStringFile.TryGetRelativeFilePath(out relativeFilePath, out failureReason) == isRelative);
         Assert.IsFalse(pathString.TryGetRelativeFilePath(out relativeFilePath));
         Assert.IsFalse(pathString.TryGetRelativeFilePath(out relativeFilePath, out failureReason));

         IEnvVarFilePath envVarFilePath;
         Assert.IsTrue(pathStringFile.TryGetEnvVarFilePath(out envVarFilePath) == isEnvVar);
         Assert.IsTrue(pathStringFile.TryGetEnvVarFilePath(out envVarFilePath, out failureReason) == isEnvVar);
         Assert.IsFalse(pathString.TryGetEnvVarFilePath(out envVarFilePath));
         Assert.IsFalse(pathString.TryGetEnvVarFilePath(out envVarFilePath, out failureReason));

         IVariableFilePath variableFilePath;
         Assert.IsTrue(pathStringFile.TryGetVariableFilePath(out variableFilePath) == isVariable);
         Assert.IsTrue(pathStringFile.TryGetVariableFilePath(out variableFilePath, out failureReason) == isVariable);
         Assert.IsFalse(pathString.TryGetVariableFilePath(out variableFilePath));
         Assert.IsFalse(pathString.TryGetVariableFilePath(out variableFilePath, out failureReason));

         IFilePath filePath;
         Assert.IsTrue(pathStringFile.TryGetFilePath(out filePath));
         Assert.IsTrue(pathStringFile.TryGetFilePath(out filePath, out failureReason));
         Assert.IsFalse(pathString.TryGetFilePath(out filePath));
         Assert.IsFalse(pathString.TryGetFilePath(out filePath, out failureReason));
      }





      [TestCase((string)null, "The parameter pathString is null.")]
      [TestCase("", "The parameter pathString is empty.")]
      public void Test_Null(string pathString, string error) {
         string failureReason;

         IAbsoluteDirectoryPath absoluteDirectoryPath;
         Assert.IsFalse(pathString.TryGetAbsoluteDirectoryPath(out absoluteDirectoryPath));
         Assert.IsFalse(pathString.TryGetAbsoluteDirectoryPath(out absoluteDirectoryPath, out failureReason));
         Assert.IsTrue(failureReason == error);

         IRelativeDirectoryPath relativeDirectoryPath;
         Assert.IsFalse(pathString.TryGetRelativeDirectoryPath(out relativeDirectoryPath));
         Assert.IsFalse(pathString.TryGetRelativeDirectoryPath(out relativeDirectoryPath, out failureReason));
         Assert.IsTrue(failureReason == error);

         IEnvVarDirectoryPath envVarDirectoryPath;
         Assert.IsFalse(pathString.TryGetEnvVarDirectoryPath(out envVarDirectoryPath));
         Assert.IsFalse(pathString.TryGetEnvVarDirectoryPath(out envVarDirectoryPath, out failureReason));
         Assert.IsTrue(failureReason == error);

         IVariableDirectoryPath variableDirectoryPath;
         Assert.IsFalse(pathString.TryGetVariableDirectoryPath(out variableDirectoryPath));
         Assert.IsFalse(pathString.TryGetVariableDirectoryPath(out variableDirectoryPath, out failureReason));
         Assert.IsTrue(failureReason == error);

         IDirectoryPath directoryPath;
         Assert.IsFalse(pathString.TryGetDirectoryPath(out directoryPath));
         Assert.IsFalse(pathString.TryGetDirectoryPath(out directoryPath, out failureReason));
         Assert.IsTrue(failureReason == error);


         IAbsoluteFilePath absoluteFilePath;
         Assert.IsFalse(pathString.TryGetAbsoluteFilePath(out absoluteFilePath));
         Assert.IsFalse(pathString.TryGetAbsoluteFilePath(out absoluteFilePath, out failureReason));
         Assert.IsTrue(failureReason == error);

         IRelativeFilePath relativeFilePath;
         Assert.IsFalse(pathString.TryGetRelativeFilePath(out relativeFilePath));
         Assert.IsFalse(pathString.TryGetRelativeFilePath(out relativeFilePath, out failureReason));
         Assert.IsTrue(failureReason == error);

         IEnvVarFilePath envVarFilePath;
         Assert.IsFalse(pathString.TryGetEnvVarFilePath(out envVarFilePath));
         Assert.IsFalse(pathString.TryGetEnvVarFilePath(out envVarFilePath, out failureReason));
         Assert.IsTrue(failureReason == error);

         IVariableFilePath variableFilePath;
         Assert.IsFalse(pathString.TryGetVariableFilePath(out variableFilePath));
         Assert.IsFalse(pathString.TryGetVariableFilePath(out variableFilePath, out failureReason));
         Assert.IsTrue(failureReason == error);

         IFilePath filePath;
         Assert.IsFalse(pathString.TryGetFilePath(out filePath));
         Assert.IsFalse(pathString.TryGetFilePath(out filePath, out failureReason));
         Assert.IsTrue(failureReason == error);
      }
   }
}
