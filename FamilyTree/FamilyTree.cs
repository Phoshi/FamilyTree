using System;
using System.Collections.Generic;
using System.Linq;
using FamilyTree.GraphImpl;

namespace FamilyTree
{
    /// <summary>
    /// This class models a family tree.
    /// </summary>
    public class FamilyTree
    {
        private readonly DirectedGraph<Person, Relationship> _graph = new DirectedGraph<Person, Relationship>();

        /**
         * Adds a person to the family tree
         * @param aPerson Person to be added
         */
        public void AddPerson(Person aPerson)
        {
            _graph.AddVertex(aPerson);
        }

        /// <summary>
        /// Returns the person matching the given details
        /// </summary>
        /// <param name="name">The person's name</param>
        /// <param name="dateOfBirth">The person's birth year</param>
        /// <returns>A person object representing the person</returns>
        private Person getPerson(string name, string dateOfBirth){
            //In order to perform logic on the person, we need to get our stored copy out.
            //We could have just kept one around in some regular mapping like cavemen, but
            //we have a copy in our graph and happen to know that somebody can be uniquely
            //identified based only on name and date of birth. 
            //Yes, it's a bit of a hack, but this way we get O(lg n) lookup times on partial 
            //matches without needing to double the storage to maintain a hashmap or something
            var mockup = new Person(name, dateOfBirth, "{{MOCKUP}}");
            return _graph.Get(mockup);
        }

        /// <summary>
        /// Returns a list of possible matches on the given name
        /// </summary>
        /// <param name="name">The person's name</param>
        /// <returns>A person object representing the person</returns>
        private IEnumerable<Person> getMatchingPeople(string name){
            //dgraph.Vertexes is an enumerable type that does a naive depth first
            //travesal of the entire graph. Which is nice, because we need an 
            //exhaustive traversal to return every single person with a 
            //matching name.
            return _graph.Vertexes.Where(person => person.Name.Equals(name));
        }

        /**
         * Links an individual to their mother. Both the individual and the
         * mother need already to appear as a Person in the family tree.
         * @param aPerson String holding individual's name
         * @param aDOB String holding individual's date of birth
         * @param mName String holding mother's name
         * @param mDOB String holding mother's date of birth
         */
        public void MakeLinkToMother(String aPerson, String aDOB,
                String mName, String mDOB){
            var child = getPerson(aPerson, aDOB);
            var mother = getPerson(mName, mDOB);

            _graph.AddEdge(child, Relationship.HasTheMotherOf, mother);
            _graph.AddEdge(mother, Relationship.IsAParentOf, child);
            _graph.AddEdge(child, Relationship.IsAChildOf, mother);
        }
        /**
         * Links an individual to their father. Both the individual and the
         * mother need already to appear as a Person in the family tree.
         * @param aPerson String holding individual's name
         * @param aDOB String holding individual's date of birth
         * @param fName String holding father's name
         * @param fDOB String holding father's date of birth
         */
        public void MakeLinkToFather(String aPerson, String aDOB,
                String fName, String fDOB)
        {
            var child = getPerson(aPerson, aDOB);
            var father = getPerson(fName, fDOB);

            _graph.AddEdge(child, Relationship.HasTheFatherOf, father);
            _graph.AddEdge(father, Relationship.IsAParentOf, child);
            _graph.AddEdge(child, Relationship.IsAChildOf, father);
        }

        /**
         * Links a newly married couple. Each member of the couple
         * needs already to appear as a Person in the family tree.
         * @param partner1Name String holding bride's name
         * @param aDOB1 String holding bride's date of birth
         * @param partner2Name String holding groom's name
         * @param aDOB2 String holding groom's date of birth
         */
        public void RecordWedding(String partner1Name, String aDOB1,
                String partner2Name, String aDOB2){
            var one = getPerson(partner1Name, aDOB1);
            var two = getPerson(partner2Name, aDOB2);

            _graph.AddUndirectedEdge(one, Relationship.IsMarriedTo, two);
        }

