using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExtensionMethods_and_ListofObjects
{
    public partial class Form1 : Form
    {
        List<Student> studentsList = new List<Student>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] firstnames = {"Forrest","Daniella","Jesse",
            "Stephanie","Ethan","William","Michael","Ross","Oleg",
            "Billy","James","Clay","Derek","Sergiu","Daniel"};

            string[] lastnames = {"Avery","Baldwin","Choi","Do","Holt",
            "Jackson","Nollette","Pendelton","Roshchuk","Teung",
            "Tolentino", "Tolliver","Vang","Ungureanu","Vasquez"};

            int[] ids = { 5, 7, 2, 12, 15, 20, 4, 9, 6, 8, 25, 18, 3, 21, 14 };
            Random rand = new Random();
            for(int i=0; i<firstnames.Length;i++)
            {
                Student student = new Student(ids[i], firstnames[i], lastnames[i]);
                for (int j = 1; j <= 7; j++)
                    student.AddGrade(rand.Next(50, 101));

                studentsList.Add(student);
            }

        }
        private void DisplayStudents(List<Student> students)
        {
            richTextBox1.Clear();
            foreach (Student s in students)
            {
                richTextBox1.AppendText(String.Format(
                    s.ID + "  " + s.Firstname + " " + s.Lastname + "  " +
                    s.AverageGrade().ToString("f1") + "\n"));
            }
        }
        private void btnGetAtLeastBStudents_Click(object sender, EventArgs e)
        {
            Predicate<Student> match = student => student.AverageGrade() >= 75;
            List<Student> BorAboveStudents = studentsList.FindAll(match);
            DisplayStudents(BorAboveStudents);
        }
        //Exercise Solutions
        ///Display List of students whose lowestgrade is below 60
        private List<Student> GetStudentsWithLowLowestGrade()
        {
            /*
             * use List<T>.FindAll(Predicate<T>)
             */
            Predicate<Student> match = student => student.GetLowestGrade() < 60;
            List<Student> failingStudents = studentsList.FindAll(match);
            return failingStudents;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //get students whose lowest grade is below 60
            List<Student> failingStudents = GetStudentsWithLowLowestGrade();
            DisplayStudents(failingStudents);
        }
        //method to return list of students whose lastnames has at least 
        //7characters
        private List<Student> GetStudentsWithLongLastnames()
        {
            Predicate<Student> match = student => student.Lastname.Length >= 8;
            List<Student> longLastnameStudents = studentsList.FindAll(match);
            return longLastnameStudents;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //get students with long lastnames
            List<Student> longLastnameStudents = GetStudentsWithLongLastnames();
            DisplayStudents(longLastnameStudents);
        }
    }
}
///Exercise: Add to this project
///Display List of students whose lowestgrade is below 60
///Display list of students whose highestgrade is above 95
///Display list of students whose lastname has at least 7 characters
