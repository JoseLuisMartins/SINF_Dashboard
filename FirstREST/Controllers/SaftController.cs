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


       
        //-----
        public HttpResponseMessage Get(string id)
        {
            HttpResponseMessage response = null;
            string body = MongoConnection.GetCollection(id);         
            
            response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return response;
        }

        // api/saft/ProductCustomers?vid=C0001
        // api/saft/CustomersBoughtProducts?vid=PT505678900_C
        // api/saft/Accounts/?vid=11
        // api/saft/Customers/?vid=ES989922456_C
        // api/saft/Products/?vid=A0001
        public HttpResponseMessage Get(string id, string vid)
        {

            HttpResponseMessage response = null;
            string body = "";


            switch (id)
            {
                case "ProductCustomers":
                    body = MongoConnection.GetProductCustomers(vid);
                    break;
                case "CustomersBoughtProducts":
                    body = MongoConnection.GetCustomerBoughtProducts(vid);
                    break;
                case "Accounts":
                    body = MongoConnection.GetCollectionById(id, "AccountID", vid);
                    break;  
                case "Customers":
                    body = MongoConnection.GetCollectionById(id, "CustomerID", vid);
                    break;  
                case "Products":
                    body = MongoConnection.GetCollectionById(id, "ProductCode", vid);
                    break;               
            }

            response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return response;
        }

        // api/saft/TotalNetSales?arg1=2016-01-01&arg2=2017-01-01
        // api/saft/SalesInvoices?arg1=2016-01-01&arg2=2017-01-01
        // api/saft/Top10Products?arg1=2016-01-01&arg2=2017-01-01 
        // api/saft/Top10Customers?arg1=2016-01-01&arg2=2017-01-01 
        // api/saft/BalanceSheet?arg1=2016-01-01&arg2=2017-01-01 
        // api/saft/IncomeStatement?arg1=2016-01-01&arg2=2017-01-01 
        // api/saft/FinancialRatios?arg1=2016-01-01&arg2=2017-01-01 
        // api/saft/NetIncome?arg1=2016-01-01&arg2=2017-01-01 
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
                case "Top10Products":
                    body = MongoConnection.GetTop10Products(arg1, arg2);
                    break;
                case "Top10Customers":
                    body = MongoConnection.GetTop10Customers(arg1, arg2);
                    break;
                case "BalanceSheet":
                    body = MongoConnection.GetBalanceSheet(arg1, arg2);
                    break;
                case "IncomeStatement":
                    body = MongoConnection.GetIncomeStatement(arg1, arg2);
                    break;
                case "FinancialRatios":
                    body = MongoConnection.GetFinancialRatios(arg1, arg2);
                    break;
                case "NetIncome":
                    body = MongoConnection.GetNetIncome(arg1, arg2);
                    break;

            }
                  
            response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return response;
        }

        // api/saft/ProductSales?vid=C0001&begin=2016-01-01&end=2017-01-01
        // api/saft/CustomerSpentValue?vid=PT505678900_C&begin=2016-01-01&end=2017-01-01
        // api/saft/AccountInRange?vid=11&begin=2016-01-01&end=2016-12-01
        public HttpResponseMessage Get(string id, string vid, string begin, string end)
        {
            
            HttpResponseMessage response = null;
            string body = "";


            switch (id)
            {
                case "ProductSales":
                    body = MongoConnection.GetProductSales(vid,begin,end);
                    break;
                case "CustomerSpentValue":
                    body = MongoConnection.GetCustomerTotalSpent(vid, begin, end);
                    break;               
            }

            response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return response;
        }

      
    }
}
