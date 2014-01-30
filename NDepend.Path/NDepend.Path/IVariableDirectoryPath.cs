
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using NDepend.Helpers;

namespace NDepend.Path {


   ///<summary>
   ///Represents a directory path on file system, prefixed with an environment variable.
   ///</summary>
   [ContractClass(typeof(IVariableDirectoryPathContract))]
   public interface IVariableDirectoryPath : IDirectoryPath, IVariablePath {

      ///<summary>
      ///Returns <see cref="VariablePathResolvingStatus"/>.<see cref="VariablePathResolvingStatus.Success"/> if <see cref="IVariablePath.AllVariables"/> of this path can be resolved from <paramref name="variablesValues"/> and the path can be resolved into a drive letter or a UNC absolute directory path.
      ///</summary>
      ///<param name="variablesValues">It is the sequence of pairs <i>[variable name/variable value]</i> used to resolve the path.</param>
      ///<param name="pathDirectoryResolved">It is the absolute directory path resolved obtained if this method returns <see cref="VariablePathResolvingStatus"/>.<see cref="VariablePathResolvingStatus.Success"/>.</param>
      VariablePathResolvingStatus TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsoluteDirectoryPath pathDirectoryResolved);

      ///<summary>
      ///Returns <see cref="VariablePathResolvingStatus"/>.<see cref="VariablePathResolvingStatus.Success"/> if <see cref="IVariablePath.AllVariables"/> of this path can be resolved from <paramref name="variablesValues"/> and the path can be resolved into a drive letter or a UNC absolute file path.
      ///</summary>
      ///<param name="variablesValues">It is the sequence of pairs <i>[variable name/variable value]</i> used to resolve the path.</param>
      ///<param name="pathDirectoryResolved">It is the absolute directory path resolved obtained if this method returns <see cref="VariablePathResolvingStatus"/>.<see cref="VariablePathResolvingStatus.Success"/>.</param>
      ///<param name="unresolvedVariables">This list contains one or several variables names unresolved, if this method returns <see cref="VariablePathResolvingStatus"/>.<see cref="VariablePathResolvingStatus.ErrorUnresolvedVariable"/>.</param>
      VariablePathResolvingStatus TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsoluteDirectoryPath pathDirectoryResolved, out IReadOnlyList<string> unresolvedVariables);


      ///<summary>
      ///Returns <i>true</i> if <see cref="IVariablePath.AllVariables"/> of this path can be resolved from <paramref name="variablesValues"/> and the path can be resolved into a drive letter or a UNC absolute directory path.
      ///</summary>
      ///<param name="variablesValues">It is the sequence of pairs <i>[variable name/variable value]</i> used to resolve the path.</param>
      ///<param name="pathDirectoryResolved">It is the absolute directory path resolved obtained if this method returns <i>true</i>.</param>
      ///<param name="failureReason">If <i>false</i> is returned, <paramref name="failureReason"/> contains the plain english description of the failure.</param>
      bool TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsoluteDirectoryPath pathDirectoryResolved, out string failureReason);

      ///<summary>
      ///Returns a new file path containing variables, representing a file with name <paramref name="fileName"/>, located in the parent's directory of this directory.
      ///</summary>
      ///<param name="fileName">The brother file name.</param>
      ///<exception cref="InvalidOperationException">This relative directory path doesn't have a parent directory.</exception>
      new IVariableFilePath GetBrotherFileWithName(string fileName);


      ///<summary>
      ///Returns a new directory path containing variables, representing a directory with name <paramref name="directoryName"/>, located in the parent's directory of this directory.
      ///</summary>
      ///<param name="directoryName">The brother directory name.</param>
      ///<exception cref="InvalidOperationException">This relative directory path doesn't have a parent directory.</exception>
      new IVariableDirectoryPath GetBrotherDirectoryWithName(string directoryName);

      ///<summary>
      ///Returns a new file path containing variables, representing a file with name <paramref name="fileName"/>, located in this directory.
      ///</summary>
      ///<param name="fileName">The child file name.</param>
      new IVariableFilePath GetChildFileWithName(string fileName);


      ///<summary>
      ///Returns a new directory path containing variables, representing a directory with name <paramref name="directoryName"/>, located in this directory.
      ///</summary>
      ///<param name="directoryName">The child directory name.</param>
      new IVariableDirectoryPath GetChildDirectoryWithName(string directoryName);
   }


   [ContractClassFor(typeof(IVariableDirectoryPath))]
   internal abstract class IVariableDirectoryPathContract : IVariableDirectoryPath {

      public VariablePathResolvingStatus TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsoluteDirectoryPath pathDirectoryResolved) {
         Contract.Requires(variablesValues != null, "variablesValues must not be null");
         throw new NotImplementedException();
      }

      public VariablePathResolvingStatus TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsoluteDirectoryPath pathDirectoryResolved, out IReadOnlyList<string> unresolvedVariables) {
         Contract.Requires(variablesValues != null, "variablesValues must not be null");
         throw new NotImplementedException();
      }

      public bool TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsoluteDirectoryPath pathDirectoryResolved, out string failureReason) {
         Contract.Requires(variablesValues != null, "variablesValues must not be null");
         throw new NotImplementedException();
      }

      public IVariableFilePath GetBrotherFileWithName(string fileName) {
         Contract.Requires(fileName != null, "fileName must not be null");
         Contract.Requires(fileName.Length > 0, "fileName must not be empty");
         throw new NotImplementedException();
      }

      public IVariableDirectoryPath GetBrotherDirectoryWithName(string directoryName) {
         Contract.Requires(directoryName != null, "directoryName must not be null");
         Contract.Requires(directoryName.Length > 0, "directoryName must not be empty");
         throw new NotImplementedException();
      }

      public IVariableFilePath GetChildFileWithName(string fileName) {
         Contract.Requires(fileName != null, "fileName must not be null");
         Contract.Requires(fileName.Length > 0, "fileName must not be empty");
         throw new NotImplementedException();
      }

      public IVariableDirectoryPath GetChildDirectoryWithName(string directoryName) {
         Contract.Requires(directoryName != null, "directoryName must not be null");
         Contract.Requires(directoryName.Length > 0, "directoryName must not be empty");
         throw new NotImplementedException();
      }


      IFilePath IDirectoryPath.GetBrotherFileWithName(string fileName) { throw new NotImplementedException(); }
      IDirectoryPath IDirectoryPath.GetBrotherDirectoryWithName(string directoryName) { throw new NotImplementedException(); }
      IFilePath IDirectoryPath.GetChildFileWithName(string fileName) { throw new NotImplementedException(); }
      IDirectoryPath IDirectoryPath.GetChildDirectoryWithName(string directoryName) { throw new NotImplementedException(); }

      public abstract bool IsChildOf(IDirectoryPath parentDirectory);
      public abstract bool IsAbsolutePath { get; }
      public abstract bool IsRelativePath { get; }
      public abstract bool IsEnvVarPath { get; }
      public abstract bool IsVariablePath { get; }
      public abstract bool IsDirectoryPath { get; }
      public abstract bool IsFilePath { get; }
      public abstract PathMode PathMode { get; }
      public abstract VariablePathResolvingStatus TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsolutePath pathResolved);
      public abstract VariablePathResolvingStatus TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsolutePath pathResolved, out IReadOnlyList<string> unresolvedVariables);
      public abstract bool TryResolve(IEnumerable<KeyValuePair<string, string>> variablesValues, out IAbsolutePath pathResolved, out string failureReason);
      public abstract string PrefixVariable { get; }
      public abstract IReadOnlyList<string> AllVariables { get; }
      IVariableDirectoryPath IVariablePath.ParentDirectoryPath { get { throw new NotImplementedException(); } }
      public abstract IDirectoryPath ParentDirectoryPath { get; }
      public abstract bool HasParentDirectory { get; }
      public abstract bool NotEquals(object obj);
      public abstract string DirectoryName { get; }
      public abstract EnvVarPathResolvingStatus TryResolve(out IAbsolutePath pathResolved);
      public abstract bool TryResolve(out IAbsolutePath pathResolved, out string failureReason);
   }

}
