using Anime_Portal.Filters;
using System.Web;
using System.Web.Mvc;

namespace Anime_Portal
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            GlobalFilters.Filters.Add(new TrackLoginsFilter());
        }
    }
}
