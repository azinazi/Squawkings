using System.Web.Mvc;
using NPoco;
using Squawkings.Models;

namespace Squawkings.Controllers
{
    public class GlobalController : Controller
    {
        private readonly IDatabase db;

        public GlobalController(IDatabase db)
        {
            this.db = db;
        }

        public ActionResult Index()
        {

            var SquawkTemplate = new SquawkTemplate();
            var squawkDisps = db.SkipTake<SquawkDisp>(0, 20, SquawkTemplate.temp1);

            return View(squawkDisps);
        }
    }
}