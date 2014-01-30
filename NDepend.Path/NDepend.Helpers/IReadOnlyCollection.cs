

using System.Collections.Generic;


namespace NDepend.Helpers {



   // These 2 interfaces exist in .NET Fx v4.5!

   ///<summary>
   ///Defines the property getter Count that defines a read-only collections.
   ///</summary>
   ///<remarks>A <see cref="IReadOnlyCollection{T}"/> object can be obtain through the extension methods <see cref="ExtensionMethodsEnumerable.ToReadOnlyWrappedCollection{T}"/> or  <see cref="ExtensionMethodsEnumerable.ToReadOnlyClonedCollection{T}"/>.</remarks>
   ///<typeparam name="T">The type parameter of the items in the collection.</typeparam>
   public interface IReadOnlyCollection<T> : IEnumerable<T> {
      ///<summary>
      ///Gets the number of elements contained in the collection.
      ///</summary>
      int Count { get; }
   }

}
