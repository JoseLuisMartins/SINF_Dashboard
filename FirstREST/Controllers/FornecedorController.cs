using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Controllers
{
    public class FornecedorController : ApiController
    {
        // api/Fornecedor/?fIds=<id_0>&fIds=<id_1>[...]
        public IEnumerable<Lib_Primavera.Model.Fornecedor> Get([FromUri] string[] fIds)
        {
            return Lib_Primavera.PriIntegration.ListaFornecedores(fIds);
        }
    }
}