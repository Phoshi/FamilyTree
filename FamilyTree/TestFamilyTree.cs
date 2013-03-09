using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FamilyTree
{
    /**
 * TestFamilyTree provides a text-based interface for a family tree
 * The interface consists of a top-level menu with options to:
 * <dl>
 * <dt>Load Data
 * <dd> - loads a set of test data. This, together with any
 * other necessary data, will used when you run your code during the
 * demonstration.</dd>
 * <dt>Input Data
 * <dd> - provides options to add a person, make a link to mother or father
 * or record a wedding, divorce or adoption </dd>
 * <dt>Make a Query
 * <dd> - provides options to list details of an individual and their
 * ancestors or descendants.</dd>
 * </dl>
 * <p>
 * Choosing an option from the input data or query menus results in a
 * call to an empty FamilyTree method. You need to implement the methods in
 * FamilyTree.
 *
 * @author      David Coward
 * @author      Jane Berry
 * @version     2
 */

    class TestFamilyTree
    {
        FamilyTree fTree1;

        /**
         * Instantiates a FamilyTree
         *
         * @see FamilyTree
         */
        TestFamilyTree()
        {
            fTree1 = new FamilyTree();
        }

        /**
         * Returns the String entered at the console.
         *
         * @return    the string that has been input.
         */

        public String getPersonName()
        {
            String personName;
            Console.WriteLine("Enter - name : ");
            personName = Console.ReadLine();
            return personName;
        }

        /**
         * Processes the response to the Input Data request, prompting for
         * additional input as required. Once the required input data has
         * been received, the relevant FamilyTree method is called.
         *
         * @see       FamilyTree
         */

        private void processInput()
        {
            String input, selection, personName, place, dOB, fDOB, mDOB, gDOB,
                    mothersName, fathersName, bridesName, groomsName;
            char iChoice;

            selection = Console.ReadLine().ToUpper();
            iChoice = selection[0];
            while (iChoice != 'X')
            {
                switch (iChoice)
                {
                    case 'A':
                        Console.WriteLine("Enter - name, DOB and place of birth ");
                        input = Console.ReadLine();
                        personName = input.Split()[0];
                        dOB = input.Split()[1];
                        place = input.Split()[2];
                        Person aPerson = new Person(personName, dOB, place);
                        fTree1.AddPerson(aPerson);
                        break;
                    case 'B':
                        Console.WriteLine("Enter - name, DOB, Mother's name and DOB: ");
                        input = Console.ReadLine();
                        personName = input.Split()[0];
                        dOB = input.Split()[1];
                        mothersName = input.Split()[2];
                        mDOB = input.Split()[3];
                        fTree1.MakeLinkToMother(personName, dOB, mothersName, mDOB);
                        break;
                    case 'C':
                        Console.WriteLine("Enter - name, DOB, Father's name and DOB: ");
                        input = Console.ReadLine();
                        personName = input.Split()[0];
                        dOB = input.Split()[1];
                        fathersName = input.Split()[2];
                        fDOB = input.Split()[3];
                        fTree1.MakeLinkToFather(personName, dOB, fathersName, fDOB);
                        break;
                    case 'D':
                        Console.WriteLine("Enter - Bride's name and DOB " +
                                "and Groom's name and DOB : ");
                        input = Console.ReadLine();
                        bridesName = input.Split()[0];
                        dOB = input.Split()[1];
                        groomsName = input.Split()[2];
                        gDOB = input.Split()[3];
                        fTree1.RecordWedding(bridesName, dOB, groomsName, gDOB);
                        break;
                    case 'E':
                        Console.WriteLine("Enter - Wife's name and DOB " +
                                "and Husband's name and DOB : ");
                        input = Console.ReadLine();
                        bridesName = input.Split()[0];
                        dOB = input.Split()[1];
                        groomsName = input.Split()[2];
                        gDOB = input.Split()[3];
                        fTree1.RecordDivorce(bridesName, dOB, groomsName, gDOB);
                        break;
                    case 'F':
                        Console.WriteLine("Enter - name and DOB of person adopted : ");
                        input = Console.ReadLine();
                        personName = input.Split()[0];
                        dOB = input.Split()[1];
                        fTree1.RecordAdoption(personName, dOB);
                        break;
                    default:
                        Console.WriteLine("\nInvalid input choice. Try again\n");
                        break; //do nothing
                }
                inputMenu();
                selection = Console.ReadLine().ToUpper();
                iChoice = selection[0];
            }
        }

        /**
         * Processes a family tree query by calling the relevant FamilyTree
         * method.
         *
         * @see       FamilyTree
         */
        private void processQuery()
    { 
        String selection, personName;
        char qChoice;
        int numOfGens;
        
        selection = Console.ReadLine().ToUpper();
        qChoice = selection[0];
        while ( qChoice != 'X') 
        {
            switch (qChoice)
            {
               case 'K':
                   fTree1.ListPersonDetails(this.getPersonName() );
                    break;                    
                case 'L':
                    fTree1.ListParentDetails(this.getPersonName() );
                    break;
                case 'M':
                    fTree1.ListChildren(this.getPersonName() );
                    break;                  
                case 'N':
                    fTree1.ListSiblings(this.getPersonName() );
                    break;
                case 'O':
                    fTree1.ListPaternalLineage(this.getPersonName() );
                    break;                    
                case 'P':
                    fTree1.ListMaternalLineage(this.getPersonName() );
                    break;
                case 'Q':
                    fTree1.ListGrandParents(this.getPersonName() );
                    break;                  
                 case 'R':
                    fTree1.ListGrandChildren(this.getPersonName() );
                    break;
                case 'S':
                    fTree1.ListCousins(this.getPersonName() );
                    break;                    
                case 'T':
                    personName = this.getPersonName(); 
                    Console.WriteLine("Now enter - number of Generations required : ");
                    numOfGens  = Convert.ToInt32(Console.ReadLine());
                    fTree1.ListGreatNGrandParents(personName, numOfGens);
                    break;
                case 'U':
                    personName = this.getPersonName(); 
                    Console.WriteLine("Now enter - number of Generations required : ");
                    numOfGens  = Convert.ToInt32(Console.ReadLine());
                    fTree1.ListGreatNGrandChildren(personName, numOfGens);
                    break;                  
            }
            queryMenu();
            selection = Console.ReadLine().ToUpper();
            qChoice = selection[0];
        }   
    }

        /**
          * Displays the top-level TestFamilyTree menu
          *
          */
        private void menu()
        {
            Console.WriteLine("\nFAMILY TREE MENU\n");
            Console.WriteLine("L\tLoad Data");
            Console.WriteLine("I\tInput Details");
            Console.WriteLine("Q\tMake a Query\n");

            Console.WriteLine("X\tEXIT\n");

            Console.WriteLine("Enter menu choice K-U, X: ");
        }
        /**
         * Displays the options that make up the Input Details menu
         *
         */

        private void inputMenu()
        {
            Console.WriteLine("\nFAMILY TREE INPUT MENU\n");
            Console.WriteLine("A\tAdd a person to the family tree");
            Console.WriteLine("B\tMake link to mother");
            Console.WriteLine("C\tMake link to father");
            Console.WriteLine("D\tRecord wedding");
            Console.WriteLine("E\tRecord divorce");
            Console.WriteLine("F\tRecord adoption\n");

            Console.WriteLine("X\tEXIT INPUT\n");

            Console.WriteLine("Enter menu choice A-F, X: ");
        }

        /**
        * Displays the query menu
        */
        private void queryMenu()
        {
            Console.WriteLine("\nFAMILY TREE QUERY MENU\n");
            Console.WriteLine("K\tList person details");
            Console.WriteLine("L\tList parent details");
            Console.WriteLine("M\tList children");
            Console.WriteLine("N\tList siblings (noting any half-siblings)");
            Console.WriteLine("O\tList paternal lineage (male line back to oldest man in the tree)");
            Console.WriteLine("P\tList maternal lineage (female line back to oldest woman in the tree)");
            Console.WriteLine("Q\tList all grandparents");
            Console.WriteLine("R\tList all grandchildren");
            Console.WriteLine("S\tList all cousins");
            Console.WriteLine("T\tList all great great… (repeated N times) grandparents");
            Console.WriteLine("U\tList all great great… (repeated N times) grandchildren\n");

            Console.WriteLine("X\tEXIT QUERY\n");

            Console.WriteLine("Enter menu choice K-U, X: ");
        }

        /**
        * Loads test data. The test data comes from 3 pre-named text files.
         * <ul>
         * <li> person.txt  - contains person details
         * <li> fathers.txt - links a person to their father
         * <li> mothers.txt - links a person to their mother
        *
        * @see       FamilyTree
        */
        private void loadData()
        {

            StreamReader pFile, fFile, mFile;
            String entry, name, dOB, place, fName, fDOB, mName, mDOB;
            String[] words;
            name = null;
            dOB = null;
            place = null;
            fName = null;
            fDOB = null;
            mName = null;
            mDOB = null;

            pFile = new System.IO.StreamReader(@"person.txt");
            while ((entry = pFile.ReadLine()) != null)
            {
                words = entry.Split(' ');
                name = words[0];
                dOB = words[1];
                place = words[2];

                if ((name != null) && (dOB != null) && (place != null))
                {
                    Person aPerson = new Person(name, dOB, place);
                    fTree1.AddPerson(aPerson);
                    Console.WriteLine(name + " " + dOB + " ");
                }
            }
            // read file fathers.txt Foreach create father link
            fFile = new System.IO.StreamReader(@"fathers.txt");
            while ((entry = fFile.ReadLine()) != null)
            {
                words = entry.Split(' ');
                name = words[0];
                dOB = words[1];
                fName = words[2];
                fDOB = words[3];

                if ((name != null) && (dOB != null) && (fName != null) && (fDOB != null))
                {
                    fTree1.MakeLinkToFather(name, dOB, fName, fDOB);
                    Console.WriteLine(name + " " + dOB + " " + fName + " " + fDOB + " Father Added");
                }
            }

            // read file mothers.txt Foreach create mother link
            mFile = new System.IO.StreamReader(@"mothers.txt");
            while ((entry = mFile.ReadLine()) != null)
            {
                words = entry.Split(' ');
                name = words[0];
                dOB = words[1];
                mName = words[2];
                mDOB = words[3];

                if ((name != null) && (dOB != null) && (mName != null) && (mDOB != null))
                {
                    fTree1.MakeLinkToMother(name, dOB, mName, mDOB);
                    Console.WriteLine(name + " " + dOB + " " + mName + " " + mDOB + " Mother Added");
                }
            }
        }
        /**
        * Displays the top and subsequent level menus.
        * @param args unused
        */
        static void Main(string[] args)
        {
            TestFamilyTree tFT = new TestFamilyTree();
            char mChoice;
            String selection;

            tFT.menu();
            selection = Console.ReadLine().ToUpper();
            mChoice = selection[0];
            while (mChoice != 'X')
            {
                switch (mChoice)
                {
                    case 'L':
                        {
                            tFT.loadData();
                            break;
                        }
                    case 'I':
                        {
                            tFT.inputMenu();
                            tFT.processInput();
                            break;
                        }
                    case 'Q':
                        {
                            tFT.queryMenu();
                            tFT.processQuery();
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("\nInvalid choice. Try again\n");
                            break;
                        }
                }
                tFT.menu();
                selection = Console.ReadLine().ToUpper();
                mChoice = selection[0];
            }
        }
    }
    
}
 