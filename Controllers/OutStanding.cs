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

namespace _10xErp.Controllers
{
	public class OutStandingController : Controller
    {
        private readonly DataHelper objHlpr = new DataHelper(); // Changed to private readonly instance  

        // GET: OutStanding
        public ActionResult Index()
        {
            SalesOrderViewModel oSalesOrderModel = new SalesOrderViewModel();

            //ViewBag.Current = "SalesOrder";
            return View(oSalesOrderModel);
        }
    }
}