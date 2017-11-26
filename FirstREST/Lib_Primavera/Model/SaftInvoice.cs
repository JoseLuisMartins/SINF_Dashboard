using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstREST.Lib_Primavera.Model
{
    public class SaftInvoice
    {
        public string InvoiceNo { get; set; }
        public string ACTUD { get; set; }
        public DocumentStatus DocumentStatus { get; set; }
        public string Hash { get; set; }
        public string HashControl { get; set; }
        public string Period { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceType { get; set; }
        public SpecialRegimes SpecialRegimes { get; set; }
        public string SourceID { get; set; }
        public string SystemEntryDate { get; set; }
        public string CustomerID { get; set; }
        public Ship ShipTo { get; set; }
        public Ship ShipFrom { get; set; }
        public string MovementStartTime { get; set; }
        [JsonProperty(PropertyName = "Line")]
        public dynamic LineJson { get; set; }
        private List<LineData> _Line;
        public List<LineData> line
        {
            get
            {
                if (_Line == null) _Line = new List<LineData>();

                _Line.Clear();

                if (LineJson is Newtonsoft.Json.Linq.JArray)
                {

                    foreach (var line in LineJson)
                    {
                        _Line.Add(line.ToObject<LineData>());
                    }
                }
                else
                {
                    _Line.Add(LineJson.ToObject<LineData>());
                }

                return _Line;
            }
        }
        public DocumentTotals DocumentTotals { get; set; }
        public WithholdingTax WithholdingTax { get; set; }
    }


    public class DocumentStatus
    {
        public string InvoiceStatus { get; set; }
        public string InvoiceStatusDate { get; set; }
        public string SourceID { get; set; }
        public string SourceBilling { get; set; }
    }

    public class SpecialRegimes
    {
        public string SelfBillingIndicator { get; set; }
        public string CashVATSchemeIndicator { get; set; }
        public string ThirdPartiesBillingIndicator { get; set; }
    }


    public class Ship
    {
        public string DeliveryDate { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public string AddressDetail { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }

    public class LineData
    {
        public string LineNumber { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public uint Quantity { get; set; }
        public string UnitOfMeasure { get; set; }
        public double UnitPrice { get; set; }
        public string TaxPointDate { get; set; }
        public string Description { get; set; }
        public double CreditAmount { get; set; }
        public Tax Tax { get; set; }
        public string SettlementAmount { get; set; }


    }


    public class Tax
    {
        public string TaxType { get; set; }
        public string TaxCountryRegion { get; set; }
        public string TaxCode { get; set; }
        public string TaxPercentage { get; set; }
    }


    public class DocumentTotals
    {
        public double TaxPayable { get; set; }
        public double NetTotal { get; set; }
        public double GrossTotal { get; set; }
    }


    public class WithholdingTax
    {
        public string WithholdingTaxAmount { get; set; }
    }

}