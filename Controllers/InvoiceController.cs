using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _10xErp.Models;
using _10xErp.Helpers;
using System.Data.SqlClient;
using System.Configuration;
using _10xErp.ServiceReferenceLayer.SAPB1;
using Newtonsoft.Json;

namespace _10xErp.Controllers
{
    public class InvoiceController : Controller
    {
        // GET: Invoice
        private readonly DataHelper objHlpr = new DataHelper(); // Changed to private readonly instance  

        public static List<SalesInvoiceModel> Orders = new List<SalesInvoiceModel>(); // In-memory store  

        //public static List<DeliveryInvoiceModel> DOrders = new List<DeliveryInvoiceModel>(); // In-memory store  for Delivery Screen

        public ActionResult Index()
        {
            SalesInvoiceModel oSalesGenReqModel = new SalesInvoiceModel();

            ViewBag.Current = "SalesOrder";
            return View(oSalesGenReqModel);
        }

        //public ActionResult IndexD() //for Delivery screen
        //{
        //    DeliveryInvoiceModel oDeliveryGenReqModel = new DeliveryInvoiceModel();

        //    ViewBag.Current = "Delivery";
        //    return View(oDeliveryGenReqModel);
        //}


        [HttpGet]
        public ActionResult InvoiceCreate()
        {
            ViewBag.UserName = User.Identity.Name;
            SalesInvoiceModel oSalesInvoiceViewModel = new SalesInvoiceModel();
            oSalesInvoiceViewModel.ItemDetailsListView = new List<ItemDetails>();
            try
            {

                oSalesInvoiceViewModel.ItemDetailsListView = new List<ItemDetails>();
                //oSalesInvoiceViewModel.SalesEmployeeList = new SelectList(GetSalesEmployeeList(), "Value", "Text");
                LoadDropdowns(string.Empty);

                oSalesInvoiceViewModel.ReqDate = DateTime.Today;
                oSalesInvoiceViewModel.postingdate = DateTime.Today;
                oSalesInvoiceViewModel.LPODate = DateTime.Today;
                oSalesInvoiceViewModel.series = Convert.ToInt32(ViewBag.seriesId);
                oSalesInvoiceViewModel.DocNum = Convert.ToInt32(ViewBag.DefaultDocNumber);

                //ViewBag.ItemList = GetAllItems();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            ViewBag.Current = "SalesOrder";

            return View(oSalesInvoiceViewModel);
        }

        //public ActionResult InvoiceCreate_D() // for Delivery screen
        //{
        //    ViewBag.UserName = User.Identity.Name;
        //    DeliveryInvoiceModel oDeliveryGenReqModel = new DeliveryInvoiceModel();
        //    oDeliveryGenReqModel.ItemDetailsListView = new List<ItemDetails>();
        //    try
        //    {

        //        oDeliveryGenReqModel.ItemDetailsListView = new List<ItemDetails>();
        //        //oSalesInvoiceViewModel.SalesEmployeeList = new SelectList(GetSalesEmployeeList(), "Value", "Text");
        //        LoadDropdowns();

        //        oDeliveryGenReqModel.ReqDate = DateTime.Today;
        //        oDeliveryGenReqModel.postingdate = DateTime.Today;
        //        oDeliveryGenReqModel.LPODate = DateTime.Today;
        //        oDeliveryGenReqModel.series = Convert.ToInt32(ViewBag.seriesId);
        //        oDeliveryGenReqModel.DocNum = Convert.ToInt32(ViewBag.DefaultDocNumber);

        //        //ViewBag.ItemList = GetAllItems();
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.Message.ToString();
        //    }
        //    ViewBag.Current = "Delivery";

        //    return View(oDeliveryGenReqModel);
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InvoiceCreate(SalesInvoiceModel oInvoiceDetails, string custCode)
        {
            ConfirmMsg cnfMsg = new ConfirmMsg();
            bool logout = false;
            string docsnum;
            ServiceLayerServices currentOdataService;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(oInvoiceDetails.CardCode))
                    {
                        if (oInvoiceDetails.ItemDetailsListView != null)
                        {

                            string strCurrentServiceURL = ConfigurationManager.AppSettings["CurrentServiceURL"]; 

                            currentOdataService = (ServiceLayerServices)System.Web.HttpContext.Current.Application["sapAppGlobal"];

                            currentOdataService = LoginLogoutAction(true);

                            Uri service = new Uri(strCurrentServiceURL);

                            currentOdataService.InitServiceContainer(strCurrentServiceURL);
                            Document oSeleInvReqst = new Document();

                            oSeleInvReqst.Requester = @Session["UserName"].ToString();
                            oSeleInvReqst.ReqType = 12;
                            // EmpID, Manag
                            oSeleInvReqst.DocObjectCode = "13";
                            oSeleInvReqst.SeriesString = "";
                            oSeleInvReqst.Series = oInvoiceDetails.series;
                            oSeleInvReqst.DocNum = oInvoiceDetails.DocNum;
                            oSeleInvReqst.CardCode = oInvoiceDetails.CardCode;
                            oSeleInvReqst.RequriedDate = Convert.ToDateTime(oInvoiceDetails.ReqDate);
                            oSeleInvReqst.DocDueDate = Convert.ToDateTime(oInvoiceDetails.ReqDate);
                            oSeleInvReqst.Comments = @Session["UserName"] + " " + oInvoiceDetails.Remarks;
                            oSeleInvReqst.DocDate = Convert.ToDateTime(oInvoiceDetails.postingdate);
                            oSeleInvReqst.TaxDate = Convert.ToDateTime(oInvoiceDetails.LPODate);
                            //oSeleInvReqst.DocDueDate = DateTime.Today.Date;
                            //oSeleInvReqst.TaxDate = DateTime.Today.Date;
                            oSeleInvReqst.NumAtCard = oInvoiceDetails.RefNo;
                            oSeleInvReqst.U_DelLocation = oInvoiceDetails.deliveryLocation;
                            oSeleInvReqst.U_Prescri = oInvoiceDetails.Prescription;
                            oSeleInvReqst.U_PrescriBy = oInvoiceDetails.PrescBy;
                            oSeleInvReqst.U_InsComp = oInvoiceDetails.InsCompany;
                            oSeleInvReqst.U_InsCrdNo = oInvoiceDetails.InsCard;
                            oSeleInvReqst.U_InsPer = oInvoiceDetails.InsPer;
                            oSeleInvReqst.U_InsAmt = oInvoiceDetails.InsAmt;
                            oSeleInvReqst.U_Contact = oInvoiceDetails.Patient;

                            oSeleInvReqst.U_IsFrmPrtal = "Y";
                            oSeleInvReqst.ContactPersonCode = !string.IsNullOrEmpty(oInvoiceDetails.contactPerson) ? 0 : Convert.ToInt32(oInvoiceDetails.contactPerson);
                            //oSeleInvReqst.U_PDTUSER = oInvoiceDetails.EmpID;

                            //oSeleInvReqst.U_LocationName = getLocationName(oInvoiceDetails.WhseCode);
                            //oSeleInvReqst.U_UserName = getEmpName(oInvoiceDetails.EmpID);
                            //DocSeries oDocSeries = getSeriesInfo("Sales Order", oInvoiceDetails.WhseCode);

                            //if (oDocSeries.SeriesId != null)
                            //{
                            //    oSeleQtnReqst.SeriesString = oDocSeries.SeriesName;
                            //    oSeleQtnReqst.Series = oDocSeries.SeriesId;
                            //}

                            //oSeleInvReqst.Series = 8;
                            var seObj = GetSalesEmployeeInfoByUId(oInvoiceDetails.EmpID);
                            oSeleInvReqst.BPL_IDAssignedToInvoice = Convert.ToInt32(oInvoiceDetails.BranchId);
                            oSeleInvReqst.PaymentGroupCode = Convert.ToInt32(oInvoiceDetails.Paymentterms);
                            //oSeleInvReqst.NumAtCard = oInvoiceDetails.CustRefNo;
                            oSeleInvReqst.DiscountPercent = Convert.ToDouble(oInvoiceDetails.DocDiscount);
                            oSeleInvReqst.SalesPersonCode = seObj.AssignedEmpId;
                            oSeleInvReqst.ShipToCode = oInvoiceDetails.Shiptocode;
                            oSeleInvReqst.PayToCode = oInvoiceDetails.Billtocode;
                            oSeleInvReqst.SalesPersonCode = Convert.ToInt32(oInvoiceDetails.Salesemp);
                            DocumentLine oLine = null;
                            int iLine = 0;
                            foreach (ItemDetails itm in oInvoiceDetails.ItemDetailsListView)
                            {
                                oLine = new DocumentLine();

                                oLine.ItemCode = itm.ItemCode;
                                oLine.LineVendor = oInvoiceDetails.CardCode;
                                oLine.Quantity = Convert.ToDouble(itm.Qty);

                                oLine.DiscountPercent = Convert.ToDouble(itm.DisPer);

                                oLine.Price = Convert.ToDouble(itm.Price);
                                oLine.VatGroup = itm.VatGrpCode;


                                oLine.UoMCode = itm.UomName;
                                oLine.UoMEntry = itm.UomEntry;
                                oLine.WarehouseCode = itm.Warehouse;
                                oLine.LineNum = iLine;
                                decimal itmPrice = Convert.ToDecimal(itm.Price);
                                oLine.U_FOC = (itm.foc == "Yes") ? "Y" : "N";
                                oLine.U_FocRemark = itm.focremarks;


                                BatchNumber objBatch = new BatchNumber();

                                objBatch.BaseLineNumber = iLine;
                                //objBatch.Quantity = Math.Round(Convert.ToDouble(Convert.ToDouble(itm.Qty) * BaseQty), 3, MidpointRounding.AwayFromZero);// Convert.ToDouble(itmBsUomQty));
                                objBatch.Quantity = Math.Round(Convert.ToDouble(itm.Qty), 3, MidpointRounding.AwayFromZero);// Convert.ToDouble(itmBsUomQty));
                                objBatch.BatchNumberProperty = Convert.ToString(itm.Batches);

                                oLine.BatchNumbers.Add(objBatch);

                                //if (itm.DisPer > 0)
                                //{
                                //    itmPrice = itmPrice - (itmPrice * (itm.DisPer / 100));
                                //}

                                iLine++;

                                //oLine.U_normalqty = Convert.ToDouble(itm.Qty);
                                //oLine.U_Addbarcode = itm.Barcode;
                                oLine.UnitPrice = Convert.ToDouble(itm.Price);
                                oLine.DiscountPercent = Convert.ToDouble(itm.DisPer);

                                
                                oSeleInvReqst.DocumentLines.Add(oLine);


                            }

                            Document doc = null;


                            doc = currentOdataService.AddSalesInvoice(oSeleInvReqst);
                            //var msg = "<div style='width:800px;margin:auto;padding:40px;border:1px solid #abd1b8;'><h2>10XSF</h2><h3>Hello, " + oInvoiceDetails.CardName + "</h3><h4>Your sales order has been successfully created!</h4><div style='background:#abd1b8;padding:20px 10px;'><p>Sales Order No: <b style='color:green'>" + doc.DocNum + "</b></p></div></div>";
                            //var email = this.getCustomerEmail(oInvoiceDetails.CardCode);
                            //SendCodeToEmail("raju.m432@gmail.com", msg);
                            //UpdateRequestTime();
                            cnfMsg.IsSucess = true;
                            cnfMsg.CnfsMsg = "Sales Invoice  Document Number :" + doc.DocNum + " Created Sucessfully";
                            TempData["SuccessMessage"] = "Sales Invoice  Document Number :" + doc.DocNum + " Created Sucessfully";
                            return RedirectToAction("InvoiceCreate");
                        }
                        else
                            TempData["ErrorMessage"] = "Make sure correct data to generate sales order !!";
                    }
                    //TempData["ErrorMessage"] = "Please Make sure to select customer before generating sales order !!";
                }
                else
                {
                    oInvoiceDetails.CardCode = oInvoiceDetails.CardCode ?? Request.Form["CustomerID"];
                    oInvoiceDetails.CardName = oInvoiceDetails.CardName ?? Request.Form["customerAutocomplete"];

                    custCode = oInvoiceDetails.CardCode ?? Request.Form["CustomerID"];

                    LoadDropdowns(custCode); // Load dropdowns again so page doesn't break

                    if (oInvoiceDetails.ItemDetailsListView == null)
                        oInvoiceDetails.ItemDetailsListView = new List<ItemDetails>();
                    Utilities.SetResultMessage("Make sure correct data to generate sales invoice !!");
                    TempData["ErrorMessage"] = "Make sure correct data to generate sales invoice !!";
                }
            }
            catch (Exception ex)
            {

                Utilities.SetResultMessage("SalesInvoice" + ex.Message.ToString());

                string Excpn = "";
                if (ex.InnerException != null)
                {
                    Utilities.SetResultMessage("SalesInvoice" + ex.Message.ToString());
                    Root rObjt = JsonConvert.DeserializeObject<Root>(ex.InnerException.Message);
                    Excpn = rObjt.error.message.value;
                    if (Excpn == "No matching records found (ODBC -2028)")
                    {
                        cnfMsg.IsSucess = true;
                        Excpn = "Sales Invoice went for Approval Successfully";
                        //cnfMsg.CnfsMsg = "Sales order went for Approval Successfully";
                        cnfMsg.Ref = "APPROVAL";
                        TempData["SuccessMessage"] = "Sales Invoice went for Approval Successfully";
                        return RedirectToAction("InvoiceCreate");
                    }
                    else
                    {
                        cnfMsg.IsSucess = false;
                        Excpn = rObjt.error.message.value;
                        TempData["ErrorMessage"] = rObjt.error.message.value;
                    }

                    cnfMsg.CnfsMsg = Excpn;
                }
                else
                {
                    Excpn = ex.Message.ToString();
                    TempData["ErrorMessage"] = ex.Message.ToString();

                }
            }
            //return Json(cnfMsg, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Create");
            oInvoiceDetails.CardCode = oInvoiceDetails.CardCode ?? Request.Form["CustomerID"];
            oInvoiceDetails.CardName = oInvoiceDetails.CardName ?? Request.Form["customerAutocomplete"];

