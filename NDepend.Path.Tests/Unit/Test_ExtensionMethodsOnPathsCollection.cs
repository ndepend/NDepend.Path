
using System.Collections.Generic;
using NUnit.Framework;
using System;
using NDepend.Test;


namespace NDepend.Path {
   [TestFixture]
   public class Test_ExtensionMethodsOnPathsCollection {

      [SetUp]
      public void SetUp() {
         TestHelper.SetUpTests();
      }


      [Test]
      public void Test_ContainsPath() {
         var array = new IPath[] {
            @"C:\File.txt".ToAbsoluteFilePath(),
            @".\".ToRelativeDirectoryPath()
         };

         Assert.IsTrue(array.ContainsPath(@".\".ToRelativeDirectoryPath()));
         Assert.IsFalse(array.ContainsPath(@"..\".ToRelativeDirectoryPath()));
      }

      [Test]
      public void Test_ContainsSamePathsThan1() {
         List<IDirectoryPath> list1 = null;
         List<IDirectoryPath> list2 = null;
         Assert.IsTrue(list1.ContainsSamePathsThan(list2));
      }

      [Test]
      public void Test_ContainsSamePathsThan2() {
         List<IAbsoluteFilePath> list1 = null;
         List<IAbsoluteFilePath> list2 = new List<IAbsoluteFilePath>();
         Assert.IsFalse(list1.ContainsSamePathsThan(list2));
         Assert.IsFalse(list2.ContainsSamePathsThan(list1));
      }

      [Test]
      public void Test_ContainsSamePathsThan3() {
         List<IAbsoluteFilePath> list1 = new List<IAbsoluteFilePath>();
         List<IAbsoluteFilePath> list2 = new List<IAbsoluteFilePath>();
         list2.Add(@"C:\Dir1\File.txt".ToAbsoluteFilePath());
         Assert.IsFalse(list1.ContainsSamePathsThan(list2));
      }

      [Test]
      public void Test_ContainsSamePathsThan_WithNullAndEmpty() {
         List<IAbsoluteFilePath> list1 = new List<IAbsoluteFilePath>();
         List<IAbsoluteFilePath> list2 = new List<IAbsoluteFilePath>();
         
         list1.Add(@"c:\FILE.txt".ToAbsoluteFilePath());
         list1.Add(null);

         list2.Add(null);
         list2.Add(@"C:\file.txt".ToAbsoluteFilePath());
         Assert.IsTrue(list1.ContainsSamePathsThan(list2));
      }



      [Test]
      public void Test_ContainsSamePathsThan_WithNullAndEmpty2() {
         List<IAbsoluteFilePath> list1 = new List<IAbsoluteFilePath>();
         List<IAbsoluteFilePath> list2 = new List<IAbsoluteFilePath>();

         list1.Add(@"c:\FILE.txt".ToAbsoluteFilePath());
         list1.Add(null);
         list1.Add(null);

         list2.Add(null);
         list2.Add(@"C:\file.txt".ToAbsoluteFilePath());
         Assert.IsFalse(list1.ContainsSamePathsThan(list2));
      }

      [Test]
      public void Test_ContainsSamePathsThan4() {
         var list1 = new List<IPath>();
         list1.Add(@"C:\Dir1\File.txt".ToAbsoluteFilePath());
         list1.Add(@"..\Dir1\File.txt".ToRelativeFilePath());
         list1.Add(@"C:\Dir1\Dir2".ToAbsoluteDirectoryPath());
         list1.Add(@"..\Dir1\Dir1".ToRelativeDirectoryPath());
         list1.Add(@"..\Dir1\Dir1\Dir2".ToRelativeDirectoryPath());
         var list2 = new List<IPath>();
         list2.Add(@"c:\dir1\File.txt".ToAbsoluteFilePath());
         list2.Add(@"..\dir1\file.txt".ToRelativeFilePath());
         list2.Add(@"c:\dir1\dir2".ToAbsoluteDirectoryPath());
         list2.Add(@"..\Dir1\Dir1".ToRelativeDirectoryPath());
         list2.Add(@"..\dir1\dir1\Dir2".ToRelativeDirectoryPath());

         Assert.IsTrue(list1.ContainsSamePathsThan(list2));
      }

      [Test]
      public void Test_ContainsSamePathsThan5() {
         var list1 = new List<IPath>();
         list1.Add(@"C:\Dir1\File.txt".ToAbsoluteFilePath());
         list1.Add(@"..\Dir1\File.txt".ToRelativeFilePath());
         var list2 = new List<IPath>();
         list2.Add(@"c:\dir1\File.txt".ToAbsoluteFilePath());
         list2.Add(@"..\dir1\Dir2\file.txt".ToRelativeFilePath());
         Assert.IsFalse(list1.ContainsSamePathsThan(list2));
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







      
      [Test]
      public void Test_ListOfPathContains_UseOurEqualsToOperator() {

         // Null and empty lists
         //Assert.IsFalse(ExtensionMethodsOnPathsCollection.Contains(null, @"E:\Path1\Path2\File.txt".ToAbsoluteFilePath()));
         List<IAbsoluteFilePath> listIn = new List<IAbsoluteFilePath>();
         Assert.IsFalse(listIn.Contains(@"E:\Path1\Path2\File.txt".ToAbsoluteFilePath()));

         // List contains null
         listIn.Add(null);
         Assert.IsTrue(listIn.Contains(null));

         listIn.Add(@"E:\Path1\Path2\File.txt".ToAbsoluteFilePath());
         Assert.IsTrue(listIn.Contains(@"e:\path1\path2\file.txt".ToAbsoluteFilePath()));


         var listIn1 = new List<IDirectoryPath>();
         listIn1.Add(@"E:\Path1\Path2".ToAbsoluteDirectoryPath());
         listIn1.Add(@"..\Path1\Path2".ToRelativeDirectoryPath());
         Assert.IsTrue(listIn1.Contains(@"..\path1\path2".ToRelativeDirectoryPath()));
      }

      [Test]
      public void Test_GetHashCodeThroughDictionary() {
         var dico = new Dictionary<IPath, string>();
         var directoryPath = @"C:\Dir1".ToAbsoluteDirectoryPath();
         var filePath = @"c:\dir1\Dir2\file.txt".ToAbsoluteFilePath();

         dico.Add(directoryPath, directoryPath.ToString());
         dico.Add(filePath, filePath.ToString());
         Assert.IsTrue(dico[filePath] == @"c:\dir1\Dir2\file.txt");
      }

      [Test, ExpectedException(typeof(ArgumentException))]
      public void Test_GetHashCodeThroughDictionary_ErrorOnPathValue() {
         var dico = new Dictionary<IPath, string>();
         var filePath1 = @"C:\Dir1\File.txt".ToAbsoluteFilePath();
         var filePath2 = @"c:\dir1\file.txt".ToAbsoluteFilePath();
         Assert.IsTrue(filePath1.Equals(filePath2));
         Assert.IsTrue(filePath1.GetHashCode() == filePath2.GetHashCode());
         dico.Add(filePath1, filePath1.ToString());
         dico.Add(filePath2, filePath2.ToString()); // <- filePath1 & filePath2 are 2 different object with same value
      }


      //
      // Test TryFindCommonPrefix directly
      //
      [Test]
      public void Test_TryFindCommonPrefix_Empty() {
         string commonPrefix;
         Assert.IsFalse(ExtensionMethodsOnPathsCollection.TryFindCommonPrefix(
            new string[] {}, 
            false, // no ignore case 
            '.',
            out commonPrefix));
      }

      [Test]
      public void Test_TryFindCommonPrefix_OneStringEmpty() {
         string commonPrefix;
         Assert.IsFalse(ExtensionMethodsOnPathsCollection.TryFindCommonPrefix(
            new string[] { "" },
            false, // no ignore case 
            '.',
            out commonPrefix));
      }

      [Test]
      public void Test_TryFindCommonPrefix_OneDot() {
         string commonPrefix;
         Assert.IsTrue(ExtensionMethodsOnPathsCollection.TryFindCommonPrefix(
            new string[] { "." },
            false, // no ignore case 
            '.',
            out commonPrefix));
         Assert.IsTrue(commonPrefix == ".");
      }

      [Test]
      public void Test_TryFindCommonPrefix_OneStr() {
         string commonPrefix;
         Assert.IsTrue(ExtensionMethodsOnPathsCollection.TryFindCommonPrefix(
            new string[] { "hello" },
            false, // no ignore case 
            '.',
            out commonPrefix));
         Assert.IsTrue(commonPrefix == "hello");
      }

      [Test]
      public void Test_TryFindCommonPrefix_TwoStrIgnoreCase() {
         string commonPrefix;
         Assert.IsTrue(ExtensionMethodsOnPathsCollection.TryFindCommonPrefix(
            new string[] { "hello", "HELLO" },
            true, // ignore case 
            '.',
            out commonPrefix));
         Assert.IsTrue(commonPrefix.ToLower() == "hello");
      }

      [Test]
      public void Test_TryFindCommonPrefix_TwoStrNoIgnoreCase() {
         string commonPrefix;
         Assert.IsFalse(ExtensionMethodsOnPathsCollection.TryFindCommonPrefix(
            new string[] { "hello", "HELLO" },
            false, // NO ignore case 
            '.',
            out commonPrefix));
      }

      [Test]
      public void Test_TryFindCommonPrefix_TwoStrIgnoreCase2() {
         string commonPrefix;
         Assert.IsTrue(ExtensionMethodsOnPathsCollection.TryFindCommonPrefix(
            new string[] { "hello.A", "HELLO.B" },
            true, // ignore case 
            '.',
            out commonPrefix));
         Assert.IsTrue(commonPrefix.ToLower() == "hello");
      }

      [Test]
      public void Test_TryFindCommonPrefix_MoreStrIgnoreCase() {
         string commonPrefix;
         Assert.IsTrue(ExtensionMethodsOnPathsCollection.TryFindCommonPrefix(
            new string[] { "hello.a", "hello.A.A", "HELLO.A.B" , "Hello.A.C"},
            true, // ignore case 
            '.',
            out commonPrefix));
         Assert.IsTrue(commonPrefix.ToLower() == "hello.a");
      }

      [Test]
      public void Test_TryFindCommonPrefix_TwoMoreStrNoIgnoreCase() {
         string commonPrefix;
         Assert.IsTrue(ExtensionMethodsOnPathsCollection.TryFindCommonPrefix(
            new string[] { "hello.a", "hello.A.A", "hello.A.B", "hello.A.C" },
            false, // NO ignore case 
            '.',
            out commonPrefix));
         Assert.IsTrue(commonPrefix == "hello");
      }

      [Test]
      public void Test_TryFindCommonPrefix_MoreStrIgnoreCase2() {
         string commonPrefix;
         Assert.IsTrue(ExtensionMethodsOnPathsCollection.TryFindCommonPrefix(
            new string[] { "hello.HELLO.ALLO.a", "hello.hello.Allo.A.A", "HELLO.HEllO.Allo.A.B", "Hello.HELLO.allo.A.C" },
            true, // ignore case 
            '.',
            out commonPrefix));
         Assert.IsTrue(commonPrefix.ToLower() == "hello.hello.allo.a");
      }

      [Test]
      public void Test_TryFindCommonPrefix_TwoMoreStrNoIgnoreCase2() {
         string commonPrefix;
         Assert.IsTrue(ExtensionMethodsOnPathsCollection.TryFindCommonPrefix(
            new string[] { "hello.allo.a", "hello.allo.A.A", "hello.allo.A.B", "hello.allo.A.C" },
            false, // NO ignore case 
            '.',
            out commonPrefix));
         Assert.IsTrue(commonPrefix == "hello.allo");
      }
   }
}
