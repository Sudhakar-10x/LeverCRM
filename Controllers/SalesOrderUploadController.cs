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
using ClosedXML.Excel;

namespace _10xErp.Controllers
{
    public class SalesOrderUploadController : Controller
    {
        private readonly DataHelper objHlpr = new DataHelper(); // Changed to private readonly instance  

        public static List<SalesOrderViewModel> Orders = new List<SalesOrderViewModel>(); // In-memory store  

        public ActionResult Index()
        {
            SalesOrderViewModel oSalesGenReqModel = new SalesOrderViewModel();

            ViewBag.Current = "SalesOrder";
            return View(oSalesGenReqModel);
        }


        [HttpPost]
        public ActionResult SaveOrder(OrderViewModel model)
        {
            // Save logic here (Save to Database)
            return Json(new { success = true, message = "Order saved successfully." });
        }



        [HttpGet]
        public ActionResult Upload()
        {
            ViewBag.UserName = User.Identity.Name;
            SalesOrderViewModel oSalesOrderViewModel = new SalesOrderViewModel();
            oSalesOrderViewModel.ItemDetailsListView = new List<ItemDetails>();
            try
            {

                oSalesOrderViewModel.ItemDetailsListView = new List<ItemDetails>();
                //oSalesOrderViewModel.SalesEmployeeList = new SelectList(GetSalesEmployeeList(), "Value", "Text");
                LoadDropdowns();

                oSalesOrderViewModel.ReqDate = DateTime.Today;
                oSalesOrderViewModel.postingdate = DateTime.Today;
                oSalesOrderViewModel.LPODate = DateTime.Today;
                oSalesOrderViewModel.series = Convert.ToInt32(ViewBag.seriesId);
                oSalesOrderViewModel.DocNum = Convert.ToInt32(ViewBag.DefaultDocNumber);

                //ViewBag.ItemList = GetAllItems();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            ViewBag.Current = "SalesOrder";

            return View(oSalesOrderViewModel);
        }

        [HttpPost]
        public ActionResult UploadExcel(HttpPostedFileBase excelFile)
        {
            if (excelFile == null || excelFile.ContentLength == 0)
                return Content("No file selected.");

            var model = new OrderViewModel();
            using (var stream = excelFile.InputStream)
            {
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    // Reading Customer Details (First Row)
                    var firstRow = worksheet.Row(2);


                    string strCust = "SELECT count(*) FROM \"OCRD\" WHERE \"CardCode\" = '" + firstRow.Cell(1).GetValue<string>() + "'" +
                       " AND \"validFor\"='Y'";
                    DataSet dsData = objHlpr.getDataSet(strCust);
                    if (dsData.Tables[0].Rows.Count > 0 && Convert.ToInt32(dsData.Tables[0].Rows[0][0].ToString()) > 0)
                    {
                        //string ShipTocodequery = "SELECT TOP 1 Address FROM CRD1 WHERE CardCode = '" + firstRow.Cell(1).GetValue<string>() + "'" +
                        //    " AND AdresType = 'S' ";

                        //DataSet dsShipData = objHlpr.getDataSet(ShipTocodequery);

                        //string shipToCode = (dsData.Tables[0].Rows.Count>0)? dsShipData.Tables[0].Rows[0][0].ToString() :"";

                        //string PayTocodequery = "SELECT TOP 1 Address FROM CRD1 WHERE CardCode = '" + firstRow.Cell(1).GetValue<string>() + "'" +
                        //    " AND AdresType = 'B' ";

                        //DataSet dsPayData = objHlpr.getDataSet(PayTocodequery);

                        //string payToCode = (dsPayData.Tables[0].Rows.Count>0) ? dsPayData.Tables[0].Rows[0][0].ToString(): "";

                        //string strSQL = " Select  OCPR.\"CntctCode\",OCPR.\"Name\"  From OCPR inner join OCRD on OCRD.\"CardCode\"= OCPR.\"CardCode\"" +
                        //" where OCRD.\"CardCode\"='" + firstRow.Cell(1).GetValue<string>() + "'";
                        //dsData = objHlpr.getDataSet(strSQL);

                        //string contactPerson =(dsData.Tables[0].Rows.Count > 0)? dsData.Tables[0].Rows[0][0].ToString() : "";
                        model.Customer = new CustomerDetails
                        {
                            CardCode = firstRow.Cell(1).GetValue<string>(),
                            CardName = firstRow.Cell(2).GetValue<string>(),
                            //ContactPerson = contactPerson,//firstRow.Cell(3).GetValue<string>(),
                            LpoNo = firstRow.Cell(3).GetValue<string>(),
                            //PaymentTerms = firstRow.Cell(5).GetValue<string>(),
                            //ShipToCode = shipToCode,//firstRow.Cell(6).GetValue<string>(),
                            //PayToCode = payToCode,//firstRow.Cell(7).GetValue<string>(),
                            //DeliveryLocation = firstRow.Cell(8).GetValue<string>(),
                            //SalesEmployee = firstRow.Cell(9).GetValue<string>(),
                            Remarks = firstRow.Cell(4).GetValue<string>(),
                            Series = firstRow.Cell(5).GetValue<string>(),
                            //Curentdate = DateTime.Now.Date
                        };

                        // Reading Item Details
                        foreach (var row in worksheet.RowsUsed().Skip(1))
                        {
                            string strItm = "SELECT count(*) FROM \"OITM\" WHERE \"ItemCode\" = '" + row.Cell(6).GetValue<string>() + "'" +
                       " AND \"validFor\"='Y'";
                            DataSet dsDataItm = objHlpr.getDataSet(strItm);
                            if (dsDataItm.Tables[0].Rows.Count > 0 && Convert.ToInt32(dsDataItm.Tables[0].Rows[0][0].ToString()) > 0)
                            {
                                model.Items.Add(new OrderItem
                                {
                                    ItemCode = row.Cell(6).GetValue<string>(),
                                    ItemName = row.Cell(7).GetValue<string>(),
                                    Qty = row.Cell(8).GetValue<int>(),
                                    //UnitPrice = row.Cell(9).GetValue<decimal>(),
                                    //Discount = row.Cell(10).GetValue<decimal>(),
                                    //Vat = row.Cell(11).GetValue<string>(),
                                    //Warehouse = row.Cell(12).GetValue<string>(),
                                    FOC = row.Cell(9).GetValue<string>(),
                                    FOCRemarks = row.Cell(10).GetValue<string>()
                                });
                            }
                        }
                    }
                    else
                    {

                    }
                }

            }

            return PartialView("_OrderPartial", model);
        }