            custCode = oInvoiceDetails.CardCode ?? Request.Form["CustomerID"];

            LoadDropdowns(custCode); // Load dropdowns again so page doesn't break

            if (oInvoiceDetails.ItemDetailsListView == null)
                oInvoiceDetails.ItemDetailsListView = new List<ItemDetails>();
            return View(oInvoiceDetails);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult InvoiceCreate_D(DeliveryInvoiceModel oInvoiceDetails)
        //{
        //    ConfirmMsg cnfMsg = new ConfirmMsg();
        //    bool logout = false;
        //    string docsnum;
        //    ServiceLayerServices currentOdataService;
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (oInvoiceDetails.ItemDetailsListView != null)
        //            {

        //                string strCurrentServiceURL = ConfigurationManager.AppSettings["CurrentServiceURL"]; //textBox_ServiceURL.Text;

        //                currentOdataService = (ServiceLayerServices)System.Web.HttpContext.Current.Application["sapAppGlobal"];

        //                currentOdataService = LoginLogoutAction(true);

        //                Uri service = new Uri(strCurrentServiceURL);

        //                currentOdataService.InitServiceContainer(strCurrentServiceURL);
        //                Document oDelvryReqst = new Document();

        //                if (oInvoiceDetails.Staff)
        //                {
        //                    oDelvryReqst.ReqType = 171;  //171 -Employee, 12 --User(Supplier)
        //                    oDelvryReqst.Requester = oInvoiceDetails.EmpID;
        //                }
        //                else
        //                {
        //                    oDelvryReqst.Requester = "manager";
        //                    oDelvryReqst.ReqType = 12;
        //                }

        //                // EmpID, Manag
        //                oDelvryReqst.DocObjectCode = "17";
        //                //oPurReqst.DocType = "dDocument_Items";
        //                oDelvryReqst.SeriesString = "";
        //                oDelvryReqst.Series = oInvoiceDetails.series;
        //                oDelvryReqst.DocNum = oInvoiceDetails.DocNum;
        //                oDelvryReqst.CardCode = oInvoiceDetails.CardCode;
        //                oDelvryReqst.RequriedDate = Convert.ToDateTime(oInvoiceDetails.ReqDate);
        //                oDelvryReqst.DocDueDate = Convert.ToDateTime(oInvoiceDetails.ReqDate);
        //                oDelvryReqst.Comments = @Session["UserName"] + " " + oInvoiceDetails.Remarks;
        //                oDelvryReqst.DocDate = Convert.ToDateTime(oInvoiceDetails.postingdate);
        //                //oSeleInvReqst.DocDueDate = DateTime.Today.Date;
        //                //oSeleInvReqst.TaxDate = DateTime.Today.Date;
        //                oDelvryReqst.NumAtCard = oInvoiceDetails.RefNo;
        //                oDelvryReqst.U_DelLocation = oInvoiceDetails.deliveryLocation;
        //                oDelvryReqst.ContactPersonCode = Convert.ToInt32(oInvoiceDetails.contactPerson);
        //                //oSeleInvReqst.U_PDTUSER = oInvoiceDetails.EmpID;

        //                //oSeleInvReqst.U_LocationName = getLocationName(oInvoiceDetails.WhseCode);
        //                //oSeleInvReqst.U_UserName = getEmpName(oInvoiceDetails.EmpID);
        //                //DocSeries oDocSeries = getSeriesInfo("Sales Order", oInvoiceDetails.WhseCode);

        //                //if (oDocSeries.SeriesId != null)
        //                //{
        //                //    oSeleQtnReqst.SeriesString = oDocSeries.SeriesName;
        //                //    oSeleQtnReqst.Series = oDocSeries.SeriesId;
        //                //}

