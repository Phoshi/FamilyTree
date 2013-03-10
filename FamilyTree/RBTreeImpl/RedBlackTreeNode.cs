using System;

namespace FamilyTree.RBTreeImpl {
    /// <summary>
    /// The internal node of a RedBlack BST
    /// </summary>
    /// <typeparam name="T">The type of the contained elements</typeparam>
    class RedBlackTreeNode<T> where T : IComparable<T>{
        /// <summary>
        /// The node's colour
        /// </summary>
        public TreeColours Colour = TreeColours.Red;

        /// <summary>
        /// The child node to the left
        /// </summary>
        public RedBlackTreeNode<T> Left;

        /// <summary>
        /// The child node to the right
        /// </summary>
        public RedBlackTreeNode<T> Right;

        /// <summary>
        /// The parent node
        /// </summary>
        public RedBlackTreeNode<T> Parent;

        /// <summary>
        /// The parent node of the parent node
        /// </summary>
        public RedBlackTreeNode<T> GrandParent { get { return getGrandparent(); } }

        /// <summary>
        /// The grandparent node's other child node (I.E. Not the parent node)
        /// </summary>
        public RedBlackTreeNode<T> Uncle { get { return getUncle(); } }

        /// <summary>
        /// The root node of the tree
        /// </summary>
        public RedBlackTreeNode<T> Root{
            get { return Parent == null ? this : Parent.Root; }
        }

        /// <summary>
        /// Whether this node has been deleted
        /// </summary>
        public bool Deleted; //TODO: Actually implement proper deletion at some point

        /// <summary>
        /// The value of this node
        /// </summary>
        public T Value;

        /// <summary>
        /// Returns the parent's parent node or null
        /// </summary>
        /// <returns>Parent's parent or null</returns>
        private RedBlackTreeNode<T> getGrandparent(){
            return Parent != null ? Parent.Parent : null;
        }

        /// <summary>
        /// Returns the parent's sibling node or null
        /// </summary>
        /// <returns>Parent's sibling or null</returns>
        private RedBlackTreeNode<T> getUncle(){
            if (GrandParent != null){
                return GrandParent.Left == Parent ? GrandParent.Right : GrandParent.Left;
            }
            return null;
        }

        /// <summary>
        /// Creates a new node wrapping the given element
        /// </summary>
        /// <param name="element">This node's value</param>
        public RedBlackTreeNode(T element){
            Value = element;
        }

        /// <summary>
        /// Rebalance the tree from this node
        /// </summary>
        /// <returns>The new root node of the tree</returns>
        public RedBlackTreeNode<T> Rebalance(){
            //Okay, so, I'm not going to *pretend* to understand /why/ a redblack tree works
            //but the rules for implementing one are fairly clear cut, so I can do that

            //This node is the root
            if (Parent == null){
                Colour = TreeColours.Black;
                return this;
            }
            //Parent is black, we can't go back
            if (Parent.Colour == TreeColours.Black){
                return Root;
            }

            //Okay, parent is red from here on in.

            //Me, my parent, and my uncle are all red. 
            if (Uncle != null && Uncle.Colour == TreeColours.Red){
                Uncle.Colour = TreeColours.Black;
                Parent.Colour = TreeColours.Black;
                GrandParent.Colour = TreeColours.Red;
                return GrandParent.Rebalance();
            }

            //I'm red, parent is red, but uncle isn't. Pivot.
            return Pivot();
        } 

        /// <summary>
        /// Do a rotate around some local node
        /// </summary>
        /// <returns>The new root node of the tree</returns>
        public RedBlackTreeNode<T> Pivot(){
            if (GrandParent.Left == Parent){
                if (Parent.Right == this){
                    return PivotLeft(false);
                }
                return Parent.PivotRight(true);
            }
            if (Parent.Left == this){
                return PivotRight(false);
            }
            return Parent.PivotLeft(true);
        }

        /// <summary>
        /// Pivot some local node rightwards
        /// </summary>
        /// <param name="recolour">Whether the node must also be recoloured</param>
        /// <returns>The new root node of the tree</returns>
        public RedBlackTreeNode<T> PivotRight(bool recolour){
            var right = Right;
            var parent = Parent;
            var grandparent = GrandParent;

            //move my right child to be the left child of my parent.
            parent.Left = right;
            if (right != null){
                right.Parent = parent;
            }

            //Shift up and make my parent my right child
            Parent = grandparent;
            if (grandparent != null){
                if (grandparent.Right == parent){
                    grandparent.Right = this;
                }
                else{
                    grandparent.Left = this;
                }
            }

            Right = parent;
            parent.Parent = this;

            if (recolour){
                parent.Colour = TreeColours.Red;
                Colour = TreeColours.Black;
                return Root;
            }
            return parent.Rebalance();
        }

