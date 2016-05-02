using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anime_Portal.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public bool Index()
        {
            return true;
        }

    }
}