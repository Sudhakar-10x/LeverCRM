using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _10xErp.Models
{
    public class SalesOrderViewModel
    {
        //[Required]
        public string CardCode { get; set; }
        //[Required]
        public string CardName { get; set; }
        public string RefNo { get; set; }
        public bool Staff { get; set; }
        public string EmpID { get; set; }
        public string BranchId { get; set; }
        public string Remarks { get; set; }
        public int MyProperty { get; set; }
        public string WhseCode { get; set; }
        public string InvNo { get; set; }
        public DateTime ReqDate { get; set; }
        public List<ItemDetails> ItemDetailsListView { get; set; }
        public List<ItemDetails> lstPrItemDetails { get; set; }
        public string Paymentterms { get; set; }
        public string Delivery { get; set; }
        public string Reason { get; set; }
        public string deliverydate { get; set; }
        public DateTime postingdate { get; set; }
        public DateTime LPODate { get; set; }
        public int series { get; set; }
        public string seriesName { get; set; }
        public string contactPerson { get; set; }
        public string DeliveryTerms { get; set; }
        public string Shiptocode { get; set; }
        public string Billtocode { get; set; }
        public string Salesemp { get; set; }
        public string Owner { get; set; }
        public string CustRefNo { get; set; }
        public int DocNum { get; set; }
        public string deliveryLocation { get; set; }
        public string DocDate { get; set; }
        public string CustomerName { get; set; }
        public decimal? TotalBeforeDiscount { get; set; }
        public decimal? DocDiscount { get; set; }
        public decimal? TotalTax { get; set; }
        public decimal? GrossTotal { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //if (string.IsNullOrWhiteSpace(CardName))
            //{
            //    yield return new ValidationResult("Customer Name is required.", new[] { nameof(CardName) });
            //}
            //if (string.IsNullOrWhiteSpace(CardCode))
            //{
            //    yield return new ValidationResult("Customer Code is required.", new[] { nameof(CardCode) });
            //}

            // Validate the ItemDetails list
            if (ItemDetailsListView != null && ItemDetailsListView.Any())
            {
                for (int i = 0; i < ItemDetailsListView.Count; i++)
                {
                    var item = ItemDetailsListView[i];

                    // Validate ItemName
                    //if (string.IsNullOrWhiteSpace(item.ItemName))
                    //{
                    //    yield return new ValidationResult($"Item Name is required for item {i + 1}.", new[] { $"ItemDetailsListView[{i}].ItemName" });
                    //}

                    // Validate ItemCode
                    if (string.IsNullOrWhiteSpace(item.ItemCode))
                    {
                        yield return new ValidationResult($"Item Code is required for item {i + 1}.", new[] { $"ItemDetailsListView[{i}].ItemCode" });
                    }

                    //// Validate Quantity
                    //if (item.Qty <= 0)
                    //{
                    //    yield return new ValidationResult($"Quantity must be greater than zero for item {i + 1}.", new[] { $"ItemDetailsListView[{i}].Qty" });
                    //}

                    //// Validate Price
                    //if (item.Price <= 0)
                    //{
                    //    yield return new ValidationResult($"Unit Price must be greater than zero for item {i + 1}.", new[] { $"ItemDetailsListView[{i}].Price" });
                    //}

                }
            }
            else
            {
                yield return new ValidationResult("At least one item must be added.", new[] { nameof(ItemDetailsListView) });
            }
        }
    }

    //public class SalesGenReqModel : IValidatableObject
    //{
    //    [Required]
    //    public string CardCode { get; set; }
    //    public string CardName { get; set; }
    //    public string RefNo { get; set; }
    //    public bool Staff { get; set; }
    //    public string EmpID { get; set; }
    //    public string BranchId { get; set; }
    //    public string Remarks { get; set; }
    //    public decimal? DocDiscount { get; set; }
    //    public int MyProperty { get; set; }
    //    public string WhseCode { get; set; }
    //    public string InvNo { get; set; }
    //    public DateTime ReqDate { get; set; }
    //    public List<ItemDetails> ItemDetailsListView { get; set; }
    //    public List<ItemDetails> lstPrItemDetails { get; set; }
    //    public string Paymentterms { get; set; }
    //    public string Delivery { get; set; }
    //    public string Reason { get; set; }
    //    public string deliverydate { get; set; }
    //    public DateTime postingdate { get; set; }
    //    public DateTime LPODate { get; set; }
    //    public int series { get; set; }
    //    public string contactPerson { get; set; }
    //    public string DeliveryTerms { get; set; }
    //    public string Shiptocode { get; set; }
    //    public string Billtocode { get; set; }
    //    public string Salesemp { get; set; }
    //    public string Owner { get; set; }
    //    public string CustRefNo { get; set; }
    //    public int DocNum { get; set; }
    //    public string deliveryLocation { get; set; }
    //    public string DocDate { get; set; }
    //    public string CustomerName { get; set; }

    //    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    //    {
    //        if (string.IsNullOrWhiteSpace(CardCode))
    //        {
    //            yield return new ValidationResult("Customer Code is required.", new[] { nameof(CardCode) });
    //        }

    //        if (ItemDetailsListView == null || !ItemDetailsListView.Any())
    //        {
    //            yield return new ValidationResult("At least one item is required.", new[] { nameof(ItemDetailsListView) });
    //        }
    //        else
    //        {
    //            for (int i = 0; i < ItemDetailsListView.Count; i++)
    //            {
    //                var item = ItemDetailsListView[i];

    //                if (string.IsNullOrWhiteSpace(item.ItemCode))
    //                {
    //                    yield return new ValidationResult($"Item Code is required at row {i + 1}.", new[] { $"ItemDetailsListView[{i}].ItemCode" });
    //                }

    //                if (string.IsNullOrWhiteSpace(item.ItemName))
    //                {
    //                    yield return new ValidationResult($"Item Name is required at row {i + 1}.", new[] { $"ItemDetailsListView[{i}].ItemName" });
    //                }

    //                if (item.Qty <= 0)
    //                {
    //                    yield return new ValidationResult($"Quantity must be greater than 0 at row {i + 1}.", new[] { $"ItemDetailsListView[{i}].Qty" });
    //                }

    //                if (item.Price <= 0)
    //                {
    //                    yield return new ValidationResult($"Unit Price must be greater than 0 at row {i + 1}.", new[] { $"ItemDetailsListView[{i}].Price" });
    //                }
    //            }
    //        }
    //    }


    //}

    //commented on 16th May'25
    //public class SalesGenReqModel : IValidatableObject
    //{
    //    //[Required]
    //    public string CardCode { get; set; }
    //    //[Required]
    //    public string CardName { get; set; }
    //    public string RefNo { get; set; }
    //    public bool Staff { get; set; }
    //    public string EmpID { get; set; }
    //    public string BranchId { get; set; }
    //    public string Remarks { get; set; }
    //    public int MyProperty { get; set; }
    //    public string WhseCode { get; set; }
    //    public string InvNo { get; set; }
    //    public DateTime ReqDate { get; set; }
    //    public List<ItemDetails> ItemDetailsListView { get; set; }
    //    public List<ItemDetails> lstPrItemDetails { get; set; }
    //    public string Paymentterms { get; set; }
    //    public string Delivery { get; set; }
    //    public string Reason { get; set; }
    //    public string deliverydate { get; set; }
    //    public DateTime postingdate { get; set; }
    //    public DateTime LPODate { get; set; }
    //    public int series { get; set; }
    //    public string contactPerson { get; set; }
    //    public string DeliveryTerms { get; set; }
    //    public string Shiptocode { get; set; }
    //    public string Billtocode { get; set; }
    //    public string Salesemp { get; set; }
    //    public string Owner { get; set; }
    //    public string CustRefNo { get; set; }
    //    public int DocNum { get; set; }
    //    public string deliveryLocation { get; set; }
    //    public string DocDate { get; set; }
    //    public string CustomerName { get; set; }
    //    public decimal? TotalBeforeDiscount { get; set; }
    //    public decimal? DocDiscount { get; set; }
    //    public decimal? TotalTax { get; set; }
    //    public decimal? GrossTotal { get; set; }

    //    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    //    {
    //        //if (string.IsNullOrWhiteSpace(CardName))
    //        //{
    //        //    yield return new ValidationResult("Customer Name is required.", new[] { nameof(CardName) });
    //        //}
    //        //if (string.IsNullOrWhiteSpace(CardCode))
    //        //{
    //        //    yield return new ValidationResult("Customer Code is required.", new[] { nameof(CardCode) });
    //        //}

    //        // Validate the ItemDetails list
    //        if (ItemDetailsListView != null && ItemDetailsListView.Any())
    //        {
    //            for (int i = 0; i < ItemDetailsListView.Count; i++)
    //            {
    //                var item = ItemDetailsListView[i];

    //                // Validate ItemName
    //                //if (string.IsNullOrWhiteSpace(item.ItemName))
    //                //{
    //                //    yield return new ValidationResult($"Item Name is required for item {i + 1}.", new[] { $"ItemDetailsListView[{i}].ItemName" });
    //                //}

    //                // Validate ItemCode
    //                if (string.IsNullOrWhiteSpace(item.ItemCode))
    //                {
    //                    yield return new ValidationResult($"Item Code is required for item {i + 1}.", new[] { $"ItemDetailsListView[{i}].ItemCode" });
    //                }

    //                //// Validate Quantity
    //                //if (item.Qty <= 0)
    //                //{
    //                //    yield return new ValidationResult($"Quantity must be greater than zero for item {i + 1}.", new[] { $"ItemDetailsListView[{i}].Qty" });
    //                //}

    //                //// Validate Price
    //                //if (item.Price <= 0)
    //                //{
    //                //    yield return new ValidationResult($"Unit Price must be greater than zero for item {i + 1}.", new[] { $"ItemDetailsListView[{i}].Price" });
    //                //}

    //            }
    //        }
    //        else
    //        {
    //            yield return new ValidationResult("At least one item must be added.", new[] { nameof(ItemDetailsListView) });
    //        }
    //    }

    //}


    public class SalesInvoiceModel : IValidatableObject
    {
        public string seriesName { get; set; }
        public int series { get; set; }
        //[Required]
        public string CardCode { get; set; }
        //[Required]
        public string CardName { get; set; }
        public string RefNo { get; set; }
        public bool Staff { get; set; }
        public string EmpID { get; set; }
        public string BranchId { get; set; }
        public string Remarks { get; set; }
        public int MyProperty { get; set; }
        public string WhseCode { get; set; }
        public string InvNo { get; set; }
        public DateTime ReqDate { get; set; }
        public List<ItemDetails> ItemDetailsListView { get; set; }
        public List<ItemDetails> lstPrItemDetails { get; set; }
        public string Paymentterms { get; set; }
        public string Delivery { get; set; }
        public string Reason { get; set; }
        public string deliverydate { get; set; }
        public DateTime postingdate { get; set; }
        public DateTime LPODate { get; set; }
        //public int series { get; set; }
        public string contactPerson { get; set; }
        public string DeliveryTerms { get; set; }
        public string Shiptocode { get; set; }
        public string Billtocode { get; set; }
        public string Salesemp { get; set; }
        public string Owner { get; set; }
        public string CustRefNo { get; set; }
        public int DocNum { get; set; }
        public string deliveryLocation { get; set; }
        public string DocDate { get; set; }
        public string CustomerName { get; set; }
        public string Prescription { get; set; }
        public string PrescBy { get; set; }
        public string Patient { get; set; }

        public string InsCard { get; set; }
        public string InsPer { get; set; }
        public string InsAmt { get; set; }

        public string InsCompany { get; set; }
        public decimal? TotalBeforeDiscount { get; set; }
        public decimal? DocDiscount { get; set; }
        public decimal? TotalTax { get; set; }
        public decimal? GrossTotal { get; set; }
        public decimal? FinalTotal { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //if (string.IsNullOrWhiteSpace(CardName))
            //{
            //    yield return new ValidationResult("Customer Name is required.", new[] { nameof(CardName) });
            //}
            //if (string.IsNullOrWhiteSpace(CardCode))
            //{
            //    yield return new ValidationResult("Customer Code is required.", new[] { nameof(CardCode) });
            //}

            // Validate the ItemDetails list
            if (ItemDetailsListView != null && ItemDetailsListView.Any())
            {
                for (int i = 0; i < ItemDetailsListView.Count; i++)
                {
                    var item = ItemDetailsListView[i];

                    // Validate ItemName
                    //if (string.IsNullOrWhiteSpace(item.ItemName))
                    //{
                    //    yield return new ValidationResult($"Item Name is required for item {i + 1}.", new[] { $"ItemDetailsListView[{i}].ItemName" });
                    //}

                    // Validate ItemCode
                    if (string.IsNullOrWhiteSpace(item.ItemCode))
                    {
                        yield return new ValidationResult($"Item Code is required for item {i + 1}.", new[] { $"ItemDetailsListView[{i}].ItemCode" });
                    }

                    //// Validate Quantity
                    //if (item.Qty <= 0)
                    //{
                    //    yield return new ValidationResult($"Quantity must be greater than zero for item {i + 1}.", new[] { $"ItemDetailsListView[{i}].Qty" });
                    //}

                    //// Validate Price
                    //if (item.Price <= 0)
                    //{
                    //    yield return new ValidationResult($"Unit Price must be greater than zero for item {i + 1}.", new[] { $"ItemDetailsListView[{i}].Price" });
                    //}

                }
            }
            else
            {
                yield return new ValidationResult("At least one item must be added.", new[] { nameof(ItemDetailsListView) });
            }
        }

    }

}