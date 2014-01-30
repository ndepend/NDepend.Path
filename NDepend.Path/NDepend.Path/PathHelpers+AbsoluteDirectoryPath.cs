using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

using NDepend.Helpers;

namespace NDepend.Path {
   partial class PathHelpers {

      private sealed class AbsoluteDirectoryPath : AbsolutePathBase, IAbsoluteDirectoryPath {
         internal AbsoluteDirectoryPath(string pathString)
            : base(pathString) {
            Debug.Assert(pathString != null);
            Debug.Assert(pathString.Length > 0);
            Debug.Assert(pathString.IsValidAbsoluteDirectoryPath());
         }

         //
         //  DirectoryName
         //
         public string DirectoryName { get { return MiscHelpers.GetLastName(m_PathString); } }

         //
         //  IsFilePath ; IsDirectoryPath
         //
         public override bool IsDirectoryPath { get { return true; } }
         public override bool IsFilePath { get { return false; } }

         //
         //  Absolute/Relative pathString conversion
         //
         IRelativeDirectoryPath IAbsoluteDirectoryPath.GetRelativePathFrom(IAbsoluteDirectoryPath pivotDirectory) {
            Debug.Assert(pivotDirectory != null);
            string pathRelativeString, failureReason;
            if (!AbsoluteRelativePathHelpers.TryGetRelativePath(pivotDirectory, this, out pathRelativeString, out failureReason)) {
               throw new ArgumentException(failureReason);
            }
            Debug.Assert(pathRelativeString != null);
            Debug.Assert(pathRelativeString.Length > 0);
            return pathRelativeString.ToRelativeDirectoryPath();
         }


         public override IRelativePath GetRelativePathFrom(IAbsoluteDirectoryPath path) {
            return (this as IAbsoluteDirectoryPath).GetRelativePathFrom(path);
         }

         public override bool CanGetRelativePathFrom(IAbsoluteDirectoryPath pivotDirectory) {
            Debug.Assert(pivotDirectory != null);  // Enforced by contract!
            string pathResultUnused, failureReasonUnused;
            return AbsoluteRelativePathHelpers.TryGetRelativePath(pivotDirectory, this, out pathResultUnused, out failureReasonUnused);
         }

         public override bool CanGetRelativePathFrom(IAbsoluteDirectoryPath pivotDirectory, out string failureReason) {
            Debug.Assert(pivotDirectory != null); // Enforced by contract
            string pathResultUnused;
            return AbsoluteRelativePathHelpers.TryGetRelativePath(pivotDirectory, this, out pathResultUnused, out failureReason);
         }


         //
         //  Path Browsing facilities
         //         
         public IAbsoluteFilePath GetBrotherFileWithName(string fileName) {
            Debug.Assert(fileName != null); // Enforced by contract
            Debug.Assert(fileName.Length > 0); // Enforced by contract
            IFilePath path = PathBrowsingHelpers.GetBrotherFileWithName(this, fileName);
            var pathTyped = path as IAbsoluteFilePath;
            Debug.Assert(pathTyped != null);
            return pathTyped;
         }

         public IAbsoluteDirectoryPath GetBrotherDirectoryWithName(string directoryName) {
            Debug.Assert(directoryName != null); // Enforced by contract
            Debug.Assert(directoryName.Length > 0); // Enforced by contract
            IDirectoryPath path = PathBrowsingHelpers.GetBrotherDirectoryWithName(this, directoryName);
            var pathTyped = path as IAbsoluteDirectoryPath;
            Debug.Assert(pathTyped != null);
            return pathTyped;
         }

         public IAbsoluteFilePath GetChildFileWithName(string fileName) {
            Debug.Assert(fileName != null); // Enforced by contract
            Debug.Assert(fileName.Length > 0); // Enforced by contract
            string pathString = PathBrowsingHelpers.GetChildFileWithName(this, fileName);
            Debug.Assert(pathString.IsValidAbsoluteFilePath());
            return new AbsoluteFilePath(pathString);
         }

