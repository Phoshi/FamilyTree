﻿using System.Linq;
using FamilyTree.GraphImpl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphTestProject {
    [TestClass]
    public class GraphUnitTests {
        [TestMethod]
        public void TestInstantiation() {
            var graph = new DirectedGraph<string, string>();
            Assert.IsNotNull(graph);
        }

        [TestMethod]
        public void TestVertexes(){
            var graph = new DirectedGraph<string, string>();
            graph.AddVertex("One");
            graph.AddVertex("Two");
            //Assert.IsTrue(graph.Vertexes.Count() == 2);
        }

        [TestMethod]
        public void TestSpeed(){
            var graph = new DirectedGraph<string, string>();
            //insert
            for (int i = 0; i < 100000; i++){
                graph.AddVertex(i.ToString());
            }
            for (int i = 40000; i < 60000; i++){
                graph.AddEdge(i.ToString(), "PlusOne", (i+1).ToString());
            }
            for (int i = 50000; i < 55000; i++){
                Assert.IsTrue(graph.TestRelationship(i.ToString(), "PlusOne", (i + 1).ToString()));
            }
        }

        [TestMethod]
        public void TestEdges(){
            var graph = new DirectedGraph<string, string>();
            graph.AddVertex("One");
            graph.AddVertex("Two");
            graph.AddEdge("One", "AreOneOff", "Two");
            Assert.IsTrue(graph.Edges.Count() == 1);

            var fromOne = graph.GetEdgesFrom("One");
            Assert.IsTrue(fromOne.First().JoinType == "AreOneOff");
            var toOne = graph.GetEdgesTo("One");
            Assert.IsTrue(!toOne.Any());
        }

        [TestMethod]
        public void TestEdgeTraversal(){
            var graph = new DirectedGraph<string, string>();
            graph.AddVertex("One");
            graph.AddVertex("Two");
            graph.AddEdge("One", "PlusOne", "Two");

            var two = graph.GetOtherEdgeInRelationship("One", "PlusOne");

            Assert.IsTrue(two == "Two");
            Assert.IsTrue(graph.TestRelationship("One", "PlusOne", "Two"));
            Assert.IsFalse(graph.TestRelationship("One", "PlusOne", "Three"));

            Assert.IsTrue(graph.GetEdgesTo("Two").Any(edge => edge.JoinType.Equals("PlusOne") && edge.One.Equals("One")));
        }

        [TestMethod]
        public void TestVertexRetrieve(){
            var graph = new DirectedGraph<string, string>();
            graph.AddVertex("One");
            graph.AddVertex("Two");
            Assert.AreEqual("One", graph.Get("One"));
        }

        [TestMethod]
        public void TestUndirectedEdges(){
            var graph = new DirectedGraph<string, string>();
            graph.AddVertex("One");
            graph.AddVertex("Two");
            graph.AddVertex("Three");

            graph.AddUndirectedEdge("One", "AreOneOff", "Two");
            Assert.IsTrue(graph.TestRelationship("One", "AreOneOff", "Two"));

            graph.RemoveUndirectedEdge("One", "AreOneOff", "Two");
            Assert.IsFalse(graph.TestRelationship("One", "AreOneOff", "Two"));
        }

        [TestMethod]
        public void TestVertexIteration(){
            var graph = new DirectedGraph<string, string>();
            for (int i = 0; i < 100; i++){
                graph.AddVertex(i.ToString());
            }
            Assert.IsTrue(graph.Vertexes.Count() == 100);
        }

        [TestMethod]
        public void TestEdgeRemove(){
            var graph = new DirectedGraph<string, string>();
            graph.AddVertex("One");
            graph.AddVertex("Two");

            graph.AddEdge("One", "PlusOneIs", "Two");
            graph.AddEdge("One", "PlusTwoIs", "Two");
            graph.RemoveEdge("One", "PlusTwoIs", "Two");

            Assert.IsFalse(graph.TestRelationship("One", "PlusTwoIs", "Two"));
        }
    }
}
