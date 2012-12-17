using System.Web.Mvc;

namespace Squawkings.Models
{
    public class FollowerInfo
    {
        public int Followers { get; set; }
        public int Followyees { get; set; }

        [HiddenInput]
        public bool Following { get; set; }

        public bool IsSameUser { get; set; }
    }
}