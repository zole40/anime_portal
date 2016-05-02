using Anime_Portal.Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

namespace Anime_Portal.Filters
{
    public class TrackLoginsFilter : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ConcurrentDictionary<string, DateTime> loggedInUsers = SecurityHelper.GetLoggedInUsers();

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (loggedInUsers.ContainsKey(HttpContext.Current.User.Identity.Name))
                {
                    loggedInUsers[HttpContext.Current.User.Identity.Name] = System.DateTime.Now;
                }
                else
                {
                    loggedInUsers.TryAdd(HttpContext.Current.User.Identity.Name, System.DateTime.Now);
                }

            }

            // remove users where time exceeds session timeout
            var keys = loggedInUsers.Where(u => DateTime.Now.Subtract(u.Value).Minutes >
                       HttpContext.Current.Session.Timeout).Select(u => u.Key);
            var time = new DateTime();
            foreach (var key in keys)
            {
                loggedInUsers.TryRemove(key, out time);
            }

        }
    }
}