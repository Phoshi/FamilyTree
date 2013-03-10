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
        /// The internal vertex container list. Contains a number of entries which link verticies to their edges.
        /// </summary>
        private readonly ITree<VertexEdgeContainer<Vertex<TVertex>, Edge<Vertex<TVertex>, TEdge>>> _vertexContainerList =
            new RedBlackTree<VertexEdgeContainer<Vertex<TVertex>, Edge<Vertex<TVertex>, TEdge>>>();

        /// <summary>
        /// Returns an enumerable of all verticies in an arbitrary order (I.E. Not input order or sorted)
        /// </summary>
        public IEnumerable<TVertex> Vertexes{
            get { return (from vertex in _vertexContainerList select vertex.Vertex.Value); }
        }

        /// <summary>
        /// Return an enumerable of edges in an arbitrary order (I.E. Not input order or sorted)
        /// </summary>
        public IEnumerable<Edge<TVertex, TEdge>> Edges{
            get{
                var allEdges = new List<Edge<TVertex, TEdge>>();
                foreach (var container in Vertexes.Select(getVertexContainer)){
                    //We can't simply return the associated edges, as those link directly to the Vertex wrapper, which would expose the graph to outsiders
                    //Thus we iterate over them and create new, safe edges
                    allEdges.AddRange(from edge in container.EdgesFrom select sanitise(edge));
                }

                return allEdges;
            }
        }

        /// <summary>
        /// Returning the internal edges would expose the vertex wrappers to the outside world
        /// So we "sanitise" them to act as a sane interface
        /// </summary>
        /// <param name="unsafeEdge">The internal edge</param>
        /// <returns>A safe edge</returns>
        private Edge<TVertex, TEdge> sanitise(Edge<Vertex<TVertex>, TEdge> unsafeEdge){
            return new Edge<TVertex, TEdge>(unsafeEdge.JoinType, unsafeEdge.One.Value, unsafeEdge.Two.Value);
        } 

        /// <summary>
        /// Adds a new vertex to the graph. Must be unique.
        /// </summary>
        /// <param name="element"></param>
        public void AddVertex(TVertex element){
            var newVertex = new Vertex<TVertex>(element);
            var vertexContainer = new VertexEdgeContainer<Vertex<TVertex>, Edge<Vertex<TVertex>, TEdge>>(newVertex);
            _vertexContainerList.Insert(vertexContainer);
        }

        /// <summary>
        /// Creates an edge between two verticies
        /// </summary>
        /// <param name="one">The start vertex</param>
        /// <param name="type">The relationship type</param>
        /// <param name="two">The end vertex</param>
        public void AddEdge(TVertex one, TEdge type, TVertex two){
            //In order to efficiently allow the "GetEdgesTo" operation, all edges must be added to both verticies' container.
            var vertexOneContainer = getVertexContainer(one);
            var vertexTwoContainer = getVertexContainer(two);

            var edge = new Edge<Vertex<TVertex>, TEdge>(type, vertexOneContainer.Vertex, vertexTwoContainer.Vertex);

            vertexOneContainer.AddEdge(edge);
            vertexTwoContainer.AddEdge(edge, toVertex: true);
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
            var vertexOneContainer = getVertexContainer(one);
            var vertexTwoContainer = getVertexContainer(two);

            var vertexToRemove =
                vertexOneContainer.EdgesFrom.First(item => item.JoinType.Equals(type) && item.Two.Value.Equals(two));

            vertexOneContainer.RemoveEdge(vertexToRemove);
            vertexTwoContainer.RemoveEdge(vertexToRemove, toVertex: true);
        }

        /// <summary>
        /// Removes an undirected edge between two verticies
        /// </summary>
        /// <param name="one">One vertex</param>
        /// <param name="edge">The relationship type</param>
        /// <param name="two">Another vertex</param>
        public void RemoveUndirectedEdge(TVertex one, TEdge edge, TVertex two){
            RemoveEdge(one, edge, two);
            RemoveEdge(two, edge, one);
        }

        /// <summary>
        /// Returns the vertex container containing a given vertex
        /// </summary>
        /// <param name="element"></param>
        /// <returns>The vertex container or null</returns>
        private VertexEdgeContainer<Vertex<TVertex>, Edge<Vertex<TVertex>, TEdge>> getVertexContainer(TVertex element){
            return _vertexContainerList.Get(item => item.Vertex.Value.CompareTo(element));
        }

        /// <summary>
        /// Returns a list of edges originating from the given node
        /// </summary>
        /// <param name="element">The node</param>
        /// <returns>An enumerable of edges</returns>
        public IEnumerable<Edge<TVertex, TEdge>> GetEdgesFrom(TVertex element){
            return from edge in getVertexContainer(element).EdgesFrom select sanitise(edge);
        }

        /// <summary>
        /// Returns a list of edges terminating at the given node
        /// </summary>
        /// <param name="element">The node</param>
        /// <returns>An enumerable of edges</returns>
        public IEnumerable<Edge<TVertex, TEdge>> GetEdgesTo(TVertex element){
            return from edge in getVertexContainer(element).EdgesTo select sanitise(edge);
        }

        /// <summary>
        /// Returns all edges linked to a vertex by a relationship
        /// </summary>
        /// <param name="element">The start vertex</param>
        /// <param name="relationship">The relationship to follow</param>
        /// <returns>An enumerable of related verticies</returns>
        public IEnumerable<TVertex> GetOtherEdgesInRelationship(TVertex element, TEdge relationship){
            return GetEdgesFrom(element).Where(edge => edge.JoinType.Equals(relationship)).Select(edge => edge.Two);
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
            return GetOtherEdgesInRelationship(element, relationship).FirstOrDefault();
        }

        /// <summary>
        /// Returns the graph's canonical copy of the matched element.
        /// Element must obviously implement a CompareTo operator which 
        /// does not require byte-for-byte equality to be considered equal
        /// </summary>
        /// <param name="element">The element to match on</param>
        /// <returns>The canonical copy</returns>
        public TVertex Get(TVertex element){
            return _vertexContainerList.Get(item => item.Vertex.Value.CompareTo(element)).Vertex.Value;
        }
    }
}
