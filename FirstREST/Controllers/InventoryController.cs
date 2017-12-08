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

        public dynamic Get(string begin, string end, string inout)
        {
            System.Diagnostics.Debug.WriteLine("HERREERERER" + inout);
            switch (inout)
            {
                case "IN":
                    return Lib_Primavera.PriIntegration.ListSTKIn(begin, end);
                case "OUT":
                    return Lib_Primavera.PriIntegration.ListSTKOut(begin, end);
                case "INC":
                    return Lib_Primavera.PriIntegration.ListSTKMovementInByFamilies(begin, end);
                case "OUTC":
                    return Lib_Primavera.PriIntegration.ListSTKMovementOutByFamilies(begin, end);
            }

            return null;
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

        public Lib_Primavera.Model.MovementLists Get(string begin, string end)
        {
            return Lib_Primavera.PriIntegration.ListSTKMovementSum(begin, end);

        }


        public dynamic Get(string date, int k)
        {
            switch (k)
            {
                case 1:
                    return Lib_Primavera.PriIntegration.TotalInventoryValueByDate(date);
                case 2:
                    return Lib_Primavera.PriIntegration.ListInventoryByDate(date);
                case 3:
                    return Lib_Primavera.PriIntegration.TotalInventoryByFamilies(date);
            }

            return null;

        }

    }
}

