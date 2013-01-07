using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Security;
using FluentValidation;
using NPoco;
using Squawkings.Models;

namespace Squawkings.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDatabase db;

        public HomeController(IDatabase db)
        {
            this.db = db;
        }

        [OutputCache(Duration = 10)]
        public ActionResult Total()
        {
            var total = new Total();
            total.TotalSquawks = db.SingleOrDefault<string>("select count(*) from Squawks ");
            total.TotalUsers = db.SingleOrDefault<string>("select count(*) from Users ");
            return PartialView(total);
        }

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Global");

            var SquawkTemplate = new SquawkTemplate();
            SquawkTemplate.Where("u.UserId=@0 or u.UserId in (select UserId from Followers where FollowerUserId=@0)", User.Identity.Name);
            var squawkDisps = db.SkipTake<SquawkDisp>(0,20,SquawkTemplate.temp1);

            var squawks = new SquawksInputModel();
            squawks.SquawksList = squawkDisps;
            return View(squawks);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SquawkInputModel squawksInputModel)
        {
            if (!ModelState.IsValid)
                return Index();

            var squawk = new Squawk();
            squawk.Content = squawksInputModel.Squawk;
            squawk.UserId = User.Identity.Id();
            squawk.CreatedAt = DateTime.UtcNow;

            db.Insert(squawk);

            return RedirectToAction("Index", "Home");
        }
    }

    public class Total
    {
        public string TotalUsers { set; get; }
        public string TotalSquawks { set; get; }
    }

    public class SquawksInputModel
    {
        public List<SquawkDisp> SquawksList { get; set; }

        [DataType(DataType.Text)]
        public string Squawk { get; set; }
    }



    [FluentValidation.Attributes.Validator(typeof(HomeValidator))]
    public class SquawkInputModel
    {
        [DataType(DataType.Text)]
        public string Squawk { get; set; }
    }

    public class HomeValidator : AbstractValidator<SquawkInputModel>
    {

        public HomeValidator()
        {
            RuleFor(s => s.Squawk)
                .NotEmpty()
                .Length(0, 400)
                .WithMessage("length shouldnt exceed 400 chars.");
        }

    }
}