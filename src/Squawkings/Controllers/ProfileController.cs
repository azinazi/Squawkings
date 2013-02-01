using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using NPoco;
using Squawkings.Models;

namespace Squawkings.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IDatabase db;

        public ProfileController(IDatabase db)
        {
            this.db = db;
        }

        [Authorize]
        public ActionResult IndexById(int userId)
        {
            var user = db.SingleById<User>(userId);
            return RedirectToAction("Index", new { username = user.UserName });
        }

        [Authorize]
        public ActionResult Index(string username)
        {

            var profile =
                db.SingleOrDefault<ProfileSquawksInputModel>(
                    "select u.FirstName,u.LastName,u.UserName,u.Bio,u.UserId,u.AvatarUrl "
                    + "from Users u where u.UserName=@0 ", username);

            var followerInfo = new FollowerInfo();
            followerInfo.Followers = db.FirstOrDefault<int>("select count(*) from Followers where UserId = @0", profile.UserId);
            followerInfo.Followyees = db.FirstOrDefault<int>("select count(*) from Followers where FollowerUserId = @0", profile.UserId);
            followerInfo.Following = db.FirstOrDefault<bool>("select 1 from Followers where FollowerUserId = @0", User.Identity.Id());
            profile.FollowerInfo = followerInfo;

            var squawkTemplate = new SquawkTemplate();
            squawkTemplate.Where("u.UserName=@0 ", username);
            var squawkDisps = db.Fetch<SquawkDisp>(squawkTemplate.temp1);
            profile.SquawksList = squawkDisps;

            profile.FollowerInfo.IsSameUser = User.Identity.Id() == profile.UserId;

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Index(ProfileSquawksInputModel model)
        {

            var following =
                db.SingleOrDefault<int>(
                    "select count(*) from Followers where FollowerUserId = @0",
                    User.Identity.Id());

            var followers = new Followers();
            followers.FollowerUserId = User.Identity.Id();
            followers.UserId = model.UserId;

            if (following == 0)
            {
                model.FollowerInfo.Following = true;
                db.Insert(followers);
            }
            else
            {
                model.FollowerInfo.Following = false;
                db.Delete(followers);
            }
            return RedirectToAction("Index", "Profile");
        }

        [Authorize]
        public ActionResult Image()
        {
            return View(new ImageInputData());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Image(ImageInputData data)
        {
            var user = db.SingleOrDefaultById<User>(User.Identity.Id());
          //  var changes = db.StartSnapshot(user);

            if (data.IsGravator)
            {
                user.IsGravatar = true;
            }
            else if (data.Image != null)
            {
                data.Image.SaveAs(Server.MapPath("~/Content/dev_images/") + User.Identity.Name + ".jpg");
                user.AvatarUrl = "content/dev_images/" + User.Identity.Name + ".jpg";
            }

            //db.Update(user, changes.UpdatedColumns());
            db.Update(user);

            return RedirectToAction("IndexById", "Profile", new { userId = User.Identity.Id() });
        }
    }

    public class ImageInputData
    {
        public bool IsGravator { get; set; }
        public string Email { get; set; }

        public HttpPostedFileBase Image { get; set; }
    }

    public class ProfileSquawksInputModel
    {
        [HiddenInput]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AvatarUrl { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public bool IsGravatar { get; set; }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        public FollowerInfo FollowerInfo { get; set; }
        public List<SquawkDisp> SquawksList { get; set; }
    }

    public class FollowerInfo
    {
        public int Followers { get; set; }
        public int Followyees { get; set; }

        [HiddenInput]
        public bool Following { get; set; }

        public bool IsSameUser { get; set; }
    }

}