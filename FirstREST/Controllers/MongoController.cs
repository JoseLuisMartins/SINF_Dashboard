using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FirstREST.Mongo;

namespace FirstREST.Controllers
{
    public class MongoController : ApiController
    {
        public string Get()
        {
            var jsonThing =
                "{ \n" +
                " \"user\":\"Batata\" \n" +
                "}";

            return "Hello Sir! Are you Lost?";
        }

        public string Get(string id)
        {
            return "Index Sir! " + id;
        }
    }
}
