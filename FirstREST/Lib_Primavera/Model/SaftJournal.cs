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

        [JsonProperty(PropertyName = "CreditLine")]
        public dynamic CreditLineJson { get; set; }
        private List<CreditLine> _creditLineJson;
        public List<CreditLine> creditLine
        {
            get
            {
                if (_creditLineJson == null) _creditLineJson = new List<CreditLine>();

                _creditLineJson.Clear();

                if (CreditLineJson is Newtonsoft.Json.Linq.JArray)
                {

                    foreach (var creditLineJson in CreditLineJson)
                    {
                        _creditLineJson.Add(creditLineJson.ToObject<CreditLine>());
                    }
                }
                else if (CreditLineJson != null)
                {
                    _creditLineJson.Add(CreditLineJson.ToObject<CreditLine>());
                }

                return _creditLineJson;
            }
        }
        [JsonProperty(PropertyName = "DebitLine")]
        public dynamic DebitLineJson { get; set; }
        private List<DebitLine> _debitLine;
        public List<DebitLine> debitLine
        {
            get
            {
                if (_debitLine == null) _debitLine = new List<DebitLine>();

                _debitLine.Clear();

                if (DebitLineJson is Newtonsoft.Json.Linq.JArray)
                {

                    foreach (var transaction in DebitLineJson)
                    {
                        _debitLine.Add(transaction.ToObject<DebitLine>());
                    }
                }
                else if (DebitLineJson != null)
                {
                    _debitLine.Add(DebitLineJson.ToObject<DebitLine>());
                }

                return _debitLine;
            }
        }
       
    }

    public class CreditLine
    {
        public uint RecordID { get; set; }
        public string AccountID { get; set; }
        public string SourceDocumentID { get; set; }
        public string SystemEntryDate { get; set; }
        public string Description { get; set; }
        public double CreditAmount { get; set; }
    }

    public class DebitLine
    {
        public uint RecordID { get; set; }
        public string AccountID { get; set; }
        public string SourceDocumentID { get; set; }
        public string SystemEntryDate { get; set; }
        public string Description { get; set; }
        public double DebitAmount { get; set; }
    }
}