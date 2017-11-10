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
        // api/Fornecedor/
        public IEnumerable<Lib_Primavera.Model.Fornecedor> Get()
        {
            return Lib_Primavera.PriIntegration.ListaFornecedores();
        }

        // api/Fornecedor/?id=<...>
        public Lib_Primavera.Model.Fornecedor Get(string id)
        {
            return Lib_Primavera.PriIntegration.GetFornecedor(id);
        }
    }
}