using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using Newtonsoft.Json;

namespace FirstREST.Controllers
{
    public class SaftController : ApiController
    {
           
        // GET: /Saft/

        public String Get()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Assets\\SAFT_DEMOSINF_01-01-2016_31-12-2016.xml"));
            string jsonText = JsonConvert.SerializeXmlNode(doc);

            return jsonText;
        }

    }
}
