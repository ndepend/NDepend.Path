using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using NDepend.Helpers;


namespace NDepend.Path {

   ///<summary>
   ///Represents an absolute path to a directory on file system.
   ///</summary>
   ///<remarks>
   ///The path represented can exist or not.
   ///The extension method <see cref="PathHelpers.ToAbsoluteDirectoryPath(string)"/> can be called to create a new IAbsoluteDirectoryPath object from a string.
   ///</remarks>
   [ContractClass(typeof(IAbsoluteDirectoryPathContract))]
   public interface IAbsoluteDirectoryPath : IDirectoryPath, IAbsolutePath {

      ///<summary>
      ///Returns a DirectoryInfo object representing this absolute directory path.
      ///</summary>
      ///<exception cref="DirectoryNotFoundException">This absolute directory path doesn't refer to an existing directory.</exception>
      ///<seealso cref="P:NDepend.Path.IAbsolutePath.Exists"/>
      DirectoryInfo DirectoryInfo { get; }

      ///<summary>
      ///Returns a read-only list of file paths absolute matching files contained in this directory.
      ///</summary>
      ///<exception cref="DirectoryNotFoundException">This absolute directory path doesn't refer to an existing directory.</exception>
      ///<seealso cref="P:NDepend.Path.IAbsolutePath.Exists"/>
      IReadOnlyList<IAbsoluteFilePath> ChildrenFilesPath { get; }

      ///<summary>
      ///Returns a read-only list of directory paths absolute matching directories contained in this directory.
      ///</summary>
      ///<exception cref="DirectoryNotFoundException">This absolute directory path doesn't refer to an existing directory.</exception>
      ///<seealso cref="P:NDepend.Path.IAbsolutePath.Exists"/>
      IReadOnlyList<IAbsoluteDirectoryPath> ChildrenDirectoriesPath { get; }


      
      ///<summary>
      ///Returns a new absolute file path representing a file with name <paramref name="fileName"/>, located in the parent's directory of this directory.
      ///</summary>
      ///<remarks>This directory nor the returned file need to exist for this operation to complete properly.</remarks>
      ///<param name="fileName">The brother file name.</param>
      ///<exception cref="InvalidOperationException">This absolute directory path doesn't have a parent directory.</exception>
      new IAbsoluteFilePath GetBrotherFileWithName(string fileName);


      ///<summary>
      ///Returns a new absolute directory path representing a directory with name <paramref name="directoryName"/>, located in the parent's directory of this directory.
      ///</summary>
      ///<remarks>This directory nor the returned directory need to exist for this operation to complete properly.</remarks>
      ///<param name="directoryName">The brother directory name.</param>
      ///<exception cref="InvalidOperationException">This absolute directory path doesn't have a parent directory.</exception>
      new IAbsoluteDirectoryPath GetBrotherDirectoryWithName(string directoryName);


      ///<summary>
      ///Returns a new absolute file path representing a file with name <paramref name="fileName"/>, located in this directory.
      ///</summary>
      ///<remarks>This directory nor the returned file need to exist for this operation to complete properly.</remarks>
      ///<param name="fileName">The child file name.</param>
      new IAbsoluteFilePath GetChildFileWithName(string fileName);

      ///<summary>
      ///Returns a new absolute directory path representing a directory with name <paramref name="directoryName"/>, located in this directory.
      ///</summary>
      ///<remarks>This directory nor the returned directory need to exist for this operation to complete properly.</remarks>
      ///<param name="directoryName">The child directory name.</param>
      ///<exception cref="InvalidOperationException">This absolute directory path doesn't have a parent directory.</exception>
      new IAbsoluteDirectoryPath GetChildDirectoryWithName(string directoryName);

      ///<summary>
      ///Compute this directory as relative from <paramref name="pivotDirectory"/>. If this directory is "C:\Dir1\Dir2" and <paramref name="pivotDirectory"/> is "C:\Dir1\Dir3", the returned relative directory is "..\Dir2".
      ///</summary>
      ///<remarks>
      ///This directory nor <paramref name="pivotDirectory"/> need to exist for this operation to complete properly.
      ///</remarks>
      ///<param name="pivotDirectory">The pivot directory from which the relative path is computed.</param>
      ///<exception cref="ArgumentException"><paramref name="pivotDirectory"/> is not on the same drive as this directory's drive.</exception>
      ///<returns>A new relative directory path representing this directory relative to <paramref name="pivotDirectory"/>.</returns>
      new IRelativeDirectoryPath GetRelativePathFrom(IAbsoluteDirectoryPath pivotDirectory);
   }


