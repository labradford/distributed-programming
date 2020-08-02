using SharedLib;
using System;
using System.Collections.Generic;
using System.Net;
using Utilities;

namespace CheckingAccountClient
{
	class Program
	{
		const string SERVICE = "http://localhost:59484/api/";
		const string CONTROLLER = "Transactions";
		static Client m_Client = new Client(SERVICE, SerializationModesEnum.Json);

		static void Main(string[] args)
		{
			MenuOptionsEnum choice = MenuOptionsEnum.Quit;
			do
			{
				choice = ConsoleHelpers.ReadEnum<MenuOptionsEnum>("Option: ");
				Console.Clear();
				
				if (choice != MenuOptionsEnum.Quit)
				{
					DisplayOptionTitle(choice);
				}

				switch (choice)
				{
					case MenuOptionsEnum.GetAllTransactions:
						GetAllTransactions();
						break;
					case MenuOptionsEnum.GetCreditsByType:
						GetCreditsByType();
						break;
					case MenuOptionsEnum.GetDebitsByType:
						GetDebitsByType();
						break;
					case MenuOptionsEnum.GetTransactionsByDateRange:
						GetTransactionsByDateRange();
						break;
					case MenuOptionsEnum.AddCredit:
						AddCredit();
						break;
					case MenuOptionsEnum.AddDebit:
						AddDebit();
						break;
				}

				Console.WriteLine();
				Console.Write("Press <ENTER> to continue...");
				Console.ReadLine();
				Console.Clear();

			} while (choice != MenuOptionsEnum.Quit);
		}

		private static void AddDebit()
		{
			DateTime date = ConsoleHelpers.ReadDate("Enter date: ", DateTime.MinValue, DateTime.MaxValue);
			DebitTypeEnum type = ConsoleHelpers.ReadEnum<DebitTypeEnum>("Enter debit type: ");
			decimal amount = ConsoleHelpers.ReadDecimal("Enter amount: ", 0, Decimal.MaxValue);
			string description = ConsoleHelpers.ReadString("Enter description: ");
			decimal fee =  ConsoleHelpers.ReadDecimal("Enter fee (if there is no fee enter 0): ", 0, Decimal.MaxValue);
			int checkNo = 0;
			if (type == DebitTypeEnum.Check)
            {
				checkNo = ConsoleHelpers.ReadInt("Enter check number: ", 1, int.MaxValue);
            }

			try
			{
				Debit debit = new Debit
				{
					Date = date,
					DebitType = type,
					CheckNo = checkNo,
					Fee = fee,
					Amount = amount,
					Description = description
				};
				m_Client.PostVoid<Debit>(debit, CONTROLLER, "AddDebit");
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Debug.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
			}
		}

		private static void AddCredit()
		{
			DateTime date = ConsoleHelpers.ReadDate("Enter date: ", DateTime.MinValue, DateTime.MaxValue);
			CreditTypeEnum type = ConsoleHelpers.ReadEnum<CreditTypeEnum>("Enter credit type: ");
			decimal amount = ConsoleHelpers.ReadDecimal("Enter amount: ");
			string description = ConsoleHelpers.ReadString("Enter description: ");
			try
			{
				Credit credit = new Credit
				{
					Date = date,
					CreditType = type,
					Amount = amount,
					Description = description
				};
				m_Client.PostVoid<Credit>(credit, CONTROLLER, "AddCredit");
			
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Debug.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
			}
		}

		private static void GetTransactionsByDateRange()
		{
			try
			{
				DateTime start = ConsoleHelpers.ReadDate("Enter start date: ", DateTime.MinValue, DateTime.MaxValue);
				DateTime end = ConsoleHelpers.ReadDate("Enter end date: ", start, DateTime.MaxValue);
				var transactions = m_Client.Get<TransactionList>(CONTROLLER, SerializationModesEnum.Json, "GetTransactionsByDateRange?start={0}&end={1}", start, end);

				Console.WriteLine();
				Console.WriteLine(string.Format("{0,-9} {1,-8} {2,-20} {3,10} {4,8} {5,10}", "Check #", "Date", "Description", "Debit", "Fee", "Credit"));
				Console.WriteLine(new string('=', 75));

				foreach (Transaction transaction in transactions.Result)
				{
					int checkNo = 0;
					decimal fee = 0;
					DateTime date = transaction.Date;
					string description = transaction.Description;
					decimal debit = 0;
					decimal credit = 0;
					Debit asDebit = transaction as Debit;
					if (asDebit != null)
					{
						checkNo = asDebit.CheckNo;
						fee = asDebit.Fee;
						debit = asDebit.Amount;
					}
					else
					{
						credit = transaction.Amount;
					}
					Console.WriteLine(string.Format("{0,7} {1:MM/dd/yyyy} {2,-20} {3,10:N2} {4,8:N2} {5,10:N2}",
						checkNo, date, description, debit, fee, credit));
				}
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Debug.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
			}
		}

