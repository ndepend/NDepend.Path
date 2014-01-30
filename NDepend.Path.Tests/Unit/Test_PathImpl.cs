
using System.Collections.Generic;
using NUnit.Framework;
using NDepend.Test;
using System.Linq;


namespace NDepend.Path {
   [TestFixture] 
   public class Test_PathImpl {
 
      [SetUp]
      public void SetUp() {
         TestHelper.SetUpTests();
      }

      [Test]
      public void Test_ToStringOrIfNullToEmptyString() {
         IPath path = null;
         Assert.IsTrue(path.ToStringOrIfNullToEmptyString() == "");
         path = @"C:\Dir".ToAbsoluteDirectoryPath();
         Assert.IsTrue(path.ToStringOrIfNullToEmptyString() == @"C:\Dir");
      }

      [Test]
      public void Test_IsNotNullAndExists() {
         IAbsolutePath path = null;
         Assert.IsFalse(path.IsNotNullAndExists());
         path = @"L:\NonExistingDir".ToAbsoluteDirectoryPath();
         Assert.IsFalse(path.IsNotNullAndExists());
      }

      [Test]
      public void Test_EqualsNullSupported() {
         IPath pathA = null;
         IPath pathB = null;
         Assert.IsTrue(pathA.EqualsNullSupported(pathB));

         var realPath = @"C:".ToAbsoluteDirectoryPath();
         pathA = realPath;
         pathB = null;
         Assert.IsFalse(pathA.EqualsNullSupported(pathB));

         pathA = null;
         pathB = realPath;
         Assert.IsFalse(pathA.EqualsNullSupported(pathB));

         pathA = realPath.ToString().ToAbsoluteDirectoryPath();
         pathB = realPath;
         Assert.IsTrue(pathA.EqualsNullSupported(pathB));

         pathA = @".".ToRelativeDirectoryPath();
         pathB = realPath;
         Assert.IsFalse(pathA.EqualsNullSupported(pathB));
      }
      


      [Test]
      public void ForbiddenCharInPath() {
         Assert.IsTrue(PathHelpers.ForbiddenCharInPath.Contains('|'));
      }


      [Test]
      public void Test_HasParentDirBasePath() {

         Assert.IsFalse(@"E:\".ToAbsoluteDirectoryPath().HasParentDirectory);
         Assert.IsFalse(@"E:\Dir1\..".ToAbsoluteDirectoryPath().HasParentDirectory);
         Assert.IsTrue(@"E:\Dir1".ToAbsoluteDirectoryPath().HasParentDirectory);

         Assert.IsTrue(@"E:\File.txt".ToAbsoluteFilePath().HasParentDirectory);
         Assert.IsTrue(@"E:\Dir1\..\File.txt".ToAbsoluteFilePath().HasParentDirectory);
         Assert.IsTrue(@"E:\Dir1\File.txt".ToAbsoluteFilePath().HasParentDirectory);

         Assert.IsFalse(@"..\".ToRelativeDirectoryPath().HasParentDirectory);
         Assert.IsTrue(@"..\Dir1\".ToRelativeDirectoryPath().HasParentDirectory);
         Assert.IsFalse(@"..\Dir1\..".ToRelativeDirectoryPath().HasParentDirectory);

         Assert.IsFalse(@".\".ToRelativeDirectoryPath().HasParentDirectory);
         Assert.IsTrue(@".\Dir1\".ToRelativeDirectoryPath().HasParentDirectory);
         Assert.IsFalse(@".\Dir1\..".ToRelativeDirectoryPath().HasParentDirectory);

         Assert.IsTrue(@"..\File.txt".ToRelativeDirectoryPath().HasParentDirectory);
         Assert.IsTrue(@"..\Dir1\File.txt".ToRelativeDirectoryPath().HasParentDirectory);
         Assert.IsTrue(@"..\Dir1\..\File.txt".ToRelativeDirectoryPath().HasParentDirectory);

         Assert.IsTrue(@".\File.txt".ToRelativeDirectoryPath().HasParentDirectory);
         Assert.IsTrue(@".\Dir1\File.txt".ToRelativeDirectoryPath().HasParentDirectory);
         Assert.IsTrue(@".\Dir1\..\File.txt".ToRelativeDirectoryPath().HasParentDirectory);
      }







      [Test]
      public void Test_TryGetCommonRootDirectory() {
         IAbsoluteDirectoryPath commonRootAbsoluteDirectory = null;

         
         var list = new List<IAbsoluteDirectoryPath>();
         Assert.IsFalse(list.TryGetCommonRootDirectory(out commonRootAbsoluteDirectory));

         // Test when only one dir
         list.Add(@"C:\File".ToAbsoluteDirectoryPath());
         Assert.IsTrue(list.TryGetCommonRootDirectory(out commonRootAbsoluteDirectory));
         Assert.IsTrue(commonRootAbsoluteDirectory.ToString() == @"C:\File");

         // Test when all dir are the same
         list.Add(@"C:\File".ToAbsoluteDirectoryPath());
         list.Add(@"C:\File".ToAbsoluteDirectoryPath());
         Assert.IsTrue(list.TryGetCommonRootDirectory(out commonRootAbsoluteDirectory));
         Assert.IsTrue(commonRootAbsoluteDirectory.ToString() == @"C:\File");

         // Test when a dir has a wrong drive
         list.Add(@"D:\File".ToAbsoluteDirectoryPath());
         Assert.IsFalse(list.TryGetCommonRootDirectory(out commonRootAbsoluteDirectory));

         // Test when the list contains a null dir
         list.Clear();
         list.Add(@"C:\File".ToAbsoluteDirectoryPath());
         list.Add(null);
         Assert.IsFalse(list.TryGetCommonRootDirectory(out commonRootAbsoluteDirectory));

         list.Clear();
         list.Add(@"C:\File".ToAbsoluteDirectoryPath());
         list.Add(null);
         Assert.IsFalse(list.TryGetCommonRootDirectory(out commonRootAbsoluteDirectory));

         // Test when the common root dir is in the list
         list.Clear();
         list.Add(@"C:\File\Debug".ToAbsoluteDirectoryPath());
         list.Add(@"C:\File\Debug\Dir1\Dir2".ToAbsoluteDirectoryPath());
         list.Add(@"C:\File\Debug\Dir1\Dir2\Dir3".ToAbsoluteDirectoryPath());
         Assert.IsTrue(list.TryGetCommonRootDirectory(out commonRootAbsoluteDirectory));
         Assert.IsTrue(commonRootAbsoluteDirectory.ToString() == @"C:\File\Debug");

         list.Add(@"C:\File".ToAbsoluteDirectoryPath());
         Assert.IsTrue(list.TryGetCommonRootDirectory(out commonRootAbsoluteDirectory));
         Assert.IsTrue(commonRootAbsoluteDirectory.ToString() == @"C:\File");

         list.Add(@"C:".ToAbsoluteDirectoryPath());
         Assert.IsTrue(list.TryGetCommonRootDirectory(out commonRootAbsoluteDirectory));
         Assert.IsTrue(commonRootAbsoluteDirectory.ToString() == @"C:");

         // Test when the common root dir is not in the list
         list.Clear();
         list.Add(@"C:\File\Debug\Dir4".ToAbsoluteDirectoryPath());
         list.Add(@"C:\File\Debug\Dir1\Dir2\Dir3".ToAbsoluteDirectoryPath());
         Assert.IsTrue(list.TryGetCommonRootDirectory(out commonRootAbsoluteDirectory));
         Assert.IsTrue(commonRootAbsoluteDirectory.ToString() == @"C:\File\Debug");

      }



   }
}

