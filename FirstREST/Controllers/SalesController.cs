using FirstREST.Mongo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;

namespace FirstREST.Controllers
{
    public class SalesController : ApiController
    {
        // api/sales?begin=''&end=''
        public HttpResponseMessage Get(string begin, string end)
        {
            string res = MongoConnection.GetCollectionByDate("Invoices", "InvoiceDate", begin, end);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(res, Encoding.UTF8, "application/json");

            return response;
        }





    }
}