using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using _10xErp.Models;
using _10xErp.Helpers;

namespace __10xErp
{
    [Authorize]
    public class AccountController : Controller
    {
        DataHelper objHlpr = new DataHelper();
        public AccountController()
        {
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        // POST: Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                try
                {
                    //   string query = " SELECT T1.\"Code\", T0.\"USER_CODE\",T1.\"BPLId\", T1.\"Warehouse\", T2.\"WhsName\",T0.\"U_SUser\"," +
                    //" T0.\"U_SPass\" FROM OUSR T0 LEFT JOIN OUDG T1 ON T0.\"DfltsGroup\" = T1.\"Code\" LEFT JOIN OWHS T2 ON T1.\"Warehouse\" = T2.\"WhsCode\" " +
                    //" where LOWER(T0.\"USER_CODE\")='" + model.UserName + "' and T0.\"U_Password\"= '" + model.Password + "' ";

                    //string query = " Select \"empID\", \"salesPrson\", " +
                    //" ISNULL(\"firstName\",'') + ' ' + ISNULL(\"middleName\",'') + ' ' + ISNULL(\"lastName\",'') as \"EmpName\" From OHEM " +
                    //" where \"ExtEmpNo\" = '" + model.UserName + "' and \"U_Password\" = '" + model.Password + "' ";

                    string query = "Select \"empID\", \"salesPrson\", ISNULL(\"U_PriceDet\",'N') as \"PriceDet\"," +
                    " ISNULL(\"firstName\",'') + ' ' + ISNULL(\"middleName\",'') + ' ' + ISNULL(\"lastName\",'') as \"EmpName\" " +
                    " From OHEM A LEFT JOIN OUSR B ON A.userId = B.USERID " +
                    " where \"ExtEmpNo\" = '" + model.UserName + "' and \"U_Password\" = '" + model.Password + "' ";
                    DataSet dsData = objHlpr.getDataSet(query);
                    if (dsData.Tables[0].Rows.Count > 0)
                    {
                        Session["UserName"] = model.UserName;
                        Session["CompanyName"] = System.Configuration.ConfigurationManager.AppSettings.Get("CompanyName").ToString();
                        Session["empID"] = dsData.Tables[0].Rows[0]["empID"].ToString();
                        Session["EmpName"] = dsData.Tables[0].Rows[0]["EmpName"].ToString();
                        Session["salesPrson"] = dsData.Tables[0].Rows[0]["salesPrson"].ToString();
                        Session["PriceDet"] = dsData.Tables[0].Rows[0]["PriceDet"].ToString();

                        // Optionally store the user's credentials for the "Remember Me" feature
                        if (model.RememberMe)
                        {
                            FormsAuthentication.SetAuthCookie(model.UserName, true);
                        }
                        else
                        {
                            FormsAuthentication.SetAuthCookie(model.UserName, false);
                        }
                        // Redirect to a home or dashboard page
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Error = "Invalid Username or Password.";
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }
            return View(model);
        }

        // Logout action
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult KeepAlive()
        {
            Session["KeepAlive"] = DateTime.Now;
            return new EmptyResult();
        }

        public ActionResult SessionExpired()
        {
            return View(); // Create a simple view to show "Session expired" message
        }

    }
    public class ImageViewModel
    {
        public string BsImg { get; set; }
        public Int32 DocNum { get; set; }
        public string CardName { get; set; }

    }
    public class RSDBObj
    {
        public string CompanyDB { get; set; }
        public string UserName { get; set; }
        //public string DBInstance { get; set; }
        public string Password { get; set; }
    }

    public class ActivityRptObj
    {
        public string name { get; set; }
        public string type { get; set; }
        public List<List<string>> value { get; set; }
    }
}