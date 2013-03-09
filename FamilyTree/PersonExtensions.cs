using System.Collections.Generic;
using System.Linq;
using FamilyTree.GraphImpl;

namespace FamilyTree {
    /// <summary>
    /// Implements relational operators on people linked by a relationship graph.
    /// </summary>
    public static class PersonExtensions {
        /// <summary>
        /// Returns an enumerable type containing the person's parents.
        /// Cannot be assumed to return a (Mother, Father) pair due to issues with adoption and imperfect record keeping
        /// </summary>
        /// <param name="self">The person</param>
        /// <param name="relationshipGraph">The relationship graph</param>
        /// <returns>An enumerable list of parents of cardinality zero or greater.</returns>
        public static IEnumerable<Person> Parents(this Person self, DirectedGraph<Person, Relationship> relationshipGraph){
            return relationshipGraph.GetOtherEdgesInRelationship(self, Relationship.IsAChildOf);
        }

        /// <summary>
        /// Returns the person's mother.
        /// In cases of multiple maternity returns a deterministic but unpredictable mother.
        /// </summary>
        /// <param name="self">The person</param>
        /// <param name="relationshipGraph">The relationship graph</param>
        /// <returns>A mother or null</returns>
        public static Person Mother(this Person self, DirectedGraph<Person, Relationship> relationshipGraph){
            return relationshipGraph.GetOtherEdgeInRelationship(self, Relationship.HasTheMotherOf);
        }

        /// <summary>
        /// Returns the person's father.
        /// In cases of multiple paternity returns a deterministic but unpredictable father.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="relationshipGraph"></param>
        /// <returns>A father or null</returns>
        public static Person Father(this Person self, DirectedGraph<Person, Relationship> relationshipGraph) {
            return relationshipGraph.GetOtherEdgeInRelationship(self, Relationship.HasTheFatherOf);
        }

        /// <summary>
        /// Returns the person's children, biological or adopted.
        /// </summary>
        /// <param name="self">The person</param>
        /// <param name="relationshipGraph">The relationship graph</param>
        /// <returns>An enumberable of children of cardinality zero or greater</returns>
        public static IEnumerable<Person> Children(this Person self, DirectedGraph<Person, Relationship> relationshipGraph){
            return relationshipGraph.GetOtherEdgesInRelationship(self, Relationship.IsAParentOf);
        }

        /// <summary>
        /// Returns the person's current partner or null.
        /// </summary>
        /// <param name="self">The person</param>
        /// <param name="relationshipGraph">The relationship graph</param>
        /// <returns>A partner or null</returns>
        public static Person Partner(this Person self, DirectedGraph<Person, Relationship> relationshipGraph){
            return relationshipGraph.GetOtherEdgeInRelationship(self, Relationship.IsMarriedTo);
        }

        /// <summary>
        /// Returns the person's ex-partners.
        /// </summary>
        /// <param name="self">The person</param>
        /// <param name="relationshipGraph">The relationship graph</param>
        /// <returns>An enumerable of ex-partners of size zero or greater</returns>
        public static IEnumerable<Person> FormerPartners(this Person self, DirectedGraph<Person, Relationship> relationshipGraph) {
            return relationshipGraph.GetOtherEdgesInRelationship(self, Relationship.IsDivorcedFrom);
        }

        /// <summary>
        /// Returns the person's siblings
        /// </summary>
        /// <param name="self">The person</param>
        /// <param name="relationshipGraph">The relationship graph</param>
        /// <returns>An enumerable of siblings of length zero or more</returns>
        public static IEnumerable<Person> Siblings(this Person self, DirectedGraph<Person, Relationship> relationshipGraph){
            return self.Parents(relationshipGraph).Children(relationshipGraph).Where(child => !child.Equals(self)).Distinct();
        }

        /// <summary>
        /// Returns a person's adoptive parents
        /// </summary>
        /// <param name="self">The person</param>
        /// <param name="relationshipGraph">The relationship graph</param>
        /// <returns>An enumerable of adoptive parents</returns>
        public static IEnumerable<Person> AdoptiveParents(this Person self, DirectedGraph<Person, Relationship> relationshipGraph){
            return relationshipGraph.GetOtherEdgesInRelationship(self, Relationship.WasAdoptedBy);
        }

        /// <summary>
        /// Returns whether the person was adopted by the given parent
        /// </summary>
        /// <param name="self">The person</param>
        /// <param name="other">The parent</param>
        /// <param name="relationshipGraph">The relationship graph</param>
        /// <returns>Whether the parent is adoptive</returns>
        public static bool WasAdoptedBy(this Person self, Person other, DirectedGraph<Person, Relationship> relationshipGraph){
            return relationshipGraph.TestRelationship(self, Relationship.WasAdoptedBy, other);
        }
    }

    /// <summary>
    /// Provides a set of LINQ-style functions for generating relationship links on sets of people.
    /// </summary>
    public static class PeopleExtensions{
        /// <summary>
        /// Returns every person related to the input set by the input relationship in the input graph.
        /// </summary>
        /// <param name="roots">The set of input people</param>
        /// <param name="relationship">The relationship to follow</param>
        /// <param name="graph">The relationship graph</param>
        /// <returns>An enumerable of every person related by the input rules</returns>
        private static IEnumerable<Person> getPeopleFromRelationship(IEnumerable<Person> roots, Relationship relationship, DirectedGraph<Person, Relationship> graph){
            var allReturns = new List<Person>();
            foreach (var person in roots) {
                allReturns.AddRange(graph.GetOtherEdgesInRelationship(person, relationship));
            }
            return allReturns;
        }

        /// <summary>
        /// Returns the parents of every member of the input
        /// </summary>
        /// <param name="self">The input people</param>
        /// <param name="relationshipGraph">The relationship graph</param>
        /// <returns>An enumerable of every recorded parent</returns>
        public static IEnumerable<Person> Parents(this IEnumerable<Person> self, DirectedGraph<Person, Relationship> relationshipGraph){
            return getPeopleFromRelationship(self, Relationship.IsAChildOf, relationshipGraph);
        }

        /// <summary>
        /// Returns the children of every member of the input
        /// </summary>
        /// <param name="self">The input people</param>
        /// <param name="relationshipGraph">The relationship graph</param>
        /// <returns>An enumerable of every recorded child</returns>
        public static IEnumerable<Person> Children(this IEnumerable<Person> self, DirectedGraph<Person, Relationship> relationshipGraph){
            return getPeopleFromRelationship(self, Relationship.IsAParentOf, relationshipGraph);
        }

        /// <summary>
        /// Returns the siblings of every member of the input
        /// </summary>
        /// <param name="self">The input people</param>
        /// <param name="relationshipGraph">The relationship graph</param>
        /// <returns>An enumerable of every recorded sibling</returns>
        public static IEnumerable<Person> Siblings(this IEnumerable<Person> self, DirectedGraph<Person, Relationship> relationshipGraph){
            var allReturns = new List<Person>();
            foreach(var person in self){
                allReturns.AddRange(person.Siblings(relationshipGraph));
            }
            return allReturns;
        } 
    }
}
