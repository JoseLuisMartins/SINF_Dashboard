using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using FirstREST.Validation;
using System.Globalization;

namespace FirstREST.Mongo
{
    public class SaftParser
    {
        public SaftParser()
        {

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Assets\\SAFT_DEMOSINF_01-01-2016_31-12-2016.xml"));
            string jsonText = JsonConvert.SerializeXmlNode(doc);


            //XMLValidation.validation();

            JToken tkn = JObject.Parse(jsonText);

            JToken auditFile = tkn.SelectToken("AuditFile");
            JToken header = auditFile.SelectToken("Header");

            Header(auditFile.SelectToken("Header"));
            MasterFiles(auditFile.SelectToken("MasterFiles"));
            GeneralLedgerEntries(auditFile.SelectToken("GeneralLedgerEntries"));
            SourceDocuments(auditFile.SelectToken("SourceDocuments"));

            MongoConnection.Add("teste", "{\"Nome\":\"Batata\"}");
        }

        public void Header(JToken header)
        {
            MongoConnection.Add("Header", header.ToString());
        }

        public void MasterFiles(JToken master)
        {
            JToken accounts = master.SelectToken("GeneralLedgerAccounts.Account");
            JToken customers = master.SelectToken("Customer");
            JToken suppliers = master.SelectToken("Supplier");
            JToken products = master.SelectToken("Product");

            MongoConnection.AddMany("Accounts", accounts.ToString());
            MongoConnection.AddMany("Customers", customers.ToString());
            MongoConnection.AddMany("Suppliers", suppliers.ToString());
            MongoConnection.AddMany("Products", products.ToString());

        }

        public void GeneralLedgerEntries(JToken ledgerEntries)
        {
            JToken journals = ledgerEntries.SelectToken("Journal");
            JObject generalInfo = FillInfo(ledgerEntries);

            MongoConnection.AddMany("Journals", journals.ToString());
            MongoConnection.Add("LedgerEntriesInfo", generalInfo.ToString());

        }

        public void SourceDocuments(JToken documents)
        {
            JToken salesInvoices = documents.SelectToken("SalesInvoices");
            JObject invoicesInfo = FillInfo(salesInvoices);
            JToken invoices = salesInvoices.SelectToken("Invoice");


            JToken movementOfGoods = documents.SelectToken("MovementOfGoods");
            JObject goodsInfo = new JObject();
            AddObj(ref goodsInfo, movementOfGoods, "NumberOfMovementLines");
            AddObj(ref goodsInfo, movementOfGoods, "TotalQuantityIssued");
            JToken stockMovements = movementOfGoods.SelectToken("StockMovement");

            
            MongoConnection.Add("InvoicesInfo", invoicesInfo.ToString());
            MongoConnection.AddMany("Invoices", invoices.ToString());

            MongoConnection.Add("GoodsInfo", goodsInfo.ToString());
            MongoConnection.AddMany("StockMovements", stockMovements.ToString());

        }



        private void AddObj(ref JObject obj, JToken tkn, String desc)
        {
            obj.Add(desc, tkn.SelectToken(desc));
        }

        private JObject FillInfo(JToken tkn)
        {
            JObject obj = new JObject();
            obj.Add("NumberOfEntries", tkn.SelectToken("NumberOfEntries"));
            obj.Add("TotalDebit", tkn.SelectToken("TotalDebit"));
            obj.Add("TotalCredit", tkn.SelectToken("TotalCredit"));

            return obj;
        }

    }
}