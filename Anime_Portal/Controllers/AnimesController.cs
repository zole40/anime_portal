using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bll.Helper;
using Bll.DTO.AnimeModel;
using Bll.Manager;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Bll;
using Anime_Portal.Helplers;
using Microsoft.AspNet.Identity.Owin;
using System.IO;
using System.Drawing;
using System.Data.Entity;
using Anime_Portal.Cache;
using Bll.DTO;

namespace Anime_Portal.Models
{
    [Authorize]
    public class AnimesController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        // GET: Animes
        public ActionResult Index()
        {
            var model = AnimeManager.getAllAnime();
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        // GET: Animes/Details/5
        public ActionResult Details(string friendlyUrl)
        {
            
            var model = AnimeManager.getAnime(friendlyUrl, User.Identity.GetUserId());
            if(model == null)
            {
                return RedirectToAction("Index");
            }
            model.Sess = AnimeManager.havesession(friendlyUrl);
            var usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = usermanager.FindById(model.Uploader);
            model.Uploader = user.UserName;
            return View(model);
        }

        [HttpGet]
        // GET: Animes/Create
        public ActionResult Create()
        {
            var model = new AnimeHeaderData();
            return View(model);
        }

        // POST: Animes/Create
        [HttpPost]
        public ActionResult Create(AnimeHeaderData model, HttpPostedFileBase picture)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (picture == null)
            {
                ModelState.AddModelError("", "Kép felötltése kötelező");
                return View(model);
            }
            // TODO: Add insert logic here
            else
            {
                string pictureUrl = FileHelper.GetFileName(model.Name, picture);
                using (var context = new AnimePortalEntities())
                {
                    context.Animes.Add(new Animes()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Category = context.Category.FirstOrDefault(name => name.Name.Equals(model.Category)).Id,
                        Uploader = User.Identity.GetUserId(),
                        FriendlyUrl = AnimeManager.GenerateSeoUrls(model),
                        PictureUrl = pictureUrl,
                        Rating = 0
                    });
                    context.SaveChanges();
                }

                var path = Path.Combine(Server.MapPath("~/Upload/AnimePicture"), pictureUrl);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                var profilePicture = Request.Files[0];
                Image img = Image.FromStream(profilePicture.InputStream);
                img = FileHelper.ResizeImage(img, 256, 200);
                // Fájl mentése a fájlrendszerve
                img.Save(path);
                // Fájl mentése a fájlrendszerve


            }
            return RedirectToAction("Index");

        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ListByUser(string Name)
        {
            string userName = Name ?? User.Identity.GetUserName();
            var usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = usermanager.FindByName(userName);
            var model = AnimeManager.getCreatedAnimes(user.Id);
            return PartialView("Index", model);
        }

