using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Squawkings.Controllers
{
    public class LogonController : Controller
    {
        //
        // GET: /Logon/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string username,string password)
        {
            if (username =="test" && password=="test")
            {
                FormsAuthentication.SetAuthCookie(username,false);
                return RedirectToAction("Home","Home");
            }
            else
            {
                ModelState.AddModelError("","Logon error happened");
                return View();
            }
        }

    }
}
