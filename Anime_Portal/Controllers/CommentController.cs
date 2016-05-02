using Bll;
using Bll.DTO.CommentModel;
using Bll.Manager;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anime_Portal.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        [AllowAnonymous]
        public ActionResult ListComments(int? CommentId, int? Type)
        {
          
            var comments = CommentManager.getComments(Type, CommentId);
            return PartialView(comments);
        }

        [AllowAnonymous]
        public ActionResult Create(int? commentId)
        {
            var model = new CommentCreateData() { CommentId = commentId };
            return PartialView(model);
        }


        // POST: Comment/Create
        [HttpPost]
        public ActionResult Create(int? CommentId, int? Type , CommentCreateData comment)
        {
            try
            {
                var usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = usermanager.FindById(User.Identity.GetUserId());
                CommentManager.CreateComment((int)Type, user.UserName, user.PictureUrl, comment, (int)CommentId);
                return RedirectToAction("Create", new { commentId = CommentId });
            }
            catch {
                return PartialView(comment);
            }
            }
    }
}
