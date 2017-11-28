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



        // api/saft/TotalNetSales?arg1=2016-01-01&arg2=2017-01-01
        // api/saft/SalesInvoices?arg1=2016-01-01&arg2=2017-01-01
        // api/saft/StockMovements?arg1=2016-01-01&arg2=2017-01-01 
        // api/saft/Accounts?arg1=AccountID&arg2=11
        // api/saft/Customers?arg1=CustomerID&arg2=ES989922456_C
        // api/saft/Products?arg1=ProductCode&arg2=A0001
        // api/saft/CustomerSpentValue?arg1=2016-01-01&arg2=2017-01-01
        public HttpResponseMessage Get(string id, string arg1, string arg2)
        {
            HttpResponseMessage response = null;
            string body = "";

            
            switch (id){
                case "TotalNetSales":
                    body = MongoConnection.GetTotalNetSales(arg1, arg2);
                    break;
                case "SalesInvoices":
                    body = MongoConnection.GetCollectionByDate("Invoices", "InvoiceDate", arg1, arg2);         
                    break;
                case "StockMovements":
                    body = MongoConnection.GetCollectionByDate("StockMovements", "MovementDate", arg1, arg2);
                    break;   
                case "Accounts":
                case "Customers":
                case "Products":
                    body = MongoConnection.GetCollectionById(id, arg1, arg2);
                    break;
                case "CustomerSpentValue":
                    body = MongoConnection.GetCustomerTotalSpent(arg1, arg2);
                    break; 

            }
                  
            response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return response;
        }

        // api/saft/ProductSales?id=C0001&begin=2016-01-01&end=2017-01-01
        public HttpResponseMessage Get(string api, string id, string begin, string end)
        {
            HttpResponseMessage response = null;
            string body = MongoConnection.GetProductSales(id,begin,end);
                      

            response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return response;
        }

      
    }
}
