using System;

namespace FamilyTree
{
    /**
 * Holds the details of an individual. You need to complete this class
 * @author David
 */
    public class Person : IComparable<Person>{
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
