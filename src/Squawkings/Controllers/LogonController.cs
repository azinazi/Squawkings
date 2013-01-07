using System.Web.Mvc;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;
using NPoco;

namespace Squawkings.Controllers
{
    public class LogonController : Controller
    {
        private readonly IDatabase db;

        public LogonController(IDatabase db)
        {
            this.db = db;
        }

        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LogonInputModel inputModel)
        {
            if (!ModelState.IsValid)
                return Index();

            bool passwordValid = false;
            LogonInputModel user = db.SingleOrDefault<LogonInputModel>("select us.UserName,u.UserId,u.Password from UserSecurityInfo u inner join Users us on us.UserId = u.UserId where us.UserName=@0", inputModel.UserName);
            if (user != null)
            {

                if (Crypto.VerifyHashedPassword(user.Password, inputModel.Password))
                    passwordValid = true;
            }
            else
            {
                
                //User newUser = new User();
                //newUser.UserName= inputData.UserName;
                //Int32 newUserId = (Int32) db.Insert(newUser);
                //UserSecurityInfo userSecurityInfo = new UserSecurityInfo();
                //userSecurityInfo.UserId = newUserId;
                //userSecurityInfo.Password = Crypto.HashPassword(inputData.Password);
                //db.Insert(userSecurityInfo);
                //FormsAuthentication.SetAuthCookie(newUserId.ToString(), user.rememberMe);
                return RedirectToAction("Index", "Home");
                
            }

            if (passwordValid)
            {

                FormsAuthentication.SetAuthCookie(user.UserId.ToString(), user.rememberMe);
                return RedirectToAction("Index","Home");
            }
            else
            {
                ModelState.AddModelError("", "You cannot logon");
                return Index();
            }
        }

        public ActionResult Logoff()
        {

            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Logon");
        }

    }


    public class LogonInputModel
    {
        [Required]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
        public bool rememberMe { get; set; }
        public int UserId { get; set; }
        [HiddenInput]
        public string ReturnUrl { get; set; }
    }
}
