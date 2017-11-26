using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstREST.Mongo
{
    public class SaftAccount
    {

        public string AccountID { get; set; }
        public string AccountDescription { get; set; }
        public double OpeningDebitBalance { get; set; }
        public double OpeningCreditBalance { get; set; }
        public double ClosingDebitBalance { get; set; }
        public double ClosingCreditBalance { get; set; }
        public string GroupingCategory { get; set; }
        public uint GroupingCode { get; set; }
        public string TaxonomyCode { get; set; }

    }
}