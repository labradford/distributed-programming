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
    public class Stock
    { 
        [DataMember]
        public string Symbol { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        public Stock() { }

        public Stock(string symbol, decimal price)
        {
            Symbol = symbol;
            Price = price;
        }

        public override string ToString()
        {
            return string.Format("{0,-6} {1,10:N2}", Symbol, Price);
        }
    }
}
