
using System;
using System.Diagnostics.Contracts;


namespace NDepend.Path {


   ///<summary>
   ///Represents a file path on file system, prefixed with an environment variable.
   ///</summary>
   [ContractClass(typeof(IEnvVarFilePathContract))]
   public interface IEnvVarFilePath : IFilePath, IEnvVarPath {

      ///<summary>
      ///Returns <see cref="EnvVarPathResolvingStatus"/>.<see cref="EnvVarPathResolvingStatus.Success"/> if this file path is prefixed with an environment variable that can be resolved into a drive letter or a UNC absolute file path.
      ///</summary>
      ///<param name="pathFileResolved">It is the absolute file path resolved returned by this method.</param>
      EnvVarPathResolvingStatus TryResolve(out IAbsoluteFilePath pathFileResolved);

      ///<summary>
      ///Returns <see cref="EnvVarPathResolvingStatus"/>.<see cref="EnvVarPathResolvingStatus.Success"/> if this file path is prefixed with an environment variable that can be resolved into a drive letter or a UNC absolute file path.
      ///</summary>
      ///<param name="pathFileResolved">It is the absolute file path resolved returned by this method.</param>
      ///<param name="failureReason">If false is returned, failureReason contains the plain english description of the failure.</param>
      bool TryResolve(out IAbsoluteFilePath pathFileResolved, out string failureReason);


      ///<summary>
      ///Returns a new file path prefixed with an environment variable, refering to a file with name <paramref name="fileName"/>, located in the same directory as this file.
      ///</summary>
      ///<param name="fileName">The brother file name</param>
      new IEnvVarFilePath GetBrotherFileWithName(string fileName);

      ///<summary>
      ///Returns a new directory path prefixed with an environment variable, representing a directory with name <paramref name="directoryName"/>, located in the same directory as this file.
      ///</summary>
      ///<param name="directoryName">The brother directory name.</param>
      new IEnvVarDirectoryPath GetBrotherDirectoryWithName(string directoryName);


      ///<summary>
      ///Returns a new file path prefixed with an environment variable, representing this file with its file name extension updated to <paramref name="newExtension"/>.
      ///</summary>
      ///<param name="newExtension">The new file extension. It must begin with a dot followed by one or many characters.</param>
      new IEnvVarFilePath UpdateExtension(string newExtension);
   }


   [ContractClassFor(typeof(IEnvVarFilePath))]
   internal abstract class IEnvVarFilePathContract : IEnvVarFilePath {
      public EnvVarPathResolvingStatus TryResolve(out IAbsoluteFilePath pathFileResolved) {
         throw new NotImplementedException();
      }

      public bool TryResolve(out IAbsoluteFilePath pathFileResolved, out string failureReason) {
         throw new NotImplementedException();
      }

      public IEnvVarFilePath GetBrotherFileWithName(string fileName) {
         Contract.Requires(fileName != null, "fileName must not be null");
         Contract.Requires(fileName.Length > 0, "fileName must not be empty");
         throw new NotImplementedException();
      }

      public IEnvVarDirectoryPath GetBrotherDirectoryWithName(string directoryName) {
         Contract.Requires(directoryName != null, "directoryName must not be null");
         Contract.Requires(directoryName.Length > 0, "directoryName must not be empty");
         throw new NotImplementedException();
      }

      public IEnvVarFilePath UpdateExtension(string newExtension) {
         Contract.Requires(newExtension != null, "newExtension must not be null");
         Contract.Requires(newExtension.Length >= 2, "newExtension must have at least two characters");
         Contract.Requires(newExtension[0] == '.', "newExtension first character must be a dot");
         throw new NotImplementedException();
      }

      IFilePath IFilePath.GetBrotherFileWithName(string fileName) { throw new NotImplementedException(); }
      IDirectoryPath IFilePath.GetBrotherDirectoryWithName(string directoryName) { throw new NotImplementedException(); }
      IFilePath IFilePath.UpdateExtension(string newExtension) { throw new NotImplementedException(); }

      public abstract bool IsChildOf(IDirectoryPath parentDirectory);
      public abstract bool IsAbsolutePath { get; }
      public abstract bool IsRelativePath { get; }
      public abstract bool IsEnvVarPath { get; }
      public abstract bool IsVariablePath { get; }
      public abstract bool IsDirectoryPath { get; }
      public abstract bool IsFilePath { get; }
      public abstract PathMode PathMode { get; }
      IEnvVarDirectoryPath IEnvVarPath.ParentDirectoryPath { get { throw new NotImplementedException(); } }
      public abstract IDirectoryPath ParentDirectoryPath { get; }
      public abstract bool HasParentDirectory { get; }
      public abstract bool NotEquals(object obj);
      public abstract string FileName { get; }
      public abstract string FileNameWithoutExtension { get; }
      public abstract string FileExtension { get; }
      public abstract bool HasExtension(string extension);
      public abstract EnvVarPathResolvingStatus TryResolve(out IAbsolutePath pathResolved);
      public abstract bool TryResolve(out IAbsolutePath pathResolved, out string failureReason);
      public abstract string EnvVar { get; }
   }

}
