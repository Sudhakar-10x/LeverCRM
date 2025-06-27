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
using DocumentFormat.OpenXml.Spreadsheet;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using __10xErp;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Globalization;

namespace _10xErp.Controllers
{
	public class OutStandingController : Controller
    {
        private readonly DataHelper objHlpr = new DataHelper(); // Changed to private readonly instance  

        // GET: OutStanding
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetCustomerListForDropDown(string searchTerm)
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

                        string query = "SELECT distinct \"CardCode\" as \"CustomerID\", \"CardName\" as \"Name\", '' as \"PhoneNo\", \"E_Mail\" as \"EmailID\" " +
                            " FROM OCRD WHERE (CardName LIKE @SearchTerm OR CardCode LIKE @SearchTerm)";

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
                                    //CustomerID = row["CustomerID"].ToString(),
                                    //Name = row["Name"].ToString(),
                                    //PhoneNo = row["PhoneNo"].ToString(),
                                    //EmailID = row["EmailID"].ToString()
                                    label = row["CustomerID"] + " - " + row["Name"],
                                    value = row["Name"].ToString(),
                                    id = row["CustomerID"].ToString()
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
        public JsonResult GetSalesEmployeeListForDropDown(string searchTerm)
        {
            DataSet ds = new DataSet();
            List<object> SlpList = new List<object>();
            try
            {
                string sqlCon = System.Configuration.ConfigurationManager.AppSettings.Get("SqlCon").ToString();
                using (SqlConnection conn = new SqlConnection(sqlCon))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;

                        string query ="Select \"SlpCode\",\"SlpName\" from OSLP where \"Active\"='Y'" +
                            "and (SlpName LIKE @SearchTerm OR SlpCode LIKE @SearchTerm) ";

                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                SlpList.Add(new
                                {
                                    //Slpcode = row["SlpCode"].ToString(),
                                    //SlpName = row["SlpName"].ToString()
                                    label = row["SlpCode"] + " - " + row["SlpName"],
                                    value = row["SlpName"].ToString(),
                                    id = row["SlpCode"].ToString()
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
            return Json(SlpList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExportReportToPDF(
            string FromCustomerCode,
            string ToCustomerCode,
            string FromSalesPersonCode,
            string ToSalesPersonCode,
            string AsOnDate)
        {
            ReportDocument reportDoc = new ReportDocument();
            //string reportPath = Server.MapPath("~/Reports/Outstanding Statement New.rpt");
            string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", "Outstanding Statement.rpt");

            string sqlCon = System.Configuration.ConfigurationManager.AppSettings.Get("SqlCon").ToString();
            using (SqlConnection conn = new SqlConnection(sqlCon))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    TableLogOnInfo crTableLogOnInfo = new TableLogOnInfo();
                    CrystalDecisions.CrystalReports.Engine.Tables crTables;
                    ReportDocument cryRpt = new ReportDocument();

                    try
                    {
                        reportDoc.Load(reportPath);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error loading report: " + ex.Message + " at path: " + reportPath);
                    }


                    ConnectionInfo crConnectionInfo = new ConnectionInfo
                    {
                        ServerName = ConfigurationManager.AppSettings["Server"],
                        DatabaseName = ConfigurationManager.AppSettings["CompanyDB"],
                        UserID = ConfigurationManager.AppSettings["DBUserId"],
                        Password = ConfigurationManager.AppSettings["DBPassword"]
                    };
                    foreach (CrystalDecisions.CrystalReports.Engine.Table table in reportDoc.Database.Tables)
                    {
                        TableLogOnInfo tableLogOnInfo = table.LogOnInfo;
                        tableLogOnInfo.ConnectionInfo = crConnectionInfo;
                        table.ApplyLogOnInfo(tableLogOnInfo);
                    }
                    reportDoc.SetParameterValue("Fromcust", FromCustomerCode);
                    reportDoc.SetParameterValue("FromSlp", FromSalesPersonCode);
                    reportDoc.SetParameterValue("ToCust", ToCustomerCode);
                    reportDoc.SetParameterValue("ToSlp", ToSalesPersonCode);
                    DateTime parsedDate = DateTime.ParseExact(AsOnDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    reportDoc.SetParameterValue("TD", parsedDate);
                    
                    

                    // Export to PDF
                    Stream stream = reportDoc.ExportToStream(ExportFormatType.PortableDocFormat);
                    reportDoc.Close();
                    reportDoc.Dispose();

                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "OutstandingStatement.pdf");
                }
            }
                   
        }
    }
}