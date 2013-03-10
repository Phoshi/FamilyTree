Family Tree
===========

This is a simple application that models a family tree style grouping of relationships between people.

Under the GraphImpl directory you will find a generic Directed Graph which depends on the RedBlack tree found under RBTreeImpl 
(Note that anything which implements the supplied ITree interface would be a drop-in replacement, but none are provided)

This was written as my second second year Data Structures and Algorithms assignment.

Justification
-------------

I decided to model the relationships as a directed graph, as this would give me the level of flexibility I needed to represent every relationship. I did not enforce tree properties on this graph as the input data should be validated by a more targeted relationship validator rather than on the data structure level.

I implemented this d-graph by storing a tree of node/relationship collections. I did this to enable very fast lookups of individual people, as a sufficiently large family tree could get expansive. In order to ensure timely lookups, I implemented a Red-Black tree--a form of self-balancing Binary Search Tree--which ensures lookups take place in O(log n) in both the worst and average case, and insertions take place in an "average" of O(log n).

The advantages of this are quite clear, as individuals can be located very quickly. Unfortunately I could not do the same to searching through an individual's relationships. While locating the person takes place in O(log n) time, the relationship search afterwards takes place in O(n) time. This is unlikely to be significant, however, as few families have enough children to make n of considerable size. As the tree structure implements the IEnumerable interface, anything which operates on an enumerable type, including LINQ, will interface cleanly. This does fall back to a naive depth-first traversal, however, so the speed benefits of a BST are largely lost.

I implemented the targeted relationship logic using .NET extension methods. The advantage of this is that the Person class can remain with a single responsibility as an immutable data class, but the relationship logic can still operate on individual people. As extension methods offer significant flexibility, I was also able to implement the same relational operators on Person collections, enabling very readable relational selectors. For example

	person.Parents(graph).Siblings(graph).Children(graph);

This construct returns the children of the brothers and sisters of a person's parents--their cousins--taking into account deduplication and correctly handling half-siblings (I.E. ignoring them). The disadvantage of using this method is likely also clear: as I did not want to introduce unrelated data into the Person class, relational operators require you to explicitly specify the relation graph they are operating on. While this does add support for one person existing in multiple relationship graphs, it also increases visual clutter. I considered making the graph a singleton class, however I consider some visual clutter to be a lesser evil than unneccesary global state.

As people do not directly store their own relationships this method incurs a performance penalty of one extra graph lookup per relationship traversal. As these lookups are fast I consider it an acceptable tradeoff for not introducing additional responsibilities into the Person class.