        //                //oSeleInvReqst.Series = 8;
        //                var seObj = GetSalesEmployeeInfoByUId(oInvoiceDetails.EmpID);
        //                oDelvryReqst.BPL_IDAssignedToInvoice = Convert.ToInt32(oInvoiceDetails.BranchId);
        //                oDelvryReqst.PaymentGroupCode = Convert.ToInt32(oInvoiceDetails.Paymentterms);
        //                //oSeleInvReqst.NumAtCard = oInvoiceDetails.CustRefNo;
        //                oDelvryReqst.DiscountPercent = Convert.ToDouble(oInvoiceDetails.DocDiscount);
        //                oDelvryReqst.SalesPersonCode = seObj.AssignedEmpId;
        //                oDelvryReqst.ShipToCode = oInvoiceDetails.Shiptocode;
        //                oDelvryReqst.PayToCode = oInvoiceDetails.Billtocode;
        //                oDelvryReqst.SalesPersonCode = Convert.ToInt32(oInvoiceDetails.Salesemp);
        //                DocumentLine oLine = null;
        //                int iLine = 0;
        //                foreach (ItemDetails itm in oInvoiceDetails.ItemDetailsListView)
        //                {
        //                    oLine = new DocumentLine();

        //                    oLine.ItemCode = itm.ItemCode;
        //                    oLine.LineVendor = oInvoiceDetails.CardCode;
        //                    oLine.Quantity = Convert.ToDouble(itm.Qty);

        //                    oLine.DiscountPercent = Convert.ToDouble(itm.DisPer);

        //                    oLine.Price = Convert.ToDouble(itm.Price);
        //                    oLine.VatGroup = itm.VatGrpCode;


        //                    oLine.UoMCode = itm.UomName;
        //                    oLine.UoMEntry = itm.UomEntry;
        //                    oLine.WarehouseCode = itm.Warehouse;
        //                    oLine.LineNum = iLine;
        //                    decimal itmPrice = Convert.ToDecimal(itm.Price);
        //                    oLine.U_FOC = (itm.foc == "Yes") ? "Y" : "N";
        //                    oLine.U_FocRemark = itm.focremarks;

        //                    //if (itm.DisPer > 0)
        //                    //{
        //                    //    itmPrice = itmPrice - (itmPrice * (itm.DisPer / 100));
        //                    //}

        //                    iLine++;

        //                    //oLine.U_normalqty = Convert.ToDouble(itm.Qty);
        //                    //oLine.U_Addbarcode = itm.Barcode;
        //                    oLine.UnitPrice = Convert.ToDouble(itm.Price);
        //                    oLine.DiscountPercent = Convert.ToDouble(itm.DisPer);
        //                    oDelvryReqst.DocumentLines.Add(oLine);


        //                }

        //                Document doc = null;


        //                doc = currentOdataService.AddSalesOrder(oDelvryReqst);
        //                var msg = "<div style='width:800px;margin:auto;padding:40px;border:1px solid #abd1b8;'><h2>10XSF</h2><h3>Hello, " + oInvoiceDetails.CardName + "</h3><h4>Your Delivery has been successfully InvoiceCreated!</h4><div style='background:#abd1b8;padding:20px 10px;'><p>Delivery No: <b style='color:green'>" + doc.DocNum + "</b></p></div></div>";
        //                //var email = this.getCustomerEmail(oInvoiceDetails.CardCode);
        //                //SendCodeToEmail("raju.m432@gmail.com", msg);
        //                //UpdateRequestTime();
        //                cnfMsg.IsSucess = true;
        //                cnfMsg.CnfsMsg = "Delivery  Document Number :" + doc.DocNum + " Created Sucessfully";

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        Utilities.SetResultMessage("Delivery" + ex.Message.ToString());

        //        string Excpn = "";
        //        if (ex.InnerException != null)
        //        {
        //            Utilities.SetResultMessage("Delivery" + ex.Message.ToString());
        //            //Need to uncomment this code
        //            //RootObject rObjt = JsonConvert.DeserializeObject<RootObject>(ex.InnerException.Message);
        //            //Excpn = rObjt.error.message.value;
        //            //if (Excpn == "No matching records found (ODBC -2028)")
        //            //{

        //            //    cnfMsg.IsSucess = true;
        //            //    Excpn = "Sales order went for Approval Successfully";
        //            //    cnfMsg.Ref = "APPROVAL";
        //            //}
        //            //else
        //            //{
        //            //    cnfMsg.IsSucess = false;
        //            //    Excpn = rObjt.error.message.value;
        //            //}
        //            //till this



        //            //string Excpn = "";
        //            //if (ex.InnerException != null)
        //            //{
        //            //    Utilities.SetResultMessage(ex.InnerException.ToString());
        //            //    //RootObject rObjt = JsonConvert.DeserializeObject<RootObject>(ex.InnerException.Message);
        //            //    //Excpn = rObjt.error.message.value;

        //            //    cnfMsg.IsSucess = false;
        //            //}
        //            //else
        //            //{
        //            //    Excpn = ex.Message.ToString();
        //            //}
        //            //LoadDropdowns();
        //            //if (oInvoiceDetails.Items.Count == null)
        //            //{
        //            //    oInvoiceDetails.ItemDetailsListView = new List<ItemDetails>();
        //            //}

        //            cnfMsg.CnfsMsg = Excpn;
        //        }
        //        else
        //        {
        //            Excpn = ex.Message.ToString();
        //        }
        //    }
        //    //return Json(cnfMsg, JsonRequestBehavior.AllowGet);
        //    return RedirectToAction("InvoiceCreate");
        //}


        //public ActionResult InvoiceCreate(SalesGenReqModel oInvoiceDetails)
        //{
        //    ConfirmMsg cnfMsg = new ConfirmMsg();
        //    bool logout = false;
        //    string docsnum;
        //    ServiceLayerServices currentOdataService;
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (oInvoiceDetails.ItemDetailsListView != null)
        //            {

        //                string strCurrentServiceURL = ConfigurationManager.AppSettings["CurrentServiceURL"]; //textBox_ServiceURL.Text;

        //                currentOdataService = (ServiceLayerServices)System.Web.HttpContext.Current.Application["sapAppGlobal"];

        //                currentOdataService = LoginLogoutAction(true);

        //                Uri service = new Uri(strCurrentServiceURL);

        //                currentOdataService.InitServiceContainer(strCurrentServiceURL);
        //                Document oSeleInvReqst = new Document();

        //                if (oInvoiceDetails.Staff)
        //                {
        //                    oSeleInvReqst.ReqType = 171;  //171 -Employee, 12 --User(Supplier)
        //                    oSeleInvReqst.Requester = oInvoiceDetails.EmpID;
        //                }
        //                else
        //                {
        //                    oSeleInvReqst.Requester = "manager";
        //                    oSeleInvReqst.ReqType = 12;
        //                }

        //                // EmpID, Manag
        //                oSeleInvReqst.DocObjectCode = "17";
        //                //oPurReqst.DocType = "dDocument_Items";
        //                oSeleInvReqst.SeriesString = "";
        //                oSeleInvReqst.Series = oInvoiceDetails.series;
        //                oSeleInvReqst.DocNum = oInvoiceDetails.DocNum;
        //                oSeleInvReqst.CardCode = oInvoiceDetails.CardCode;
        //                //oSeleInvReqst.RequriedDate = Convert.ToDateTime(oInvoiceDetails.ReqDate);
        //                //oSeleInvReqst.DocDueDate = Convert.ToDateTime(oInvoiceDetails.ReqDate);
        //                oSeleInvReqst.Comments = oInvoiceDetails.Remarks;
        //                oSeleInvReqst.DocDate = Convert.ToDateTime(oInvoiceDetails.DocDate);
        //                //oSeleInvReqst.CreationDate = DateTime.Today;
        //                //oSeleInvReqst.TaxDate = DateTime.Today;
        //                //oSeleInvReqst.Doc = "USD";
        //                oSeleInvReqst.NumAtCard = oInvoiceDetails.RefNo;
        //                oSeleInvReqst.U_DelLocation = oInvoiceDetails.deliveryLocation;
        //                oSeleInvReqst.ContactPersonCode = Convert.ToInt32(oInvoiceDetails.contactPerson);

        //                var seObj = GetSalesEmployeeInfoByUId(oInvoiceDetails.EmpID);
        //                oSeleInvReqst.BPL_IDAssignedToInvoice = Convert.ToInt32(oInvoiceDetails.BranchId);
        //                oSeleInvReqst.PaymentGroupCode = Convert.ToInt32(oInvoiceDetails.Paymentterms);
        //                oSeleInvReqst.DiscountPercent = Convert.ToDouble(oInvoiceDetails.DocDiscount);
        //                oSeleInvReqst.SalesPersonCode = seObj.AssignedEmpId;
        //                //oSeleInvReqst.DocDate = oInvoiceDetails.ReqDate;
        //                oSeleInvReqst.DocDueDate = Convert.ToDateTime(oInvoiceDetails.deliverydate);
        //                oSeleInvReqst.ShipToCode = oInvoiceDetails.Shiptocode;
        //                oSeleInvReqst.PayToCode = oInvoiceDetails.Billtocode;
        //                oSeleInvReqst.TaxDate = oInvoiceDetails.ReqDate;
        //                oSeleInvReqst.SalesPersonCode = Convert.ToInt32(oInvoiceDetails.Salesemp);
        //                DocumentLine oLine = null;
        //                int iLine = 0;
        //                foreach (ItemDetails itm in oInvoiceDetails.ItemDetailsListView)
        //                {
        //                    oLine = new DocumentLine();

