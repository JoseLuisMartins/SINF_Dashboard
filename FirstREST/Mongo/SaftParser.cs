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
using FirstREST.Lib_Primavera.Model;
using System.Text;


namespace FirstREST.Mongo
{
    public class SaftParser
    {
        string year;

        public SaftParser(string year)
        {

            XmlDocument doc = new XmlDocument();
            this.year = year;
            doc.LoadXml(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Assets\\SAFT_DEMOSINF_01-01-" + year + "_31-12-" + year + ".xml"));
            string jsonText = JsonConvert.SerializeXmlNode(doc);
         

            JToken tkn = JObject.Parse(jsonText);

            JToken auditFile = tkn.SelectToken("AuditFile");
            JToken header = auditFile.SelectToken("Header");

            Header(auditFile.SelectToken("Header"));
            MasterFiles(auditFile.SelectToken("MasterFiles"));
            GeneralLedgerEntries(auditFile.SelectToken("GeneralLedgerEntries"));
            SourceDocuments(auditFile.SelectToken("SourceDocuments"));

            
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



            if (accounts != null)
            {
                string accountsJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<List<SaftAccount>>(accounts.ToString()));
                MongoConnection.AddMany("Accounts" + year, accountsJson);
            }
            if (customers != null)
                MongoConnection.AddMany("Customers", customers.ToString());
            if (suppliers != null)
                MongoConnection.AddMany("Suppliers", suppliers.ToString());
            if (products != null)
                MongoConnection.AddMany("Products", products.ToString());

        }

       
       
        public void GeneralLedgerEntries(JToken ledgerEntries)
        {
            JToken journals = ledgerEntries.SelectToken("Journal");
            JObject generalInfo = FillInfo(ledgerEntries);



            if (journals != null)
            {
                string journalsJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<List<SaftJournal>>(journals.ToString()));
                MongoConnection.AddMany("Journals", journalsJson);
            }
            if(generalInfo != null)
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


            

            if(invoices != null) {
                string invoicesJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<List<SaftInvoice>>(invoices.ToString()));
                MongoConnection.AddMany("Invoices", invoicesJson);
            }
            if (invoicesInfo != null)            
                MongoConnection.Add("InvoicesInfo", invoicesInfo.ToString());
            
                
              
            if (goodsInfo != null)
                MongoConnection.Add("GoodsInfo", goodsInfo.ToString());
           
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