using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5Client
{
    public class Student
    {
        public enum GradeEnum
        {
            PreSchool = -1,
            Kindergarten = 0,
            First = 1,
            Second,
            Third,
            Fourth,
            Fifth,
            Sixth,
            Seventh,
            Eighth,
            Freshmen,
            Sophomore,
            Junior,
            Senior,
            College
        }

        public int ID { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public DateTime DOB { get; set; }

        public float GPA { get; set; }

        public GradeEnum Grade { get; set; }

        public override string ToString()
        {
            return string.Format(
            "{0:000-00-0000} {1,-20} {2,-15} {3:yyyy-MM-dd} {4,-12} {5:0.000}",
            ID, LastName, FirstName, DOB, Grade, GPA);
        }
    }
}
