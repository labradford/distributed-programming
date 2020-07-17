using MathServiceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MathServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = null;
            Console.WriteLine("Service starting...");

            try
            {
                host = new ServiceHost(typeof(MathService));
                host.Open();
                Console.Write("Press <ENTER> to close the service...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                if (host != null)
                {
                    host.Close();
                }
            }
        }
    }
}
