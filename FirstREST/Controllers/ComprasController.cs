using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FirstREST.Lib_Primavera;

namespace FirstREST.Controllers
{
    public class ComprasController : ApiController
    {
        // api/Compras/?begin=<beginDate>&end=<endDate>
        public int Get(string begin = "", string end = "")
        {
            return (int)(Lib_Primavera.PriIntegration.getDatedPurchases(begin,end) * -1);
        }

        // /api/Compras/backlog?begin=2016-01-01&end=2018-01-01
        public IEnumerable<Lib_Primavera.Model.PurchasesBacklog> Get(string id, string begin = "", string end = "")
        {
            return Lib_Primavera.PriIntegration.getDatedPurchasesBacklog(begin, end);
        }
    }
}