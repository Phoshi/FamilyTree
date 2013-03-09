using System;
using System.Collections.Generic;

namespace FamilyTree.RBTreeImpl{
    /// <summary>
    /// Implements the abstract interface for an iterable tree structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface ITree<T> : IEnumerable<T> where T : IComparable<T>{
        /// <summary>
        /// Inserts an element into the tree
        /// </summary>
        /// <param name="element">The element to insert</param>
        void Insert(T element);

        /// <summary>
        /// Returns if the tree contains this element
        /// </summary>
        /// <param name="element">The element to look for</param>
        /// <returns>Whether the element exists</returns>
        bool Contains(T element);

        /// <summary>
        /// Returns the element which fulfils the given predicate
        /// </summary>
        /// <param name="pred">A function of signature (T) => int, where int is a CompareTo style comparison return</param>
        /// <returns>The element matching the given function</returns>
        T Get(Func<T, int> pred);

        /// <summary>
        /// Removes the given item from the tree
        /// </summary>
        /// <param name="element">The item to remove</param>
        void Remove(T element);
    }
}