        //                    oLine.ItemCode = itm.ItemCode;
        //                    oLine.LineVendor = oInvoiceDetails.CardCode;
        //                    oLine.Quantity = Convert.ToDouble(itm.Qty);

        //                    oLine.DiscountPercent = Convert.ToDouble(itm.DisPer);

        //                    oLine.Price = Convert.ToDouble(itm.Price);
        //                    oLine.VatGroup = itm.VatGrpCode;


        //                    oLine.UoMCode = itm.UomName;
        //                    oLine.UoMEntry = itm.UomEntry;
        //                    oLine.WarehouseCode = itm.Warehouse;
        //                    oLine.LineNum = iLine;
        //                    decimal itmPrice = Convert.ToDecimal(itm.Price);
        //                    oLine.U_FOC = (itm.foc == "Yes") ? "Y" : "N";
        //                    oLine.U_FocRemark = itm.focremarks;

        //                    iLine++;

        //                    oLine.UnitPrice = Convert.ToDouble(itm.Price);
        //                    oLine.DiscountPercent = Convert.ToDouble(itm.DisPer);
        //                    oSeleInvReqst.DocumentLines.Add(oLine);


        //                }

        //                Document doc = null;


        //                doc = currentOdataService.AddSalesOrder(oSeleInvReqst);
        //                var msg = "<div style='width:800px;margin:auto;padding:40px;border:1px solid #abd1b8;'><h2>10XSF</h2><h3>Hello, " + oInvoiceDetails.CardName + "</h3><h4>Your sales order has been successfully created!</h4><div style='background:#abd1b8;padding:20px 10px;'><p>Sales Order No: <b style='color:green'>" + doc.DocNum + "</b></p></div></div>";
        //                //var email = this.getCustomerEmail(oInvoiceDetails.CardCode);
        //                //SendCodeToEmail("raju.m432@gmail.com", msg);
        //                //UpdateRequestTime();
        //                cnfMsg.IsSucess = true;
        //                cnfMsg.CnfsMsg = "Sales Order  Document Number :" + doc.DocNum + " Created Sucessfully";

        //            }
        //        }
        //        else
        //        {
        //            LoadDropdowns();
        //            if (oInvoiceDetails.ItemDetailsListView == null)
        //            {
        //                oInvoiceDetails.ItemDetailsListView = new List<ItemDetails>();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        Utilities.SetResultMessage("SalesOrder" + ex.Message.ToString());

        //        string Excpn = "";
        //        if (ex.InnerException != null)
        //        {
        //            Utilities.SetResultMessage(ex.InnerException.ToString());
        //            //RootObject rObjt = JsonConvert.DeserializeObject<RootObject>(ex.InnerException.Message);
        //            //Excpn = rObjt.error.message.value;

        //            cnfMsg.IsSucess = false;
        //        }
        //        else
        //        {
        //            Excpn = ex.Message.ToString();
        //        }
        //        LoadDropdowns();
        //        if (oInvoiceDetails.ItemDetailsListView == null)
        //        {
        //            oInvoiceDetails.ItemDetailsListView = new List<ItemDetails>();
        //        }

        //        cnfMsg.CnfsMsg = Excpn;
        //    }
        //    //return RedirectToAction("Index");
        //    return View(oInvoiceDetails);
        //}

        public AssignedEmpModel GetSalesEmployeeInfoByUId(string empCode)

        {

            List<AssignedEmpModel> lstActTypes = new List<AssignedEmpModel>();

            AssignedEmpModel valModel = new AssignedEmpModel();

            try

            {


                string strSQL = "SELECT T2.\"SlpCode\", T2.\"SlpName\",T2.\"U_PDTUSERID\"  FROM OUSR T0  INNER JOIN OUDG T1 ON T0.\"DfltsGroup\" = T1.\"Code\" INNER JOIN OSLP T2 ON T1.\"SalePerson\" = T2.\"SlpCode\" where T0.\"USER_CODE\"='" + empCode + "'";

                DataSet dsData = objHlpr.getDataSet(strSQL);

                if (dsData.Tables[0].Rows.Count > 0)

                {

                    valModel.AssignedEmpId = Convert.ToInt32(dsData.Tables[0].Rows[0]["SlpCode"]);

                    valModel.AssignedEmpName = dsData.Tables[0].Rows[0]["SlpName"].ToString();

                    valModel.Memo = dsData.Tables[0].Rows[0]["SlpName"].ToString();

                    var pdtuser = dsData.Tables[0].Rows[0]["U_PDTUSERID"].ToString();

                    valModel.PDTUSERID = pdtuser.TrimStart('0');

                    valModel.EmpName = dsData.Tables[0].Rows[0]["SlpName"].ToString(); //dsData.Tables[0].Rows[0]["firstName"].ToString() + " " + dsData.Tables[0].Rows[0]["middleName"].ToString() + " " + dsData.Tables[0].Rows[0]["lastName"].ToString();

                    // lstActTypes.Add(valModel);

                }

            }

            catch (Exception ex)

            {

            }

            return valModel;//Json(lstActTypes, JsonRequestBehavior.AllowGet);

        }

