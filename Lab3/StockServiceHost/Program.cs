using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;
using StockServiceLib;

namespace StockServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = null;
            try
            {
                Console.WriteLine("Starting service...");
                host = new ServiceHost(typeof(StockService));
                host.Open();
                Console.WriteLine("Service started.");
                Console.WriteLine("Press <ENTER> to stop service...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred starting the service: {0}", ex.Message);
            }
            finally
            {
                if (host != null)
                {
                    host.Close();
                }
            }
            Console.Write("Press <ENTER> to quit...");
            Console.ReadLine();
        }
    }
}
