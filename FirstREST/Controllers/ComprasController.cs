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

        public int Get(string fornecedor, string begin = "", string end = "")
        {
            return (int)(Lib_Primavera.PriIntegration.getDatedPurchasesByFornecedor(begin,end,fornecedor) * -1);
        }
    }
}