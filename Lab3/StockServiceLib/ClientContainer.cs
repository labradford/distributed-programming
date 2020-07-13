using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLib;

namespace StockServiceLib
{
    public class ClientContainer
    {
        public IStockCallback ClientCallback { get; set; }

        public bool IsActive { get; set; }

        public ClientContainer() { }

        public ClientContainer(IStockCallback clientCallback, bool isActive)
        {
            ClientCallback = clientCallback;
            IsActive = isActive;
        }
    }
}
