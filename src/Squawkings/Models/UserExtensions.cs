using System;
using System.Security.Principal;

namespace Squawkings.Models
{
    public static class UserExtensions
    {
        public static int Id(this IIdentity identity)
        {
            return identity.IsAuthenticated ? Convert.ToInt32(identity.Name) : 0;
        }
    }
}