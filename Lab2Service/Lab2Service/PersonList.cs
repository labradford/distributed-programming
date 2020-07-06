using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Lab2Service
{
    [CollectionDataContract]
    public class PersonList : List<Person>
    {
        public PersonList() { }
        public PersonList(IEnumerable<Person> source) : base(source) { }
    }
}