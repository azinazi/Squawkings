using System.Collections.Generic;
using System.Web.Mvc;

namespace Squawkings.Models
{
    public class ProfileSquawks
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

        public FollowerInfo followerInfo { get; set; }
        public List<SquawkDisp> SquawksList { get; set; }
    }
}