   [ContractClassFor(typeof(IAbsoluteDirectoryPath))]
   abstract class IAbsoluteDirectoryPathContract : IAbsoluteDirectoryPath {
      public string DirectoryName {
         get { throw new NotImplementedException(); }
      }

      public DirectoryInfo DirectoryInfo {
         get { throw new NotImplementedException(); }
      }

      public IReadOnlyList<IAbsoluteFilePath> ChildrenFilesPath {
         get { throw new NotImplementedException(); }
      }

      public IReadOnlyList<IAbsoluteDirectoryPath> ChildrenDirectoriesPath {
         get { throw new NotImplementedException(); }
      }

      public IAbsoluteFilePath GetBrotherFileWithName(string fileName) {
         Contract.Requires(fileName != null, "fileName must not be null");
         Contract.Requires(fileName.Length > 0, "fileName must not be empty");
         throw new NotImplementedException();
      }

      public IAbsoluteDirectoryPath GetBrotherDirectoryWithName(string directoryName) {
         Contract.Requires(directoryName != null, "directoryName must not be null");
         Contract.Requires(directoryName.Length > 0, "directoryName must not be empty");
         throw new NotImplementedException();
      }

      public IAbsoluteFilePath GetChildFileWithName(string fileName) {
         Contract.Requires(fileName != null, "fileName must not be null");
         Contract.Requires(fileName.Length > 0, "fileName must not be empty");
         throw new NotImplementedException();
      }

      public IAbsoluteDirectoryPath GetChildDirectoryWithName(string directoryName) {
         Contract.Requires(directoryName != null, "directoryName must not be null");
         Contract.Requires(directoryName.Length > 0, "directoryName must not be empty");
         throw new NotImplementedException();
      }

      IDirectoryPath IPath.ParentDirectoryPath {
         get { throw new NotImplementedException(); }
      }

      public IAbsoluteDirectoryPath ParentDirectoryPath {
         get {

            throw new NotImplementedException();
         }
      }

      IRelativeDirectoryPath IAbsoluteDirectoryPath.GetRelativePathFrom(IAbsoluteDirectoryPath pivotDirectory) {
         Contract.Requires(pivotDirectory != null, "pivotDirectory must not be null");
         throw new NotImplementedException();
      }
      public abstract IRelativePath GetRelativePathFrom(IAbsoluteDirectoryPath pivotDirectory);

      IFilePath IDirectoryPath.GetBrotherFileWithName(string fileName) { throw new NotImplementedException(); }
      IDirectoryPath IDirectoryPath.GetBrotherDirectoryWithName(string directoryName) { throw new NotImplementedException(); }
      IFilePath IDirectoryPath.GetChildFileWithName(string fileName) { throw new NotImplementedException(); }
      IDirectoryPath IDirectoryPath.GetChildDirectoryWithName(string directoryName) { throw new NotImplementedException(); }

      public abstract bool IsChildOf(IDirectoryPath parentDirectory);
      public abstract bool CanGetRelativePathFrom(IAbsoluteDirectoryPath pivotDirectory);
      public abstract bool CanGetRelativePathFrom(IAbsoluteDirectoryPath pivotDirectory, out string failureReason);
      public abstract bool IsAbsolutePath { get; }
      public abstract bool IsRelativePath { get; }
      public abstract bool IsEnvVarPath { get; }
      public abstract bool IsVariablePath { get; }
      public abstract bool IsDirectoryPath { get; }
      public abstract bool IsFilePath { get; }
      public abstract PathMode PathMode { get; }
      public abstract bool HasParentDirectory { get; }
      public abstract bool NotEquals(object obj);
      public abstract AbsolutePathKind Kind { get; }
      public abstract IDriveLetter DriveLetter { get; }
      public abstract string UNCServer { get; }
      public abstract string UNCShare { get; }
      public abstract bool TryResolveEnvironmentVariable(out IAbsolutePath pathResolved);
      public abstract bool OnSameVolumeThan(IAbsolutePath pathAbsoluteOther);
      public abstract bool Exists { get; }
   }

}
