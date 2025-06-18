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
    public class CreditMemoController : Controller
    {
        private readonly DataHelper objHlpr = new DataHelper(); // Changed to private readonly instance  
        // GET: CreditMemo
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
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
                    " WHERE T0.\"ItemCode\" = '" + dsData.Tables[0].Rows[0]["ItemCode"].ToString() + "' " +
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
    }
}