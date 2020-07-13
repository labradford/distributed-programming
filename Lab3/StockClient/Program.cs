using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using SharedLib;
using Utilities;
using System.ServiceModel;
using System.Net;

namespace StockClient
{
    class Program
    {
        enum MenuChoicesEnum
        {
            Quit = 0,
            AddStock,
            GetStockQuote,
            StartMonitoring
        }

        static IStockService m_Proxy = null;
        static StockMonitor m_Monitor = null;
        static void Main(string[] args)
        {
            try
            {
                MenuChoicesEnum choice = MenuChoicesEnum.Quit;
                m_Monitor = new StockMonitor();
                m_Proxy = ProxyGen.GetChannel<IStockService>(m_Monitor);
                m_Proxy.Login();

                do
                {
                    Console.Clear();
                    choice = ConsoleHelpers.ReadEnum<MenuChoicesEnum>("Enter selection: ");
                    switch (choice)
                    {
                        case MenuChoicesEnum.GetStockQuote:
                            GetStockQuote();
                            break;
                        case MenuChoicesEnum.AddStock:
                            AddStock();
                            break;
                        case MenuChoicesEnum.StartMonitoring:
                            MonitorStocks();
                            break;
                    }
                    Console.WriteLine("Press <ENTER> to continue...");
                    Console.ReadLine();
                } while (choice != MenuChoicesEnum.Quit);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An error occurred: {0}", ex.Message);
                Console.ResetColor();
            }
            finally
            {
                if (m_Proxy != null)
                {
                    m_Proxy.Logout();
                }
            }

            Console.Write("Press <ENTER> to quit...");
            Console.ReadLine();
        }

        public static void AddStock()
        {
            string symbol = ConsoleHelpers.ReadString("Enter stock symbol: ");
            decimal price = ConsoleHelpers.ReadDecimal("Enter initial price: ");
            try
            {
                m_Proxy.AddNewStock(symbol, price);
                Console.WriteLine("Stock Added");
            }
            catch (FaultException ex)
            {
                Console.WriteLine("Could not add stock: {0}", ex.Reason);
            }

        }

        public static void GetStockQuote()
        {
            string symbol = ConsoleHelpers.ReadString("Enter stock symbol: ");
            try
            {
                Console.WriteLine(m_Proxy.GetStockQuote(symbol));
            }
            catch (FaultException ex)
            {
                Console.WriteLine("Could not retrieve stock quote: {0}", ex.Reason);
            }

        }

        public static void MonitorStocks()
        {
            Console.WriteLine("Stock Monitoring has started. Press <ENTER> to stop.");
            try
            {
                m_Proxy.StartTickerMonitoring();
                Console.ReadLine();
                m_Proxy.StopTickerMonitoring();
            }
            catch (FaultException ex)
            {
                Console.WriteLine("Something went wrong: {0}", ex.Reason);
            }      
        }
    }
}
