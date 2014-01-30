
using System.Collections.Generic;
using System.Diagnostics;

using NDepend.Helpers;

namespace NDepend.Path {

   partial class PathHelpers {

      private sealed class VariableFilePath : VariablePathBase, IVariableFilePath {

         internal VariableFilePath(string pathString)
            : base(pathString) {
            Debug.Assert(pathString != null);
            Debug.Assert(pathString.Length > 0);
            Debug.Assert(pathString.IsValidVariableFilePath());
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
         public VariablePathResolvingStatus TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsoluteFilePath pathFileResolved) {
            Debug.Assert(variablesValues != null);
            IReadOnlyList<string> unresolvedVariablesUnused;
            return TryResolve(variablesValues, out pathFileResolved, out unresolvedVariablesUnused);
         }

         public VariablePathResolvingStatus TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsoluteFilePath pathFileResolved, out IReadOnlyList<string> unresolvedVariables) {
            Debug.Assert(variablesValues != null);
            string pathStringResolved;
            if (!base.TryResolve(variablesValues, out pathStringResolved, out unresolvedVariables)) {
               pathFileResolved = null;
               return VariablePathResolvingStatus.ErrorUnresolvedVariable;
            }
            if (!pathStringResolved.IsValidAbsoluteFilePath()) {
               pathFileResolved = null;
               return VariablePathResolvingStatus.ErrorVariableResolvedButCannotConvertToAbsolutePath;
            }
            pathFileResolved = pathStringResolved.ToAbsoluteFilePath();
            return VariablePathResolvingStatus.Success;
         }

         public bool TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsoluteFilePath pathFileResolved, out string failureReason) {
            Debug.Assert(variablesValues != null);
            IReadOnlyList<string> unresolvedVariables;
            var status = TryResolve(variablesValues, out pathFileResolved, out unresolvedVariables);
            switch (status) {
               default:
                  Debug.Assert(status == VariablePathResolvingStatus.Success);
                  failureReason = null;
                  return true;
               case VariablePathResolvingStatus.ErrorUnresolvedVariable:
                  Debug.Assert(unresolvedVariables != null);
                  Debug.Assert(unresolvedVariables.Count > 0);
                  failureReason = VariablePathHelpers.GetUnresolvedVariableFailureReason(unresolvedVariables);
                  return false;
               case VariablePathResolvingStatus.ErrorVariableResolvedButCannotConvertToAbsolutePath:
                  failureReason = base.GetVariableResolvedButCannotConvertToAbsolutePathFailureReason(variablesValues, "file");
                  return false;
            }
         }
   

         public override VariablePathResolvingStatus TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsolutePath pathResolved) {
            IAbsoluteFilePath pathFileResolved;
            var resolvingStatus = this.TryResolve(variablesValues, out pathFileResolved);
            pathResolved = pathFileResolved;
            return resolvingStatus;
         }

         public override VariablePathResolvingStatus TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsolutePath pathResolved, out IReadOnlyList<string> unresolvedVariables) {
            IAbsoluteFilePath pathFileResolved;
            var resolvingStatus = this.TryResolve(variablesValues, out pathFileResolved, out unresolvedVariables);
            pathResolved = pathFileResolved;
            return resolvingStatus;
         }

         public override bool TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsolutePath pathResolved, out string failureReason) {
            IAbsoluteFilePath pathFileResolved;
            var b = this.TryResolve(variablesValues, out pathFileResolved, out failureReason);
            pathResolved = pathFileResolved;
            return b;
         }




         //
         //  Path Browsing facilities
         //
         public IVariableFilePath GetBrotherFileWithName(string fileName) {
            Debug.Assert(fileName != null); // Enforced by contract
            Debug.Assert(fileName.Length > 0); // Enforced by contract
            IFilePath path = PathBrowsingHelpers.GetBrotherFileWithName(this, fileName);
            var pathTyped = path as IVariableFilePath;
            Debug.Assert(pathTyped != null);
            return pathTyped;
         }

         public IVariableDirectoryPath GetBrotherDirectoryWithName(string directoryName) {
            Debug.Assert(directoryName != null); // Enforced by contract
            Debug.Assert(directoryName.Length > 0); // Enforced by contract
            IDirectoryPath path = PathBrowsingHelpers.GetBrotherDirectoryWithName(this, directoryName);
            var pathTyped = path as IVariableDirectoryPath;
            Debug.Assert(pathTyped != null);
            return pathTyped;
         }

         public IVariableFilePath UpdateExtension(string newExtension) {
            // All these 3 assertions have been checked by contract!
            Debug.Assert(newExtension != null);
            Debug.Assert(newExtension.Length >= 2);
            Debug.Assert(newExtension[0] == '.');
            string pathString = PathBrowsingHelpers.UpdateExtension(this, newExtension);
            Debug.Assert(pathString.IsValidVariableFilePath());
            return new VariableFilePath(pathString);
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
