
namespace NDepend.Helpers {

   ///<summary>
   ///Represents a read-only collection of elements that can be accessed by index.
   ///</summary>
   ///<remarks>A <see cref="IReadOnlyList{T}"/> object can be obtain through the extension methods <see cref="ExtensionMethodsEnumerable.ToReadOnlyWrappedList{T}"/> or  <see cref="ExtensionMethodsEnumerable.ToReadOnlyClonedList{T}"/>.</remarks>
   ///<typeparam name="T">The type parameter of the items in the collection.</typeparam>
   public interface IReadOnlyList<T> : IReadOnlyCollection<T> {
      ///<summary>
      ///Gets the element at the specified index in the read-only list.
      ///</summary>
      ///<param name="index">The zero-based index of the element to get.</param>
      T this[int index] { get; }
   }
}
