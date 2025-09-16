using _10xErp.Helpers;
using _10xErp.Models;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _10xErp.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataHelper objHlpr = new DataHelper();
        public ActionResult Index()
        {
            string salesPrson = Session["salesPrson"]?.ToString();

            var model = new SalesQuotationViewModel();
            //DataSet ds = new DataSet();
            
            try
            {
                //\"CardCode\" = '" + supplierCode + "' AND
                //\"CardCode\" = '" + supplierCode + "' AND

                //string sqlQry = "Select (SELECT COUNT(*) FROM OQUT WHERE  \"DocStatus\" = 'O') as \"OpenQuotations\", " +
                //                          "(SELECT COUNT(*) FROM OQUT WHERE  \"DocStatus\" = 'C') as \"ConvertedOrders\", " +
                //                          "(SELECT DATENAME(MONTH, DocDate) as MonthName, COUNT(*) as Cnt FROM OQUT GROUP BY DATENAME(MONTH, DocDate), MONTH(DocDate) ORDER BY MONTH(DocDate)) as \"MonthlyQuotations\", " +
                //                          "(SELECT DATENAME(MONTH, DocDate) as MonthName, COUNT(*) as Cnt FROM ORDR GROUP BY DATENAME(MONTH, DocDate), MONTH(DocDate) ORDER BY MONTH(DocDate)) as \"MonthlyOrders\" " +
                //                          "FROM DUMMY";

                string sqlQry = "Select (SELECT COUNT(*) FROM OQUT WHERE \"SlpCode\" = '" + salesPrson + "' and \"DocStatus\" = 'O') as \"OpenQuotations\", " +
                                          "(SELECT COUNT(*) FROM OQUT WHERE \"SlpCode\" = '" + salesPrson + "' and \"DocStatus\" = 'C') as \"ConvertedOrders\" ";

                DataSet ds = objHlpr.getDataSet(sqlQry);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    model.OpenQuotations = Convert.ToInt32(ds.Tables[0].Rows[0]["OpenQuotations"].ToString());
                    model.ConvertedOrders = Convert.ToInt32(ds.Tables[0].Rows[0]["ConvertedOrders"].ToString());
                    model.MonthlyQuotations = GetMonthlyData("SELECT DATENAME(MONTH, DocDate) as MonthName, COUNT(*) as Cnt FROM OQUT Where \"SlpCode\" = '" + salesPrson + "' GROUP BY DATENAME(MONTH, DocDate), MONTH(DocDate) ORDER BY MONTH(DocDate)");
                    model.MonthlyOrders = GetMonthlyData("SELECT DATENAME(MONTH, DocDate) as MonthName, COUNT(*) as Cnt FROM ORDR Where \"SlpCode\" = '" + salesPrson + "' GROUP BY DATENAME(MONTH, DocDate), MONTH(DocDate) ORDER BY MONTH(DocDate)");
                    //model.MonthlyQuotations = Convert.ToInt32(ds.Tables[0].Rows[0]["MonthlyQuotations"].ToString());
                    //model.MonthlyOrders = Convert.ToInt32(ds.Tables[0].Rows[0]["MonthlyOrders"].ToString());
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Dashboard error: " + ex.Message);
            }

            return View(model);
        }

        private List<DataChart> GetMonthlyData(string query)
        {
            var data = new List<DataChart>();
            string sqlCon = ConfigurationManager.AppSettings["SqlCon"].ToString();
            using (SqlConnection con = new SqlConnection(sqlCon))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    data.Add(new DataChart
                    {
                        Label = rdr["MonthName"].ToString(),
                        Value = Convert.ToInt32(rdr["Cnt"])
                    });
                }
            }
            return data;
        }

        public JsonResult GetTopItems()
        {
            string supplierCode = Session["SupplierId"]?.ToString();
            var data = GetTopPurchasedItems(supplierCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private ChartDataModel GetTopPurchasedItems(string supplierCode)
        {
            var model = new ChartDataModel
            {
                Labels = new List<string>(),
                Values = new List<decimal>()
            };

            string query = " SELECT TOP 5 T1.\"ItemCode\", SUM(T1.\"Quantity\") AS \"Qty\" " +
            " FROM POR1 T1 INNER JOIN OPOR T0 ON T0.\"DocEntry\" = T1.\"DocEntry\" " +
            " WHERE T0.\"CardCode\" = '" + supplierCode + "' GROUP BY T1.\"ItemCode\" ORDER BY \"Qty\" DESC";

            string sqlCon = ConfigurationManager.AppSettings["SqlCon"].ToString();
            using (SqlConnection con = new SqlConnection(sqlCon))
            using (SqlCommand cmd = new SqlCommand(query, con))
  
            {
                //cmd.Parameters.AddWithValue("@CardCode", supplierCode);
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.Labels.Add(reader["ItemCode"].ToString());
                    model.Values.Add(Convert.ToDecimal(reader["Qty"]));
                }
                con.Close();
            }

            return model;
        }

        public JsonResult GetMonthlyPurchaseTrend()
        {
            string supplierCode = Session["SupplierId"]?.ToString();
            var data = GetMonthlyPurchaseData(supplierCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private ChartDataModel GetMonthlyPurchaseData(string supplierCode)
        {
            var model = new ChartDataModel
            {
                Labels = new List<string>(),
                Values = new List<decimal>()
            };

            //string query = " SELECT DATENAME(MONTH, T0.\"DocDate\") AS \"Month\", SUM(T0.\"DocTotal\") AS \"Total\" " +
            //" FROM OPOR T0 WHERE T0.\"CardCode\" = '" + supplierCode + "' AND YEAR(T0.\"DocDate\") = YEAR(NOW()) " +
            //" GROUP BY DATENAME(MONTH, T0.\"DocDate\"), MONTH(T0.\"DocDate\") ORDER BY MONTH(T0.\"DocDate\") ";

            string query = "SELECT TO_VARCHAR(T0.\"DocDate\", 'Month') AS \"Month\", SUM(T0.\"DocTotal\") AS \"Total\" " +
            " FROM OPOR T0 WHERE T0.\"CardCode\" = '" + supplierCode + "' AND YEAR(T0.\"DocDate\") = YEAR(CURRENT_DATE) " +
            " GROUP BY TO_VARCHAR(T0.\"DocDate\", 'Month'), MONTH(T0.\"DocDate\") ORDER BY MONTH(T0.\"DocDate\")";

            string sqlCon = ConfigurationManager.AppSettings["SqlCon"].ToString();
            using (SqlConnection con = new SqlConnection(sqlCon))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                //cmd.Parameters.AddWithValue("@CardCode", supplierCode);
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.Labels.Add(reader["Month"].ToString());
                    model.Values.Add(Convert.ToDecimal(reader["Total"]));
                }
                con.Close();
            }

            return model;
        }

        public ActionResult Minor()
        {
            ViewData["SubTitle"] = "Simple example of second view";
            ViewData["Message"] = "Data are passing to view by ViewData from controller";

            return View();
        }
    }
}