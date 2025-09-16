using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _10xErp.Models
{
    public class ItemViewModels
    {
        public string Barcode { get; set; }
        public string CardCode { get; set; }
        public string ItemCode { get; set; }
        public string WhseCode { get; set; }
        public string EID { get; set; }
        public string ItemGrpCode { get; set; }
        public bool? Staff { get; set; }
        public string FromWhsCode { get; set; }
    }

    public class ItemDetails
    {
        public int LineNum { get; set; }
        public string Type { get; set; }
        public string TextDescription { get; set; }

        //[Required(ErrorMessage = "Item Code is required")]
        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public string BrandName { get; set; }

        public string ItemRef { get; set; }

        public decimal? Price { get; set; }
        public decimal? OriginalPrice { get; set; }

        public string SupplierCode { get; set; }

        public string SupplierName { get; set; }

        public string ItemGrp { get; set; }

        public string FrgnName { get; set; }

        public decimal DisPer { get; set; }

        public int UomCode { get; set; }
        public int UomEntry { get; set; }

        public string UomName { get; set; }

        public string Packing { get; set; }

        public string BaseUOM { get; set; }

        public string SubGrp { get; set; }

        public string SubSubGrp { get; set; }

        public string ItemCategory { get; set; }

        public string CountryOfOrigin { get; set; }
        public string costcenter { get; set; }
        public string SubSubSubGrp { get; set; }
        public List<BatchDetails> lstBatchdetails { get; set; }
        public List<UomDetails> lstUomDetails { get; set; }
        public List<VATModel> lstVAT { get; set; }
        public List<CostcenterModel> lstCoctCenter { get; set; }
        public List<InventoryDetails> lstWarehouse { get; set; }
        public ConfirmMsg CnfMsg { get; set; }

        public decimal? WhInstockQty { get; set; }

        public string SerialNumberStatus { get; set; }

        public bool IsSerNumEnbaled { get; set; }

        public string BinCode { get; set; }
        public decimal? Qty { get; set; }
        public decimal LineTotal { get; set; }
        public decimal GrossPriceAfterDiscount { get; set; }
        public decimal VAT { get; set; }
        public double? VatRate { get; set; }
        public string VatGrpCode { get; set; }
        public decimal? Committed { get; set; }
        public decimal? OverallAvlQty { get; set; }
        public string DocEntry { get; set; }
        public string lineNo { get; set; }
        public bool IsChanged { get; set; }

        public string foc { get; set; }
        public string focremarks { get; set; }

        public string FreeText { get; set; }
        public string Remarks { get; set; }
        public string Warehouse { get; set; }
        public string UomCodes { get; set; }
        //[NotMapped]
        public int Index { get; set; } 

        public string Batches { get; set; }
        public string BarCode { get; set; }
        public string ExpiryDate { get; set; }
        public int BatchQty { get; set; }
    }

    public class InventoryDetails
    {
        public decimal? InStock { get; set; }
        public decimal? Commited { get; set; }
        public decimal? Ordered { get; set; }
        public string WhsName { get; set; }
        public string WhsCode { get; set; }
        //public List<BatchDetails> lstBatchdetails { get; set; }

    }


    public class BatchDetails
    {
        public string BatchNum { get; set; }
        public string WhsCode { get; set; }

        public string Expirydate { get; set; }

        public decimal? BatchQty { get; set; }

    }
    public class UomDetails
    {
        public int UomCode { get; set; }

        public string UomName { get; set; }

        public decimal? BaseQty { get; set; }

        public decimal? AltQty { get; set; }

        public string UomCodeName { get; set; }

    }

    public class AssignedEmpModel
    {
        public int? AssignedEmpId { get; set; }
        public string AssignedEmpName { get; set; }
        public string Memo { get; set; }
        public string Mobile { get; set; }
        public string EmpName { get; set; }
        public string PDTUSERID { get; set; }
    }
    public class CostcenterModel
    {
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }
    }

    public class WarehouseViewModel
    {
        public string WhseCode { get; set; }

        public string WhseName { get; set; }

        public int? BranchID { get; set; }

        public bool IsBinEnabled { get; set; }

    }
    public class VATModel
    {
        public string VATCode { get; set; }
        public string VATName { get; set; }
        public double? VatRate { get; set; }
    }

    public class ProductDetailsModel
    {

        public string CodeBars { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int UomCode { get; set; }
        public string UomName { get; set; }
        public string Qty { get; set; }
        public decimal Price { get; set; }

        public string ItmsGrpNam { get; set; }
        public string FrgnName { get; set; }

        public string Remarks { get; set; }

        public decimal? InStock { get; set; }
        public decimal? Commited { get; set; }
        public decimal? Ordered { get; set; }
        public string WhsName { get; set; }
        public string WhsCode { get; set; }

        public string ItemType { get; set; }

        public string Uomgrp { get; set; }

        public string Manufacturer { get; set; }
        public string ManagedBy { get; set; }
        public string IssueprimalyBy { get; set; }
        public string ManagementMethod { get; set; }
        public string Itemstatus { get; set; }
        public string ShippingType { get; set; }

        public string Prefvendor { get; set; }
        public string PurVatGroup { get; set; }
        public string SalVatGroup { get; set; }
        public string SaleUomcode { get; set; }

        public string InvUomcode { get; set; }

        //UDF Details
        public string MOHRegCode { get; set; }
        public string MOHRegExp { get; set; }
        public string ShelfLife { get; set; }
        public string ShelfLifeMode { get; set; }
        public string Supplier { get; set; }
        public string Prency { get; set; }
        public string DelSys { get; set; }
        public string Agency { get; set; }
        public string COO { get; set; }
        public string SalesRepName { get; set; }
        public string Disc { get; set; }

        public List<InventoryDetails> lstInvDetails { get; set; }
    }
    public class OrderViewModel
    {
        public CustomerDetails Customer { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class CustomerDetails
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        //public string ContactPerson { get; set; }
        public string LpoNo { get; set; }
        //public string PaymentTerms { get; set; }
        //public string ShipToCode { get; set; }
        //public string PayToCode { get; set; }
        //public string DeliveryLocation { get; set; }
        //public string SalesEmployee { get; set; }
        public string Remarks { get; set; }
        public string Series { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Curentdate { get; set; }
    }

    public class OrderItem
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UOM { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public string Vat { get; set; }
        public string Warehouse { get; set; }
        public string FOC { get; set; }
        public string FOCRemarks { get; set; }
    }
    public class DNPrintResponse
    {
        public bool IsSucess { get; set; }
        public string CnfsMsg { get; set; }
    }

}