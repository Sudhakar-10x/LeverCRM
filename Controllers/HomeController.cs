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
        private readonly HanaDataHelper objHlpr = new HanaDataHelper();
        public ActionResult Index()
        {
            return View();
        }
    }
}