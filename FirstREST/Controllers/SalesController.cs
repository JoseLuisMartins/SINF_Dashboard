using FirstREST.Mongo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FirstREST.Controllers
{
    public class SalesController : ApiController
    {
        public String Get(string begin, string end)
        {
            return MongoConnection.GetCollectionByDate("Invoices", "InvoiceDate", begin, end);
        }
    }
}