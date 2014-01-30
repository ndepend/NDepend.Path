
using System.Diagnostics;


namespace NDepend.Path {

   partial class PathHelpers {

      private sealed class EnvVarFilePath : EnvVarPathBase, IEnvVarFilePath {

         internal EnvVarFilePath(string pathString)
            : base(pathString) {
            Debug.Assert(pathString != null);
            Debug.Assert(pathString.Length > 0);
            Debug.Assert(pathString.IsValidEnvVarFilePath());
         }

         public override bool IsDirectoryPath { get { return false; } }
         public override bool IsFilePath { get { return true; } }


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
         // Path resolving
         //
         public EnvVarPathResolvingStatus TryResolve(out IAbsoluteFilePath pathFileResolved) {
            pathFileResolved = null;
            string pathStringResolved;
            if (!base.TryResolveEnvVar(out pathStringResolved)) {
               return EnvVarPathResolvingStatus.ErrorUnresolvedEnvVar;
            }
            if (!pathStringResolved.IsValidAbsoluteFilePath()) {
               return EnvVarPathResolvingStatus.ErrorEnvVarResolvedButCannotConvertToAbsolutePath;
            }
            pathFileResolved = pathStringResolved.ToAbsoluteFilePath();
            return EnvVarPathResolvingStatus.Success;
         }

         public bool TryResolve(out IAbsoluteFilePath pathFileResolved, out string failureReason) {
            var resolvingStatus = this.TryResolve(out pathFileResolved);
            switch (resolvingStatus) {
               default:
                  Debug.Assert(resolvingStatus == EnvVarPathResolvingStatus.Success);
                  Debug.Assert(pathFileResolved != null);
                  failureReason = null;
                  return true;
               case EnvVarPathResolvingStatus.ErrorUnresolvedEnvVar:
                  failureReason = this.GetErrorUnresolvedEnvVarFailureReason();
                  return false;
               case EnvVarPathResolvingStatus.ErrorEnvVarResolvedButCannotConvertToAbsolutePath:
                  failureReason = this.GetErrorEnvVarResolvedButCannotConvertToAbsolutePathFailureReason();
                  return false;
            }
         }

         public override EnvVarPathResolvingStatus TryResolve(out IAbsolutePath pathResolved) {
            IAbsoluteFilePath pathFileResolved;
            var resolvingStatus = this.TryResolve(out pathFileResolved);
            pathResolved = pathFileResolved;
            return resolvingStatus;
         }

         public override bool TryResolve(out IAbsolutePath pathResolved, out string failureReason) {
            IAbsoluteFilePath pathFileResolved;
            var b = this.TryResolve(out pathFileResolved, out failureReason);
            pathResolved = pathFileResolved;
            return b;
         }



         //
         //  Path Browsing facilities
         //
         public IEnvVarFilePath GetBrotherFileWithName(string fileName) {
            Debug.Assert(fileName != null); // Enforced by contract
            Debug.Assert(fileName.Length > 0); // Enforced by contract
            IFilePath path = PathBrowsingHelpers.GetBrotherFileWithName(this, fileName);
            var pathTyped = path as IEnvVarFilePath;
            Debug.Assert(pathTyped != null);
            return pathTyped;
         }

         public IEnvVarDirectoryPath GetBrotherDirectoryWithName(string directoryName) {
            Debug.Assert(directoryName != null); // Enforced by contract
            Debug.Assert(directoryName.Length > 0); // Enforced by contract
            IDirectoryPath path = PathBrowsingHelpers.GetBrotherDirectoryWithName(this, directoryName);
            var pathTyped = path as IEnvVarDirectoryPath;
            Debug.Assert(pathTyped != null);
            return pathTyped;
         }

         public IEnvVarFilePath UpdateExtension(string newExtension) {
            // All these 3 assertions have been checked by contract!
            Debug.Assert(newExtension != null);
            Debug.Assert(newExtension.Length >= 2);
            Debug.Assert(newExtension[0] == '.');
            string pathString = PathBrowsingHelpers.UpdateExtension(this, newExtension);
            Debug.Assert(pathString.IsValidEnvVarFilePath());
            return new EnvVarFilePath(pathString);
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


      }

   }

}
