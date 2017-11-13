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
    public class ArtigosController : ApiController
    {
        //
        // GET: /Artigos/

        public IEnumerable<Lib_Primavera.Model.Artigo> Get()
        {

            return Lib_Primavera.PriIntegration.ListaArtigos();
        }


        // GET api/artigo/?fornecedor=
        public IEnumerable<Lib_Primavera.Model.Artigo> Get(string fornecedor)
        {
            return Lib_Primavera.PriIntegration.GetArtigos_Fornecedor(fornecedor);
        }

    }
}