         public IAbsoluteDirectoryPath GetChildDirectoryWithName(string directoryName) {
            Debug.Assert(directoryName != null); // Enforced by contract
            Debug.Assert(directoryName.Length > 0); // Enforced by contract
            string pathString = PathBrowsingHelpers.GetChildDirectoryWithName(this, directoryName);
            Debug.Assert(pathString.IsValidAbsoluteDirectoryPath());
            return new AbsoluteDirectoryPath(pathString);
         }

         // explicit impl from IDirectoryPath
         IFilePath IDirectoryPath.GetBrotherFileWithName(string fileName) {
            Debug.Assert(fileName != null); // Enforced by contracts
            Debug.Assert(fileName.Length > 0); // Enforced by contracts
            return this.GetBrotherFileWithName(fileName);
         }

         IDirectoryPath IDirectoryPath.GetBrotherDirectoryWithName(string directoryName) {
            Debug.Assert(directoryName != null); // Enforced by contracts
            Debug.Assert(directoryName.Length > 0); // Enforced by contracts
            return this.GetBrotherDirectoryWithName(directoryName);
         }

         IFilePath IDirectoryPath.GetChildFileWithName(string fileName) {
            Debug.Assert(fileName != null); // Enforced by contracts
            Debug.Assert(fileName.Length > 0); // Enforced by contracts
            return this.GetChildFileWithName(fileName);
         }

         IDirectoryPath IDirectoryPath.GetChildDirectoryWithName(string directoryName) {
            Debug.Assert(directoryName != null); // Enforced by contracts
            Debug.Assert(directoryName.Length > 0); // Enforced by contracts
            return this.GetChildDirectoryWithName(directoryName);
         }


         //
         //  Operations that requires physical access
         //
         public override bool Exists {
            get {
               // 6Dec2013: Take care, if a server is not available, trying to access it can trigger a half-minute time-out!
               //           http://stackoverflow.com/questions/5152647/how-to-quickly-check-if-unc-path-is-available
               return Directory.Exists(m_PathString);
            }
         }

         public DirectoryInfo DirectoryInfo {
            get {
               if (!this.Exists) {
                  throw new DirectoryNotFoundException(m_PathString);
               }
               // 4May2011 Need to append DirectorySeparatorChar to get the Directory info
               //          else for example the pathString of "C:" would become "." !!
               var pathForDirectoryInfo = m_PathString + MiscHelpers.DIR_SEPARATOR_CHAR;
               return new DirectoryInfo(pathForDirectoryInfo);
            }
         }

         public IReadOnlyList<IAbsoluteFilePath> ChildrenFilesPath {
            get {
               DirectoryInfo directoryInfo = this.DirectoryInfo;
               FileInfo[] filesInfos = directoryInfo.GetFiles();
               var childrenFilesPath = new List<IAbsoluteFilePath>();
               foreach (FileInfo fileInfo in filesInfos) {
                  childrenFilesPath.Add(fileInfo.FullName.ToAbsoluteFilePath());
               }
               return childrenFilesPath.ToReadOnlyWrappedList();
            }
         }
         public IReadOnlyList<IAbsoluteDirectoryPath> ChildrenDirectoriesPath {
            get {
               DirectoryInfo directoryInfo = this.DirectoryInfo;
               DirectoryInfo[] directoriesInfos = directoryInfo.GetDirectories();
               var childrenDirectoriesPath = new List<IAbsoluteDirectoryPath>();
               foreach (DirectoryInfo childDirectoryInfo in directoriesInfos) {
                  childrenDirectoriesPath.Add(childDirectoryInfo.FullName.ToAbsoluteDirectoryPath());
               }
               return childrenDirectoriesPath.ToReadOnlyWrappedList();
            }
         }

      }
   }
}