        public ServiceLayerServices MainLoginLogoutAction(bool bLogin = true)
        {
            string strCurrentServiceURL = ConfigurationManager.AppSettings["CurrentServiceURL"];
            SboCred currentConnectionInfo = new SboCred();
            ServiceLayerServices currentOdataService = new ServiceLayerServices();
            try
            {



                // SAPUserInfo objSap = getSAPUserInfo(macId);
                //currentConnectionInfo.CompanyDB = objSap.CompanyDB;
                //currentConnectionInfo.UserName = objSap.UserName;
                //currentConnectionInfo.Password = objSap.Password;

                currentConnectionInfo.CompanyDB = ConfigurationManager.AppSettings["CompanyDB"];
                currentConnectionInfo.UserName = ConfigurationManager.AppSettings["UserName"];
                currentConnectionInfo.Password = ConfigurationManager.AppSettings["Password"];
                //currentConnectionInfo.UserName = (Session["SUser"] != "" || Session["SUser"]!=null ) ? Session["SUser"].ToString() : ConfigurationManager.AppSettings["UserName"];
                //currentConnectionInfo.Password = (Session["SPass"] != "" || Session["SPass"] != null) ? Session["SPass"].ToString() : ConfigurationManager.AppSettings["Password"];

                if (bLogin)
                {

                    currentOdataService.InitServiceContainer(strCurrentServiceURL);

                    if (!currentConnectionInfo.IsValid())
                    {
                        Utilities.SetResultMessage("Make sure correct user name, password and company database provided");

                    }

                    B1Session session = currentOdataService.LoginServer(currentConnectionInfo);

                    if (null != session)
                    {
                        currentOdataService.loginTrnsTime = DateTime.Now;

                        string strDisplay = currentOdataService.GetRequestHeaders() + currentOdataService.GetResponsetHeaders() + Newtonsoft.Json.JsonConvert.SerializeObject(session, Formatting.Indented);



                        Utilities.SetResultMessage(strDisplay);


                    }
                    else
                    {



                        Utilities.SetResultMessage("Failed to login, please make sure server is running and the credentials are correct.");
                    }
                }
                else
                {

                    currentOdataService.LogoutServer();


                    Utilities.SetResultMessage("Logout from service successfully");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return currentOdataService;
        }
        public ServiceLayerServices LoginLogoutAction(bool bLogin = true)
        {
            string strCurrentServiceURL = ConfigurationManager.AppSettings["CurrentServiceURL"];
            SboCred currentConnectionInfo = new SboCred();
            ServiceLayerServices currentOdataService = new ServiceLayerServices();
            try
            {



                // SAPUserInfo objSap = getSAPUserInfo(macId);
                //currentConnectionInfo.CompanyDB = objSap.CompanyDB;
                //currentConnectionInfo.UserName = objSap.UserName;
                //currentConnectionInfo.Password = objSap.Password;

                currentConnectionInfo.CompanyDB = ConfigurationManager.AppSettings["CompanyDB"];
                //currentConnectionInfo.UserName = ConfigurationManager.AppSettings["UserName"];
                //currentConnectionInfo.Password = ConfigurationManager.AppSettings["Password"];
                currentConnectionInfo.UserName = (Session["SUser"] != "" || Session["SUser"] != null) ? Session["SUser"].ToString() : ConfigurationManager.AppSettings["UserName"];
                currentConnectionInfo.Password = (Session["SPass"] != "" || Session["SPass"] != null) ? Session["SPass"].ToString() : ConfigurationManager.AppSettings["Password"];

                if (bLogin)
                {

                    currentOdataService.InitServiceContainer(strCurrentServiceURL);

                    if (!currentConnectionInfo.IsValid())
                    {
                        Utilities.SetResultMessage("Make sure correct user name, password and company database provided");

                    }

                    B1Session session = currentOdataService.LoginServer(currentConnectionInfo);

                    if (null != session)
                    {
                        currentOdataService.loginTrnsTime = DateTime.Now;

                        string strDisplay = currentOdataService.GetRequestHeaders() + currentOdataService.GetResponsetHeaders() + Newtonsoft.Json.JsonConvert.SerializeObject(session, Formatting.Indented);



                        Utilities.SetResultMessage(strDisplay);


                    }
                    else
                    {



                        Utilities.SetResultMessage("Failed to login, please make sure server is running and the credentials are correct.");
                    }
                }
                else
                {

                    currentOdataService.LogoutServer();


                    Utilities.SetResultMessage("Logout from service successfully");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return currentOdataService;
        }

        [HttpGet]
        public JsonResult GetCustomerListForDropDown(string customertype)
        {
            DataSet ds = new DataSet();
            List<object> customerList = new List<object>();
            try
            {
                string sqlCon = System.Configuration.ConfigurationManager.AppSettings.Get("SqlCon").ToString();
                using (SqlConnection conn = new SqlConnection(sqlCon))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;

                        string query = @"  
               SELECT CardCode as CustomerID, CardName as Name, '' as PhoneNo, E_Mail as EmailID  
               FROM OCRD";

                        cmd.CommandText = query;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                customerList.Add(new
                                {
                                    CustomerID = row["CustomerID"].ToString(),
                                    Name = row["Name"].ToString(),
                                    PhoneNo = row["PhoneNo"].ToString(),
                                    EmailID = row["EmailID"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return Json(customerList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CustomerGrid(string customerType)
        {
            ViewBag.CustomerID = customerType;
            return PartialView("~/Views/Shared/CustomerGrid.cshtml");
        }

        [HttpGet]
        public JsonResult GetCustomerList(string searchTerm, string customertype)
        {
            DataSet ds = new DataSet();
            List<object> customerList = new List<object>();
            try
            {
                string sqlCon = System.Configuration.ConfigurationManager.AppSettings.Get("SqlCon").ToString();
                using (SqlConnection conn = new SqlConnection(sqlCon))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;

                        string query = @"  
               SELECT CardCode as CustomerID, CardName as Name, '' as PhoneNo, E_Mail as EmailID  
               FROM OCRD  
               WHERE (CardName LIKE @SearchTerm OR CardCode LIKE @SearchTerm)  
           ";

                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                customerList.Add(new
                                {
                                    CustomerID = row["CustomerID"].ToString(),
                                    Name = row["Name"].ToString(),
                                    PhoneNo = row["PhoneNo"].ToString(),
                                    EmailID = row["EmailID"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return Json(customerList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCustomer(string CustId)
        {
            VendorViewModel customerViewModelObj = null;
            ConfirmMsg cnfMsg = new ConfirmMsg();
            try
            {

                string query = "SELECT \"CardCode\", \"CardName\",\"SlpCode\",\"ListNum\"," +
                    " (select T.\"descript\" FROM OCRD C LEFT OUTER JOIN OTER T on C.\"Territory\"=T.\"territryID\" " +
                    " where C.\"CardCode\"=OCRD.\"CardCode\") as \"Location\",\"GroupNum\"," +
                    " (select \"ExtraDays\" from OCTG where \"GroupNum\"=OCRD.\"GroupNum\") as \"ExtraDays\" From OCRD WHERE \"CardType\" = 'C' AND \"frozenFor\" = 'N' "
                + " AND (LOWER(\"CardCode\") LIKE '%' + LOWER('" + CustId + "') + '%' OR LOWER(CardName) LIKE '%' + LOWER('" + CustId + "') + '%') "
                + " ORDER BY \"CardCode\" ";
                DataSet dsData = objHlpr.getDataSet(query);
                customerViewModelObj = new VendorViewModel()
                {
                    CardCode = dsData.Tables[0].Rows[0]["CardCode"].ToString(),
                    CardName = dsData.Tables[0].Rows[0]["CardName"].ToString(),
                    deliveryLocation = dsData.Tables[0].Rows[0]["Location"].ToString(),
                    Paymentterms = dsData.Tables[0].Rows[0]["GroupNum"].ToString(),
                    ExtraDays = Convert.ToInt32(dsData.Tables[0].Rows[0]["ExtraDays"].ToString()),
                    SlpCode = Convert.ToInt32(dsData.Tables[0].Rows[0]["SlpCode"].ToString()),
                    ListNum = dsData.Tables[0].Rows[0]["ListNum"].ToString()
                };
                if (customerViewModelObj != null)
                {
                    customerViewModelObj.lstContactPerson = GetContactPersonList(CustId);
                    customerViewModelObj.lstShiptoAddress = GetAddress(CustId, "S");
                    customerViewModelObj.lstBilltoAddress = GetAddress(CustId, "B");
                    customerViewModelObj.lstTerritory = GetTerritoryList();
                    cnfMsg.IsSucess = true;
                    cnfMsg.CnfsMsg = "";
                }
                else
                {

                    cnfMsg.IsSucess = false;
                    cnfMsg.CnfsMsg = "Customer not found";

                }
            }
            catch (Exception ex)
            {
                cnfMsg.IsSucess = false;
                cnfMsg.CnfsMsg = ex.Message.ToString();

            }
            if (customerViewModelObj == null)
            {
                customerViewModelObj = new VendorViewModel();
            }
            customerViewModelObj.CnfMsg = cnfMsg;

            return Json(customerViewModelObj, JsonRequestBehavior.AllowGet);


        }

        [AllowAnonymous]
        public List<CustContactPerson> GetContactPersonList(string custCode)
        {
            List<CustContactPerson> lstContactPerson = new List<CustContactPerson>();
            try
            {

                string strSQL = " Select  OCPR.\"CntctCode\",OCPR.\"Name\"  From OCPR inner join OCRD on OCRD.\"CardCode\"= OCPR.\"CardCode\"" +
                     " where (OCRD.\"CardCode\"='" + custCode + "' or OCRD.\"CardName\"='" + custCode + "')";
                DataSet dsData = objHlpr.getDataSet(strSQL);
                foreach (DataRow dr in dsData.Tables[0].Rows)
                {
                    CustContactPerson valModel = new CustContactPerson()
                    {
                        ContactCode = dr["CntctCode"].ToString(),
                        Name = dr["Name"].ToString(),

                    };

                    lstContactPerson.Add(valModel);
                }
            }
            catch (Exception ex)
            {

            }
            return lstContactPerson;
        }

        public List<BPAddressModel> GetAddress(string bpCardCode, string Addresstype)
        {
            List<BPAddressModel> lstAddressDocs = new List<BPAddressModel>();
            try
            {
                if (bpCardCode != null)
                {

                    var AddrsQry = "SELECT T1.\"Address\", T1.\"Address2\", T1.\"Address3\", T1.\"Street\", T1.\"Block\"," +
                        " (select \"Name\" from OCRY where \"Code\"=T1.\"Country\") as \"Country\",T1.\"GlblLocNum\", T1.\"City\", T1.\"ZipCode\", T1.\"County\"," +
                        " (select \"Name\" from OCST where \"Code\"=T1.\"State\" and Country = T1.\"Country\") as \"State\", T1.\"AdresType\" FROM " +
                                   "\"OCRD\"  T0 INNER JOIN CRD1 T1 ON T0.\"CardCode\" = T1.\"CardCode\" WHERE T1.\"AdresType\" ='" + Addresstype + "' " +
                                   " and (T1.\"CardCode\" ='" + bpCardCode + "' or T0.\"CardName\" like '%" + bpCardCode + "%')";

                    var ds = objHlpr.getDataSet(AddrsQry);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        BPAddressModel obj = new BPAddressModel
                        {
                            //billTo = dr["Address2"].ToString(),
                            Address = dr["Address"].ToString(),
                            Address2 = dr["Address2"].ToString(),
                            Address3 = dr["Address3"].ToString(),
                            //addressName2 = dr["Address2"].ToString(),
                            //addressName3 = dr["Address3"].ToString(),
                            Street = dr["Street"].ToString(),
                            Block = dr["Block"].ToString(),
                            City = dr["City"].ToString(),
                            ZIPCode = dr["ZipCode"].ToString(),
                            Country = dr["Country"].ToString(),
                            State = dr["State"].ToString(),
                            FedaralTaxID = dr["GlblLocNum"].ToString()
                        };
                        lstAddressDocs.Add(obj);
                    }


                }
            }
            catch (Exception e)
            {
            }
            finally
            {

            }
            return lstAddressDocs;

        }

        [AllowAnonymous]
        public List<TerritoryModel> GetTerritoryList()
        {
            List<TerritoryModel> lstTerritory = new List<TerritoryModel>();
            try
            {

                string strSQL = " select territryID,descript from  OTER where inactive='N'";
                DataSet dsData = objHlpr.getDataSet(strSQL);
                foreach (DataRow dr in dsData.Tables[0].Rows)
                {
                    TerritoryModel valModel = new TerritoryModel()
                    {
                        Code = dr["descript"].ToString(),
                        Name = dr["descript"].ToString(),

                    };

                    lstTerritory.Add(valModel);
                }
            }
            catch (Exception ex)
            {

            }
            return lstTerritory;
        }

        public ActionResult GetNewItemRow(int id)
        {
            try
            {
                // Ensure the partial view exists and the model is valid
                return PartialView("~/Views/Shared/_ItemRow.cshtml", new ItemDetails { Index = id });
            }
            catch (Exception ex)
            {
                // Log the error (use your logging mechanism)
                Utilities.SetResultMessage($"Error in GetNewItemRow: {ex.Message}");
                return new HttpStatusCodeResult(500, "Internal Server Error");
            }
        }


        public ActionResult _ItemRow(ItemDetails address)
        {
            return PartialView("~/Views/Shared/_ItemRow", address);
        }


        public ActionResult ItemSearchGrid()
        {
            var items = GetAllItems();
            return PartialView("_ItemSearchGrid", items);
        }

        public ActionResult ItemGrid()
        {
            //var items = GetAllItems();
            return PartialView();
        }

        public List<ItemDetails> GetAllItems()
        {
            List<ItemDetails> items = new List<ItemDetails>();
            try
            {
                string strSQL = "select ItemCode,ItemName from OITM";
                DataSet dsData = objHlpr.getDataSet(strSQL);
                foreach (DataRow dr in dsData.Tables[0].Rows)
                {
                    ItemDetails valModel = new ItemDetails()
                    {
                        ItemCode = dr["ItemCode"].ToString(),
                        ItemName = dr["ItemName"].ToString(),
                    };

                    items.Add(valModel);
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return items;
        }


        public JsonResult GetItemsList()
        {
            List<SelectListItem> itemsList = new List<SelectListItem>();
            string strSQL = "select Top 30 ItemCode,ItemName from OITM";

            DataSet ds = objHlpr.getDataSet(strSQL);
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    itemsList.Add(new SelectListItem
                    {
                        Value = row["ItemCode"].ToString(),
                        Text = row["ItemName"].ToString()
                    });
                }
            }
            return Json(itemsList, JsonRequestBehavior.AllowGet);
        }
        private void LoadDropdowns(string custCode)


        {

            ViewBag.SalesEmployeesList = GetSalesEmployeeList()
                ?.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                }) ?? new List<SelectListItem>();

            ViewBag.PaymentTermsList = GetPaymentTermsList()
                ?.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                }) ?? new List<SelectListItem>();

            ViewBag.SeriesList = GetSeriesList()
                ?.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                }) ?? new List<SelectListItem>();

            ViewBag.TerritoryList = GetTerritoryList()
                ?.Select(x => new SelectListItem
                {
                    Value = x.Code,
                    Text = x.Name
                }) ?? new List<SelectListItem>();

            // ✅ Add this for Contact Person dropdown
            ViewBag.ContactPersonList = GetContactPersonList(custCode)
                ?.Select(x => new SelectListItem
                {
                    Value = x.ContactCode,
                    Text = x.Name
                }) ?? new List<SelectListItem>();

            // ✅ Billing Address Dropdown
            ViewBag.BillToAddressList = GetAddress(custCode, "B")
                ?.Select(x => new SelectListItem
                {
                    Value = x.Address, // You can use a unique value like Address + City
                    Text = $"{x.Address}, {x.City}, {x.State}"
                }) ?? new List<SelectListItem>();

            // ✅ Shipping Address Dropdown
            ViewBag.ShipToAddressList = GetAddress(custCode, "S")
                ?.Select(x => new SelectListItem
                {
                    Value = x.Address, // Again, use a unique identifier if needed
                    Text = $"{x.Address}, {x.City}, {x.State}"
                }) ?? new List<SelectListItem>();

        }
        //private void LoadDropdowns()
        //{
        //    ViewBag.SalesEmployeesList = GetSalesEmployeeList()
        //        .Select(x => new SelectListItem
        //        {
        //            Value = x.Value,
        //            Text = x.Text
        //        });
        //    ViewBag.PaymentTermsList = GetPaymentTermsList()
        //      .Select(x => new SelectListItem
        //      {
        //          Value = x.Value,
        //          Text = x.Text
        //      });
        //    ViewBag.SeriesList = GetSeriesList()
        //    .Select(x => new SelectListItem
        //    {
        //        Value = x.Value,
        //        Text = x.Text
        //    });
        //    ViewBag.TerritoryList = GetTerritoryList()
        //    .Select(x => new SelectListItem
        //    {
        //        Value = x.Code,
        //        Text = x.Name
        //    });
        //}

        //public JsonResult GetSeries(string ObjectCode)
        //{
        //    List<SeriesModel> lstSeriesModel = new List<SeriesModel>();
        //    string query = "SELECT Series,SeriesName,NextNumber FROM NNM1 WHERE ObjectCode = '" + ObjectCode + "' and Indicator like '%" + DateTime.Now.Year.ToString() + "%' ";

        //    DataSet ds = objHlpr.getDataSet(query);
        //    foreach (DataRow dr in ds.Tables[0].Rows)
        //    {
        //        lstSeriesModel.Add(new SeriesModel
        //        {
        //            Series = dr["Series"].ToString(),
        //            SeriesName = dr["SeriesName"].ToString(),
        //            NextNumber = dr["NextNumber"].ToString()
        //        });
        //    }

        //    return Json(lstSeriesModel, JsonRequestBehavior.AllowGet);
        //}

        private List<SelectListItem> GetSeriesList()
        {
            List<SelectListItem> seriesList = new List<SelectListItem>();
            var seriesDocDict = new Dictionary<string, int>();
            int docNum = 0;
            string seriesId = string.Empty;
            string query = "SELECT Series,SeriesName, ISNULL(NextNumber,0) as NextNumber FROM NNM1 WHERE ObjectCode = '13' and Indicator like '%" + DateTime.Now.Year.ToString() + "%' ";

            DataSet ds = objHlpr.getDataSet(query);
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    seriesList.Add(new SelectListItem
                    {
                        Value = row["Series"].ToString(),
                        Text = row["SeriesName"].ToString()
                    });
                    seriesId = row["Series"].ToString();
                    docNum = Convert.ToInt32(row["NextNumber"]);
                    seriesDocDict[seriesId] = docNum;
                }
                ViewBag.seriesId = seriesId;
                ViewBag.DefaultDocNumber = docNum;
                ViewBag.SeriesDocDict = Newtonsoft.Json.JsonConvert.SerializeObject(seriesDocDict); // JSON string
            }
            return seriesList;
        }

        private List<SelectListItem> GetSalesEmployeeList()
        {
            List<SelectListItem> salesList = new List<SelectListItem>();
            string strSQL = "select \"SlpCode\", \"SlpName\" from \"OSLP\" where \"Active\"='Y' and \"Locked\"='N'";

            DataSet ds = objHlpr.getDataSet(strSQL);
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    salesList.Add(new SelectListItem
                    {
                        Value = row["SlpCode"].ToString(),
                        Text = row["SlpName"].ToString()
                    });
                }
            }
            return salesList;
        }

        private List<SelectListItem> GetPaymentTermsList()
        {
            List<SelectListItem> CustGrpList = new List<SelectListItem>();
            string strSQL = " SELECT T0.\"GroupNum\", T0.\"PymntGroup\" FROM OCTG T0";

            DataSet ds = objHlpr.getDataSet(strSQL);
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    CustGrpList.Add(new SelectListItem
                    {
                        Value = row["GroupNum"].ToString(),
                        Text = row["PymntGroup"].ToString()
                    });
                }
            }
            return CustGrpList;
        }

        [HttpGet]
        public JsonResult GetItemList(string searchTerm, string customertype)
        {
            DataSet ds = new DataSet();
            List<object> itemList = new List<object>();
            string sqlCon = System.Configuration.ConfigurationManager.AppSettings.Get("SqlCon").ToString();
            using (SqlConnection conn = new SqlConnection(sqlCon))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    string query = @"  
               SELECT ItemCode, ItemName, '' as PhoneNo, '' as EmailID  
               FROM OITM  
               WHERE (ItemName LIKE @SearchTerm OR ItemCode LIKE @SearchTerm) ";

                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        itemList.Add(new
                        {
                            ItemCode = row["ItemCode"].ToString(),
                            ItemName = row["ItemName"].ToString(),
                            PhoneNo = row["PhoneNo"].ToString(),
                            EmailID = row["EmailID"].ToString()
                        });
                    }
                }
            }

            return Json(itemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemDetailsbyBarcode(string barcode)
        {
            ItemDetails objItemDetails = null;
            List<BatchDetails> lstBatch = new List<BatchDetails>();
            List<UomDetails> lstUom = new List<UomDetails>();
            List<InventoryDetails> lstInvntoryDts = new List<InventoryDetails>();
            List<VATModel> vatList = new List<VATModel>();
            try
            {
                string mng = User.Identity.Name;
                var objitemquery = "SELECT T0.\"ItemCode\",T0.\"ItemName\",T3.\"ItmsGrpNam\",T0.\"OnHand\",T0.\"IsCommited\"," +
                    " T0.\"FrgnName\",T2.\"UomCode\",T0.\"ManBtchNum\",T0.\"ManSerNum\",T0.\"VatGourpSa\", " +
                    " (select \"Price\" from \"ITM1\" where \"ItemCode\" = T0.\"ItemCode\" and \"PriceList\" = '1') as \"Price\", " +
                    " (select \"OUDG\".\"Warehouse\" from \"OUSR\" inner join \"OUDG\" on \"OUDG\".\"Code\"=\"OUSR\".\"DfltsGroup\" " +
                    " where OUSR.\"USER_CODE\"='" + @Session["UserName"] + "') as \"Warehouse\"," +
                    " ISNULL((select \"OITW\".\"OnHand\" from \"OUSR\" inner join \"OUDG\" on \"OUDG\".\"Code\"=\"OUSR\".\"DfltsGroup\" " +
                    " inner join \"OITW\" on \"OITW\".\"WhsCode\"=\"OUDG\".\"Warehouse\" where OUSR.\"USER_CODE\"='" + @Session["UserName"] + "'" +
                    " and \"OITW\".\"ItemCode\" = T0.\"ItemCode\"),0) as \"WhInstockQty\",(select \"UomEntry\" from OUOM " +
                    " where \"UomCode\"=T2.\"UomCode\" ) as \"UomEntry\",ISNULL((select ISNULL(\"Discount\",0) from SPP1 " +
                    " where ListNum=16 and ItemCode=T0.\"ItemCode\" and Cast(getdate() as date) between \"Fromdate\"" +
                    " and ISNULL(\"Todate\",getdate())),0)  as \"Discount\"" +
                    " FROM OITM T0 INNER JOIN OUOM T2 ON T0.\"InvntryUom\"=T2.\"UomCode\" " +
                    " INNER JOIN OITB T3 ON T0.\"ItmsGrpCod\"=T3.\"ItmsGrpCod\"" +
                    " WHERE T0.\"CodeBars\"='" + barcode + "' ";
                DataSet dsData = objHlpr.getDataSet(objitemquery);
                if (dsData.Tables[0].Rows.Count > 0)
                    objItemDetails = new ItemDetails()
                    {
                        ItemCode = dsData.Tables[0].Rows[0]["ItemCode"].ToString(),
                        ItemName = dsData.Tables[0].Rows[0]["ItemName"].ToString(),
                        DisPer = Convert.ToDecimal(dsData.Tables[0].Rows[0]["Discount"].ToString()),
                        Price = Convert.ToDecimal(dsData.Tables[0].Rows[0]["Price"].ToString()),
                        UomName = dsData.Tables[0].Rows[0]["UomCode"].ToString(),
                        UomEntry = Convert.ToInt32(dsData.Tables[0].Rows[0]["UomEntry"].ToString()),
                        Warehouse = dsData.Tables[0].Rows[0]["Warehouse"].ToString(),
                        VatGrpCode = dsData.Tables[0].Rows[0]["VatGourpSa"].ToString(),
                        WhInstockQty = Convert.ToDecimal(dsData.Tables[0].Rows[0]["WhInstockQty"].ToString()),
                        foc = "No"
                    };
                var batchDetails = "SELECT T0.\"ItemCode\",T0.\"BatchNum\",T0.\"WhsCode\",FORMAT(T1.\"ExpDate\", 'MM/yyyy') as \"ExpDate\" , " +
                    " T0.\"Quantity\" FROM \"OIBT\" T0 INNER JOIN OBTN T1 ON T0.\"ItemCode\" = T1.\"ItemCode\" AND T0.\"BatchNum\" = T1.\"DistNumber\" " +
                    " WHERE T0.\"ItemCode\" = '" + dsData.Tables[0].Rows[0]["ItemCode"].ToString()  + "' " +
                    " and T0.\"Quantity\" >0 and Cast(T1.\"ExpDate\" as date) > getdate() " +
                    " and T0.\"WhsCode\" ='" + dsData.Tables[0].Rows[0]["Warehouse"].ToString() + "' ORDER BY T1.\"ExpDate\" ";
                DataSet dsbatchdetails = objHlpr.getDataSet(batchDetails);
                if (dsbatchdetails.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsbatchdetails.Tables[0].Rows)
                    {
                        BatchDetails lstBatch1 = new BatchDetails
                        {
                            BatchNum = Convert.ToString(dr["BatchNum"]),
                            Expirydate = dr["ExpDate"].ToString(),
                            WhsCode = dr["WhsCode"].ToString(),
                            BatchQty = Convert.ToDecimal(dr["Quantity"])
                        };
                        lstBatch.Add(lstBatch1);
                    }

                }
                objItemDetails.lstBatchdetails = lstBatch;

                string ItemCode = dsData.Tables[0].Rows[0]["ItemCode"].ToString();
                var query = " SELECT u.\"UomEntry\" AS UomCode,u.UomName,g.BaseQty FROM OITM o " +
                    " INNER JOIN UGP1 g ON o.UgpEntry = g.UgpEntry INNER JOIN OUOM u ON g.UomEntry = u.UomEntry " +
                    " WHERE o.ItemCode = '" + ItemCode + "'";
                DataSet dsdetails = objHlpr.getDataSet(query);
                if (dsdetails.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsdetails.Tables[0].Rows)
                    {
                        UomDetails lstUom1 = new UomDetails
                        {
                            UomCode = Convert.ToInt32(dr["UomCode"]),
                            UomName = dr["UomName"].ToString(),
                            BaseQty = Convert.ToDecimal(dr["BaseQty"])
                        };
                        lstUom.Add(lstUom1);
                    }

                }
                objItemDetails.lstUomDetails = lstUom;


                var strVatQry = "SELECT T0.\"Code\", T0.\"Name\", T0.\"Rate\" FROM OVTG T0 WHERE \"Locked\"='N' and \"Category\"='O' " +
                    " and \"InActive\"='N'";

                DataSet dsVat = objHlpr.getDataSet(strVatQry);
                if (dsVat.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsVat.Tables[0].Rows)
                    {
                        vatList.Add(new VATModel
                        {
                            VATCode = dr["Code"].ToString(),
                            VATName = dr["Name"].ToString(),
                            VatRate = Convert.ToDouble(dr["Rate"])
                        });
                        if (objItemDetails.VatGrpCode == dr["Code"].ToString())
                        {
                            objItemDetails.VAT = Convert.ToDecimal(dr["Rate"]);
                        }
                    }
                    objItemDetails.lstVAT = vatList;
                }


                try
                {
                    if (ItemCode != null)
                    {
                        var Qry = "select T1.\"WhsCode\",T1.\"WhsName\",T0.\"OnHand\",T0.\"IsCommited\",T0.\"OnOrder\" from OITW T0 " +
                            " inner join OWHS T1 on T1.\"WhsCode\"=T0.\"WhsCode\" where T0.\"ItemCode\"='" + ItemCode + "'";

                        DataSet dsWarehouse = objHlpr.getDataSet(Qry);
                        foreach (DataRow dr in dsWarehouse.Tables[0].Rows)
                        {
                            InventoryDetails obj = new InventoryDetails
                            {
                                WhsCode = dr["WhsCode"].ToString(),
                                WhsName = dr["WhsName"].ToString(),
                                InStock = DBNull.Value.Equals(dr["OnHand"]) ? 0 : Convert.ToDecimal(dr["OnHand"]),
                                //Ordered = DBNull.Value.Equals(dr["OnOrder"]) ? 0 : Convert.ToDecimal(dr["OnOrder"]),
                                //Commited = DBNull.Value.Equals(dr["IsCommited"]) ? 0 : Convert.ToDecimal(dr["IsCommited"])
                                //lstBatchdetails = new List<BatchDetails>()
                            };
                            //var WhsbatchDetails = "SELECT T0.\"ItemCode\",T0.\"BatchNum\",T0.\"WhsCode\",FORMAT(T1.\"ExpDate\", 'MM/yyyy') as \"ExpDate\", " +
                            //" T0.\"Quantity\" FROM \"OIBT\" T0 INNER JOIN OBTN T1 ON T0.\"ItemCode\" = T1.\"ItemCode\" AND T0.\"BatchNum\" = T1.\"DistNumber\" " +
                            //" WHERE T0.\"ItemCode\" = '" + ItemCode + "' " +
                            //" and T0.\"Quantity\" > 0 and Cast(T1.\"ExpDate\" as date) > getdate() " +
                            //" and T0.\"WhsCode\" ='" + dr["WhsCode"].ToString() + "' ORDER BY T1.\"ExpDate\"";

                            //DataSet dsWhsbatchdetails = objHlpr.getDataSet(WhsbatchDetails);
                            //if (dsWhsbatchdetails.Tables[0].Rows.Count > 0)
                            //{
                            //    foreach (DataRow drBatch in dsWhsbatchdetails.Tables[0].Rows)
                            //    {
                            //        BatchDetails lstBatch1 = new BatchDetails
                            //        {
                            //            BatchNum = Convert.ToString(drBatch["BatchNum"]),
                            //            Expirydate = drBatch["ExpDate"].ToString(),
                            //            WhsCode = drBatch["WhsCode"].ToString(),
                            //            BatchQty = Convert.ToDecimal(drBatch["Quantity"])
                            //        };
                            //        obj.lstBatchdetails.Add(lstBatch1);
                            //    }
                            //}

                            lstInvntoryDts.Add(obj);

                        }
                        objItemDetails.lstWarehouse = lstInvntoryDts;
                    }
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }


            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Json(objItemDetails, JsonRequestBehavior.AllowGet);
        }

