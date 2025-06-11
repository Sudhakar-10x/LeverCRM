using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _10xErp.Models
{
    public class CustomerViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Reference { get; set; }
        public string ContactPerson { get; set; }
        public string PaymentTerms { get; set; }
        public string Series { get; set; }
    }

    public class CustomerModal
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Currency { get; set; }
        public double Balance { get; set; }
        public double OrderBal { get; set; }
        public string CustGrp { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPerTel { get; set; }
        public string ContactPerEmail { get; set; }
        public string NotifyParty { get; set; }
        public string CreditLimit { get; set; }


        public string CardFName { get; set; }
        public string CardType { get; set; }
        public string Cellular { get; set; }
        public string BPStatus { get; set; }
        public string Territory { get; set; }
        public string SalesEmp { get; set; }
        public string DebtLimit { get; set; }
        public string LicTradNum { get; set; }
        public string Fax { get; set; }
        public string Pricelist { get; set; }

        public string PaymentTerms { get; set; }


        public List<BPAddressModel> lstBillToAddr { get; set; }

        public List<BPAddressModel> lstShipToAddr { get; set; }

        public int ShipToCount { get; set; }
        public int BillToCount { get; set; }
        public string UserId { get; set; }

    }

    public class BPAddressModel
    {
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string County { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Block { get; set; }
        public string PoBox { get; set; }
        public string BuildingNo { get; set; }

        public string ZIPCode { get; set; }
        public string State { get; set; }
        public string FedaralTaxID { get; set; }

    }

    public class VendorViewModel
    {
        public string ListNum { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public decimal? CustBalance { get; set; }
        public string deliveryLocation { get; set; }
        public string Paymentterms { get; set; }
        public int? SlpCode { get; set; }
        public int? ExtraDays { get; set; }
        public List<CustContactPerson> lstContactPerson { get; set; }
        public List<BPAddressModel> lstShiptoAddress { get; set; }
        public List<BPAddressModel> lstBilltoAddress { get; set; }
        public List<TerritoryModel> lstTerritory { get; set; }
        public ConfirmMsg CnfMsg { get; set; }
        
    }

    public class SlpModel
    {
        public string SlpCode { get; set; }
        public string SlpName { get; set; }

    }

    public class ConfirmMsg
    {
        public string CnfsMsg { get; set; }
        public bool IsSucess { get; set; }
        public string Ref { get; set; }
    }

    public class CustContactPerson
    {
        public string ContactCode { get; set; }
        public string Name { get; set; }

    }

    public class TerritoryModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class CustomertermModel
    {
        public string Code { get; set; }
        public string Name { get; set; }

    }

    public class SeriesModel
    {
        public string SeriesCode { get; set; }
        public string SeriesName { get; set; }
        public string NextNumber { get; set; }

    }

}