using System.IO;
using System.Diagnostics;
using NDepend.Path;

namespace NDepend.Test.Unit {
   public static class DirForTest {

      public static IAbsoluteDirectoryPath ExecutingAssemblyDir {
         get {
            // If this following line doesn't work, it is because of ShadowCopyCache with NUnit
            return System.Reflection.Assembly.GetExecutingAssembly().Location.ToAbsoluteFilePath().ParentDirectoryPath;
         }
      }

      public static IAbsoluteFilePath ExecutingAssemblyFilePath {
         get {
            return ExecutingAssemblyDir.GetChildFileWithName(ExecutingAssemblyFileName);
         }
      }

      private static string ExecutingAssemblyFileName {
         get {
            string executingAssemblyFileLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return System.IO.Path.GetFileName(executingAssemblyFileLocation);
         }
      }


      public static IAbsoluteDirectoryPath DirAbsolute {
         get {
            return Dir.ToAbsoluteDirectoryPath();
         }
      }

      public static string Dir {
         get {
            return ExecutingAssemblyDir.GetChildDirectoryWithName("DirForTest").ToString();
         }
      }


      public static IAbsoluteDirectoryPath GetDirOfUnitTestWithName(string unitTestName) {
         IAbsoluteDirectoryPath ndependRootPath = ExecutingAssemblyDir.ParentDirectoryPath;
         IAbsoluteDirectoryPath unitTestPath = ndependRootPath.GetChildDirectoryWithName("NDepend.Test.Dirs");
         IAbsoluteDirectoryPath result = unitTestPath.GetChildDirectoryWithName(unitTestName);
         Debug.Assert(result.Exists);
         return result;
      }


      public static IAbsoluteDirectoryPath GetBinDebugDir() {
         IAbsoluteDirectoryPath binDebug = DirAbsolute.ParentDirectoryPath.GetChildDirectoryWithName("Debug");
         Debug.Assert(binDebug.Exists);
         return binDebug;
      }


      public static void EnsureDirForTestExistAndEmpty() {
         string dir = Dir;

      RETRY:  // 29Nov2010: retry until it is ok!!
         try {
            // Clear the older dir
            if (!Directory.Exists(dir)) {
               Directory.CreateDirectory(dir);
            } else {
               var subDirs = Directory.GetDirectories(dir);
               var subFiles = Directory.GetFiles(dir);

               if (subFiles.Length > 0) {
                  foreach (var filePath in subFiles) {
                     File.Delete(filePath);
                  }
               }

               if (subDirs.Length > 0) {
                  foreach (var dirPath in subDirs) {
                     Directory.Delete(dirPath, true);
                  }
               }
            }

            if (!Directory.Exists(dir)) { goto RETRY; }
            if (Directory.GetDirectories(dir).Length > 0) { goto RETRY; }
            if (Directory.GetFiles(dir).Length > 0) { goto RETRY; }
         } catch {
            goto RETRY;
         }
         var dirInfo = new DirectoryInfo(dir);
         Debug.Assert(dirInfo.Exists);
         Debug.Assert(dirInfo.GetFiles().Length == 0);
         Debug.Assert(dirInfo.GetDirectories().Length == 0);
      }
      public static void Delete() {
         string dir = Dir;
         if (Directory.Exists(dir)) {
            Directory.Delete(dir, true);
         }
      }



      public static string ExecutingAssemblyFilePathInDirForTest {
         get {
            return Dir.ToAbsoluteDirectoryPath().GetChildFileWithName(ExecutingAssemblyFileName).ToString();
         }
      }

      public static void CopyExecutingAssemblyFileInDirForTest() {
         File.Copy(ExecutingAssemblyDir.GetChildFileWithName(ExecutingAssemblyFileName).ToString(),
                   ExecutingAssemblyFilePathInDirForTest);
      }
   }
}