        #region Search by Item code logic
        [HttpGet]
        public ActionResult GetItemDetails(string itemCode)
        {
            ItemDetails objItemDetails = null;
            List<UomDetails> lstUom = new List<UomDetails>();
            List<InventoryDetails> lstInvntoryDts = new List<InventoryDetails>();
            List<VATModel> vatList = new List<VATModel>();
            try
            {
                string mng = User.Identity.Name;
                var objitemquery = "SELECT T0.\"ItemCode\",T0.\"ItemName\",T3.\"ItmsGrpNam\",T0.\"OnHand\",T0.\"IsCommited\"," +
                    " T0.\"FrgnName\",T2.\"UomCode\",T0.\"ManBtchNum\",T0.\"ManSerNum\",T0.\"VatGourpSa\", " +
                    " (select \"Price\" from \"ITM1\" where \"ItemCode\" = T0.\"ItemCode\" and \"PriceList\" = '1') as \"Price\", " +
                    " (select \"OUDG\".\"Warehouse\" from \"OUSR\" inner join \"OUDG\" on \"OUDG\".\"Code\"=\"OUSR\".\"DfltsGroup\" " +
                    " where OUSR.\"USER_CODE\"='" + @Session["UserName"] + "') as \"Warehouse\"," +
                    " (select \"OITW\".\"OnHand\" from \"OUSR\" inner join \"OUDG\" on \"OUDG\".\"Code\"=\"OUSR\".\"DfltsGroup\" " +
                    " inner join \"OITW\" on \"OITW\".\"WhsCode\"=\"OUDG\".\"Warehouse\" where OUSR.\"USER_CODE\"='" + @Session["UserName"] + "'" +
                    " and \"OITW\".\"ItemCode\" = T0.\"ItemCode\") as \"WhInstockQty\",(select \"UomEntry\" from OUOM " +
                    " where \"UomCode\"=T2.\"UomCode\" ) as \"UomEntry\", (select ISNULL(\"Discount\",0) from SPP1 " +
                    " where ListNum=16 and ItemCode=T0.\"ItemCode\" and Cast(getdate() as date) between \"Fromdate\"" +
                    " and ISNULL(\"Todate\",getdate()))  as \"Discount\"" +
                    " FROM OITM T0 INNER JOIN OUOM T2 ON T0.\"InvntryUom\"=T2.\"UomCode\" " +
                    " INNER JOIN OITB T3 ON T0.\"ItmsGrpCod\"=T3.\"ItmsGrpCod\"" +
                    " WHERE T0.\"ItemCode\"='" + itemCode + "' ";
                DataSet dsData = objHlpr.getDataSet(objitemquery);
                if (dsData.Tables[0].Rows.Count > 0)
                    objItemDetails = new ItemDetails()
                    {
                        ItemCode = dsData.Tables[0].Rows[0]["ItemCode"].ToString(),
                        ItemName = dsData.Tables[0].Rows[0]["ItemName"].ToString(),
                        DisPer = Convert.ToDecimal(dsData.Tables[0].Rows[0]["Discount"].ToString()),
                        Price = Convert.ToDecimal(dsData.Tables[0].Rows[0]["Price"].ToString()),
                        UomName = dsData.Tables[0].Rows[0]["UomCode"].ToString(),
                        UomEntry = Convert.ToInt32(dsData.Tables[0].Rows[0]["UomEntry"].ToString()),
                        Warehouse = dsData.Tables[0].Rows[0]["Warehouse"].ToString(),
                        VatGrpCode = dsData.Tables[0].Rows[0]["VatGourpSa"].ToString(),
                        WhInstockQty = Convert.ToDecimal(dsData.Tables[0].Rows[0]["WhInstockQty"].ToString()),
                        foc = "No"
                    };


                var query = " SELECT u.\"UomEntry\" AS UomCode,u.UomName,g.BaseQty FROM OITM o " +
                    " INNER JOIN UGP1 g ON o.UgpEntry = g.UgpEntry INNER JOIN OUOM u ON g.UomEntry = u.UomEntry " +
                    " WHERE o.ItemCode = '" + itemCode + "'";
                DataSet dsdetails = objHlpr.getDataSet(query);
                if (dsdetails.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsdetails.Tables[0].Rows)
                    {
                        UomDetails lstUom1 = new UomDetails
                        {
                            UomCode = Convert.ToInt32(dr["UomCode"]),
                            UomName = dr["UomName"].ToString(),
                            BaseQty = Convert.ToDecimal(dr["BaseQty"])
                        };
                        lstUom.Add(lstUom1);
                    }

                }
                objItemDetails.lstUomDetails = lstUom;


                var strVatQry = "SELECT T0.\"Code\", T0.\"Name\", T0.\"Rate\" FROM OVTG T0 WHERE \"Locked\"='N' and \"Category\"='O' and \"InActive\"='N'";

                DataSet dsVat = objHlpr.getDataSet(strVatQry);
                if (dsVat.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsVat.Tables[0].Rows)
                    {
                        vatList.Add(new VATModel
                        {
                            VATCode = dr["Code"].ToString(),
                            VATName = dr["Name"].ToString(),
                            VatRate = Convert.ToDouble(dr["Rate"])
                        });
                        if (objItemDetails.VatGrpCode == dr["Code"].ToString())
                        {
                            objItemDetails.VAT = Convert.ToDecimal(dr["Rate"]);
                        }
                    }
                    objItemDetails.lstVAT = vatList;
                }


