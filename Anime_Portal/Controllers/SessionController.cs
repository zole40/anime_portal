using Bll;
using Bll.DTO;
using Bll.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anime_Portal.Controllers
{
    [Authorize]
    public class SessionController : Controller
    {

        [AllowAnonymous]
        public ActionResult getSession(string friendlyUrl)
        {
            var session = SessionManager.getSessions(friendlyUrl);
            return PartialView("Session", session);
        }

        [AllowAnonymous]
        public JsonResult getSessionJson(string friendlyUrl)
        {
            var session = SessionManager.getSessions(friendlyUrl);
            return Json(session, JsonRequestBehavior.AllowGet);
        }



        // POST: Session/Create
        [HttpPost]
        private ActionResult Create(string friendlyUrl, int? number)
        {
            if (number != null)
            {
                using (var context = new AnimePortalEntities())
                {
                    var animeId = context.Animes.FirstOrDefault(anime => anime.FriendlyUrl.Equals(friendlyUrl)).Id;
                    context.Session.Add(new Session()
                    {

                        Number = (int)number,
                        AnimeID = animeId
                    });
                    context.SaveChanges();
                }
                // TODO: Add insert logic here
            }
            return RedirectToAction("Details", "Animes", new { friendlyUrl = friendlyUrl });
        }


        // POST: Session/Delete/5
        [HttpPost]
        private ActionResult Delete(string friendlyUrl, int? number)
        {
            using (var context = new AnimePortalEntities())
            {
                var animeId = context.Animes.FirstOrDefault(anime => anime.FriendlyUrl.Equals(friendlyUrl)).Id;
                var session = context.Session.FirstOrDefault(sess => sess.AnimeID == animeId && sess.Number == number);
                if (VideoManager.remove(session.Id))
                {
                    context.Session.Remove(session);
                    context.SaveChanges();
                }
                return RedirectToAction("Details", "Animes", new { friendlyUrl = friendlyUrl });
            }
        }

        [HttpPost]
        public ActionResult Sessions(string friendlyUrl, string type, int?[] number)
        {
            if (type.Equals("Add"))
            {
                return Create(friendlyUrl, number[0]);
            }

          else  if (type.Equals("Delete"))
            {
                return Delete(friendlyUrl, number[1]);
            }

            return RedirectToAction("Details", "Animes", new { friendlyUrl = friendlyUrl });
        }
    }
}
