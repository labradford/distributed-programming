using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace SharedLib
{
    [XmlRoot(ElementName = "transactions")]
    public class TransactionList : List<Transaction>
    {
        public TransactionList() { }

        public TransactionList(IEnumerable<Transaction> collection) : base(collection) { }

        public static TransactionList Load(string filename)
        {
            TransactionList list = new TransactionList();
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(TransactionList));
                using (StreamReader reader = new StreamReader(filename))
                {
                    if (reader != null)
                    {
                        list = ser.Deserialize(reader) as TransactionList;
                    }
                }
            }
            catch { /* File doesn't exist, will be created later. */ }
            return list;
        }
        public void Save(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(TransactionList));
            using (StreamWriter writer = new StreamWriter(filename))
            {
                ser.Serialize(writer, this);
            }
        }
    }
}
