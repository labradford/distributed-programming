using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SharedLib
{
    public class Debit : Transaction
    {
        [XmlElement(ElementName = "debitType")]
        public DebitTypeEnum DebitType { get; set; }

        [XmlElement(ElementName = "checkNo")]
        public int CheckNo { get; set; }

        [XmlElement(ElementName = "fee")]
        public decimal Fee { get; set; }
    }
}
