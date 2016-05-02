using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Anime_Portal.Helper
{
    public static class SecurityHelper
    {
        public static ConcurrentDictionary<string, DateTime> GetLoggedInUsers()
        {
            ConcurrentDictionary<string, DateTime> loggedInUsers = new ConcurrentDictionary<string, DateTime>();

            if (HttpContext.Current != null)
            {
                loggedInUsers = (ConcurrentDictionary<string, DateTime>)HttpContext.Current.Application["loggedinusers"];
                if (loggedInUsers == null)
                {
                    loggedInUsers = new ConcurrentDictionary<string, DateTime>();
                    HttpContext.Current.Application["loggedinusers"] = loggedInUsers;
                }
            }
            return loggedInUsers;

        }
    }
}