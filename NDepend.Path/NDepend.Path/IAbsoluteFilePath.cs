using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace NDepend.Path {


   ///<summary>
   ///Represents an absolute path to a file on file system.
   ///</summary>
   ///<remarks>
   ///The path represented can exist or not.
   ///The extension method <see cref="PathHelpers.ToAbsoluteFilePath(string)"/> can be called to create a new IAbsoluteFilePath object from a string.
   ///</remarks>
   [ContractClass(typeof(IAbsoluteFilePathContract))]
   public interface IAbsoluteFilePath : IFilePath, IAbsolutePath {

      ///<summary>
      ///Returns a FileInfo object corresponding to this absolute file path.
      ///</summary>
      ///<exception cref="FileNotFoundException">This absolute directory path doesn't refer to an existing directory.</exception>
      ///<seealso cref="P:NDepend.Path.IAbsolutePath.Exists"/>
      FileInfo FileInfo { get; }

      ///<summary>
      ///Compute this file as relative from <paramref name="pivotDirectory"/>. If this file is "C:\Dir1\Dir2\File.txt" and <paramref name="pivotDirectory"/> is "C:\Dir1\Dir3", the returned relative file path is "..\Dir2\File.txt".
      ///</summary>
      ///<remarks>
      ///This file nor <paramref name="pivotDirectory"/> need to exist for this operation to complete properly.
      ///</remarks>
      ///<param name="pivotDirectory">The pivot directory from which the relative path is computed.</param>
      ///<exception cref="ArgumentException"><paramref name="pivotDirectory"/> is not on the same drive as this file's drive.</exception>
      ///<returns>A new relative file path representing this file relative to <paramref name="pivotDirectory"/>.</returns>
      new IRelativeFilePath GetRelativePathFrom(IAbsoluteDirectoryPath pivotDirectory);



      ///<summary>
      ///Returns a new absolute file path refering to a file with name <paramref name="fileName"/>, located in the same directory as this file.
      ///</summary>
      ///<remarks>This file nor the returned file need to exist for this operation to complete properly.</remarks>
      ///<param name="fileName">The brother file name</param>
      new IAbsoluteFilePath GetBrotherFileWithName(string fileName);

      ///<summary>
      ///Returns a new absolute directory path representing a directory with name <paramref name="directoryName"/>, located in the same directory as this file.
      ///</summary>
      ///<remarks>This file nor the returned directory need to exist for this operation to complete properly.</remarks>
      ///<param name="directoryName">The brother directory name.</param>
      new IAbsoluteDirectoryPath GetBrotherDirectoryWithName(string directoryName);

      ///<summary>
      ///Returns a new absolute file path representing this file with its file name extension updated to <paramref name="newExtension"/>.
      ///</summary>
      ///<remarks>
      ///The returned file nor this file need to exist for this operation to complete properly.
      ///</remarks>
      ///<param name="newExtension">The new file extension. It must begin with a dot followed by one or many characters.</param>
      new IAbsoluteFilePath UpdateExtension(string newExtension);
   }


   [ContractClassFor(typeof(IAbsoluteFilePath))]
   abstract class IAbsoluteFilePathContract : IAbsoluteFilePath {

      public FileInfo FileInfo {
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

      public IAbsoluteFilePath UpdateExtension(string newExtension) {
         Contract.Requires(newExtension != null, "newExtension must not be null");
         Contract.Requires(newExtension.Length >= 2, "newExtension must have at least two characters");
         Contract.Requires(newExtension[0] == '.', "newExtension first character must be a dot");
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

     
      IRelativeFilePath IAbsoluteFilePath.GetRelativePathFrom(IAbsoluteDirectoryPath pivotDirectory) {
         Contract.Requires(pivotDirectory != null, "pivotDirectory must not be null");
         throw new NotImplementedException();
      }
      public abstract IRelativePath GetRelativePathFrom(IAbsoluteDirectoryPath pivotDirectory);

      public string FileName {
         get { throw new NotImplementedException(); }
      }

      public string FileNameWithoutExtension {
         get { throw new NotImplementedException(); }
      }

      public string FileExtension {
         get { throw new NotImplementedException(); }
      }

      public bool HasExtension(string extension) {
         throw new NotImplementedException();
      }


      IFilePath IFilePath.GetBrotherFileWithName(string fileName) { throw new NotImplementedException(); }
      IDirectoryPath IFilePath.GetBrotherDirectoryWithName(string directoryName) { throw new NotImplementedException(); }
      IFilePath IFilePath.UpdateExtension(string newExtension) { throw new NotImplementedException(); }
      public abstract bool CanGetRelativePathFrom(IAbsoluteDirectoryPath pivotDirectory);
      public abstract bool CanGetRelativePathFrom(IAbsoluteDirectoryPath pivotDirectory, out string failureReason);

      public abstract bool IsChildOf(IDirectoryPath parentDirectory);
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
