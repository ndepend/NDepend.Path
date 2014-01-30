using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using NDepend.Helpers;
using NDepend.Test;
using NUnit.Framework;

namespace NDepend.Path {

   [TestFixture]
   public class Test_VariablePath {

      [SetUp]
      public void SetUp() {
         TestHelper.SetUpTests();
      }

      [TestCase(" ", false)]
      [TestCase("A", true)]
      [TestCase("a", true)]
      [TestCase("1", true)]
      [TestCase("_", true)]
      [TestCase("ABc12_", true)]
      [TestCase("ABc12_%", false)]
      public void Test_IsValidPathVariableName(string s, bool b) {
         Assert.IsTrue(s.IsValidPathVariableName() == b);
      }

      [Test]
      public void Test_IsValidPathVariableName_1024Char() {
         Assert.IsTrue(new string('c', 1024).IsValidPathVariableName());
         Assert.IsFalse(new string('c', 1025).IsValidPathVariableName());
      }


      [TestCase("", true)]
      [TestCase(" ", true)]
      [TestCase("A", true)]
      [TestCase("a", true)]
      [TestCase("*", false)]
      [TestCase("a?a", false)]
      public void Test_IsValidPathVariableValue(string s, bool b) {
         Assert.IsTrue(s.IsValidPathVariableValue() == b);
      }

      [Test]
      public void Test_IsValidPathVariableValue_1024Char() {
         Assert.IsTrue(new string('c', 1024).IsValidPathVariableValue());
         Assert.IsFalse(new string('c', 1025).IsValidPathVariableValue());
      }


