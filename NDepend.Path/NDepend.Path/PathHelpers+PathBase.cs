
using System;
using System.Diagnostics;
using System.Xml;



namespace NDepend.Path {

   partial class PathHelpers {



      private abstract class PathBase : IPath {
         
         //
         //  Ctor
         //
         protected PathBase(string pathString) {
            Debug.Assert(pathString != null);
            Debug.Assert(pathString.Length > 0);

            // At this point we know pathString is a valid path
            // but we need to normalize and resolve inner dir of path string
            var pathStringNormalized = AbsoluteRelativePathHelpers.NormalizeAndResolveInnerSpecialDir(pathString);
            m_PathString = pathStringNormalized;
         }


         //
         //  Private and immutable states
         //
         protected readonly string m_PathString;
         public override string ToString() { return m_PathString; }

         public abstract bool IsAbsolutePath { get; }
         public abstract bool IsRelativePath { get; }
         public abstract bool IsEnvVarPath { get; }
         public abstract bool IsVariablePath { get; }
         public abstract bool IsDirectoryPath { get; }
         public abstract bool IsFilePath { get; }
         public abstract PathMode PathMode { get; }

         public abstract IDirectoryPath ParentDirectoryPath { get; }
         

         //
         // ParentDirectoryPath
         //
         public virtual bool HasParentDirectory {
            get {
               return MiscHelpers.HasParentDirectory(m_PathString);
            }
         }


         public bool IsChildOf(IDirectoryPath parentDirectory) {
            Debug.Assert(parentDirectory != null);
            var parentDirectoryString = parentDirectory.ToString();

            // Don't accept equals pathString!
            var parentDirectoryStringLength = parentDirectoryString.Length;
            if (m_PathString.Length <= parentDirectoryStringLength) { return false; }

            // Possible since at this point (m_PathString.Length > parentDirectoryStringLength)
            var c = m_PathString[parentDirectoryStringLength];

            // Need to check that char at pos pathStringLength is a separator, 
            // else @"D:/Foo bar" is considered as a child of @"D:/Foo".
            // Note that m_PathString is normalized in ctor, hence its separator(s) are DIR_SEPARATOR_CHAR.
            if (c != MiscHelpers.DIR_SEPARATOR_CHAR) { return false; }

            string parentPathLowerCase = parentDirectory.ToString().ToLower();
            string thisPathLowerCase = m_PathString.ToLower();
            return thisPathLowerCase.IndexOf(parentPathLowerCase) == 0;
         }



         //
         // Comparison
         //
         private bool PrivateEquals(IPath path) {
            Debug.Assert(path != null);
            if (this.PathMode != path.PathMode) {
               return false;
            }
            // A FilePath could be equal to a DirectoryPath
            if (this.IsDirectoryPath != path.IsDirectoryPath) {
               return false;
            }
            return String.Compare(this.m_PathString, path.ToString(), true) == 0;
         }


         public bool NotEquals(object obj) {
            return !Equals(obj);
         }

         public override bool Equals(object obj) {
            var path = obj as IPath;
            if (!ReferenceEquals(path, null)) {
               // Comparaison of content.
               return this.PrivateEquals(path);
            }
            return false;
         }



         //
         //  GetHashCode() when pathString is key in Dictionnary
         //
         public override int GetHashCode() {
            return m_PathString.ToLower().GetHashCode() +
               (this.IsAbsolutePath ? 1231 : 5677) +
               (this.IsFilePath ? 1457 : 3461);
         }


 

      }
   }
}
