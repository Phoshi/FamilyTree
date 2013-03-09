using System;
using System.Collections.Generic;
using System.Linq;

namespace FamilyTree
{
    /**
 * Holds the details of an individual. You need to complete this class
 * @author David
 */
    public class Person : IComparable<Person>{
        private static List<Person> _people = new List<Person>(); 
        public static Person Get(string name, string dateofbirth){
            return (from person in _people where person.Name == name && person.DateOfBirth == dateofbirth select person).FirstOrDefault();
        }
        public static IEnumerable<Person> Get(string name){
            return (from person in _people where person.Name == name select person);
        }  

        public string Name { get; internal set; }
        public string DateOfBirth { get; internal set; }
        public string BirthPlace { get; internal set; }

        /** Creates a new instance of Person */
        public Person(){
            Name = "No record on file";
        }
        public Person(String aName, String aDOB, String aBirthPlace){
            Name = aName;
            DateOfBirth = aDOB;
            BirthPlace = aBirthPlace;

            _people.Add(this);
        }

        public int CompareTo(Person other){
            if (Name.CompareTo(other.Name) != 0){
                return Name.CompareTo(other.Name);
            }
            return DateOfBirth.CompareTo(other.DateOfBirth);
        }

        public override string ToString(){
            return string.Format("{0} ({1} - {2})", Name, BirthPlace, DateOfBirth);
        }
    }
}