        /**
         * Records a divorce. Each member of the couple
         * needs already to appear as a Person in the family tree.
         * @param partner1Name String holding wife's name
         * @param aDOB1 String holding wife's date of birth
         * @param partner2Name String holding husband's name
         * @param aDOB2 String holding husband's date of birth
         */
        public void RecordDivorce(String partner1Name, String aDOB1,
                String partner2Name, String aDOB2)
        {
            var one = getPerson(partner1Name, aDOB1);
            var two = getPerson(partner2Name, aDOB2);

            //Presumably once you've divorced somebody you are no longer married to them,
            //which is only really useful for in-memory tree updates.
            _graph.RemoveUndirectedEdge(one, Relationship.IsMarriedTo, two);
            _graph.AddUndirectedEdge(one, Relationship.IsDivorcedFrom, two);
        }

        /**
         * List the details of the person whose name is given. Note you need to do
         * something about the possibility of duplicate names appearing.
         * @param personName
         */
        public void ListPersonDetails(String personName){
            var people = filterPeopleByUserInput(getMatchingPeople(personName));

            foreach (var person in people){
                //Dump as much information as we can without it being overloading.
                Console.WriteLine("Personal Details: {0}", person);
                Console.WriteLine("=================");

                //An uninitialised instance of person will convert to a string showing no records,
                //meaning we can use the null coalescing operator to quickly define an escape case.
                Console.WriteLine("\tMother: {0}", person.Mother(_graph) ?? new Person());
                Console.WriteLine("\tFather: {0}", person.Father(_graph) ?? new Person());

                //Not splitting this into adoptive mother and father because I'm not quite certain
                //that you can't be adopted by more than two people. Certainly don't want to be
                //discriminatory, two mums or dads is fine.
                if (person.AdoptiveParents(_graph).Any()){
                    Console.WriteLine("\tWas adopted by {0}", string.Join(" and ", person.AdoptiveParents(_graph)));
                }
                if (person.Siblings(_graph).Any()){
                    Console.WriteLine("\tSiblings: {0}", string.Join(", ", person.Siblings(_graph)));
                }
                if (person.Children(_graph).Any()){
                    Console.WriteLine("\tChildren: {0}", string.Join(", ", person.Children(_graph)));
                }
                if (person.Partner(_graph) != null){
                    Console.WriteLine("\tPartner: {0}", person.Partner(_graph));
                }
                if (person.FormerPartners(_graph).Any()){
                    Console.WriteLine("\tEx-Partner{1}: {0}", string.Join(", ", person.FormerPartners(_graph)),
                                      person.FormerPartners(_graph).Count() == 1 ? "" : "s");
                }
                Console.WriteLine();
            }
        }
        /**
         * List the details of the parent of the person whose name is given. Note
         * you need to do something about the possibility of duplicate names
         * appearing.
         * @param personName
         */
        public void ListParentDetails(String personName){
            var people = filterPeopleByUserInput(getMatchingPeople(personName));

            foreach (var child in people){
                var mother = child.Mother(_graph);
                var father = child.Father(_graph);

                Console.WriteLine("Parental Details: {0}", child);
                Console.WriteLine("=================");

                Console.WriteLine("\tMother: {0}", mother ?? new Person());
                Console.WriteLine("\tFather: {0}", father ?? new Person());
                if (child.AdoptiveParents(_graph).Any()) {
                    Console.WriteLine("\tWas adopted by {0}", string.Join(" and ", child.AdoptiveParents(_graph)));
                }
                Console.WriteLine();
            }

        }

