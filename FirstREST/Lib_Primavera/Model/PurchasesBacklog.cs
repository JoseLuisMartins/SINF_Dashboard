using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstREST.Lib_Primavera.Model
{
    public class PurchasesBacklog
    {
        public string Entidade
        {
            get;
            set;
        }

        public string Artigo
        {
            get;
            set;
        }

        public DateTime DataEntrega
        {
            get;
            set;
        }

        public double Quantidade
        {
            get;
            set;
        }

        public double Total
        {
            get;
            set;
        }
    }
}