        /// <summary>
        /// Pivot some local node leftwards
        /// </summary>
        /// <param name="recolour">Whether the node must also be recoloured</param>
        /// <returns>The new root node of the tree</returns>
        public RedBlackTreeNode<T> PivotLeft(bool recolour){
            var left = Left;
            var parent = Parent;
            var grandparent = GrandParent;

            parent.Right = left;
            if (left != null){
                left.Parent = parent;
            }

            Parent = grandparent;
            if (grandparent != null){
                if (grandparent.Right == parent){
                    grandparent.Right = this;
                }
                else{
                    grandparent.Left = this;
                }
            }
            Left = parent;
            parent.Parent = this;

            if (recolour){
                parent.Colour = TreeColours.Red;
                Colour = TreeColours.Black;
                return Root;
            }
            return parent.Rebalance();
        }

        /// <summary>
        /// Insert some element into the tree
        /// </summary>
        /// <param name="element">The element to insert</param>
        /// <returns>The new root node of the tree</returns>
        public RedBlackTreeNode<T> Insert(T element){
            if (Value.CompareTo(element) > 0){ // Me > element
                if (Left == null){
                    //Empty subtree! We can insert, then rebalance.
                    Left = new RedBlackTreeNode<T>(element){Parent = this};
                    return Left.Rebalance();
                }
                return Left.Insert(element);
            }
            if (Value.CompareTo(element) < 0){ // element > Me
                if (Right == null){ 
                    //Empty subtree! We can insert, then rebalance.
                    Right = new RedBlackTreeNode<T>(element){Parent = this};
                    return Right.Rebalance();
                }
                return Right.Insert(element);
            }

            // element == me
            if (Deleted){
                Deleted = false;
            }
            else{
                //I am not a RBTreeSet. Duplicates probably aren't intended.
                throw new ArgumentException("Item already exists in tree!");
            }
            return Root;
        }

        /// <summary>
        /// Return the node containing the given element
        /// </summary>
        /// <param name="element">The element to locate</param>
        /// <returns>A node object containing the element</returns>
        public RedBlackTreeNode<T> GetNodeAt(T element){
            if (Value.Equals(element)){
                return this;
            }
            if (Value.CompareTo(element) < 0 && Right != null){
                return Right.GetNodeAt(element);
            }
            if (Value.CompareTo(element) > 0 && Left != null){
                return Left.GetNodeAt(element);
            }
            return null;
        }

        /// <summary>
        /// Returns the node containing a value which satisfies some predicate
        /// </summary>
        /// <param name="pred">A function of signature (T) => int, where int is a CompareTo style comparison</param>
        /// <returns>The node containing the correct value or null</returns>
        public RedBlackTreeNode<T> Get(Func<T, int> pred){
            if (pred(Value) == 0 && !Deleted){
                return this;
            }
            if (pred(Value) < 0 && Right != null){
                return Right.Get(pred);
            }
            if (pred(Value) > 0 && Left != null){
                return Left.Get(pred);
            }
            return null;
        } 

        /// <summary>
        /// Returns whether the tree with this node as a root contains the specified element
        /// </summary>
        /// <param name="element">The element to find</param>
        /// <returns>Whether it exists within the tree</returns>
        public bool Contains(T element){
            var node = GetNodeAt(element);
            return node != null && !node.Deleted;
        }

        /// <summary>
        /// Removes the entry in this (sub)tree, if it exists
        /// </summary>
        /// <param name="element">The element</param>
        public void Remove(T element){
            var node = GetNodeAt(element);
            if (node == null){
                return;
            }

            node.Deleted = true;
        }

        /// <summary>
        /// Return the next possible right turn up a tree
        /// </summary>
        /// <returns>The next node to traverse in a depth-first traversal or null</returns>
        public RedBlackTreeNode<T> GetNextIterationPoint(){
            //Go up and find the next right turn
            if (Parent == null){
                return null;
            }

            if (Parent.Left == this && Parent.Right != null){
                return Parent.Right;
            }
            return Parent.GetNextIterationPoint();
        }
    }
}
