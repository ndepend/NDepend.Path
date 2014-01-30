using System.Diagnostics;
using System.IO;
using System;

namespace NDepend.Path {


   partial class PathHelpers {

      private sealed class AbsoluteFilePath : AbsolutePathBase, IAbsoluteFilePath {
         internal AbsoluteFilePath(string pathString)
            : base(pathString) {
            Debug.Assert(pathString != null);
            Debug.Assert(pathString.Length > 0);
            Debug.Assert(pathString.IsValidAbsoluteFilePath());
         }

         //
         //  File Name and File Name Extension
         //
         public string FileName { get { return FileNameHelpers.GetFileName(m_PathString); } }
         public string FileNameWithoutExtension { get { return FileNameHelpers.GetFileNameWithoutExtension(m_PathString); } }
         public string FileExtension { get { return FileNameHelpers.GetFileNameExtension(m_PathString); } }
         public bool HasExtension(string extension) {
            // All these 3 assertions have been checked by contract!
            Debug.Assert(extension != null);
            Debug.Assert(extension.Length >= 2);
            Debug.Assert(extension[0] == '.');
            return FileNameHelpers.HasExtension(m_PathString, extension);
         }

         //
         //  IsFilePath ; IsDirectoryPath
         //
         public override bool IsDirectoryPath { get { return false; } }
         public override bool IsFilePath { get { return true; } }

         //
         //  Absolute/Relative pathString conversion
         //
         IRelativeFilePath IAbsoluteFilePath.GetRelativePathFrom(IAbsoluteDirectoryPath pivotDirectory) {
            Debug.Assert(pivotDirectory != null); // Enforced by contract
            string pathRelative, failureReason;
            if (!AbsoluteRelativePathHelpers.TryGetRelativePath(pivotDirectory, this, out pathRelative, out failureReason)) {
               throw new ArgumentException(failureReason);
            }
            Debug.Assert(pathRelative != null);
            Debug.Assert(pathRelative.Length > 0);
            return new RelativeFilePath(pathRelative + MiscHelpers.DIR_SEPARATOR_CHAR + this.FileName);
         }

         public override IRelativePath GetRelativePathFrom(IAbsoluteDirectoryPath pivotDirectory) {
            Debug.Assert(pivotDirectory != null); // Enforced by contract
            return (this as IAbsoluteFilePath).GetRelativePathFrom(pivotDirectory);
         }

         public override bool CanGetRelativePathFrom(IAbsoluteDirectoryPath pivotDirectory) {
            Debug.Assert(pivotDirectory != null); // Enforced by contract
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
            Debug.Assert(fileName != null);  // Enforced by contract
            Debug.Assert(fileName.Length > 0);  // Enforced by contract
            IFilePath path = PathBrowsingHelpers.GetBrotherFileWithName(this, fileName);
            var pathTyped = path as IAbsoluteFilePath;
            Debug.Assert(pathTyped != null);
            return pathTyped;
         }

         public IAbsoluteDirectoryPath GetBrotherDirectoryWithName(string directoryName) {
            Debug.Assert(directoryName != null);  // Enforced by contract
            Debug.Assert(directoryName.Length > 0);  // Enforced by contract
            IDirectoryPath path = PathBrowsingHelpers.GetBrotherDirectoryWithName(this, directoryName);
            var pathTyped = path as IAbsoluteDirectoryPath;
            Debug.Assert(pathTyped != null);
            return pathTyped;
         }

         public IAbsoluteFilePath UpdateExtension(string newExtension) {
            // All these 3 assertions have been checked by contract!
            Debug.Assert(newExtension != null);
            Debug.Assert(newExtension.Length >= 2);
            Debug.Assert(newExtension[0] == '.');
            string pathString = PathBrowsingHelpers.UpdateExtension(this, newExtension);
            Debug.Assert(pathString.IsValidAbsoluteFilePath());
            return new AbsoluteFilePath(pathString);
         }

         // Explicit Impl methods
         IFilePath IFilePath.GetBrotherFileWithName(string fileName) {
            Debug.Assert(fileName != null); // Enforced by contract
            Debug.Assert(fileName.Length > 0); // Enforced by contract
            return this.GetBrotherFileWithName(fileName);
         }

         IDirectoryPath IFilePath.GetBrotherDirectoryWithName(string directoryName) {
            Debug.Assert(directoryName != null); // Enforced by contract
            Debug.Assert(directoryName.Length > 0); // Enforced by contract
            return this.GetBrotherDirectoryWithName(directoryName);
         }

         IFilePath IFilePath.UpdateExtension(string newExtension) {
            // All these 3 assertions have been checked by contract!
            Debug.Assert(newExtension != null);
            Debug.Assert(newExtension.Length >= 2);
            Debug.Assert(newExtension[0] == '.');
            return this.UpdateExtension(newExtension);
         }


         //
         //  Operations that requires physical access
         //
         public override bool Exists {
            get {
               // 6Dec2013: Take care, if a server is not available, trying to access it can trigger a half-minute time-out!
               //           http://stackoverflow.com/questions/5152647/how-to-quickly-check-if-unc-path-is-available
               return File.Exists(m_PathString);
            }
         }

         public FileInfo FileInfo {
            get {
               if (!this.Exists) {
                  throw new FileNotFoundException(m_PathString);
               }
               return new FileInfo(m_PathString);
            }
         }




      }
   }
}