        [HttpGet]
        // GET: Animes/Edit/5
        public ActionResult Edit(string friendlyUrl)
        {
            bool creator;
            bool edit = AnimeManager.isEditor(friendlyUrl, User.Identity.GetUserId(), out creator);
            if (User.IsInRole("Administrators") || edit)
            {
                var model = AnimeManager.getAnime(friendlyUrl, User.Identity.GetUserId());
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        // POST: Animes/Edit?friendlyUrl
        [HttpPost]
        public ActionResult Edit(string friendlyUrl, AnimeHeaderData model, HttpPostedFileBase picture)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // TODO: Add insert logic here
            else
            {
                bool creator;
                bool edit = AnimeManager.isEditor(friendlyUrl, User.Identity.GetUserId(), out creator);
                if (User.IsInRole("Administrators") || edit)
                {
                    string pictureUrl = FileHelper.GetFileName(model.Name, picture);
                    using (var context = new AnimePortalEntities())
                    {
                        var result = context.Animes.SingleOrDefault(url => url.FriendlyUrl.Equals(friendlyUrl));
                        result.Name = model.Name;
                        if (picture != null)
                        {
                            result.PictureUrl = pictureUrl;
                        }
                        result.Description = model.Description;
                        context.Animes.Attach(result);
                        context.Entry(result).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    if (picture != null)
                    {
                        var path = Path.Combine(Server.MapPath("~/Upload/AnimePicture"), pictureUrl);
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                        var profilePicture = Request.Files[0];
                        Image img = Image.FromStream(profilePicture.InputStream);
                        img = FileHelper.ResizeImage(img, 256, 200);
                        // Fájl mentése a fájlrendszerve
                        img.Save(path);
                        // Fájl mentése a fájlrendszerve
                    }

                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Management()
        {
            var model = AnimeManager.management(User.Identity.GetUserId());
            var usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            foreach (var item in model)
            {
                var user = usermanager.FindById(item.UserId);
                item.UserName = user.UserName;
            }
            return View(model);
        }

        public ActionResult accept(string friendlyUrl,string userId)
        {
            AnimeHeaderData anime;
            if (AnimeManager.acceptRequest(friendlyUrl, userId, out anime))
            {
                var usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = usermanager.FindById(userId);

                MailHeaderData mail;
                MailManager.send(User.Identity.Name, user.UserName, "Szerkeztési jog megadva", User.Identity.Name + " szezkeztése engedélyt adott a következőhöz: " + anime.Name, out mail);
                MailCache.Current.add(mail);
            }
            return RedirectToAction("Management");

        }

        public ActionResult reject(string friendlyUrl, string userId)
        {
            AnimeHeaderData anime;
            if (AnimeManager.rejectRequest(friendlyUrl, userId, out anime))
            {
                var usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = usermanager.FindById(userId);

                MailHeaderData mail;
                MailManager.send(User.Identity.Name, user.UserName, "Szerkeztési jog megtagadva", User.Identity.Name + " megtagadta a szekeztési engedélyt adott a következőhöz: " + anime.Name, out mail);
                MailCache.Current.add(mail);
            }
            return RedirectToAction("Management");

        }



        // POST: Animes/Delete?friendlyUrl
        [HttpPost]
        public ActionResult Delete(string friendlyUrl)
        {

            bool creator;
            bool edit = AnimeManager.isEditor(friendlyUrl, User.Identity.GetUserId(), out creator);
            if (User.IsInRole("Administrators") || creator)
            {
                var succes = AnimeManager.delete(friendlyUrl);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ListCategory()
        {
            var model = CategoryManager.ListCategory();
            return PartialView(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult ListCategoryJson()
        {
            var model = CategoryManager.ListCategory();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ListbyCategory(int? CategoryID)
        {
            var model = AnimeManager.getCategory(CategoryID);
            return View("Index", model);
        }

        [HttpGet]
        public ActionResult Favorite()
        {
            var model = AnimeManager.getFavoriteAnimes(User.Identity.GetUserId());
            return View("Index", model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> FavoritePartial(string Name)
        {
            var usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = await usermanager.FindByNameAsync(Name);
            var model = AnimeManager.getFavoriteAnimes(user.Id);
            return PartialView("Favorite", model);
        }

        [HttpGet]
        public ActionResult Created()
        {
            var model = AnimeManager.getCreatedAnimes(User.Identity.GetUserId());
            return View("Index", model);
        }

        [HttpGet]
        public ActionResult Editor()
        {
            var model = AnimeManager.getEditingAnimes(User.Identity.GetUserId());
            return View("Index", model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Search(string search)
        {
            var model = AnimeManager.search(search);
            return View("Index", model);
        }


        [HttpPost]
        public JsonResult Rating(int? rating, string friendlyUrl)
        {
            var rat = RatingManager.addRating(rating, friendlyUrl, User.Identity.GetUserId());
            using (var context = new AnimePortalEntities())
            {
                var result = context.Animes.SingleOrDefault(url => url.FriendlyUrl.Equals(friendlyUrl));
                result.Rating = rat;
                context.Animes.Attach(result);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
            return Json(rat, JsonRequestBehavior.DenyGet);
        }

        public ActionResult addFavorite(string friendlyUrl)
        {
            var succes = AnimeManager.addFavorite(friendlyUrl, User.Identity.GetUserId());
            return RedirectToAction("Details", new { friendlyUrl = friendlyUrl });
        }

        public ActionResult removeFavorite(string friendlyUrl)
        {
            
            var succes = AnimeManager.removeFavorite(friendlyUrl, User.Identity.GetUserId());
            return RedirectToAction("Details", new { friendlyUrl = friendlyUrl });
        }

        public ActionResult EditorRequest(string friendlyUrl)
        {
            AnimeHeaderData anime;
            if(AnimeManager.editorRequest(friendlyUrl,User.Identity.GetUserId(),out anime)){
                var usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user =  usermanager.FindById(anime.Uploader);

                MailHeaderData mail;
                MailManager.send(User.Identity.Name,user.UserName, "Szerkeztési jog kérése",User.Identity.Name + " engedélyt kért a következő szerkeztéséhez : " + anime.Name, out mail);
                MailCache.Current.add(mail);
            }
            return RedirectToAction("Details", new { friendlyUrl = friendlyUrl }); 
        }

        public ActionResult CancelRequest(string friendlyUrl)
        {
            AnimeHeaderData anime;
            if (AnimeManager.cancelRequest(friendlyUrl, User.Identity.GetUserId(), out anime))
            {
                var usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = usermanager.FindById(anime.Uploader);

                MailHeaderData mail;
                MailManager.send(User.Identity.Name, user.UserName, "Szerkeztési jog visszavonása", User.Identity.Name + " visszavonta kérését a következő szerkeztéséhez : " + anime.Name, out mail);
                MailCache.Current.add(mail);
            }
            return RedirectToAction("Details", new { friendlyUrl = friendlyUrl });
        }
    }
}