        [HttpPost]

        public ActionResult CreateSO(OrderViewModel oSODetails,string custCode)
        {
            ConfirmMsg cnfMsg = new ConfirmMsg();
            bool logout = false;
            string docsnum;
            ServiceLayerServices currentOdataService;
            try
            {
                //if (ModelState.IsValid)
                //{
                //    if (oSODetails.ItemDetailsListView != null)
                //    {

                string strCurrentServiceURL = ConfigurationManager.AppSettings["CurrentServiceURL"]; //textBox_ServiceURL.Text;

                currentOdataService = (ServiceLayerServices)System.Web.HttpContext.Current.Application["sapAppGlobal"];

                currentOdataService = LoginLogoutAction(true);

                Uri service = new Uri(strCurrentServiceURL);

                currentOdataService.InitServiceContainer(strCurrentServiceURL);
                Document oSeleOrdrReqst = new Document();

                oSeleOrdrReqst.Requester = "manager";
                oSeleOrdrReqst.ReqType = 12;
                // EmpID, Manag
                oSeleOrdrReqst.DocObjectCode = "17";
                //oPurReqst.DocType = "dDocument_Items";
                //oSeleOrdrReqst.SeriesString = "";
                oSeleOrdrReqst.Series = Convert.ToInt32(oSODetails.Customer.Series);
                //oSeleOrdrReqst.DocNum = oSODetails.DocNum;
                oSeleOrdrReqst.CardCode = oSODetails.Customer.CardCode;
                oSeleOrdrReqst.RequriedDate = Convert.ToDateTime(DateTime.Now.Date);
                oSeleOrdrReqst.DocDueDate = Convert.ToDateTime(DateTime.Now.Date);
                oSeleOrdrReqst.Comments = @Session["UserName"] + " " + oSODetails.Customer.Remarks;
                oSeleOrdrReqst.DocDate = Convert.ToDateTime(DateTime.Now.Date);
                //oSeleOrdrReqst.DocDueDate = DateTime.Today.Date;
                //oSeleOrdrReqst.TaxDate = DateTime.Today.Date;
                oSeleOrdrReqst.NumAtCard = oSODetails.Customer.LpoNo;

                //oSeleOrdrReqst.U_PDTUSER = oSODetails.EmpID;

                //oSeleOrdrReqst.U_LocationName = getLocationName(oSODetails.WhseCode);
                //oSeleOrdrReqst.U_UserName = getEmpName(oSODetails.EmpID);
                //DocSeries oDocSeries = getSeriesInfo("Sales Order", oSODetails.WhseCode);

                //if (oDocSeries.SeriesId != null)
                //{
                //    oSeleQtnReqst.SeriesString = oDocSeries.SeriesName;
                //    oSeleQtnReqst.Series = oDocSeries.SeriesId;
                //}

                //oSeleOrdrReqst.Series = 8;
                //var seObj = GetSalesEmployeeInfoByUId(oSODetails.EmpID);
                //oSeleOrdrReqst.BPL_IDAssignedToInvoice = Convert.ToInt32(oSODetails.BranchId);

                //oSeleOrdrReqst.NumAtCard = oSODetails.CustRefNo;
                //oSeleOrdrReqst.DiscountPercent = Convert.ToDouble(oSODetails.DocDiscount);

                string strBPData = " SELECT OCRD.\"CardCode\",\"CardName\",\"SlpCode\",\"ListNum\",ISNULL(OCPR.\"CntctCode\",'') as CntctPrsn," +
                    " (select T.\"descript\" FROM \"OCRD\" C LEFT OUTER JOIN \"OTER\" T on C.\"Territory\"=T.\"territryID\" " +
                    " where C.\"CardCode\"=OCRD.\"CardCode\") as Location," +
                    " \"GroupNum\",(select \"ExtraDays\" from OCTG where \"GroupNum\"=OCRD.\"GroupNum\") as \"ExtraDays\",OCRD.\"BillToDef\",OCRD.\"ShipToDef\" " +
                    " From OCRD inner join OCPR on OCPR.\"CardCode\"=OCRD.\"CardCode\" and OCPR.\"Name\" = OCRD.\"CntctPrsn\" " +
                    " WHERE \"CardType\" = 'C' AND \"frozenFor\" = 'N' AND LOWER(OCRD.\"CardCode\") LIKE '%' + LOWER('" + oSODetails.Customer.CardCode + "') + '%' ORDER BY OCRD.\"CardCode\" ";
                DataSet dsData = objHlpr.getDataSet(strBPData);
                if (dsData.Tables[0].Rows.Count > 0)
                {
                    oSeleOrdrReqst.U_DelLocation = dsData.Tables[0].Rows[0]["Location"].ToString();
                    oSeleOrdrReqst.ContactPersonCode = dsData.Tables[0].Rows[0]["CntctPrsn"].ToString() == "" ? 0 : Convert.ToInt32(dsData.Tables[0].Rows[0]["CntctPrsn"].ToString());
                    oSeleOrdrReqst.ShipToCode = dsData.Tables[0].Rows[0]["ShipToDef"].ToString();
                    oSeleOrdrReqst.PayToCode = dsData.Tables[0].Rows[0]["BillToDef"].ToString();
                    oSeleOrdrReqst.SalesPersonCode = Convert.ToInt32(dsData.Tables[0].Rows[0]["SlpCode"].ToString());
                    oSeleOrdrReqst.PaymentGroupCode = Convert.ToInt32(dsData.Tables[0].Rows[0]["GroupNum"].ToString());
                }
                DocumentLine oLine = null;
                int iLine = 0;
                foreach (OrderItem itm in oSODetails.Items)
                {
                    oLine = new DocumentLine();

                    oLine.ItemCode = itm.ItemCode;
                    oLine.LineVendor = oSODetails.Customer.CardCode;
                    oLine.Quantity = Convert.ToDouble(itm.Qty);
                    var objitemquery = "SELECT T0.\"ItemCode\",T0.\"ItemName\",T3.\"ItmsGrpNam\",T0.\"OnHand\",T0.\"IsCommited\"," +
                    " T0.\"FrgnName\",T2.\"UomCode\",T0.\"ManBtchNum\",T0.\"ManSerNum\",T0.\"VatGourpSa\", " +
                    " (select \"Price\" from \"ITM1\" where \"ItemCode\" = '" + itm.ItemCode + "' and \"PriceList\" = '1') as \"Price\", " +
                    " (select \"OUDG\".\"Warehouse\" from \"OUSR\" inner join \"OUDG\" on \"OUDG\".\"Code\"=\"OUSR\".\"DfltsGroup\" " +
                    " where OUSR.\"USER_CODE\"='" + @Session["UserName"] + "') as \"Warehouse\"," +
                    " (select ISNULL(\"Discount\",0) from SPP1 " +
                    " where ListNum=16 and ItemCode='" + itm.ItemCode + "' and Cast(getdate() as date) between \"Fromdate\"" +
                    " and ISNULL(\"Todate\",getdate()))  as \"Discount\"" +
                    " FROM OITM T0 INNER JOIN OUOM T2 ON T0.\"InvntryUom\"=T2.\"UomCode\" " +
                    " INNER JOIN OITB T3 ON T0.\"ItmsGrpCod\"=T3.\"ItmsGrpCod\"" +
                    " WHERE (T0.\"ItemCode\"='" + itm.ItemCode + "') ";
                    DataSet dsItemData = objHlpr.getDataSet(objitemquery);
                    if (dsItemData.Tables[0].Rows.Count > 0)
                    {
                        oLine.DiscountPercent = Convert.ToDouble(dsItemData.Tables[0].Rows[0]["Discount"].ToString());

                        oLine.Price = Convert.ToDouble(dsItemData.Tables[0].Rows[0]["Price"].ToString());
                        oLine.VatGroup = dsItemData.Tables[0].Rows[0]["VatGourpSa"].ToString();


                        oLine.UoMCode = dsItemData.Tables[0].Rows[0]["UomCode"].ToString();

                        oLine.UnitPrice = (itm.FOC == "Yes") ? 0 : Convert.ToDouble(dsItemData.Tables[0].Rows[0]["Price"].ToString());
                        oLine.WarehouseCode = dsItemData.Tables[0].Rows[0]["Warehouse"].ToString();
                        
                        oLine.U_FOC = (itm.FOC == "Yes") ? "Y" : "N";
                        oLine.U_FocRemark = itm.FOCRemarks;
                        oLine.LineNum = iLine;
                    }

                   

                    //if (itm.DisPer > 0)
                    //{
                    //    itmPrice = itmPrice - (itmPrice * (itm.DisPer / 100));
                    //}

                    iLine++;

                    //oLine.U_normalqty = Convert.ToDouble(itm.Qty);
                    //oLine.U_Addbarcode = itm.Barcode;

                    oSeleOrdrReqst.DocumentLines.Add(oLine);


                }

                Document doc = null;


                doc = currentOdataService.AddSalesOrder(oSeleOrdrReqst);
                var msg = "<div style='width:800px;margin:auto;padding:40px;border:1px solid #abd1b8;'><h2>10XSF</h2><h3>Hello, " + oSODetails.Customer.CardName + "</h3><h4>Your sales order has been successfully created!</h4><div style='background:#abd1b8;padding:20px 10px;'><p>Sales Order No: <b style='color:green'>" + doc.DocNum + "</b></p></div></div>";
                //var email = this.getCustomerEmail(oSODetails.CardCode);
                //SendCodeToEmail("raju.m432@gmail.com", msg);
                //UpdateRequestTime();
                cnfMsg.IsSucess = true;
                cnfMsg.CnfsMsg = "Sales Order  Document Number :" + doc.DocNum + " Created Sucessfully";
                TempData["SuccessMessage"] = "Sales Order  Document Number :" + doc.DocNum + " Created Sucessfully";
                //    }
                //}
            }
            catch (Exception ex)
            {

                Utilities.SetResultMessage("SalesOrder" + ex.Message.ToString());

                string Excpn = "";
                if (ex.InnerException != null)
                {
                    Utilities.SetResultMessage("SalesOrder" + ex.Message.ToString());
                    Root rObjt = JsonConvert.DeserializeObject<Root>(ex.InnerException.Message);
                    Excpn = rObjt.error.message.value;
                    if (Excpn == "No matching records found (ODBC -2028)")
                    {
                        cnfMsg.IsSucess = true;
                        Excpn = "Sales order went for Approval Successfully";
                        //cnfMsg.CnfsMsg = "Sales order went for Approval Successfully";
                        cnfMsg.Ref = "APPROVAL";
                        TempData["SuccessMessage"] = "Sales order went for Approval Successfully";
                        return RedirectToAction("Upload");
                    }
                    else
                    {
                        cnfMsg.IsSucess = false;
                        Excpn = rObjt.error.message.value;
                    }

                    cnfMsg.CnfsMsg = Excpn;
                }
                else
                {
                    Excpn = ex.Message.ToString();
                }

                cnfMsg.CnfsMsg = Excpn;
            }
            //return Json(cnfMsg, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Upload");
            oSODetails.Customer.CardCode = oSODetails.Customer.CardCode ?? Request.Form["CustomerID"];
            oSODetails.Customer.CardName = oSODetails.Customer.CardName ?? Request.Form["customerAutocomplete"];

            custCode = oSODetails.Customer.CardCode ?? Request.Form["CustomerID"];

           
            if (oSODetails.Items == null)
                oSODetails.Items = new List<OrderItem>();
            return View(oSODetails);
        }

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
                currentConnectionInfo.UserName = (Session["SUser"] != "") ? Session["SUser"].ToString() : ConfigurationManager.AppSettings["UserName"];
                currentConnectionInfo.Password = (Session["SPass"] != "") ? Session["SPass"].ToString() : ConfigurationManager.AppSettings["Password"];

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
                return PartialView("~/Views/SalesOrderUpload/_ItemRow.cshtml", new ItemDetails { Index = id });
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