        /**
         * List the details of the children of the person whose name is given. Note
         * you need to do something about the possibility of duplicate names
         * appearing.
         * @param personName
         */
        public void ListChildren(String personName){
            var parents = filterPeopleByUserInput(getMatchingPeople(personName));

            foreach (var parent in parents){
                var kids = parent.Children(_graph);

                if (kids.Any()){
                    Console.WriteLine("Children of {0}:", parent);
                    Console.WriteLine("=================");

                    foreach (var kid in kids){
                        Console.WriteLine("\t{0}", kid);
                    }
                }
                else{
                    Console.WriteLine("No children on record for {0}", parent);
                }

                Console.WriteLine();
            }
        }
        /**
         * List the details of the siblings of the person whose name is given. Note
         * you need to do something about the possibility of duplicate names
         * appearing.
         * @param personName
         */
        public void ListSiblings(String personName){
            var possibleMatches = filterPeopleByUserInput(getMatchingPeople(personName));
            foreach (var child in possibleMatches){
                var kids = child.Siblings(_graph);

                if (kids.Any()){
                    Console.WriteLine("Siblings of {0}:", child);
                    Console.WriteLine("=================");

                    foreach (var kid in kids){
                        Console.WriteLine("\t{0} {1}", kid,
                                          //If the intersection of the two childrens' parents is not equal
                                          //to the same set of parents, then they are not full siblings 
                                          //Conservative. Will only list (half) in cases where it can be confirmed.
                                          (kid.Parents(_graph).Intersect(child.Parents(_graph)).Count() ==
                                           child.Parents(_graph).Count())
                                              ? ""
                                              : "(Half)"
                            );
                    }
                }
                else{
                    Console.WriteLine("No siblings on record for {0}.", child);
                }
                Console.WriteLine();
            }
        }
        /**
         * List the details of the ancestors along the paternal line  of the person
         * whose name is given. Note you need to do something about the possibility
         * of duplicate names appearing.
         * @param personName
         */
        public void ListPaternalLineage(String personName){
            var possibleMatches = filterPeopleByUserInput(getMatchingPeople(personName));

            foreach (var child in possibleMatches){
                var father = child.Father(_graph);

                Console.WriteLine("Paternal Lineage for {0}:", child);
                Console.WriteLine("=================");

                int indent = 1;
                while (father != null){
                    Console.Write(string.Concat(Enumerable.Repeat("\t", indent++)));
                    Console.WriteLine(father);
                    father = father.Father(_graph);
                }
                Console.WriteLine();
            }
        }

        /**
         * List the details of the ancestors along the maternal line  of the person
         * whose name is given. Note you need to do something about the possibility
         * of duplicate names appearing.
         * @param personName
         */
        public void ListMaternalLineage(String personName){
            var possibleMatches = filterPeopleByUserInput(getMatchingPeople(personName));

            foreach (var child in possibleMatches){
                var mother = child.Mother(_graph);

                Console.WriteLine("Maternal Lineage for {0}:", child);
                Console.WriteLine("=================");

                int indent = 1;
                while (mother != null){
                    Console.Write(string.Concat(Enumerable.Repeat("\t", indent++)));
                    Console.WriteLine(mother);
                    mother = mother.Mother(_graph);
                }
                Console.WriteLine();
            }
        }

        /**
         * List the details of the grandparents of the person
         * whose name is given. Note you need to do something about the possibility
         * of duplicate names appearing.
         * @param personName
         */
        public void ListGrandParents(String personName){
            var possibleMatches = filterPeopleByUserInput(getMatchingPeople(personName));

            foreach (var child in possibleMatches){
                var grandparents = child.Parents(_graph).Parents(_graph);

                if (grandparents.Any()){
                    Console.WriteLine("Grandparents for {0}:", child);
                    Console.WriteLine("=================");

                    foreach (var grandparent in grandparents){
                        Console.WriteLine("\t{0}", grandparent);
                    }
                }
                else{
                    Console.WriteLine("No grandparents on record for {0}.", child);
                }
                Console.WriteLine();
            }
        }
        /**
         * List the details of the grandchildren of the person whose name is given.
         * Note you need to do something about the possibility of duplicate names
         * appearing.
         * @param personName
         */
        public void ListGrandChildren(String personName){
            var possibleMatches = filterPeopleByUserInput(getMatchingPeople(personName));

            foreach (var grandparent in possibleMatches){
                var kids = grandparent.Children(_graph).Children(_graph);

                if (kids.Any()){
                    Console.WriteLine("Grandchildren for {0}:", grandparent);
                    Console.WriteLine("=================");

                    foreach (var kid in kids){
                        Console.WriteLine("\t{0}", kid);
                    }
                }
                else{
                    Console.WriteLine("No grandchildren on record for {0}.", grandparent);
                }
                Console.WriteLine();
            }
        }

