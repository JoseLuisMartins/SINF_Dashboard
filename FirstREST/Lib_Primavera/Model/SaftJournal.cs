using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstREST.Mongo
{
    public class SaftJournal
    {
        public string JournalID { get; set; }
        public string Description { get; set; }
        [JsonProperty(PropertyName = "Transaction")]
        public dynamic TransactionJson { get; set; }
        private List<Transaction> _transaction;
        public List<Transaction> transaction
        {
            get
            {
                if (_transaction == null) _transaction = new List<Transaction>();

                _transaction.Clear();

                if (TransactionJson is Newtonsoft.Json.Linq.JArray)
                {

                    foreach (var transaction in TransactionJson)
                    {
                        _transaction.Add(transaction.ToObject<Transaction>());
                    }
                }
                else if(TransactionJson != null)
                {
                    _transaction.Add(TransactionJson.ToObject<Transaction>());
                }

                return _transaction;
            }
        }
    }

    public class Transaction
    {
        public string TransactionID { get; set; }
        public string Period { get; set; }
        public string TransactionDate { get; set; }
        public string SourceID { get; set; }
        public string Description { get; set; }
        public string DocArchivalNumber { get; set; }
        public string TransactionType { get; set; }
        public string GLPostingDate { get; set; }
        public TransactionLines Lines { get; set; }

    }

    public class TransactionLines
    {
        public TransactionLine CreditLine { get; set; }
        public TransactionLine DebitLine { get; set; }
       
    }

    public class TransactionLine
    {
        public uint RecordID { get; set; }
        public uint AccountID { get; set; }
        public string SourceDocumentID { get; set; }
        public string SystemEntryDate { get; set; }
        public string Description { get; set; }
        public double CreditAmount { get; set; }
    }
}