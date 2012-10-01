using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Squawkings.Controllers
{

    public class HomeController : Controller
    {
        private static List<SquawkDisp> GetSquawkDisps()
        {
            var squawks = new List<SquawkDisp>
					  {
						  new SquawkDisp() { Username = "test", FullName = "Test User", Time = DateTime.Now.AddMinutes(0), Content = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."},
						  new SquawkDisp() { Username = "test2", FullName = "Test Two", Time = DateTime.Now.AddMinutes(-1), Content = "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."},
						  new SquawkDisp() { Username = "test", FullName = "Test User", Time = DateTime.Now.AddMinutes(-2), Content = "Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."},
						  new SquawkDisp() { Username = "test2", FullName = "Test Two", Time = DateTime.Now.AddMinutes(-3), Content = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."},
						  new SquawkDisp() { Username = "test2", FullName = "Test Two", Time = DateTime.Now.AddMinutes(-4), Content = "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."},
						  new SquawkDisp() { Username = "test", FullName = "Test User", Time = DateTime.Now.AddMinutes(-5), Content = "Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."},
					  };
            return squawks;
        }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            List<SquawkDisp> squawkDisps = GetSquawkDisps();

            return View(squawkDisps);
        }

    }

    public class SquawkDisp
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public string AvatarUrl { get; set; }
    }
}
