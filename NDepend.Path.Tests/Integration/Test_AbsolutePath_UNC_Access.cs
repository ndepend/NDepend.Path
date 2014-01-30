using System;
using System.IO;
using NDepend.Helpers.Network;
using NDepend.Test;
using NUnit.Framework;

namespace NDepend.Path {

   [TestFixture]
   public class Test_AbsolutePath_UNC_Access {

      [SetUp]
      public void SetUp() {
         TestHelper.SetUpTests();
      }



      [Test]
      public void Test_UNCShare_Access() {

         var uncPaths = UNCNetworkHelper.GetExistingUNCShares();
         foreach(var uncPath in uncPaths) {
            Assert.IsTrue(uncPath.Exists);

            // Read dirs and files
            var childrenDirs = uncPath.ChildrenDirectoriesPath;
            var chidrenFiles = uncPath.ChildrenFilesPath;

            // Create / Delete a dir
            var dirTODELETE = uncPath.GetChildDirectoryWithName("TODELETE_CreatedByNDependUnitTest");

            try {
               Assert.IsFalse(dirTODELETE.Exists);
               Directory.CreateDirectory(dirTODELETE.ToString());
            }
            catch (UnauthorizedAccessException) {
               continue;
            }
            Assert.IsTrue(dirTODELETE.Exists);
            Directory.Delete(dirTODELETE.ToString());
            Assert.IsFalse(dirTODELETE.Exists);
         }

      }


   }

}

