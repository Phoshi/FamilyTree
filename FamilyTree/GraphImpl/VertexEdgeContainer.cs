using System;
using System.Collections.Generic;
using System.Linq;
using FamilyTree.RBTreeImpl;

namespace FamilyTree.GraphImpl {
    /// <summary>
    /// Contains a combination of vertex type and associated edges
    /// </summary>
    /// <typeparam name="TVertex">Vertex type</typeparam>
    /// <typeparam name="TEdge">Type of edge to associate</typeparam>
    class VertexEdgeContainer<TVertex, TEdge> : IComparable<VertexEdgeContainer<TVertex, TEdge>> where TVertex : IComparable<TVertex> where TEdge : IComparable<TEdge>{

        /// <summary>
        /// The wrapped vertex
        /// </summary>
        public TVertex Vertex { get; internal set; }

        /// <summary>
        /// The internal list of edges which originate at the wrapped vertex
        /// </summary>
        private readonly ITree<TEdge> _edgesFrom = new RedBlackTree<TEdge>();
        
        /// <summary>
        /// The edges which originate at the wrapped vertex
        /// </summary>
        public IEnumerable<TEdge> EdgesFrom { get { return (from edge in _edgesFrom select edge); } }

        /// <summary>
        /// The internal list of edges which terminate at the wrapped vertex
        /// </summary>
        private readonly ITree<TEdge> _edgesTo = new RedBlackTree<TEdge>();

        /// <summary>
        /// The edges which terminate at the wrapped vertex
        /// </summary>
        public IEnumerable<TEdge> EdgesTo { get { return (from edge in _edgesTo select edge); } }

        /// <summary>
        /// Instantiate a new VertexEdgeWrapper wrapping the given vertex
        /// </summary>
        /// <param name="vertex">The vertex to wrap</param>
        public VertexEdgeContainer(TVertex vertex){
            Vertex = vertex;
        }

        /// <summary>
        /// Adds an associated edge type to the vertex
        /// </summary>
        /// <param name="edge">The edge to associate</param>
        /// <param name="toVertex">Whether this edge is originating or terminating</param>
        public void AddEdge(TEdge edge, bool toVertex = false){
            if (toVertex) {
                _edgesTo.Insert(edge);
            }
            else{
                _edgesFrom.Insert(edge);
            }
        }

        /// <summary>
        /// Removes an associated edge type from the vertex
        /// </summary>
        /// <param name="edge">The edge to disassociate</param>
        /// <param name="toVertex">Whether this edge is originating or terminating</param>
        public void RemoveEdge(TEdge edge, bool toVertex = false){
            if (toVertex) {
                _edgesTo.Remove(edge);
            }
            else {
                _edgesFrom.Remove(edge);
            }
        }

        /// <summary>
        /// Compares the wrapped verticies
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(VertexEdgeContainer<TVertex, TEdge> other){
            return Vertex.CompareTo(other.Vertex);
        }
    }
}
