using System;
using System.Collections;
using System.Collections.Generic;

namespace FamilyTree.RBTreeImpl{
    class RedBlackEnumerator<T> : IEnumerator<T> where T : IComparable<T>{
        private RedBlackTreeNode<T> _root;
        private RedBlackTreeNode<T> _pointer; 

        private List<RedBlackTreeNode<T>> _visited = new List<RedBlackTreeNode<T>>(); 

        private bool isVisited(RedBlackTreeNode<T> node){
            return _visited.Contains(node);
        }

        public RedBlackEnumerator(RedBlackTreeNode<T> tree){
            _root = tree;
            _pointer = null;
        }

        public void Dispose(){
            _root = null;
            _pointer = null;
        }

        public bool MoveNext(){
            if (_pointer == null){
                _pointer = _root;
                return _root != null;
            }

            if (_pointer.Left != null && !isVisited(_pointer.Left)){
                _pointer = _pointer.Left;
            }
            else if (_pointer.Right != null && !isVisited(_pointer.Right)){
                _pointer = _pointer.Right;
            }
            else{
                _pointer = _pointer.GetNextIterationPoint();
                if (_pointer == null){
                    return false;
                }
            }

            if (_pointer.Deleted) {
                return MoveNext();
            }
            return true;
        }

        public void Reset(){
            _pointer = _root;
        }

        public T Current{
            get{
                _visited.Add(_pointer);
                return _pointer.Value;
            }
        }

        object IEnumerator.Current{
            get { return Current; }
        }
    }
}