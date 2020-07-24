using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml.Serialization;

namespace Lab5.Models
{
    [XmlRoot(ElementName = "students")]
    public class StudentList : List<Student>
    {
        public static StudentList Load(string filename)
        {
            StudentList list = new StudentList();
            XmlSerializer ser = new XmlSerializer(typeof(Models.StudentList));
            using (StreamReader reader = new StreamReader(filename))
            {
                if (reader != null)
                {
                    list = ser.Deserialize(reader) as Models.StudentList;
                }
            }
            return list;
        }
        public void Save(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Models.StudentList));
            using (StreamWriter writer = new StreamWriter(filename))
            {
                ser.Serialize(writer, this);
            }
        }
    }
}