using Anime_Portal.Helplers;
using Bll;
using Bll.DTO.VideoModel;
using Bll.Manager;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace Anime_Portal.Controllers
{
    [Authorize]
    public class VideosController : Controller
    {

        [AllowAnonymous]
        public ActionResult ListVideos(int id)
        {
            var videos = VideoManager.getVideos(id);
            return PartialView(videos);
        }

        [AllowAnonymous]
        public ActionResult ListByUser(string userName)
        {
            var usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = usermanager.FindByName(userName);

            var videos = VideoManager.getVideosByUser(user.Id);
            return PartialView("ListVideos", videos);
        }


        [AllowAnonymous]
        public ActionResult Similar(int id, int videoId)
        {
            var videos = VideoManager.getSimilarVideos(id, videoId);
            return PartialView(videos);
        }

        // GET: Videos/Details/5

        [AllowAnonymous]
        public ActionResult Details(int? videoId)
        {
            var model = VideoManager.getVideoData(videoId, User.Identity.GetUserId());
            if(model == null)
            {
                return RedirectToAction("Index", "Animes");
            }
            var usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = usermanager.FindById(model.Uploader);
            model.Uploader = user.UserName;
            return View(model);
        }

        [HttpGet]
        // GET: Videos/Create
        public ActionResult Create(string friendlyUrl)
        {
            var model = new VideoDataHeader();
            model.AnimeFriendlyUrl = friendlyUrl;
            return View(model);
        }

        // POST: Videos/Create
        [HttpPost]
        public ActionResult Create(string friendlyUrl, VideoDataHeader model, HttpPostedFileBase video)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if(video == null)
            {
                ModelState.AddModelError("","Video felöltése kötelező");

                return View(model);
            }
            // TODO: Add insert logic here
            else
            {
                string pictureUrl = model.AnimeFriendlyUrl + model.Session + model.Number;
                using (var context = new AnimePortalEntities())
                {
                    var animeId = context.Animes.FirstOrDefault(anime => anime.FriendlyUrl.Equals(friendlyUrl)).Id;
                        
                    context.Video.Add(new Video()
                    {
                        Name = model.Name,
                        AnimeID = animeId,
                        SessionID = model.Session,
                        Number = model.Number,
                        Uploder = User.Identity.GetUserId(),
                        VideoUrl = pictureUrl,
                        Uplload = DateTime.Now
                    });
                    context.SaveChanges();
                }

                var path = Path.Combine(Server.MapPath("~/Upload/Videos"), pictureUrl);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                var profilePicture = Request.Files[0];

                // Fájl mentése a fájlrendszerve
                video.SaveAs(path);
                // Fájl mentése a fájlrendszerve


            }
            return RedirectToAction("Details","Animes", new {friendlyUrl = friendlyUrl });

        }

        // GET: Videos/Edit/5
        public ActionResult Edit(int? videoId)
        {
            var model = VideoManager.getVideoData(videoId, User.Identity.GetUserId());
            if (model.Editor || User.IsInRole("Administrators"))
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Details", new { videoId = videoId });

            }

        }

        // POST: Videos/Edit/5
        [HttpPost]
        public ActionResult Edit(int? id, VideoDataHeader model, HttpPostedFileBase video)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // TODO: Add insert logic here
            else
            {
                if (AnimeManager.isEditor(id, User.Identity.GetUserId()) || User.IsInRole("Administrators"))
                {
                    string pictureUrl = model.AnimeFriendlyUrl + model.Session + model.Number;
                    using (var context = new AnimePortalEntities())
                    {
                        var result = context.Video.FirstOrDefault(vid => vid.ID == id);
                        result.Name = model.Name;
                        result.Number = model.Number;
                        //result.SessionID = model.SessionId;
                        if (video != null)
                        {
                            result.VideoUrl = pictureUrl;
                        }
                        context.Video.Attach(result);
                        context.Entry(result).State = EntityState.Modified;
                        context.SaveChanges();
                    }


                    if (video != null)
                    {
                        var path = Path.Combine(Server.MapPath("~/Upload/Videos"), pictureUrl);
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                        var profilePicture = Request.Files[0];

                        // Fájl mentése a fájlrendszerve
                        video.SaveAs(path);
                        // Fájl mentése a fájlrendszerve
                    }
                }
            }
            return RedirectToAction("Index", "Animes");
        }

        // POST: Videos/Delete/5
        [HttpPost]
        public ActionResult Delete(int? videoId)
        {
            if (AnimeManager.isEditor(videoId, User.Identity.GetUserId()) || User.IsInRole("Administrators"))
            {
                using (var context = new AnimePortalEntities())
                {
                    var result = context.Video.FirstOrDefault(vid => vid.ID == videoId);
                    context.Video.Remove(result);
                    context.SaveChanges();
                }
            }
            return RedirectToAction("Index", "Animes");
        }
    }
}
