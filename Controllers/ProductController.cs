using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _10xErp.Models;
using _10xErp.Helpers;
using System.Configuration;
using System.Data.SqlClient;

namespace _10xErp.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataHelper objHlpr = new DataHelper();
        // GET: Product
        public ActionResult Index()
        {
            //List<ProductDetailsModel> productsList = GetProductList();

            List<ProductDetailsModel> productsList = new List<ProductDetailsModel>();


            ViewBag.Current = "Product";
            return View(productsList);


        }

        public ActionResult ViewProductDetails(string itemCode)
        {
            ViewBag.Current = "View";
            ProductDetailsModel productDetails = new ProductDetailsModel();
            productDetails = GetProductDetails(itemCode);
            return View(productDetails);
        }

        private List<ProductDetailsModel> GetProductListOLD()
        {
            List<ProductDetailsModel> lstItems = new List<ProductDetailsModel>();

            try
            {

                var sqlQry = "select distinct T1.\"ItemCode\", T1.\"ItemName\", T5.\"ItmsGrpNam\", T1.\"CodeBars\" " +
                    " from OITM T1 Inner join OBCD T0 on T0.\"ItemCode\"=T1.\"ItemCode\" " +
                    " Inner Join OITB T5 on T5.\"ItmsGrpCod\"=T1.\"ItmsGrpCod\" " +
                    " left JOIN ITM2 T3 on T3.\"ItemCode\"=T1.\"ItemCode\" where T1.\"validFor\"='Y' order by T1.\"ItemCode\" ";

                DataSet ds = objHlpr.getDataSet(sqlQry);
                var si = 0;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    ProductDetailsModel docObj = new ProductDetailsModel()
                    {

                        ItemCode = dr["ItemCode"].ToString(),
                        CodeBars = dr["CodeBars"].ToString(),
                        ItmsGrpNam = dr["ItmsGrpNam"].ToString(),
                        ItemName = DBNull.Value.Equals(dr["ItemName"]) ? "" : dr["ItemName"].ToString()
                    };
                    docObj.lstInvDetails = GetInventoryDetails(dr["ItemCode"].ToString());
                    lstItems.Add(docObj);
                    //si++;
                    //if (si == 200)
                    //{
                    //    break;
                    //}
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }


            return lstItems;
        }

        public JsonResult GetProductList()
        {
            List<ProductDetailsModel> itemList = new List<ProductDetailsModel>();
            DataSet ds = new DataSet();

            try
            {
                string sqlCon = ConfigurationManager.AppSettings["SqlCon"].ToString();
                using (SqlConnection conn = new SqlConnection(sqlCon))
                {
                    var query = "select distinct T1.\"ItemCode\", T1.\"ItemName\", T5.\"ItmsGrpNam\", T1.\"CodeBars\" " +
                    " from OITM T1 Inner join OBCD T0 on T0.\"ItemCode\"=T1.\"ItemCode\" " +
                    " Inner Join OITB T5 on T5.\"ItmsGrpCod\"=T1.\"ItmsGrpCod\" " +
                    " left JOIN ITM2 T3 on T3.\"ItemCode\"=T1.\"ItemCode\" where T1.\"validFor\"='Y' order by T1.\"ItemCode\" ";


                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds);

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            itemList.Add(new ProductDetailsModel
                            {
                                ItemCode = row["ItemCode"].ToString(),
                                ItemName = row["ItemName"].ToString(),
                                ItmsGrpNam = row["ItmsGrpNam"].ToString(),
                                CodeBars = row["CodeBars"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Optionally log error
                Console.WriteLine(ex.Message);
            }

            return Json(new { data = itemList }, JsonRequestBehavior.AllowGet);
        }

        public ProductDetailsModel GetProductDetails(string itemCode)
        {
            ConfirmMsg cnfMsg = new ConfirmMsg();
            //List<ProductDetailsModel> lstItems = new List<ProductDetailsModel>();
            ProductDetailsModel objItem = new ProductDetailsModel();
            try
            {

                var sqlQry = "select T1.\"ItemCode\",T0.\"BcdCode\", T1.\"ItemName\",T1.\"InvntryUom\",T1.\"PriceUnit\",T5.\"ItmsGrpNam\", " +
                    "(select \"UgpName\" from OUGP where \"UgpEntry\"= t1.\"UgpEntry\")as \"Uomgrp\", '' as \"Pricelist\",T1.\"CodeBars\"," +
                    "(select \"FirmName\" from OMRC where \"FirmCode\"= T1.\"FirmCode\") as \"Manufacturer\",T1.\"SWW\", " +
                    "(select \"TrnspName\" from OSHP where \"TrnspCode\"=T1.\"ShipType\") as \"ShippingType\", " +
                    "(Case when T1.\"ManBtchNum\"='Y' then 'Batches' when T1.\"ManSerNum\"='Y' then 'Serial Numbers' else '' end) as \"ManagedBy\"," +
                    "(Case when T1.\"MngMethod\"='A' then 'On Every Transacation' else 'On Release Only' end) as \"MngMethod\"," +
                    "(case when T1.\"IssuePriBy\"=0 then 'Serial and Batch Numbers' else 'Bin Locations' end) as \"IssueprimalyBy\", " +
                    "(case when T1.\"validFor\"='Y' then 'Active' else 'Inactive' end) as \"ItemStatus\", T1.\"FrgnName\"," +
                    "(Case when T1.\"ItemType\" ='I' then 'Items' when T1.\"ItemType\" ='L' then 'Labour' else 'Travel' end) as \"ItemType\",T1.\"CardCode\"," +
                    "(select \"Name\" from OVTG where \"Code\"= T1.\"VatGroupPu\") as \"VATGROUPPU\",T1.\"SalUnitMsr\"," +
                    "(select \"Name\" from OVTG where \"Code\"= T1.\"VatGourpSa\") as \"VATGROUPSA\",T1.\"InvntryUom\",T1.\"INUoMEntry\",T1.\"U_MOHRegCode\"," +
                    " T1.\"U_MOHRegExp\",(select \"Descr\" from UFD1 where TableID='OITM' and FldValue=T1.\"U_SLFMode\" and FieldID=3) as \"U_SLFMode\"," +
                    " T1.\"U_SLF\",T1.\"U_SuppCode\",T1.\"U_PrincCoy\",T1.\"U_Agency\",T1.\"U_DelSys\"," +
                    " T1.\"U_COO\",T1.\"U_SalesRepName\",T1.\"U_Disc\" from OITM T1 " +
                    " Inner join OBCD T0 on T0.\"ItemCode\"=T1.\"ItemCode\" Inner Join OITB T5 on T5.\"ItmsGrpCod\"=T1.\"ItmsGrpCod\" " +
                    "left JOIN ITM2 T3 on T3.\"ItemCode\"=T1.\"ItemCode\" " +
                    " where T1.\"validFor\"='Y' and T1.\"ItemCode\" = '" + itemCode + "' order by T1.\"ItemCode\" ";

                DataSet ds = objHlpr.getDataSet(sqlQry);
                var si = 0;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    ProductDetailsModel docObj = new ProductDetailsModel()
                    {

                        ItemCode = dr["ItemCode"].ToString(),
                        CodeBars = dr["CodeBars"].ToString(),
                        ItmsGrpNam = dr["ItmsGrpNam"].ToString(),
                        //WhsCode = dr["WhsCode"].ToString(),
                        //WhsName = dr["WhsName"].ToString(),
                        ItemName = DBNull.Value.Equals(dr["ItemName"]) ? "" : dr["ItemName"].ToString(),
                        //InStock = DBNull.Value.Equals(dr["OnHand"]) ? 0 : Convert.ToDecimal(dr["OnHand"]),
                        Price = DBNull.Value.Equals(dr["PriceUnit"]) ? 0 : Convert.ToDecimal(dr["PriceUnit"]),
                        UomName = DBNull.Value.Equals(dr["InvntryUom"]) ? "" : Convert.ToString(dr["InvntryUom"]),
                        FrgnName = dr["FrgnName"].ToString(),
                        ItemType = dr["ItemType"].ToString(),
                        Uomgrp = dr["Uomgrp"].ToString(),
                        Manufacturer = dr["Manufacturer"].ToString(),
                        ManagedBy = dr["ManagedBy"].ToString(),
                        ManagementMethod = dr["MngMethod"].ToString(),
                        IssueprimalyBy = dr["IssueprimalyBy"].ToString(),
                        Itemstatus = dr["ItemStatus"].ToString(),
                        ShippingType = dr["ShippingType"].ToString(),
                        Prefvendor = dr["CardCode"].ToString(),
                        PurVatGroup = dr["VATGROUPPU"].ToString(),
                        SaleUomcode = dr["SalUnitMsr"].ToString(),
                        SalVatGroup = dr["VATGROUPSA"].ToString(),
                        InvUomcode = dr["InvntryUom"].ToString(),
                        MOHRegCode = dr["U_MOHRegCode"].ToString(),
                        MOHRegExp = dr["U_MOHRegExp"].ToString(),
                        ShelfLife = dr["U_SLF"].ToString(),
                        ShelfLifeMode = dr["U_SLFMode"].ToString(),
                        Prency = dr["U_PrincCoy"].ToString(),
                        Supplier = dr["U_SuppCode"].ToString(),
                        DelSys = dr["U_DelSys"].ToString(),
                        Agency = dr["U_Agency"].ToString(),
                        COO = dr["U_COO"].ToString(),
                        SalesRepName = dr["U_SalesRepName"].ToString(),
                        Disc = dr["U_Disc"].ToString(),
                    };
                    objItem = docObj;
                    objItem.lstInvDetails = GetInventoryDetails(dr["ItemCode"].ToString());

                    //docObj.lstInvDetails = GetInventoryDetails(dr["ItemCode"].ToString());
                    //lstItems.Add(docObj);
                    //si++;
                    //if (si == 200)
                    //{
                    //    break;
                    //}
                }



            }
            catch (Exception ex)
            {

            }

            return objItem;

        }

        public List<InventoryDetails> GetInventoryDetails(string ItemCode)
        {
            List<InventoryDetails> lstInvntoryDts = new List<InventoryDetails>();
            try
            {
                if (ItemCode != null)
                {
                    var Qry = "select T1.\"WhsCode\",T1.\"WhsName\",T0.\"OnHand\",T0.\"IsCommited\",T0.\"OnOrder\" from OITW T0 " +
                        " inner join OWHS T1 on T1.\"WhsCode\"=T0.\"WhsCode\" where T0.\"ItemCode\"='" + ItemCode + "'";

                    var ds = objHlpr.getDataSet(Qry);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        InventoryDetails obj = new InventoryDetails
                        {
                            WhsCode = dr["WhsCode"].ToString(),
                            WhsName = dr["WhsName"].ToString(),
                            InStock = DBNull.Value.Equals(dr["OnHand"]) ? 0 : Convert.ToDecimal(dr["OnHand"]),
                            Ordered = DBNull.Value.Equals(dr["OnOrder"]) ? 0 : Convert.ToDecimal(dr["OnOrder"]),
                            Commited = DBNull.Value.Equals(dr["IsCommited"]) ? 0 : Convert.ToDecimal(dr["IsCommited"])
                        };
                        lstInvntoryDts.Add(obj);
                    }


                }
            }
            catch (Exception e)
            {
            }
            finally
            {

            }
            return lstInvntoryDts;

        }


    }
}