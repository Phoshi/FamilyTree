using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FamilyTree.GraphImpl {
    /// <summary>
    /// A vertex container
    /// </summary>
    /// <typeparam name="T">The type of the vertex item</typeparam>
    class Vertex<T> : IComparable<T>, IComparable, IComparable<Vertex<T>> where T : IComparable<T>{
        /// <summary>
        /// The vertex item
        /// </summary>
        public T Value { get; internal set; }

        /// <summary>
        /// Creates a new vertex wrapper for the specified item
        /// </summary>
        /// <param name="value">The item to wrap</param>
        public Vertex(T value){
            Value = value;
        }

        /// <summary>
        /// Compares the vertex item with the other wrapper's vertex item
        /// </summary>
        /// <param name="other">The other wrapper</param>
        /// <returns>Standard CompareTo result</returns>
        public int CompareTo(Vertex<T> other){
            return Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Attempts a comparison between this wrapper's value and the given value
        /// </summary>
        /// <param name="other">The other value</param>
        /// <returns>Standard CompareTo Result</returns>
        public int CompareTo(T other){
            return Value.CompareTo(other);
        }

        /// <summary>
        /// Returns a textual representation of the vertex item
        /// </summary>
        /// <returns>The string representation of Value</returns>
        public override string ToString(){
            return Value.ToString();
        }

        /// <summary>
        /// Attempts a comparison with object, however does not guarantee a sane result unless object is of type Vertex.
        /// Will throw NotSupportexException in that case.
        /// </summary>
        /// <param name="obj">An object to compare</param>
        /// <returns>Standard CompareTo Result.</returns>
        public int CompareTo(object obj){
            if (obj is Vertex<T>){
                return CompareTo((Vertex<T>) obj);
            }
            throw new NotSupportedException("Cannot compare with given type.");
        }
    }
}
