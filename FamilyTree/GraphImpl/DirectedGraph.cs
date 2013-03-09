using System;
using System.Collections.Generic;
using System.Linq;
using FamilyTree.RBTreeImpl;

namespace FamilyTree.GraphImpl {
    /// <summary>
    /// Implements a directed graph
    /// </summary>
    /// <typeparam name="TVertex">The vertex (node) type</typeparam>
    /// <typeparam name="TEdge">The edge (relationship / link) type</typeparam>
    public class DirectedGraph<TVertex, TEdge> where TVertex : class, IComparable<TVertex>{
        /// <summary>
        /// The internal vertex list
        /// </summary>
        private ITree<Vertex<TVertex>> _vertexes = new RedBlackTree<Vertex<TVertex>>();

        /// <summary>
        /// The internal edge list
        /// </summary>
        private ITree<Edge<Vertex<TVertex>, TEdge>> _edges = new RedBlackTree<Edge<Vertex<TVertex>, TEdge>>();

        /// <summary>
        /// Returns an enumerable of all verticies in an arbitrary order (I.E. Not input order or sorted)
        /// </summary>
        public IEnumerable<TVertex> Vertexes{get { return (from vertex in _vertexes select vertex.Value); }}

        /// <summary>
        /// Return an enumerable of edges in an arbitrary order (I.E. Not input order or sorted)
        /// </summary>
        public IEnumerable<Edge<TVertex, TEdge>> Edges {get{
            return
                (from edge in _edges
                 select new Edge<TVertex, TEdge>(edge.JoinType, edge.Vertexes.First().Value, edge.Vertexes.Last().Value));
        }}

        /// <summary>
        /// Adds a new vertex to the graph. Must be unique.
        /// </summary>
        /// <param name="element"></param>
        public void AddVertex(TVertex element){
            var newVertex = new Vertex<TVertex>(element);
            _vertexes.Insert(newVertex);
        }

        /// <summary>
        /// Creates an edge between two verticies
        /// </summary>
        /// <param name="one">The start vertex</param>
        /// <param name="type">The relationship type</param>
        /// <param name="two">The end vertex</param>
        public void AddEdge(TVertex one, TEdge type, TVertex two){
            var vertexOne = getVertex(one);
            var vertexTwo = getVertex(two);

            var edge = new Edge<Vertex<TVertex>, TEdge>(type, vertexOne, vertexTwo);
            _edges.Insert(edge);
        }

        /// <summary>
        /// Creates an undirected edge between two verticies
        /// </summary>
        /// <param name="one">One vertex</param>
        /// <param name="type">The relationship type</param>
        /// <param name="two">Another vertex</param>
        public void AddUndirectedEdge(TVertex one, TEdge type, TVertex two){
            AddEdge(one, type, two);
            AddEdge(two, type, one);
        }

        /// <summary>
        /// Removes an edge between two verticies
        /// </summary>
        /// <param name="one">The start vertex</param>
        /// <param name="type">The relationship type</param>
        /// <param name="two">The end vertex</param>
        public void RemoveEdge(TVertex one, TEdge type, TVertex two){
            var edges = GetEdgesFrom(one);
            var edgeToRemove = (from edge in edges where edge.JoinType.Equals(type) select edge).FirstOrDefault();

            if (edgeToRemove != null){
                var literalEdge = (from edge in _edges
                                  where
                                      edge.One.Value.Equals(edgeToRemove.One) && 
                                      edge.Two.Value.Equals(edgeToRemove.Two) &&
                                      edge.JoinType.Equals(edgeToRemove.JoinType)
                                  select edge).First();
                
                _edges.Remove(literalEdge);
            }

        }

        /// <summary>
        /// Removes an undirected edge between two verticies
        /// </summary>
        /// <param name="one">One vertex</param>
        /// <param name="edge">The relationship type</param>
        /// <param name="two">Another vertex</param>
        public void RemoveUndirectedEdge(TVertex one, TEdge edge, TVertex two){
            RemoveEdge(one, edge, two);
            RemoveEdge(two, edge, two);
        }

        /// <summary>
        /// Returns the vertex container containing a given vertex
        /// </summary>
        /// <param name="element"></param>
        /// <returns>The vertex container or null</returns>
        private Vertex<TVertex> getVertex(TVertex element){
            return _vertexes.Get(item => item.Value.CompareTo(element));
        }

        /// <summary>
        /// Returns a list of edges originating from the given node
        /// </summary>
        /// <param name="element">The node</param>
        /// <returns>An enumerable of edges</returns>
        public IEnumerable<Edge<TVertex, TEdge>> GetEdgesFrom(TVertex element){
            return (from edge in Edges where edge.One.Equals(element) select edge);
        }

        /// <summary>
        /// Returns a list of edges terminating at the given node
        /// </summary>
        /// <param name="element">The node</param>
        /// <returns>An enumerable of edges</returns>
        public IEnumerable<Edge<TVertex, TEdge>> GetEdgesTo(TVertex element){
            return (from edge in Edges where edge.Two.Equals(element) select edge);
        }

        /// <summary>
        /// Returns all edges linked to a vertex by a relationship
        /// </summary>
        /// <param name="element">The start vertex</param>
        /// <param name="relationship">The relationship to follow</param>
        /// <returns>An enumerable of related verticies</returns>
        public IEnumerable<TVertex> GetOtherEdgesInRelationship(TVertex element, TEdge relationship){
            var allEdges = GetEdgesFrom(element);
            var correctEdges = allEdges.Where(edge => edge.JoinType.Equals(relationship));
            return from edge in correctEdges select edge.Two;
        }

        /// <summary>
        /// Tests whether a given relationship exists
        /// </summary>
        /// <param name="one">The originating vertex</param>
        /// <param name="relationship">The relationship type</param>
        /// <param name="two">The terminating vertex</param>
        /// <returns></returns>
        public bool TestRelationship(TVertex one, TEdge relationship, TVertex two){
            return GetEdgesFrom(one).Where(edge => edge.JoinType.Equals(relationship)).Any(edge => edge.Two.Equals(two));
        }

        /// <summary>
        /// Returns the edge linked to a vertex by a relationship.
        /// If there are multiple possible candidates, then the same edge being returned after a graph update cannot be guaranteed.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="relationship"></param>
        /// <returns></returns>
        public TVertex GetOtherEdgeInRelationship(TVertex element, TEdge relationship){
            var edges = GetOtherEdgesInRelationship(element, relationship);
            if (edges.Any()){
                return edges.First();
            }
            return null;
        }
    }
}
