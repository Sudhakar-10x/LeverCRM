using _10xErp;
using _10xErp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace __10xErp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            SalesOrderController objNew = new SalesOrderController();
            ServiceLayerServices currentOdataService = objNew.MainLoginLogoutAction(true);
            System.Web.HttpContext.Current.Application.Lock();
            System.Web.HttpContext.Current.Application["sapAppGlobal"] = currentOdataService;
            System.Web.HttpContext.Current.Application.UnLock();
        }
    }
}
