using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SharedLib
{
    public class Credit : Transaction
    {
        [XmlElement(ElementName = "creditType")]
        public CreditTypeEnum CreditType { get; set; }
    }
}
