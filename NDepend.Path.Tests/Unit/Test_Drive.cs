using System;
using System.IO;
using NDepend.Path;
using NUnit.Framework;

namespace NDepend.Test.Integration.NDepend.Path.Unit {

   [TestFixture]
   public class Test_Drive {

      [SetUp]
      public void SetUp() {
         TestHelper.SetUpTests();
      }

      [Test]
      public void Test_DriveAPI() {
         var path = @"C:\Dir".ToAbsoluteDirectoryPath();
         var driveLetter = path.DriveLetter;
         Assert.IsTrue(driveLetter.Letter == 'C');
         Assert.IsFalse(driveLetter.Equals(null));
         Assert.IsFalse(driveLetter.Equals(new object()));
         Assert.IsTrue(driveLetter.Equals(@"c:\UUDir".ToAbsoluteDirectoryPath().DriveLetter));
         Assert.IsNotNull(driveLetter.DriveInfo);
      }

      [Test]
      public void Test_OnSameVolumeThan_Directory() {
         var dirAbsolute = @"C:\Dir".ToAbsoluteDirectoryPath();
         Assert.IsTrue(dirAbsolute.OnSameVolumeThan(@"c:\dir1".ToAbsoluteDirectoryPath()));
         Assert.IsFalse(dirAbsolute.OnSameVolumeThan(@"E:\dir1".ToAbsoluteDirectoryPath()));
      }

      [Test]
      public void Test_OnSameVolumeThan_File() {
         var fileAbsolute = @"C:\File.txt".ToAbsoluteFilePath();
         Assert.IsTrue(fileAbsolute.OnSameVolumeThan(@"c:\File.txt1".ToAbsoluteFilePath()));
         Assert.IsFalse(@"C:\File.txt".ToAbsoluteFilePath().OnSameVolumeThan(@"E:\t".ToAbsoluteFilePath()));
      }

      [Test]
      public void Test_DriveLetter_NotEquals() {
         Assert.IsTrue(@"C:\Dir".ToAbsoluteDirectoryPath().DriveLetter.NotEquals(@"E:\Dir".ToAbsoluteDirectoryPath()));
      }

      [Test]
      public void Test_GetHashCode() {
         Assert.IsTrue(@"C:\Dir".ToAbsoluteDirectoryPath().DriveLetter.GetHashCode() == @"c:\Dir".ToAbsoluteDirectoryPath().DriveLetter.GetHashCode());
         Assert.IsFalse(@"C:\Dir".ToAbsoluteDirectoryPath().DriveLetter.GetHashCode() == @"E:\Dir".ToAbsoluteDirectoryPath().DriveLetter.GetHashCode());
      }

      [Test, ExpectedException(typeof(DriveNotFoundException))]
      public void Test_DriveInfoDontExist() {
         var driveLetter = @"K:\Dir".ToAbsoluteFilePath().DriveLetter;
         var driveInfo = driveLetter.DriveInfo;
      }



      [Test]
      public void Test_UNCPath_HasNoDriveLetter() {
         bool b = false;
         try {
            var driveLetter = @"\\server\share\Dir".ToAbsoluteFilePath().DriveLetter;
         }
         catch (InvalidOperationException ex) {
            b = true;
            Assert.IsTrue(ex.Message == @"The property getter DriveLetter must be called on a pathString of kind DriveLetter.");
         }
         Assert.IsTrue(b);
      }
   }
}