		private static void GetDebitsByType()
		{
			try
			{
				var type = ConsoleHelpers.ReadEnum<DebitTypeEnum>("Enter debit type: ");
				var transactions = m_Client.Get<TransactionList>(CONTROLLER, SerializationModesEnum.Json, "GetDebitsByType?debitType={0}", type);

				Console.WriteLine();
				Console.WriteLine(string.Format("{0,-9} {1, -9} {2,-10} {3,-20} {4,10} {5,10}", "Type", "Check #", "Date", "Description", "Debit", "Fee"));
				Console.WriteLine(new string('=', 75));

				foreach (Transaction transaction in transactions.Result)
				{
					Debit asDebit = transaction as Debit;
					DebitTypeEnum debitType = asDebit.DebitType;
					int checkNo = asDebit.CheckNo;
					DateTime date = asDebit.Date;
					string description = asDebit.Description;
					decimal fee = asDebit.Fee;
					decimal debit = asDebit.Amount;

					Console.WriteLine(string.Format("{0,-9} {1, -9} {2:MM/dd/yyyy} {3,-20} {4,10:N2} {5,10:N2}",
						type, checkNo, date, description, debit, fee));
				}
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Debug.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
			}
		}

		private static void GetCreditsByType()
		{
			try
			{
				var type = ConsoleHelpers.ReadEnum<CreditTypeEnum>("Enter credit type: ");
				var transactions = m_Client.Get<TransactionList>(CONTROLLER, SerializationModesEnum.Json, "GetCreditsByType?creditType={0}", type);

				Console.WriteLine();
				Console.WriteLine(string.Format("{0,-11} {1,-10} {2,-20} {3,10:N2}", "Type", "Date", "Description", "Credit"));
				Console.WriteLine(new string('=', 55));

				foreach (Transaction transaction in transactions.Result)
				{
					Credit asCredit = transaction as Credit;
					CreditTypeEnum creditType = asCredit.CreditType;
					DateTime date = asCredit.Date;
					string description = asCredit.Description;
					decimal credit = asCredit.Amount;
					
					Console.WriteLine(string.Format("{0,-11} {1:MM/dd/yyyy} {2,-20} {3,10:N2}",
						creditType, date, description, credit));
				}
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Debug.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
			}
		}


		private static void GetAllTransactions()
		{
			try
			{
				var transactions = m_Client.Get<TransactionList>(CONTROLLER, SerializationModesEnum.Json, "GetAllTransactions");
				Console.WriteLine(string.Format("{0,-9} {1,-8} {2,-20} {3,10} {4,8} {5,10} {6,12}", "Check #", "Date", "Description", "Debit", "Fee", "Credit", "Balance"));
				Console.WriteLine(new string('=', 83));

				decimal balance = 0;
				foreach (Transaction transaction in transactions.Result)
				{
					int checkNo = 0;
					decimal fee = 0;
					DateTime date = transaction.Date;
					string description = transaction.Description;
					decimal debit = 0;
					decimal credit = 0;
					Debit asDebit = transaction as Debit;
					if (asDebit != null)
					{
						checkNo = asDebit.CheckNo;
						fee = asDebit.Fee;
						debit = asDebit.Amount;
						balance -= fee;
						balance -= debit;
					} 
					else
                    {
						credit = transaction.Amount;
						balance += credit;
                    }

					Console.WriteLine(string.Format("{0,7} {1:MM/dd/yyyy} {2,-20} {3,10:N2} {4,8:N2} {5,10:N2} {6,12:N2}",
						checkNo, date, description, debit, fee, credit, balance));
				}
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Debug.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
			}
		}

		/// <summary>
		/// Displays the screen title for the given menu choice
		/// </summary>
		/// <param name="choice">Menu choice</param>
		private static void DisplayOptionTitle(MenuOptionsEnum choice)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(choice.WordBreakMixedCase());
			ConsoleHelpers.WriteBorder('=', choice.WordBreakMixedCase().Length);
			Console.WriteLine();
			Console.ResetColor();
		}
	}
}
