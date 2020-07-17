using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLib;
using System.Numerics;
using Utilities;

namespace MathClient
{
    class Program
    {
        static IMathService m_Proxy = null;
        static int m_Tests = 250;
        static BigInteger m_StartNum = BigInteger.Parse("1234567890123456");

        static void Main(string[] args)
        {
            m_Proxy = ProxyGen.GetChannel<IMathService>();
            m_Tests = ConsoleHelpers.ReadInt("Enter how many iterations for the test: ");
            m_StartNum = BigInteger.Parse(ConsoleHelpers.ReadString("Enter a start number for calculating square roots: "));

            // start the service
            m_Proxy.Sqrt(144);

            TimeSpan syncTime = TestSync();
            TimeSpan asyncTime = TestAsync();

            Console.WriteLine();
            Console.WriteLine(new string('=', 50));
            Console.WriteLine("Synchronous Test:");
            Console.WriteLine($"Total Seconds: {syncTime}");
            Console.WriteLine(new string('=', 50));

            Console.WriteLine();
            Console.WriteLine(new string('=', 50));
            Console.WriteLine("Asynchronous Test:");
            Console.WriteLine($"Total Seconds: {asyncTime}");
            Console.WriteLine(new string('=', 50));

            Console.WriteLine("Press <ENTER> to quit...");
            Console.ReadLine();
        }

        private static TimeSpan TestSync()
        {
            DateTime start = DateTime.Now;
            for (BigInteger i = m_StartNum; i < m_StartNum + m_Tests; i++)
            {
                bool isPrime = m_Proxy.IsPrime(i);
                if (isPrime)
                {
                    Console.WriteLine("{0} is prime", i);
                }
            }
            DateTime end = DateTime.Now;
            return end.Subtract(start); 
        }

        private static TimeSpan TestAsync()
        {
            DateTime start = DateTime.Now;
            Parallel.For(0, m_Tests, i =>
            {
                bool isPrime = m_Proxy.IsPrime(m_StartNum + i);
                if (isPrime)
                {
                    Console.WriteLine("{0} is prime", m_StartNum + i);
                }
            });
            DateTime end = DateTime.Now;
            return end.Subtract(start);
        }
    }
}

