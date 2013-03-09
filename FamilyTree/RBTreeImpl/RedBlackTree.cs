using System;
using System.Collections;
using System.Collections.Generic;

namespace FamilyTree.RBTreeImpl {
    /// <summary>
    /// Implements a Red Black Binary Search Tree containing elements of type T
    /// </summary>
    /// <typeparam name="T">Element type. Must implement IComparable</typeparam>
    class RedBlackTree<T> : ITree<T> where T : IComparable<T>{
        /// <summary>
        /// The root node of the tree
        /// </summary>
        private RedBlackTreeNode<T> _tree;

        public void Insert(T element){
            if (_tree == null){
                _tree = new RedBlackTreeNode<T>(element){Colour = TreeColours.Black};
            }
            else{
                _tree = _tree.Insert(element);
            }
        }

        public bool Contains(T element){
            return _tree.Contains(element);
        }

        public T Get(Func<T, int> pred){
            return _tree.Get(pred).Value;
        }

        public void Remove(T element){
            if (Contains(element)){
                _tree.Remove(element);
            }
        }

        public IEnumerator<T> GetEnumerator(){
            return new RedBlackEnumerator<T>(_tree);
        }

        IEnumerator IEnumerable.GetEnumerator(){
            return GetEnumerator();
        }
    }
}