      [TestCase(@"$(v)", @"$(v)", "v")]
      [TestCase(@"$(v)\", @"$(v)", "v")]
      [TestCase(@"$(v)\\\", @"$(v)", "v")]
      [TestCase(@"$(v)\$(v)", @"$(v)\$(v)", "v")]
      [TestCase(@"$(v)\$(V)", @"$(v)\$(V)", "v")]
      [TestCase(@"$(v)\\\$(V)\\\\\", @"$(v)\$(V)", "v")]
      [TestCase(@"$(v1)$(v2_3)", @"$(v1)$(v2_3)", "v1;v2_3")]
      [TestCase(@"$(v1)\$(v2)", @"$(v1)\$(v2)", "v1;v2")]
      [TestCase(@"$(v1)\\\$(v2)", @"$(v1)\$(v2)", "v1;v2")]
      [TestCase(@"$(v1)\\\$(v2)$(v2)", @"$(v1)\$(v2)$(v2)", "v1;v2")]
      [TestCase(@"$(v1)\\\$(v2)$(vvvv3)", @"$(v1)\$(v2)$(vvvv3)", "v1;v2;vvvv3")]
      [TestCase(@"$(v1)\$(v2)\..", @"$(v1)", "v1")]  // Important here, v2 disappeared!!
      [TestCase(@"$(v1)\$(v2)\..\$(v3)\..", @"$(v1)", "v1")]
      [TestCase(@"$(v1)\$(v2)\$(v3)\..\..", @"$(v1)", "v1")]
      public void Test_VariablePath_DirAndFileOK(string path, string pathNormalized, string variables) {
         Assert.IsTrue(path.IsValidVariableDirectoryPath());
         var dirPath = path.ToVariableDirectoryPath();
         Assert.IsFalse(dirPath.HasParentDirectory); // None of these path has parent dir!

         CheckVariables(variables, dirPath);
         Assert.IsTrue(dirPath.ToString() == pathNormalized);

         //
         // Test fileOK by adding "\File.txt !"
         //
         var filePath = (path + @"\File.txt").ToVariableFilePath();

         CheckVariables(variables, filePath);
         Assert.IsTrue(filePath.ToString() == pathNormalized + @"\File.txt");
         Assert.IsTrue(filePath.FileName == "File.txt");
         Assert.IsTrue(filePath.FileNameWithoutExtension == "File");
         Assert.IsTrue(filePath.FileExtension == ".txt");
         Assert.IsTrue(filePath.HasExtension(".TxT"));
      }

      private static void CheckVariables(string variables, IVariablePath path) {
         var allVariables1 = variables.Split(';');
         var allVariables2 = path.AllVariables;
         Assert.IsTrue(allVariables1.Length == allVariables2.Count);
         for (var i = 0; i < allVariables1.Length; i++) {
            var var1 = allVariables1[i];
            var var2 = allVariables2[i];
            Assert.IsTrue(var1 == var2);
         }
         Assert.IsTrue(path.PrefixVariable == allVariables1[0]);
      }


      [TestCase(@"", "The parameter pathString is empty.")]
      [TestCase(@"C:\", "A variable with the syntax $(variableName) must be defined at the beginning of the path string.")]
      [TestCase(@".\Dir", "A variable with the syntax $(variableName) must be defined at the beginning of the path string.")]
      [TestCase(@"$(var", @"Variable syntax error : Found variable opening ""$("" without closing parenthesis, at position 0.")]
      [TestCase(@"$(var)$(variable", @"Variable syntax error : Found variable opening ""$("" without closing parenthesis, at position 6.")]
      [TestCase(@"$(var)$(variable+)", @"Variable syntax error : Found variable with name $(variable+). A variable name must contain only upper/lower case letters, digits and underscore characters.")]
      [TestCase(@"$()", @"Variable syntax error : Found variable with empty name at position 0.")]
      [TestCase(@"$(v)$()", @"Variable syntax error : Found variable with empty name at position 4.")]
      [TestCase(@"$(v)\..\..\..\Dir", @"The pathString {$(v)\..\..\..\Dir} references the parent dir \..\ of the root dir {$(v)}, it cannot be resolved.")]
      [TestCase(@"$(v)\..\Dir", @"The pathString {$(v)\..\Dir} references the parent dir \..\ of the root dir {$(v)}, it cannot be resolved.")]
      [TestCase(@"$(v)\Dir1\Dir2\..\..\..", @"The pathString {$(v)\Dir1\Dir2\..\..\..} references the parent dir \..\ of the root dir {$(v)}, it cannot be resolved.")]
      public void Test_VariablePath_DirKO(string path, string failureReason1) {
         Assert.IsFalse(path.IsValidVariableDirectoryPath());
         string failureReason2;
         Assert.IsFalse(path.IsValidVariableDirectoryPath(out failureReason2));

         Assert.IsTrue(failureReason1 == failureReason2);
         var exThrown = false;
         try {
            var dirPath = path.ToVariableDirectoryPath();
         }
         catch (ArgumentException ex) {
            Assert.IsTrue(ex.Message.StartsWith(failureReason1));
            exThrown = true;
         }
         Assert.IsTrue(exThrown);

         //
         // Test FileKO as well!
         //
         Assert.IsFalse(path.IsValidVariableFilePath());
         Assert.IsFalse(path.IsValidVariableFilePath(out failureReason2));
         Assert.IsTrue(failureReason1 == failureReason2);
         exThrown = false;
         try {
            var filePath = path.ToVariableFilePath();
         } catch (ArgumentException ex) {
            Assert.IsTrue(ex.Message.StartsWith(failureReason1));
            exThrown = true;
         }
         Assert.IsTrue(exThrown);
      }

      [TestCase(@"$(var)")]
      [TestCase(@"$(var)$(var2)")]
      [TestCase(@"$(var1)\$(var2)")]
      public void Test_VariablePath_DirOK_FileKO(string path) {
         const string failureReason1 = @"The parameter pathString is not a file path because it doesn't have at least one parent directory.";
         Assert.IsFalse(path.IsValidVariableFilePath());
         Assert.IsTrue(path.IsValidVariableDirectoryPath());
         string failureReason2;
         Assert.IsFalse(path.IsValidVariableFilePath(out failureReason2));
         Assert.IsTrue(failureReason1 == failureReason2);
         var exThrown = false;
         try {
            var filePath = path.ToVariableFilePath();
         } catch (ArgumentException ex) {
            Assert.IsTrue(ex.Message.StartsWith(failureReason1));
            exThrown = true;
         }
         Assert.IsTrue(exThrown);
      }

      [Test]
      public void Test_VariablePath_Dir_PathStringNull() {
         string path = null;
         Assert.IsFalse(path.IsValidVariableDirectoryPath());
         string failureReason2;
         Assert.IsFalse(path.IsValidVariableDirectoryPath(out failureReason2));
         Assert.IsTrue(failureReason2 == "The parameter pathString is null.");
         var exThrown = false;
         try {
            var dirPath = path.ToVariableDirectoryPath();
         } catch (ArgumentNullException) {
            exThrown = true;
         }
         Assert.IsTrue(exThrown);
      }

      [Test]
      public void Test_VariablePath_File_PathStringNull() {
         string path = null;
         Assert.IsFalse(path.IsValidVariableFilePath());
         string failureReason2;
         Assert.IsFalse(path.IsValidVariableFilePath(out failureReason2));
         Assert.IsTrue(failureReason2 == "The parameter pathString is null.");
         var exThrown = false;
         try {
            var filePath = path.ToVariableFilePath();
         } catch (ArgumentNullException) {
            exThrown = true;
         }
         Assert.IsTrue(exThrown);
      }


      [TestCase(@"$(var)\Dir", "Dir", @"$(var)", @"var")]
      [TestCase(@"$(var)\Dir\Dir1\..", "Dir", @"$(var)", @"var")]
      [TestCase(@"$(var)$(var2)\Dir\Dir1\/DIR2//", "DIR2", @"$(var)$(var2)\Dir\Dir1", @"var;var2")]
      public void Test_Variable_Dir_ParentDirectoryOk(string path, string dirName, string parentDirPathString, string variables) {
         var dirPath = path.ToVariableDirectoryPath();
         Assert.IsTrue(dirPath.HasParentDirectory);
         Assert.IsTrue(dirPath.DirectoryName == dirName);
         var parentDirPath1 = dirPath.ParentDirectoryPath;
         Assert.IsTrue(parentDirPath1.ToString() == parentDirPathString);
         var parentDirPath2 = (dirPath as IDirectoryPath).ParentDirectoryPath;
         Assert.IsTrue(parentDirPath2.ToString() == parentDirPathString);

         CheckVariables(variables, dirPath);
         CheckVariables(variables, parentDirPath1);
      }

      [TestCase(@"$(var)\File.txt", "Dir", @"$(var)", @"var")]
      [TestCase(@"$(var)\Dir\Dir1\..\File.txt", "Dir", @"$(var)\Dir", @"var")]
      [TestCase(@"$(var)$(var2)\Dir\Dir1\/DIR2//File.txt", "DIR2", @"$(var)$(var2)\Dir\Dir1\DIR2", @"var;var2")]
      public void Test_Variable_File_ParentDirectoryOk(string path, string dirName, string parentDirPathString, string variables) {
         var filePath = path.ToVariableFilePath();
         Assert.IsTrue(filePath.HasParentDirectory);
         Assert.IsTrue(filePath.FileName == "File.txt");

         var parentDirPath1 = filePath.ParentDirectoryPath;
         Assert.IsTrue(parentDirPath1.ToString() == parentDirPathString);
         var parentDirPath2 = (filePath as IFilePath).ParentDirectoryPath;
         Assert.IsTrue(parentDirPath2.ToString() == parentDirPathString);

         CheckVariables(variables, filePath);
         CheckVariables(variables, parentDirPath1);
      }

      [TestCase(@"$(Var)")]
      [TestCase(@"$(Var)\$(Var2)")]
      [TestCase(@"$(Var)\$(Var2)\")]
      public void Test_Variable_Dir_ParentDirectoryKO(string str) {
         var variablePath = str.ToVariableDirectoryPath();
         Assert.IsFalse(variablePath.HasParentDirectory);

         bool exThrown = false;
         try {
            var parentDirPath = variablePath.ParentDirectoryPath;
         } catch (InvalidOperationException ex) {
            exThrown = true;
            Assert.IsTrue(ex.Message == @"Can't get the parent dir from the pathString """ + variablePath.ToString() + @"""");
         }
         Assert.IsTrue(exThrown);
      }



      [TestCase(@"$(SolutionDir)DirA", @"C:\DirB\DirA")]
      [TestCase(@"$(SolutionDir)\DirA", @"C:\DirB\DirA")]
      [TestCase(@"$(SolutionDir)\$(var1)\$(var2)", @"C:\DirB\var1\var2\Var3")]
      [TestCase(@"$(SolutionDir)\$(var1)//\$(VAR1)/$(VaR2)//", @"C:\DirB\var1\var1\var2\Var3")]
      [TestCase(@"$(SolutionDir)\$(var3)", @"C:\DirB")]
      [TestCase(@"$(SolutionDir)\$(var3)\$(var1)", @"C:\DirB\var1")]
      [TestCase(@"$(SolutionDir)\$(var4)", @"C:\DirB")]
      [TestCase(@"$(SolutionDir)\$(var4)\$(var1)", @"C:\DirB\var1")]
      [TestCase(@"$(SolutionDir)\$(var4)\$(var3)", @"C:\DirB")]
      public void Test_Resolving_Dir_File_OK(string pathIn, string pathResolved) {
         

         var dico = new Dictionary<string, string> {
            {"SolutionDir", @"C:\DirB\"},
            {"VAR1", @"var1"},
            {"VAR2", @"var2\Var3"},
            {"var3", null}, // Null and empty value supported!
            {"var4", ""}
         };

         // Dir
         {
            var variablePath = pathIn.ToVariableDirectoryPath();
            IAbsoluteDirectoryPath path1, path2, path3;
            Assert.IsTrue(variablePath.TryResolve(dico, out path1) == VariablePathResolvingStatus.Success);
            Assert.IsTrue(path1.ToString() == pathResolved);

            IReadOnlyList<string> unresolvedVariables;
            Assert.IsTrue(variablePath.TryResolve(dico, out path2, out unresolvedVariables) ==
                          VariablePathResolvingStatus.Success);
            Assert.IsNull(unresolvedVariables);
            Assert.IsTrue(path2.Equals(path1));

            string failureReason;
            Assert.IsTrue(variablePath.TryResolve(dico, out path3, out failureReason));
            Assert.IsNull(unresolvedVariables);
            Assert.IsTrue(path3.Equals(path1));
         }

         // File
         {
            var variablePath = (pathIn + @"\File.txt").ToVariableFilePath();
            IAbsoluteFilePath path1, path2, path3;
            Assert.IsTrue(variablePath.TryResolve(dico, out path1) == VariablePathResolvingStatus.Success);
            Assert.IsTrue(path1.ToString() == pathResolved + @"\File.txt");

            IReadOnlyList<string> unresolvedVariables;
            Assert.IsTrue(variablePath.TryResolve(dico, out path2, out unresolvedVariables) ==
                          VariablePathResolvingStatus.Success);
            Assert.IsNull(unresolvedVariables);
            Assert.IsTrue(path2.Equals(path1));

            string failureReason;
            Assert.IsTrue(variablePath.TryResolve(dico, out path3, out failureReason));
            Assert.IsNull(failureReason);
            Assert.IsTrue(path3.Equals(path1));
         }


         // Dir Untyped
         {
            IVariablePath variablePath = pathIn.ToVariableDirectoryPath();
            IAbsolutePath path1, path2, path3;
            Assert.IsTrue(variablePath.TryResolve(dico, out path1) == VariablePathResolvingStatus.Success);
            Assert.IsTrue(path1.ToString() == pathResolved);

            IReadOnlyList<string> unresolvedVariables;
            Assert.IsTrue(variablePath.TryResolve(dico, out path2, out unresolvedVariables) ==
                          VariablePathResolvingStatus.Success);
            Assert.IsNull(unresolvedVariables);
            Assert.IsTrue(path2.Equals(path1));

            string failureReason;
            Assert.IsTrue(variablePath.TryResolve(dico, out path3, out failureReason));
            Assert.IsNull(unresolvedVariables);
            Assert.IsTrue(path3.Equals(path1));
         }

         // File Untyped
         {
            IVariablePath variablePath = (pathIn + @"\File.txt").ToVariableFilePath();
            IAbsolutePath path1, path2, path3;
            Assert.IsTrue(variablePath.TryResolve(dico, out path1) == VariablePathResolvingStatus.Success);
            Assert.IsTrue(path1.ToString() == pathResolved + @"\File.txt");

            IReadOnlyList<string> unresolvedVariables;
            Assert.IsTrue(variablePath.TryResolve(dico, out path2, out unresolvedVariables) ==
                          VariablePathResolvingStatus.Success);
            Assert.IsNull(unresolvedVariables);
            Assert.IsTrue(path2.Equals(path1));

            string failureReason;
            Assert.IsTrue(variablePath.TryResolve(dico, out path3, out failureReason));
            Assert.IsNull(failureReason);
            Assert.IsTrue(path3.Equals(path1));
         }
      }



      [TestCase(null, false, "")]
      [TestCase(@"$(var)", false, "")]
      [TestCase(@"$(var)\Dir1", true, "Dir1")]
      [TestCase(@"$(var)\File.txt", true, "File.txt")]
      [TestCase(@"$(var)\Dir1\File.txt", true, "File.txt")]
      [TestCase(@"$(var)\$(var)", false, "")]
      [TestCase(@"$(var)\Fi$(var)le.txt", false, "")]
      [TestCase(@"\\", false, "")]
      [TestCase(@"\\server", false, "")]
      [TestCase(@"\\server\share\", false, "")]
      [TestCase(@"$(var)\aa\.", false, "")]
      [TestCase(@"$(var)\bb\aa\.", false, "")]
      [TestCase(@"$(var)\server\share\aa\.", false, "")]
      [TestCase(@"$(var)\server\share\bb\aa\.", false, "")]
      [TestCase(@"C:\dir\file", false, "")]
      [TestCase(@"%E%\dir\file", false, "")]
      [TestCase(@"%%E%\dir\file", false, "")]
      [TestCase(@"$$(var)\file", false, "")]

      // Don't allow fileName to contains "$(" even if in theory it'd be possible
      // But user shouldn't mess up with variable path containing the string "$(".
      [TestCase(@"$(var)\$(var", false, "")]
      [TestCase(@"$(var)\dir\d$(", false, "")]
      [TestCase(@"$(var)\dir\d$d", true, "d$d")] 
      public void Test_IsValidVariableFilePath(string pathString, bool isValid, string fileName) {
         var b = pathString.IsValidVariableFilePath();
         Assert.IsTrue(b == isValid);
         if (b) {
            Assert.IsTrue(pathString.ToVariableFilePath().FileName == fileName);
         }
      }


      //
      // Test ErrorUnresolvedVariable KO!
      //
      [Test]
      public void Test_Resolving_Dir_1KO() {
         var dico = new Dictionary<string, string> {
            {"SolutionDir", @"C:\DirB\"},
            {"VAR1", @"var1"}
         };

         var variablePath = @"$(SolutionDir)\$(var1)\$(var2)".ToVariableDirectoryPath();
         IAbsoluteDirectoryPath path1, path2, path3;
         Assert.IsTrue(variablePath.TryResolve(dico, out path1) == VariablePathResolvingStatus.ErrorUnresolvedVariable);

         IReadOnlyList<string> unresolvedVariables;
         Assert.IsTrue(variablePath.TryResolve(dico, out path2, out unresolvedVariables) == VariablePathResolvingStatus.ErrorUnresolvedVariable);
         Assert.IsNotNull(unresolvedVariables);
         Assert.IsTrue(unresolvedVariables.Count == 1);
         Assert.IsTrue(unresolvedVariables[0] == @"var2");

         string failureReason;
         Assert.IsFalse(variablePath.TryResolve(dico, out path3, out failureReason));
         Assert.IsTrue(failureReason == @"The following variable cannot be resolved: $(var2)");
      }

      [Test]
      public void Test_Resolving_Dir_3KO() {
         var dico = new Dictionary<string, string>();

         var variablePath = @"$(SolutionDir)\$(var1)\$(var2)".ToVariableDirectoryPath();
         IAbsoluteDirectoryPath path1, path2, path3;
         Assert.IsTrue(variablePath.TryResolve(dico, out path1) == VariablePathResolvingStatus.ErrorUnresolvedVariable);
         
         IReadOnlyList<string> unresolvedVariables;
         Assert.IsTrue(variablePath.TryResolve(dico, out path2, out unresolvedVariables) == VariablePathResolvingStatus.ErrorUnresolvedVariable);
         Assert.IsNotNull(unresolvedVariables);
         Assert.IsTrue(unresolvedVariables.Count == 3);
         Assert.IsTrue(unresolvedVariables[0] == @"SolutionDir");
         Assert.IsTrue(unresolvedVariables[1] == @"var1");
         Assert.IsTrue(unresolvedVariables[2] == @"var2");

         string failureReason;
         Assert.IsFalse(variablePath.TryResolve(dico, out path3, out failureReason));
         Assert.IsTrue(failureReason == @"The following variables cannot be resolved: $(SolutionDir) $(var1) $(var2)");
      }

      [Test]
      public void Test_Resolving_File_1KO() {
         var dico = new Dictionary<string, string> {
            {"SolutionDir", @"C:\DirB\"},
            {"VAR1", @"var1"}
         };

         var variablePath = @"$(SolutionDir)\$(var1)\$(var2)\File.txt".ToVariableFilePath();
         IAbsoluteFilePath path1, path2, path3;
         Assert.IsTrue(variablePath.TryResolve(dico, out path1) == VariablePathResolvingStatus.ErrorUnresolvedVariable);

         IReadOnlyList<string> unresolvedVariables;
         Assert.IsTrue(variablePath.TryResolve(dico, out path2, out unresolvedVariables) == VariablePathResolvingStatus.ErrorUnresolvedVariable);
         Assert.IsNotNull(unresolvedVariables);
         Assert.IsTrue(unresolvedVariables.Count == 1);
         Assert.IsTrue(unresolvedVariables[0] == @"var2");

         string failureReason;
         Assert.IsFalse(variablePath.TryResolve(dico, out path3, out failureReason));
         Assert.IsTrue(failureReason == @"The following variable cannot be resolved: $(var2)");
      }

      [Test]
      public void Test_Resolving_File_3KO() {
         var dico = new Dictionary<string, string>();

         var variablePath = @"$(SolutionDir)\$(var1)\$(var2)\File.txt".ToVariableFilePath();
         IAbsoluteFilePath path1, path2, path3;
         Assert.IsTrue(variablePath.TryResolve(dico, out path1) == VariablePathResolvingStatus.ErrorUnresolvedVariable);

         IReadOnlyList<string> unresolvedVariables;
         Assert.IsTrue(variablePath.TryResolve(dico, out path2, out unresolvedVariables) == VariablePathResolvingStatus.ErrorUnresolvedVariable);
         Assert.IsNotNull(unresolvedVariables);
         Assert.IsTrue(unresolvedVariables.Count == 3);
         Assert.IsTrue(unresolvedVariables[0] == @"SolutionDir");
         Assert.IsTrue(unresolvedVariables[1] == @"var1");
         Assert.IsTrue(unresolvedVariables[2] == @"var2");

         string failureReason;
         Assert.IsFalse(variablePath.TryResolve(dico, out path3, out failureReason));
         Assert.IsTrue(failureReason == @"The following variables cannot be resolved: $(SolutionDir) $(var1) $(var2)");
      }


      //
      // Test ErrorVariableResolvedButCannotConvertToAbsolutePath
      //
      [Test]
      public void Test_Resolving_Dir_KO() {
         var dico = new Dictionary<string, string> {
            {"SolutionDir", @"XYZ\DirB\"},
            {"VAR1", @"var1"}
         };

         var variablePath = @"$(SolutionDir)\$(var1)".ToVariableDirectoryPath();
         IAbsoluteDirectoryPath path1, path2, path3;
         Assert.IsTrue(variablePath.TryResolve(dico, out path1) == VariablePathResolvingStatus.ErrorVariableResolvedButCannotConvertToAbsolutePath);

         IReadOnlyList<string> unresolvedVariables;
         Assert.IsTrue(variablePath.TryResolve(dico, out path2, out unresolvedVariables) == VariablePathResolvingStatus.ErrorVariableResolvedButCannotConvertToAbsolutePath);
         Assert.IsNull(unresolvedVariables);
         
         string failureReason;
         Assert.IsFalse(variablePath.TryResolve(dico, out path3, out failureReason));
         Assert.IsTrue(failureReason == @"All variable(s) have been resolved, but the resulting string {XYZ\DirB\\var1} cannot be converted to an absolute directory path.");
      }

      [Test]
      public void Test_Resolving_File_KO() {
         var dico = new Dictionary<string, string> {
            {"SolutionDir", @"XYZ\DirB\"},
            {"VAR1", @"var1"}
         };

         var variablePath = @"$(SolutionDir)\$(var1)\File.txt".ToVariableFilePath();
         IAbsoluteFilePath path1, path2, path3;
         Assert.IsTrue(variablePath.TryResolve(dico, out path1) == VariablePathResolvingStatus.ErrorVariableResolvedButCannotConvertToAbsolutePath);

         IReadOnlyList<string> unresolvedVariables;
         Assert.IsTrue(variablePath.TryResolve(dico, out path2, out unresolvedVariables) == VariablePathResolvingStatus.ErrorVariableResolvedButCannotConvertToAbsolutePath);
         Assert.IsNull(unresolvedVariables);

         string failureReason;
         Assert.IsFalse(variablePath.TryResolve(dico, out path3, out failureReason));
         Assert.IsTrue(failureReason == @"All variable(s) have been resolved, but the resulting string {XYZ\DirB\\var1\File.txt} cannot be converted to an absolute file path.");
      }

      [Test]
      public void Test_Resolving_Dir_KO_CaseNullOrEmptyVariableName() {
         var dico = new List<KeyValuePair<string,string>> {
            new KeyValuePair<string, string>(null, @"XYZ\DirB\"),
            new KeyValuePair<string,string>("", @"var1")
         };

         var variablePath = @"$(SolutionDir)\$(var1)".ToVariableDirectoryPath();
         IAbsoluteDirectoryPath path1;
         Assert.IsTrue(variablePath.TryResolve(dico, out path1) == VariablePathResolvingStatus.ErrorUnresolvedVariable);
      }

      //
      // Path browsing
      //
      [Test]
      public void Test_VariablePathBrowsing_File() {
         var file = @"$(Var)\File.txt".ToVariableFilePath();
         Assert.IsTrue(file.GetBrotherFileWithName("MyFile.exe").ToString() == @"$(Var)\MyFile.exe");
         Assert.IsTrue(file.GetBrotherDirectoryWithName("Dir").ToString() == @"$(Var)\Dir");
         Assert.IsTrue(file.UpdateExtension(".bin").ToString() == @"$(Var)\File.bin");

         // Just a quick test!
         file = @"$(Var)\File".ToVariableFilePath();
         Assert.IsTrue(file.UpdateExtension(".bin").ToString() == @"$(Var)\File.bin");
      }

      [Test]
      public void Test_VariablePathBrowsing_FileUntyped() {
         IFilePath file = @"$(Var)\File.txt".ToVariableFilePath();
         Assert.IsTrue(file.GetBrotherFileWithName("MyFile.exe").ToString() == @"$(Var)\MyFile.exe");
         Assert.IsTrue(file.GetBrotherDirectoryWithName("Dir").ToString() == @"$(Var)\Dir");
         Assert.IsTrue(file.UpdateExtension(".bin").ToString() == @"$(Var)\File.bin");
      }

      [Test]
      public void Test_VariablePathBrowsing_Directory() {
         var dir = @"$(Var)\Dir".ToVariableDirectoryPath();
         Assert.IsTrue(dir.GetBrotherDirectoryWithName("MyDir").ToString() == @"$(Var)\MyDir");
         Assert.IsTrue(dir.GetBrotherFileWithName("File.txt").ToString() == @"$(Var)\File.txt");
         Assert.IsTrue(dir.GetChildDirectoryWithName("MyDir").ToString() == @"$(Var)\Dir\MyDir");
         Assert.IsTrue(dir.GetChildFileWithName("File.txt").ToString() == @"$(Var)\Dir\File.txt");

         var bThrown = false;
         dir = @"$(Var)".ToVariableDirectoryPath();
         try {
            Assert.IsTrue(dir.GetBrotherDirectoryWithName("MyDir").ToString() == @"$(Var)\MyDir");
         }
         catch (InvalidOperationException ex) {
            Assert.IsTrue(ex.Message == @"Can't get the parent dir from the pathString ""$(Var)""");
            bThrown = true;
         }
         Assert.IsTrue(bThrown);
      }

      [Test]
      public void Test_VariablePathBrowsing_DirectoryUntyped() {
         IDirectoryPath dir = @"$(Var)\Dir".ToVariableDirectoryPath();
         Assert.IsTrue(dir.GetBrotherDirectoryWithName("MyDir").ToString() == @"$(Var)\MyDir");
         Assert.IsTrue(dir.GetBrotherFileWithName("File.txt").ToString() == @"$(Var)\File.txt");
         Assert.IsTrue(dir.GetChildDirectoryWithName("MyDir").ToString() == @"$(Var)\Dir\MyDir");
         Assert.IsTrue(dir.GetChildFileWithName("File.txt").ToString() == @"$(Var)\Dir\File.txt");
      }

   }
}

