using System.Collections.Generic;
using System.Web;
using System.Web.Helpers;
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
                db.SingleOrDefault<ProfileSquawks>(
                    "select u.FirstName,u.LastName,u.UserName,u.Bio,u.UserId,u.AvatarUrl "
                    + "from Users u where u.UserName=@0 ", username);

            var followerInfo = new FollowerInfo();
            followerInfo.Followers = db.FirstOrDefault<int>("select count(*) from Followers where UserId = @0", profile.UserId);
            followerInfo.Followyees = db.FirstOrDefault<int>("select count(*) from Followers where FollowerUserId = @0", profile.UserId);
            followerInfo.Following = db.FirstOrDefault<bool>("select 1 from Followers where FollowerUserId = @0", User.Identity.Id());
            profile.followerInfo = followerInfo;

            var SquawkTemplate = new SquawkTemplate();
            SquawkTemplate.Where("u.UserName=@0 ", username);
            var squawkDisps = db.Fetch<SquawkDisp>(SquawkTemplate.temp1);
            profile.SquawksList = squawkDisps;

            profile.followerInfo.IsSameUser = User.Identity.Id() == profile.UserId;

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Index(ProfileSquawks model)
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
                model.followerInfo.Following = true;
                db.Insert(followers);
            }
            else
            {
                model.followerInfo.Following = false;
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
            var changes = Snapshotter.Start(user);

            if (data.IsGravator)
            {
                var hashedEmail = Crypto.Hash(user.Email, "md5").ToLower();
                user.AvatarUrl = "http://www.gravatar.com/avatar/" + hashedEmail + ".png";
                user.IsGravatar = true;
            }
            else if (data.Image != null)
            {
                string url = Server.MapPath("~/Content/dev_images/") + User.Identity.Name + ".jpg";
                data.Image.SaveAs(url);

                user.AvatarUrl = "content/dev_images/" + User.Identity.Name + ".jpg";
            }

            db.Update(user, changes.UpdatedColumns());

            return RedirectToAction("IndexById", "Profile", new { userId = User.Identity.Id() });
        }
    }

    public class ImageInputData
    {
        public bool IsGravator { get; set; }
        public string Email { get; set; }

        public HttpPostedFileBase Image { get; set; }
    }
}