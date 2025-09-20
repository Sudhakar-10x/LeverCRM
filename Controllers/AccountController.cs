using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using _10xErp.Models;
using _10xErp.Helpers;
using System.Configuration;

namespace _10xErp
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly HanaDataHelper _hana;

        public AccountController()
        {
            

            _hana = new HanaDataHelper();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

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

                    string query = " SELECT \"USER_CODE\",\"U_Pass\" FROM \"OUSR\" A WHERE \"USER_CODE\" = '" + model.UserName + "' AND \"U_Pass\" = '" + model.Password + "'";
                    DataSet dt = _hana.GetDataset(query, HanaDataHelper.HanaCmdType.SqlText);

                    if (dt.Tables[0].Rows.Count > 0)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

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
                    ViewBag.Error = "Login failed: " + ex.Message;
                    return View(model);
                }
            }
        }

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
            return View();
        }
    }
}