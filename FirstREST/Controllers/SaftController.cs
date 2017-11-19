using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FirstREST.Mongo;
using System.Text;
using System.Globalization;
namespace FirstREST.Controllers
{
    public class SaftController : ApiController
    {

        // GET: /Saft/
        public HttpResponseMessage Get()
        {
            string res = MongoConnection.GetCollection("Header");
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(res, Encoding.UTF8, "application/json");

            return response;
        }



        public HttpResponseMessage Get(string id)
        {
            string res = MongoConnection.GetCollection(id);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(res, Encoding.UTF8, "application/json");

            return response;
        }

      

        // api/saft/TotalNetSales?begin=2016-01-01&end=2017-01-01
        // api/saft/SalesInvoices?begin=2016-01-01&end=2017-01-01
        // api/saft/StockMovements?begin=2016-01-01&end=2017-01-01 
        public HttpResponseMessage Get(string id, string begin, string end)
        {
            HttpResponseMessage response = null;
            string body = "";


            switch (id){
                case "TotalNetSales":
                    string salesInvoices = MongoConnection.GetCollectionByDate("Invoices", "InvoiceDate", begin, end);
                    body = getSalesValue(salesInvoices);
                    break;
                case "SalesInvoices":
                    body = MongoConnection.GetCollectionByDate("Invoices", "InvoiceDate", begin, end);         
                    break;
                case "StockMovements":
                    body = MongoConnection.GetCollectionByDate("StockMovements", "MovementDate", begin, end);
                    break;         
            }
                  
            response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return response;
        }


        //auxiliary
        private string getSalesValue(string salesInvoices)
        {
            double total = 0;

            JArray array = JArray.Parse(salesInvoices);
            foreach (JObject invoice in array.Children<JObject>())
            {

                string invoiceType = invoice.SelectToken("InvoiceType").ToString();
                double netTotal = double.Parse(invoice.SelectToken("DocumentTotals")["NetTotal"].ToString(), CultureInfo.InvariantCulture);
                total += (invoiceType.Equals("NC", StringComparison.Ordinal)) ? -netTotal : netTotal;
            }

            JObject obj = new JObject();
            obj.Add("TotalNetSales", total);

            return obj.ToString();
        }

        //get product sales value 
        private string getProductSalesValue(string salesInvoices)
        {
            double total = 0;

            JArray array = JArray.Parse(salesInvoices);
            foreach (JObject invoice in array.Children<JObject>())
            {

                string invoiceType = invoice.SelectToken("InvoiceType").ToString();
                double netTotal = double.Parse(invoice.SelectToken("DocumentTotals")["NetTotal"].ToString(), CultureInfo.InvariantCulture);
                total += (invoiceType.Equals("NC", StringComparison.Ordinal)) ? -netTotal : netTotal;
            }

            JObject obj = new JObject();
            obj.Add("TotalNetSales", total);

            return obj.ToString();
        }
    }
}
