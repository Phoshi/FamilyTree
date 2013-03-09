using System;
using System.Collections.Generic;
using System.Linq;

namespace FamilyTree.GraphImpl {
    /// <summary>
    /// Implements an edge between two verticies
    /// </summary>
    /// <typeparam name="TVertex">The type of the joined verticies</typeparam>
    /// <typeparam name="TRelationship">The type of the relationship</typeparam>
    public class Edge<TVertex, TRelationship> : IComparable<Edge<TVertex, TRelationship>> where TVertex : IComparable<TVertex>{
        /// <summary>
        /// The internal vertex list.
        /// </summary>
        private readonly TVertex[] _vertexes = new TVertex[2];

        /// <summary>
        /// The relationship joining the two verticies
        /// </summary>
        public TRelationship JoinType { get; internal set; }

        /// <summary>
        /// An enumerable of both linked verticies
        /// </summary>
        public IEnumerable<TVertex> Vertexes{
            get { return Array.AsReadOnly(_vertexes); }
        }

        /// <summary>
        /// The start vertex
        /// </summary>
        public TVertex One { get { return _vertexes.First(); } }

        /// <summary>
        /// The end vertex
        /// </summary>
        public TVertex Two { get { return _vertexes.Last(); } }

        /// <summary>
        /// Create a new edge linking two verticies with a relationship type
        /// </summary>
        /// <param name="type">The type of join</param>
        /// <param name="one">The originating vertex</param>
        /// <param name="two">The terminating vertex</param>
        public Edge(TRelationship type, TVertex one, TVertex two){
            JoinType = type;
            _vertexes[0] = one;
            _vertexes[1] = two;
        }

        /// <summary>
        /// Compares this edge to another edge of the same type
        /// </summary>
        /// <param name="other">The other edge</param>
        /// <returns>-1 for less than, 0 for equal, 1 for greater than</returns>
        public int CompareTo(Edge<TVertex, TRelationship> other){
            if (One.CompareTo(other.One) != 0){
                return One.CompareTo(other.One);
            }
            if (Two.CompareTo(other.Two) != 0){
                return Two.CompareTo(other.Two);
            }
            return String.Compare(JoinType.ToString(), other.JoinType.ToString(), StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns a textual representation of this edge
        /// </summary>
        /// <returns></returns>
        public override string ToString(){
            return string.Format("{0} --{1}--> {2}", One, JoinType, Two);
        }
    }
}
