using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using SharedLib;

namespace Final.Controllers
{
    public class TransactionsController : ApiController
    {
        private string FilePath
        {
            get
            {
                return HttpContext.Current.Server.MapPath("~/App_Data/transaction.xml");
            }
        }
        // GET: api/GetAllTransactions
        public TransactionList GetAllTransactions()
        {
            return TransactionList.Load(FilePath);
        }

        // GET: api/GetTransactionsByDateRange
        public TransactionList GetTransactionsByDateRange(DateTime start, DateTime end)
        {
            var data = TransactionList.Load(FilePath).AsQueryable();
            return new TransactionList(data.Where(t => t.Date >= start && t.Date <= end).ToList()); // return only when date falls between start and end, don't include totals, sort as above
        }

        // GET: api/GetCreditsByType
        public TransactionList GetCreditsByType(CreditTypeEnum creditType)
        {
            var data = TransactionList.Load(FilePath).AsQueryable();
            return new TransactionList(data.Where(t => (t is Credit) && (t as Credit).CreditType == creditType).ToList()); // return only transcations where credit type is included in credit type enum, sort as above
        }

        // GET: api/GetDebitsByType
        public TransactionList GetDebitsByType(DebitTypeEnum debitType)
        {
            var data = TransactionList.Load(FilePath).AsQueryable();
            return new TransactionList(data.Where(t => (t is Debit) && (t as Debit).DebitType == debitType).ToList()); // return only transcations where credit type is included in credit type enum, sort as above
        }

        // POST: api/AddDebit
        [HttpPost()]
        public void AddDebit([FromBody] Debit debit)
        {
            var data = TransactionList.Load(FilePath);
            data.Add(debit);
            data.Sort();
            data.Save(FilePath);
        }

        // POST: api/AddCredit
        [HttpPost()]
        public void AddCredit([FromBody] Credit credit)
        {
            var data = TransactionList.Load(FilePath);
            data.Add(credit);
            data.Sort();
            data.Save(FilePath);
        }

    }
}
