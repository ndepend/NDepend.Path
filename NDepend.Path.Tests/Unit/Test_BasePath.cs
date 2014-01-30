
using System;
using NDepend.Test;
using NUnit.Framework;

namespace NDepend.Path {

   [TestFixture]
   public class Test_BasePath {

      [SetUp]
      public void SetUp() {
         TestHelper.SetUpTests();
      }


      [Test]
      public void Test_HasParentDirectory() {
         var path = @"C:\Dir".ToAbsoluteDirectoryPath();
         Assert.IsTrue(path.HasParentDirectory);
         Assert.IsTrue(path.ParentDirectoryPath.ToString() == @"C:");
      }

      [Test, ExpectedException(typeof(InvalidOperationException))]
      public void Test_NotHasParentDirectory() {
         var path = @"C:\".ToAbsoluteDirectoryPath();
         Assert.IsFalse(path.HasParentDirectory);
         var parentDir = path.ParentDirectoryPath;
      }


      [TestCase(@".\"), ExpectedException(typeof(InvalidOperationException))]
      [TestCase(@"..\")]
      [TestCase(@"..\.\")]
      public void Test_NotHasParentDirectoryRelative(string invalidRelativePathString) {
         var path = invalidRelativePathString.ToRelativeDirectoryPath();
         Assert.IsFalse(path.HasParentDirectory);
         var parentDir = path.ParentDirectoryPath;
      }

      [TestCase(@"..\..\", @"..")]
      [TestCase(@"..\..", @"..")]
      public void Test_HasParentDirectoryRelative(string validRelativePathString, string parentDirectoryPath) {
         var path = validRelativePathString.ToRelativeDirectoryPath();
         Assert.IsTrue(path.HasParentDirectory);
         Assert.IsTrue(path.ParentDirectoryPath.ToString() == parentDirectoryPath);
      }




      [Test]
      public void Test_Path_Equals_NotEquals_HashCode_ToString() {
         var path1 = @"C:\Dir".ToAbsoluteDirectoryPath();
         var path2 = @"c:\dir".ToAbsoluteDirectoryPath();
         var path3 = @"C:\dirXYZ".ToAbsoluteDirectoryPath();
         Assert.IsFalse(path1 == path2);
         Assert.IsFalse(path1 == path3);

         Assert.IsFalse(path1.Equals(null));

         Assert.IsTrue(path1.Equals(path2));
         Assert.IsFalse(path1.Equals(path3));

         Assert.IsTrue(path1.NotEquals(path3));
         Assert.IsFalse(path1.NotEquals(path2));

         Assert.IsTrue(path1.ToString() == @"C:\Dir");
         Assert.IsTrue(path2.ToString() == @"c:\dir");

         Assert.IsTrue(path1.GetHashCode() == path2.GetHashCode());
         Assert.IsFalse(path1.GetHashCode() == path3.GetHashCode());
      }

   }
}