        /**
         * List the details of the cousins of the person whose name is given.
         * Note you need to do something about the possibility of duplicate names
         * appearing.
         * @param personName
         */
        public void ListCousins(String personName){
            var possibleMatches = filterPeopleByUserInput(getMatchingPeople(personName));

            foreach (var child in possibleMatches){
                var cousins = child.Parents(_graph).Siblings(_graph).Children(_graph);

                if (cousins.Any()){
                    Console.WriteLine("Cousins for {0}:", child);
                    Console.WriteLine("=================");

                    foreach (var cousin in cousins){
                        Console.WriteLine("\t{0}", cousin);
                    }
                }
                else{
                    Console.WriteLine("No cousins on record for {0}.", child);
                }
                Console.WriteLine();
            }
        }
        /**
         * List the details of the N generations of ancestors of the person whose
         * name is given. Note you need to do something about the possibility of
         * duplicate names appearing.
         * @param personName
         * @param numberOfGenerations 1=parents,2=grandparents,
         *                            3=great-grandparents etc.
         */
        public void ListGreatNGrandParents(String personName, int numberOfGenerations){
            var potentialMatches = filterPeopleByUserInput(getMatchingPeople(personName));

            foreach (var child in potentialMatches){
                IEnumerable<Person> parents = new List<Person>{child};

                for (int i = 0; i < numberOfGenerations; i++){
                    parents = parents.Parents(_graph);
                }

                if (parents.Any()){
                    Console.WriteLine("{0}-removed parents for {1}:", numberOfGenerations, child);
                    Console.WriteLine("=================");

                    foreach (var parent in parents){
                        Console.WriteLine("\t{0}", parent);
                    }
                }
                else{
                    Console.WriteLine("No records for {0}-removed parents for {1}.", numberOfGenerations, child);
                }
                Console.WriteLine();
            }

        }
        /**
         * List the details of the N generations of children of the person whose
         * name is given. Note you need to do something about the possibility of
         * duplicate names appearing.
         * @param personName
         * @param numberOfGenerations 1=children,2=grandchildren,
         *                            3=great-grandchildren etc.
         */
        public void ListGreatNGrandChildren(String personName, int numberOfGenerations){
            var potentialMatches = filterPeopleByUserInput(getMatchingPeople(personName));

            foreach (var child in potentialMatches) {
                IEnumerable<Person> children = new List<Person> { child };

                for (int i = 0; i < numberOfGenerations; i++) {
                    children = children.Children(_graph);
                }

                if (children.Any()){
                    Console.WriteLine("{0}-removed children for {1}:", numberOfGenerations, child);
                    Console.WriteLine("=================");

                    foreach (var returnChild in children){
                        Console.WriteLine("\t{0}", returnChild);
                    }
                }
                else{
                    Console.WriteLine("No records for {0}-removed children for {1}:", numberOfGenerations, child);
                }
                Console.WriteLine();
            }
        }

        /**
         * Records an adoption.
         * @param personName
         * @param aDOB
         */
        public void RecordAdoption(String personName, String aDOB){
            var person = getPerson(personName, aDOB);
            var parents = person.Parents(_graph);
            foreach (var parent in parents){
                _graph.AddEdge(person, Relationship.WasAdoptedBy, parent);
            }
        }

        private IEnumerable<Person> filterPeopleByUserInput(IEnumerable<Person> people){
            if (people.Count() < 2){
                //If there are zero or one people in the collection, we have nothing to filter.
                return people;
            }
            Console.WriteLine("Possible Matches: {0}", string.Join(", ", people));
            Console.Write("Enter Date Of Birth of desired person (or blank for all): ");
            var input = Console.ReadLine();

            if (people.Any(person => person.DateOfBirth == input)){
                //If the date of birth matches any input, return that person.
                return people.Where(person => person.DateOfBirth == input);
            }

            if (string.IsNullOrWhiteSpace(input)){
                //If the user gave no input, return the whole collection
                return people;
            }

            //Otherwise try again, because those are the only options.
            Console.WriteLine("No such person with that date of birth. Do try again.");
            return filterPeopleByUserInput(people);
        } 
    }   
}
