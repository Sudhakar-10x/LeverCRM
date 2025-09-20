using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using _10xErp.Models; // adjust namespace

namespace _10xErp.Controllers
{
    public class ActivityController : Controller
    {
        private readonly HanaDataHelper _hana = new HanaDataHelper();

        // Index Page
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetActivityList()
        {
            string query = @"
        SELECT 
            T0.""ClgCode""      AS ""ActivityID"",
            T0.""CntctDate""    AS ""Date"",
            T0.""BeginTime""    AS ""Start"",
            T0.""ENDTime""      AS ""End"",
            T0.""CardCode""     AS ""CustomerCode"",
            T0.""CntctSbjct""   AS ""SubjectCode"",
            T0.""Details""      AS ""Details"",
            T0.""Action""       AS ""Type"",
            T0.""status""       AS ""Status"",
            T0.""SlpCode""      AS ""SalesEmployee""
        FROM ""OCLG"" T0
        WHERE T0.""CntctDate"" >= ADD_DAYS(CURRENT_DATE, -30)";

            DataSet ds = _hana.GetDataset(query, HanaDataHelper.HanaCmdType.SqlText);

            // Convert DataSet to JSON
            var activityList = new List<dynamic>();
            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    activityList.Add(new
                    {
                        ActivityID = row["ActivityID"],
                        Date = row["Date"] == DBNull.Value ? "" : row["Date"].ToString(), // <-- keep DB value as-is
                        Start = row["Start"],
                        End = row["End"],
                        CustomerCode = row["CustomerCode"],
                        SubjectCode = row["SubjectCode"],
                        Details = row["Details"],
                        Type = row["Type"],
                        Status = row["Status"],
                        SalesEmployee = row["SalesEmployee"]
                    });
                }
            }

            return Json(new { data = activityList }, JsonRequestBehavior.AllowGet);
        }

    }
}
