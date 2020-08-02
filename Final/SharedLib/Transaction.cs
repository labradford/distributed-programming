using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SharedLib
{
    [XmlInclude(typeof(Credit))]
    [XmlInclude(typeof(Debit))]
    [XmlType(TypeName = "transaction")]
    public class Transaction : IComparable
    {
        [XmlElement(ElementName = "date")]
        public DateTime Date { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "amount")]
        public decimal Amount { get; set; }

        public override string ToString()
        {
            return string.Format("{0:MM/dd/yyyy} {1,-20} {2,10:N2}", Date, Description, Amount);
        }

        public int CompareTo(object obj)
        {
            if (obj == null) { return 1; }

            Transaction other = obj as Transaction;
            if (other == null)
            {
                throw new ArgumentException("Object is not a Transaction");
            }

            int result = this.Date.CompareTo(other.Date);
            if (result != 0)
            {
                return result;
            }

            bool otherIsCredit = other is Credit;
            bool thisIsCredit = this is Credit;
            if (thisIsCredit && !otherIsCredit)
            {
                return -1;
            }
            else if (otherIsCredit && !thisIsCredit)
            {
                return 1;
            }

            result = this.Amount.CompareTo(other.Amount);
            if (result != 0)
            {
                return result;
            }
            return this.Description.CompareTo(other.Description);
        }
    }
}
