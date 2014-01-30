

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NDepend.Path {
   [TestFixture]
   public class APIPresentation {

      [Test]
      public void Method() {

         //
         // Support for absolute file and directory path, drive letter and absolute
         //
         var absoluteFilePath = @"C:\Dir\File.txt".ToAbsoluteFilePath();
         Assert.IsTrue(absoluteFilePath.FileName == "File.txt");
         Assert.IsTrue(absoluteFilePath.Kind == AbsolutePathKind.DriveLetter);
         Assert.IsTrue(absoluteFilePath.DriveLetter.Letter == 'C');

         var absoluteUNCDirectoryPath = @"\\Server\Share\Dir".ToAbsoluteDirectoryPath();
         Assert.IsTrue(absoluteUNCDirectoryPath.DirectoryName == "Dir");
         Assert.IsTrue(absoluteUNCDirectoryPath.Kind == AbsolutePathKind.UNC);
         Assert.IsTrue(absoluteUNCDirectoryPath.UNCServer == "Server");
         Assert.IsTrue(absoluteUNCDirectoryPath.UNCShare == "Share");

         //
         // Support for path browsing
         //
         Assert.IsTrue(absoluteFilePath.ParentDirectoryPath.ToString() == @"C:\Dir");
         Assert.IsTrue(absoluteUNCDirectoryPath.GetChildFileWithName("File.txt").ToString() ==
                       @"\\Server\Share\Dir\File.txt");
         Assert.IsTrue(absoluteUNCDirectoryPath.GetBrotherDirectoryWithName("Dir2").ToString() == @"\\Server\Share\Dir2");
         if (absoluteUNCDirectoryPath.Exists) {
            var filePaths = absoluteUNCDirectoryPath.ChildrenFilesPath;
            var dirPaths = absoluteUNCDirectoryPath.ChildrenDirectoriesPath;
         }

         //
         // Support for relative path
         //
         var absoluteDirectoryPath = @"C:\DirA\DirB".ToAbsoluteDirectoryPath();
         var relativeFilePath = @"..\DirC\File.txt".ToRelativeFilePath();
         var absoluteFilePath2 = relativeFilePath.GetAbsolutePathFrom(absoluteDirectoryPath);
         Assert.IsTrue(absoluteFilePath2.ToString() == @"C:\DirA\DirC\File.txt");
         var relativeFilePath2 = absoluteFilePath2.GetRelativePathFrom(absoluteDirectoryPath);
         Assert.IsTrue(relativeFilePath2.Equals(relativeFilePath)); // Use Equals() for path comparison, dont use ==


         //
         // Support for path prefixed with an environment variable
         //
         var envVarFilePath = @"%ENVVAR%\DirB\File.txt".ToEnvVarFilePath();
         Assert.IsTrue(envVarFilePath.EnvVar == "%ENVVAR%");

         IAbsoluteFilePath absoluteFilePath3;
         Assert.IsTrue(envVarFilePath.TryResolve(out absoluteFilePath3) ==
                       EnvVarPathResolvingStatus.ErrorUnresolvedEnvVar);

         Environment.SetEnvironmentVariable("ENVVAR", @"NotAValidPath");
         Assert.IsTrue(envVarFilePath.TryResolve(out absoluteFilePath3) ==
                       EnvVarPathResolvingStatus.ErrorEnvVarResolvedButCannotConvertToAbsolutePath);

         Environment.SetEnvironmentVariable("ENVVAR", @"C:\DirA");
         Assert.IsTrue(envVarFilePath.TryResolve(out absoluteFilePath3) == EnvVarPathResolvingStatus.Success);
         Assert.IsTrue(absoluteFilePath3.ToString() == @"C:\DirA\DirB\File.txt");
         Environment.SetEnvironmentVariable("ENVVAR", "");

         //
         // Support for path that contain variable(s)
         //
         var variableFilePath = @"$(VarA)\DirB\$(VarC)\File.txt".ToVariableFilePath();
         Assert.IsTrue(variableFilePath.PrefixVariable == "VarA");
         Assert.IsTrue(variableFilePath.AllVariables.First() == "VarA");
         Assert.IsTrue(variableFilePath.AllVariables.ElementAt(1) == "VarC");

         IAbsoluteFilePath absoluteFilePath4;
         Assert.IsTrue(variableFilePath.TryResolve(
            new KeyValuePair<string, string>[0],
            out absoluteFilePath4) == VariablePathResolvingStatus.ErrorUnresolvedVariable);

         Assert.IsTrue(variableFilePath.TryResolve(
            new[] {new KeyValuePair<string, string>("VarA", "NotAValidPath"),
                   new KeyValuePair<string, string>("VarC", "DirC")},
            out absoluteFilePath4) == VariablePathResolvingStatus.ErrorVariableResolvedButCannotConvertToAbsolutePath);

         Assert.IsTrue(variableFilePath.TryResolve(
            new[] {new KeyValuePair<string, string>("VarA", @"C:\DirA"),
                   new KeyValuePair<string, string>("VarC", "DirC")},
            out absoluteFilePath4) == VariablePathResolvingStatus.Success);
         Assert.IsTrue(absoluteFilePath4.ToString() == @"C:\DirA\DirB\DirC\File.txt");

         //
         // Support for path normalisation
         //
         Assert.IsTrue(@"C://DirA/\DirB//".ToDirectoryPath().ToString() == @"C:\DirA\DirB");
         Assert.IsTrue(@"%ENVVAR%\DirA\..\DirB".ToDirectoryPath().ToString() == @"%ENVVAR%\DirB");
         Assert.IsTrue(@".\..".ToDirectoryPath().ToString() == "..");
         Assert.IsTrue(@".\..\.\Dir".ToDirectoryPath().ToString() == @"..\Dir");

         //
         // Support for paths collection
         //
         IAbsoluteDirectoryPath commonRootDir;
         Assert.IsTrue(new[] {
            @"C:\Dir".ToAbsoluteDirectoryPath(),
            @"C:\Dir\Dir1".ToAbsoluteDirectoryPath(),
            @"C:\Dir\Dir2\Dir3".ToAbsoluteDirectoryPath(),
         }.TryGetCommonRootDirectory(out commonRootDir));
         Assert.IsTrue(commonRootDir.ToString() == @"C:\Dir");

         //
         // Possibility to work with IFilePath IDirectoryPath
         // and be abstracted from the underlying kind (Absolute/Relative/EnvVar/Variable)
         //
         foreach (var s in new[] {
            @"C:\Dir\File.txt",
            @"\\Server\Share\Dir\File.txt",
            @"..\..\Dir\File.txt",
            @"%ENVVAR%\Dir\File.txt",
            @"$(Var)\Dir\File.txt",
         }) {
            var filePath = s.ToFilePath();
            Assert.IsTrue(filePath.FileName == @"File.txt");
            Assert.IsTrue(filePath.HasParentDirectory);
            Assert.IsTrue(filePath.ParentDirectoryPath.DirectoryName == @"Dir");
         }


         //
         // Support for Try... syntax to avoid exception
         //
         IDirectoryPath directoryPath;
         Assert.IsFalse(@"NotAValidPath".TryGetDirectoryPath(out directoryPath));
         string failureReason;
         Assert.IsFalse(@"NotAValidPath".IsValidDirectoryPath(out failureReason));
         Assert.IsTrue(failureReason == @"The string ""NotAValidPath"" is not a valid directory path.");

         //
         // Smart enough to distinguish between File and Directory path when possible
         //
         Assert.IsFalse(@"C:".IsValidFilePath());
         Assert.IsFalse(@"\\Server\Share".IsValidFilePath());
         Assert.IsTrue(@"C:".IsValidDirectoryPath());
         Assert.IsTrue(@"\\Server\Share".IsValidDirectoryPath());

         Assert.IsTrue(@"C:\Dir".IsValidFilePath());
         Assert.IsTrue(@"\\Server\Share\Dir".IsValidFilePath());
         Assert.IsTrue(@"C:\Dir".IsValidDirectoryPath());
         Assert.IsTrue(@"\\Server\Share\Dir".IsValidDirectoryPath());
      }

   }
}
