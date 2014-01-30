


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using NDepend.Helpers;

namespace NDepend.Path {

   partial class PathHelpers {

      private abstract class VariablePathBase : PathBase, IVariablePath {


         protected VariablePathBase(string pathString) :
            base(pathString) {
            Debug.Assert(pathString != null);
            Debug.Assert(pathString.Length > 0);
            Debug.Assert(pathString.IsNormalized());

            Debug.Assert(m_PathString != null);
            Debug.Assert(m_PathString.Length > 0);
            Debug.Assert(m_PathString.IsNormalized());

            // It is important to use m_PathString and not pathString in IsAVariablePath() !
            // Indeed, since InnerSpecialDir have been resolved, some variable might have disappeared
            // like if pathString was "$(v1)\$(v2)\.." and m_PathString became "$(v1)"
            IReadOnlyList<string> variables;
            string failureReasonUnused;
            var b = VariablePathHelpers.IsAVariablePath(m_PathString, out variables, out failureReasonUnused);
            Debug.Assert(b);
            Debug.Assert(variables != null);
            Debug.Assert(variables.Count > 0);
            Debug.Assert(variables.All(v => v != null));
            Debug.Assert(variables.All(v => v.Length > 0));
            m_Variables = variables;
         }


         public override bool IsAbsolutePath { get { return false; } }
         public override bool IsRelativePath { get { return false; } }
         public override bool IsEnvVarPath { get { return false; } }
         public override bool IsVariablePath { get { return true; } }
         public override PathMode PathMode { get { return PathMode.Variable; } }



         //
         // Resolving op
         //
         public string PrefixVariable { get { return m_Variables[0]; } }

         private readonly IReadOnlyList<string> m_Variables;
         public IReadOnlyList<string> AllVariables { get { return m_Variables; } }


         // This methods are implemented in VariableFilePath and VariableDirectoryPath.
         public abstract VariablePathResolvingStatus TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsolutePath pathResolved);
         public abstract VariablePathResolvingStatus TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsolutePath pathResolved, out IReadOnlyList<string> unresolvedVariables);
         public abstract bool TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsolutePath pathResolved, out string failureReason);

         protected bool TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out string pathStringResolved, out IReadOnlyList<string> unresolvedVariables) {
            Debug.Assert(variablesValues != null);

            var pathString = m_PathString;

            var unresolvedVariablesList = new List<string>();
            var nbVariablesToResolve = m_Variables.Count;
            Debug.Assert(nbVariablesToResolve > 0);
            for (var i = 0; i < nbVariablesToResolve; i++) {
               var variableNameToResolve = m_Variables[i];
               Debug.Assert(variableNameToResolve != null);
               Debug.Assert(variableNameToResolve.Length > 0);
               bool resolved = false;
               foreach (var pair in variablesValues) {
                  var pairVariableName = pair.Key;
                  // Support these two cases!
                  if (pairVariableName == null) { continue; }
                  if (pairVariableName.Length == 0) { continue; } // 
                  if(String.Compare(pairVariableName, variableNameToResolve, true) != 0) { continue; } // true for ignore case! variable names are case insensitive
                  resolved = true;
                  var variableValue = pair.Value;
                  if (variableValue == null) {
                     // Treat null variableValue as empty string.
                     variableValue = "";
                  }
                  pathString = VariablePathHelpers.ReplaceVariableWithValue(pathString, variableNameToResolve, variableValue);
               }
               if (!resolved) {
                  unresolvedVariablesList.Add(variableNameToResolve);
               }
            }

            if (unresolvedVariablesList.Count > 0) {
               unresolvedVariables = unresolvedVariablesList.ToReadOnlyWrappedList();
               pathStringResolved = null;
               return false;
            }

            unresolvedVariables = null;
            pathStringResolved = pathString;
            return true;
         }


         protected string GetVariableResolvedButCannotConvertToAbsolutePathFailureReason(IEnumerable<KeyValuePair<string, string>> variablesValues, string fileOrDirectory) {
            Debug.Assert(variablesValues != null);
            Debug.Assert(fileOrDirectory != null);
            Debug.Assert(fileOrDirectory.Length > 0);

            // Need to obtain again pathStringResolved to include it into the failureReason!
            string pathStringResolved;
            IReadOnlyList<string> unresolvedVariablesUnused;
            var b = TryResolve(variablesValues, out pathStringResolved, out unresolvedVariablesUnused);
            Debug.Assert(b);
            return @"All variable(s) have been resolved, but the resulting string {" + pathStringResolved + "} cannot be converted to an absolute " + fileOrDirectory + " path.";
         }


         //
         // ParentDirectoryPath 
         // Special impls in VariablePathHelpers !
         //
         IVariableDirectoryPath IVariablePath.ParentDirectoryPath {
            get {
               string parentPath = VariablePathHelpers.GetParentDirectory(m_PathString);
               return parentPath.ToVariableDirectoryPath();
            }
         }
         public override IDirectoryPath ParentDirectoryPath {
            get {
               string parentPath = VariablePathHelpers.GetParentDirectory(m_PathString);
               return parentPath.ToVariableDirectoryPath();
            }
         }

         public override bool HasParentDirectory {
            get {
               return VariablePathHelpers.HasParentDirectory(m_PathString);
            }
         }
      }
   }
}
