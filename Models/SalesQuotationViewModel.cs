using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _10xErp.Models
{
	//public class SalesQuotationViewModel
	//{
 //       public string CardName { get; set; }
 //       public string CardCode { get; set; }
 //       public string RefNo { get; set; }
 //       public bool Staff { get; set; }
 //       public string EmpID { get; set; }
 //       public string Series { get; set; }
 //       public string BranchId { get; set; }
 //       public string Remarks { get; set; }
 //       public decimal? DocDiscount { get; set; }
 //       public decimal? DocAmount { get; set; }
 //       public int MyProperty { get; set; }
 //       public string WhseCode { get; set; }
 //       public string InvNo { get; set; }
 //       public DateTime ReqDate { get; set; }
 //       public List<ItemDetails> ItemDetailsListView { get; set; }
 //       public string Paymentterms { get; set; }
 //       public string Delivery { get; set; }
 //       public string Reason { get; set; }
 //       public string offerV { get; set; }
 //       public string DocDate { get; set; }
 //       public int SalesEmployee { get; set; }
 //       public int DocEntry { get; set; }
 //   }

    public class SalesQuotationViewModel
    {
        public int? OpenQuotations { get; set; }
        public int? ConvertedOrders { get; set; }
        public List<DataChart> MonthlyQuotations { get; set; }
        public List<DataChart> MonthlyOrders { get; set; }
        //public int? MonthlyQuotations { get; set; }
        //public int? MonthlyOrders { get; set; }
        public int? DocNum { get; set; }
        public bool Staff { get; set; }
        public DateTime? DocDate { get; set; }
        public DateTime? offerV { get; set; }

        //[Required(ErrorMessage = "Please Select Customer")]
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string ContactPerson { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalDiscount { get; set; }
        public decimal? NetAmount { get; set; }
        public decimal? TotalBeforeDisc { get; set; }
        public double DisPer { get; set; }
        public string costcenter { get; set; }
        public List<VATModel> lstVAT { get; set; }
        public List<CostcenterModel> lstCoctCenter { get; set; }
        public List<WarehouseViewModel> lstWarehouse { get; set; }
        public decimal? Discount { get; set; }
        public decimal? totalDisc { get; set; }
        public decimal? Tax { get; set; }

        public int DocEntry { get; set; }
        public ConfirmMsg CnfMsg { get; set; }
        public DateTime? PostingDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string CustRef { get; set; }
        public string SalesEmployee { get; set; }
        public string PaymentTerms { get; set; }
        public string Series { get; set; }
        public decimal? DocAmount { get; set; }
        public string DocStatus { get; set; }
        public string Remarks { get; set; }
        public short? NumberOfInstallments { get; set; }
        public short? DaysOverDue { get; set; }
        public string Branch { get; set; }
        public string EmpID { get; set; }
        public string BranchId { get; set; }
        public decimal? DocDiscount { get; set; }
        public string WhseCode { get; set; }
        public string Value { get; set; }
        //public List<SalesQtnLineItm> lstSOLineDetails { get; set; }
        public List<ItemDetails> ItemDetailsListView { get; set; }
    }

    public class ItemAvailabilityModel1
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UOM { get; set; }
        public decimal TotalStock { get; set; }
        public decimal DeliveryQty { get; set; }
        public decimal PurchasePrice { get; set; }
        public string Currency { get; set; }
        public decimal ItemCost { get; set; }
        public decimal Value { get; set; }
        public decimal SalesPrice { get; set; }

        // Dynamic warehouse stock columns (pivoted)
        // Since warehouse codes are dynamic, we can keep them in a dictionary
        public Dictionary<string, decimal> WarehouseStocks { get; set; } = new Dictionary<string, decimal>();
    }

    public class ItemAvailabilityModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UOM { get; set; }
        public decimal TotalStock { get; set; }
        public decimal DeliveryQty { get; set; }
        public decimal PurchasePrice { get; set; }
        public string Currency { get; set; }
        public decimal ItemCost { get; set; }
        public decimal Value { get; set; }
        public decimal SalesPrice { get; set; }
        public Dictionary<string, decimal> WarehouseStocks { get; set; } = new Dictionary<string, decimal>();
        public decimal ADSR { get; set; }
        public decimal DXBSR { get; set; }
        public decimal FGRAKSTR { get; set; }
        public decimal HOSPSTR { get; set; }
        public decimal HOSR { get; set; }
        public decimal HOSTR { get; set; }
        public decimal HOSTR2 { get; set; }
        public decimal INDAR18A { get; set; }
        public decimal INDAR5 { get; set; }
        public decimal INDAR6B { get; set; }
        public decimal JNPSR { get; set; }
        public decimal MAINTSTR { get; set; }
        public decimal RAKWH { get; set; }
        public decimal RESWH { get; set; }
        public decimal TRANSIT { get; set; }


    }
    public class ReportFilterModel
    {
        public string ItemCodeOrName { get; set; }
        public bool ShowInactive { get; set; }
        public bool ShowAvailable { get; set; }
        public string ServiceType { get; set; }
        public string Branch { get; set; }
        public string Warehouse { get; set; }
    }

    public class DataChart
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }
    public class ChartDataModel
    {
        public string MonthYear { get; set; }
        public decimal? TotalValue { get; set; }

        public List<string> Labels { get; set; }
        public List<decimal> Values { get; set; }
    }

    public class SalesQtnLineItm
    {
        public string ItemType { get; set; }
        public int? lineNo { get; set; }
        public int? DocEntry { get; set; }
        public double actQty { get; set; }
        public string status { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string BrandName { get; set; }
        public string ItemRef { get; set; }
        public double Price { get; set; }
        public double PriceAfterDiscount { get; set; }
        public double GrossPriceAfterDiscount { get; set; }
        public string Barcode { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string ItemGrp { get; set; }
        public string FrgnName { get; set; }
        public double DisPer { get; set; }
        public int UomEntry { get; set; }
        public string UomName { get; set; }
        public string Packing { get; set; }
        public string BaseUOM { get; set; }
        public string SubGrp { get; set; }
        public string SubSubGrp { get; set; }
        public string ItemCategory { get; set; }
        public string CountryOfOrigin { get; set; }
        public string SubSubSubGrp { get; set; }
        public List<UomDetails> lstUomDetails { get; set; } = new List<UomDetails>();
        public ConfirmMsg CnfMsg { get; set; }
        public double WhInstockQty { get; set; }
        public string SerialNumberStatus { get; set; }
        public bool IsSerNumEnbaled { get; set; }
        public string BinCode { get; set; }
        public double Qty { get; set; }
        public double ConfirmQty { get; set; }
        public double LineTotalPrice { get; set; }
        public string VatGrpCode { get; set; }
        public string Remarks { get; set; }
        public string FreeText { get; set; }
        public string Type { get; set; }
        public double VatRate { get; set; }
        public string costcenter { get; set; }
        public string warehouse { get; set; }
    }

}