                try
                {
                    if (itemCode != null)
                    {
                        var Qry = "select T1.\"WhsCode\",T1.\"WhsName\",T0.\"OnHand\",T0.\"IsCommited\",T0.\"OnOrder\" from OITW T0 " +
                            " inner join OWHS T1 on T1.\"WhsCode\"=T0.\"WhsCode\" where T0.\"ItemCode\"='" + itemCode + "'";

                        DataSet dsWarehouse = objHlpr.getDataSet(Qry);
                        foreach (DataRow dr in dsWarehouse.Tables[0].Rows)
                        {
                            InventoryDetails obj = new InventoryDetails
                            {
                                WhsCode = dr["WhsCode"].ToString(),
                                WhsName = dr["WhsName"].ToString(),
                                InStock = DBNull.Value.Equals(dr["OnHand"]) ? 0 : Convert.ToDecimal(dr["OnHand"]),
                                //Ordered = DBNull.Value.Equals(dr["OnOrder"]) ? 0 : Convert.ToDecimal(dr["OnOrder"]),
                                //Commited = DBNull.Value.Equals(dr["IsCommited"]) ? 0 : Convert.ToDecimal(dr["IsCommited"])
                            };
                            lstInvntoryDts.Add(obj);
                        }
                        objItemDetails.lstWarehouse = lstInvntoryDts;
                    }
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }


            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Json(objItemDetails, JsonRequestBehavior.AllowGet);
        }
        #endregion
        [HttpGet]
        public JsonResult SearchItemNames(string term)
        {
            DataSet ds = new DataSet();
            List<object> itemList = new List<object>();
            string sqlCon = System.Configuration.ConfigurationManager.AppSettings.Get("SqlCon").ToString();
            using (SqlConnection conn = new SqlConnection(sqlCon))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    string query = @"  
               SELECT ItemCode, ItemName, '' as PhoneNo, '' as EmailID  
               FROM OITM  
               WHERE (ItemName LIKE @term OR ItemCode LIKE @term) ";

                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@term", "%" + term + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        itemList.Add(new
                        {
                            ItemCode = row["ItemCode"].ToString(),
                            ItemName = row["ItemName"].ToString(),
                            PhoneNo = row["PhoneNo"].ToString(),
                            EmailID = row["EmailID"].ToString()
                        });
                    }
                }
            }

            return Json(itemList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchItems2(string searchTerm)
        {
            var allItems = GetAllItems();
            var filteredItems = allItems
                .Where(i => i.ItemCode.Contains(searchTerm) || i.ItemName.Contains(searchTerm))
                .Take(100)
                .Select(i => new
                {
                    Value = i.ItemCode,
                    Text = i.ItemName
                })
                .ToList();

            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchItems(string searchTerm)
        {
            var allItems = GetAllItems();
            var lowerSearchTerm = searchTerm?.ToLower() ?? "";

            var filteredItems = allItems
                .Where(i =>
                    (!string.IsNullOrEmpty(i.ItemCode) && i.ItemCode.ToLower().Contains(lowerSearchTerm)) ||
                    (!string.IsNullOrEmpty(i.ItemName) && i.ItemName.ToLower().Contains(lowerSearchTerm))
                )
                .Take(100)
                .Select(i => new
                {
                    Value = i.ItemCode,
                    Text = i.ItemName
                })
                .ToList();

            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetItemName(string id)
        {
            var allItems = GetAllItems(); // Use your ADO.NET or in-memory list
            var item = allItems
                .Where(x => x.ItemCode == id)
                .Select(x => new
                {
                    Value = x.ItemCode,
                    Text = x.ItemName
                })
                .FirstOrDefault();

            return Json(item, JsonRequestBehavior.AllowGet);
        }



    }
}