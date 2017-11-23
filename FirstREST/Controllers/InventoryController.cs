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
    public class InventoryController : ApiController
    {
        //
        // GET: /Inventory/

        public IEnumerable<Lib_Primavera.Model.Inventory> Get()
        {

            return Lib_Primavera.PriIntegration.ListInventory();
        }


        // GET api/Inventory/A001    
        public Inventory Get(string id)
        {
            Lib_Primavera.Model.Inventory artigo = Lib_Primavera.PriIntegration.GetInventory(id);
            if (artigo == null)
            {
                throw new HttpResponseException(
                  Request.CreateResponse(HttpStatusCode.NotFound));
            }
            else
            {
                return artigo;
            }
        }

        // GET api/Inventory/?begin='2016-01-01&2017-1-01'    
        public IEnumerable<Lib_Primavera.Model.Inventory> Get(string begin, string end)
        {
            return Lib_Primavera.PriIntegration.ListInventoryByDate(begin,end);
           
        }

    }
}

