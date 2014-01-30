using System;
using System.Diagnostics;


namespace NDepend.Path
{

   partial class PathHelpers {

      private abstract class RelativePathBase : PathBase, IRelativePath {

         protected RelativePathBase(string pathString) : 
            base(pathString) {
            Debug.Assert(pathString != null);
            Debug.Assert(pathString.Length > 0);
         }

         public override bool IsAbsolutePath { get { return false; } }
         public override bool IsRelativePath { get { return true; } }
         public override bool IsEnvVarPath { get { return false; } }
         public override bool IsVariablePath { get { return false; } }
         public override PathMode PathMode { get { return PathMode.Relative; } }


         IRelativeDirectoryPath IRelativePath.ParentDirectoryPath {
            get {
               string parentPath = MiscHelpers.GetParentDirectory(m_PathString);
               return parentPath.ToRelativeDirectoryPath();
            }
         }
         public override IDirectoryPath ParentDirectoryPath {
            get {
               string parentPath = MiscHelpers.GetParentDirectory(m_PathString);
               return parentPath.ToRelativeDirectoryPath();
            }
         }


         //
         //  Absolute/Relative pathString conversion
         //
         public abstract IAbsolutePath GetAbsolutePathFrom(IAbsoluteDirectoryPath path);

         public bool CanGetAbsolutePathFrom(IAbsoluteDirectoryPath path) {
            Debug.Assert(path != null); // Enforced by contract
            string pathAbsoluteUnused, failureReasonUnused;
            return AbsoluteRelativePathHelpers.TryGetAbsolutePathFrom(path, this, out pathAbsoluteUnused, out failureReasonUnused);
         }

         public bool CanGetAbsolutePathFrom(IAbsoluteDirectoryPath path, out string failureReason) {
            Debug.Assert(path != null); // Enforced by contract
            string pathAbsoluteUnused;
            return AbsoluteRelativePathHelpers.TryGetAbsolutePathFrom(path, this, out pathAbsoluteUnused, out failureReason);
         }
      }
   }
}
