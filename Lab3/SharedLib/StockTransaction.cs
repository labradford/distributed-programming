using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace SharedLib
{
    [DataContract]
    public class StockTransaction
    {
        [DataMember]
        public Stock Stock { get; set; }

        [DataMember]
        public DateTime Time { get; set; }

        [DataMember]
        public decimal Change { get; set; }

        [DataMember]
        public int Shares { get; set; }

        public StockTransaction() { }

        public StockTransaction(Stock stock, DateTime time, decimal change, int shares)
        {
            Stock = stock;
            Time = time;
            Change = change;
            Shares = shares;
        }

        public override string ToString()
        {
            char direction = '=';
            if (Change < 0)
            {
                direction = '\u25BC';
            }
            else if (Change > 0)
            {
                direction = '\u25B2';
            }
            return string.Format("{0:yyyy-MM-dd HH:mm:ss} {1} {2} {3,10:N2} [{4,8:N0]}", Time, Stock, direction, Change, Shares);
        }
    }
}
