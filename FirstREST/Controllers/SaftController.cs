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
namespace FirstREST.Controllers
{
    public class SaftController : ApiController
    {
           
        // GET: /Saft/

        public String Get()
        {
            return MongoConnection.GetCollection("Header");
        }

        public String Get(string id)
        {
            return MongoConnection.GetCollection(id);
        }

    }
}
