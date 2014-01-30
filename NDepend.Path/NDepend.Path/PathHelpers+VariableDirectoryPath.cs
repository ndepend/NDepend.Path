
using System.Collections.Generic;
using System.Diagnostics;

using NDepend.Helpers;

namespace NDepend.Path {

   partial class PathHelpers {

      private sealed class VariableDirectoryPath : VariablePathBase, IVariableDirectoryPath {

         internal VariableDirectoryPath(string pathString)
            : base(pathString) {
            Debug.Assert(pathString != null);
            Debug.Assert(pathString.Length > 0);
            Debug.Assert(pathString.IsValidVariableDirectoryPath());
         }

         //
         //  DirectoryName
         //
         public string DirectoryName { get { return MiscHelpers.GetLastName(m_PathString); } }

         public override bool IsDirectoryPath { get { return true; } }
         public override bool IsFilePath { get { return false; } }




         //
         // Path resolving
         //
         public VariablePathResolvingStatus TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsoluteDirectoryPath pathDirectoryResolved) {
            Debug.Assert(variablesValues != null);
            IReadOnlyList<string> unresolvedVariablesUnused;
            return TryResolve(variablesValues, out pathDirectoryResolved, out unresolvedVariablesUnused);
         }

         public VariablePathResolvingStatus TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsoluteDirectoryPath pathDirectoryResolved, out IReadOnlyList<string> unresolvedVariables) {
            Debug.Assert(variablesValues != null);
            string pathStringResolved;
            if (!base.TryResolve(variablesValues, out pathStringResolved, out unresolvedVariables)) {
               pathDirectoryResolved = null;
               return VariablePathResolvingStatus.ErrorUnresolvedVariable;
            }
            if (!pathStringResolved.IsValidAbsoluteDirectoryPath()) {
               pathDirectoryResolved = null;
               return VariablePathResolvingStatus.ErrorVariableResolvedButCannotConvertToAbsolutePath;
            }
            pathDirectoryResolved = pathStringResolved.ToAbsoluteDirectoryPath();
            return VariablePathResolvingStatus.Success;
         }

         public bool TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsoluteDirectoryPath pathDirectoryResolved, out string failureReason) {
            Debug.Assert(variablesValues != null);
            IReadOnlyList<string> unresolvedVariables;
            var status = TryResolve(variablesValues, out pathDirectoryResolved, out unresolvedVariables);
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
                  failureReason = base.GetVariableResolvedButCannotConvertToAbsolutePathFailureReason(variablesValues, "directory");
                  return false;
            }
         }


         public override VariablePathResolvingStatus TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsolutePath pathResolved) {
            IAbsoluteDirectoryPath pathDirectoryResolved;
            var resolvingStatus = this.TryResolve(variablesValues, out pathDirectoryResolved);
            pathResolved = pathDirectoryResolved;
            return resolvingStatus;
         }

         public override VariablePathResolvingStatus TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsolutePath pathResolved, out IReadOnlyList<string> unresolvedVariables) {
            IAbsoluteDirectoryPath pathDirectoryResolved;
            var resolvingStatus = this.TryResolve(variablesValues, out pathDirectoryResolved, out unresolvedVariables);
            pathResolved = pathDirectoryResolved;
            return resolvingStatus;
         }

         public override bool TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsolutePath pathResolved, out string failureReason) {
            IAbsoluteDirectoryPath pathDirectoryResolved;
            var b = this.TryResolve(variablesValues, out pathDirectoryResolved, out failureReason);
            pathResolved = pathDirectoryResolved;
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

         public IVariableFilePath GetChildFileWithName(string fileName) {
            Debug.Assert(fileName != null); // Enforced by contract
            Debug.Assert(fileName.Length > 0); // Enforced by contract
            string pathString = PathBrowsingHelpers.GetChildFileWithName(this, fileName);
            Debug.Assert(pathString.IsValidVariableFilePath());
            return new VariableFilePath(pathString);
         }

         public IVariableDirectoryPath GetChildDirectoryWithName(string directoryName) {
            Debug.Assert(directoryName != null); // Enforced by contract
            Debug.Assert(directoryName.Length > 0); // Enforced by contract
            string pathString = PathBrowsingHelpers.GetChildDirectoryWithName(this, directoryName);
            Debug.Assert(pathString.IsValidVariableDirectoryPath());
            return new VariableDirectoryPath(pathString);
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
      }

   }

}
