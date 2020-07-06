using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Lab2Service
{
    [DataContract]
    public class Student : Person
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string Major { get; set; }
        [DataMember]
        public float Units { get; set; }
        [DataMember]
        public float GPA { get; set; }
    }
}