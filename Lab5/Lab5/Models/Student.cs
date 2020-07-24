using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Lab5.Models
{
    [XmlType(TypeName = "student")]
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

        [XmlAttribute(AttributeName = "id")]
        public int ID { get; set; }
        [XmlElement(ElementName = "lastName")]
        public string LastName { get; set; }
        [XmlElement(ElementName = "firstName")]
        public string FirstName { get; set; }
        [XmlElement(ElementName = "dob")]
        public DateTime DOB { get; set; }
        [XmlElement(ElementName = "gpa")]
        public float GPA { get; set; }
        [XmlElement(ElementName = "grade")]
        public GradeEnum Grade { get; set; }

    }
}