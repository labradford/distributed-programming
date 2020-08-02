using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckingAccountClient
{
	public class Consts
	{
		public const string CONTENT_XML = "application/xml";
		public const string CONTENT_JSON = "application/json";
	}


	// For a list of common http application content types:
	// http://en.wikipedia.org/wiki/Internet_media_type#List_of_common_media_types
	public enum SerializationModesEnum
	{
		Undefined = 0,
		Xml = 1,
		Json = 2
	}

	public enum MenuOptionsEnum
	{
		Quit = 0,
		GetAllTransactions,
		GetTransactionsByDateRange,
		GetCreditsByType,
		GetDebitsByType,
		AddCredit,
		AddDebit
	}
}
