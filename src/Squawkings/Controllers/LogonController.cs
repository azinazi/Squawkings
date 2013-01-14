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
        private readonly IAuthentication auth;

        public LogonController(IDatabase db)
        {
            this.db = db;
        }

        public LogonController(IDatabase db, IAuthentication auth)
        {
            this.db = db;
            this.auth = auth;
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

            var user = db.SingleOrDefault<UserInfo>("select u.UserId,u.Password from UserSecurityInfo u inner join Users us on us.UserId = u.UserId where us.UserName=@0", inputModel.UserName);
            if (user != null)
            {

                if (Crypto.VerifyHashedPassword(user.Password, inputModel.Password))
                {
                    auth.Authenticate(user.UserId.ToString(), inputModel.rememberMe);
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "You cannot logon");
            return Index();

        }

        public ActionResult Logoff()
        {

            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Logon");
        }

    }

    public class UserInfo
    {
        public string Password { get; set; }
        public int UserId { get; set; }
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

    public interface IAuthentication
    {
        void Authenticate(string userName, bool rememberMe);
    }

    public class Authentication : IAuthentication
    {
        public void Authenticate(string userName, bool rememberMe)
        {
            FormsAuthentication.SetAuthCookie(userName, rememberMe);
        }

    }
}