        private void LoadDropdowns()
        {
            ViewBag.SalesEmployeesList = GetSalesEmployeeList()
                .Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                });
            ViewBag.PaymentTermsList = GetPaymentTermsList()
              .Select(x => new SelectListItem
              {
                  Value = x.Value,
                  Text = x.Text
              });
            ViewBag.SeriesList = GetSeriesList()
            .Select(x => new SelectListItem
            {
                Value = x.Value,
                Text = x.Text
            });
            ViewBag.TerritoryList = GetTerritoryList()
            .Select(x => new SelectListItem
            {
                Value = x.Code,
                Text = x.Name
            });
        }

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
            string query = "SELECT Series,SeriesName, ISNULL(NextNumber,0) as NextNumber FROM NNM1 WHERE ObjectCode = '17' and Indicator like '%" + DateTime.Now.Year.ToString() + "%' ";

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
                    " (select \"Price\" from \"ITM1\" where \"ItemCode\" = '" + itemCode + "' and \"PriceList\" = '1') as \"Price\", " +
                    " (select \"OUDG\".\"Warehouse\" from \"OUSR\" inner join \"OUDG\" on \"OUDG\".\"Code\"=\"OUSR\".\"DfltsGroup\" " +
                    " where OUSR.\"USER_CODE\"='" + @Session["UserName"] + "') as \"Warehouse\"," +
                    " (select ISNULL(\"Discount\",0) from SPP1 " +
                    " where ListNum=16 and ItemCode='" + itemCode + "' and Cast(getdate() as date) between \"Fromdate\"" +
                    " and ISNULL(\"Todate\",getdate()))  as \"Discount\"" +
                    " FROM OITM T0 INNER JOIN OUOM T2 ON T0.\"InvntryUom\"=T2.\"UomCode\" " +
                    " INNER JOIN OITB T3 ON T0.\"ItmsGrpCod\"=T3.\"ItmsGrpCod\"" +
                    " WHERE (T0.\"ItemCode\"='" + itemCode + "') ";
                DataSet dsData = objHlpr.getDataSet(objitemquery);
                if (dsData.Tables[0].Rows.Count > 0)
                    objItemDetails = new ItemDetails()
                    {
                        ItemCode = dsData.Tables[0].Rows[0]["ItemCode"].ToString(),
                        ItemName = dsData.Tables[0].Rows[0]["ItemName"].ToString(),
                        DisPer = Convert.ToDecimal(dsData.Tables[0].Rows[0]["Discount"].ToString()),
                        Price = Convert.ToDecimal(dsData.Tables[0].Rows[0]["Price"].ToString()),
                        UomName = dsData.Tables[0].Rows[0]["UomCode"].ToString(),
                        Warehouse = dsData.Tables[0].Rows[0]["Warehouse"].ToString(),
                        VatGrpCode = dsData.Tables[0].Rows[0]["VatGourpSa"].ToString(),
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
                                WhsName = dr["WhsName"].ToString()
                                //InStock = DBNull.Value.Equals(dr["OnHand"]) ? 0 : Convert.ToDecimal(dr["OnHand"]),
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
                    //label = i.ItemCode + " - " + i.ItemName,
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