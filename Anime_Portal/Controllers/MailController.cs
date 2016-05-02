using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bll.DTO;
using Bll.Manager;
using Anime_Portal.Cache;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace Anime_Portal.Controllers
{
    [Authorize]
    public class MailController : Controller
    {
        [HttpPost]
        public void Send(string From, MailHeaderData model)
        {
            MailHeaderData mail;
            var usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = usermanager.FindByName(model.To);
            if (model.To != null && user == null)
            {
                MailManager.send("admin", From, "Sikertelen", "A következő felhasználó nem létezik" + model.To,out mail);
                MailCache.Current.add(mail);
                return;
            }
            if (MailManager.send(From, model.To ?? "", model.Title, model.Text, out mail))
            {
                MailCache.Current.add(mail);
            }
        }

        public ActionResult Send(int? mailId,string type)
        {
            MailHeaderData mail;
            if (mailId != null)
            {

                if (!MailCache.Current.getMail(User.Identity.GetUserName(), mailId, out mail))
                {
                    mail = MailManager.getMail(mailId);
                }
                if (type.Equals("inbox"))
                {
                    string name = mail.From;
                    mail.From = mail.To;
                    mail.To = name;
                }
            }
            else
            {
                mail = new MailHeaderData()
                {
                    From = User.Identity.GetUserName(),
                };
            }

            return PartialView(mail);
          
        }

        public ActionResult getMail(int? mailId,string type)
        {
            MailHeaderData mail;
            if (!MailCache.Current.getMail(User.Identity.GetUserName(), mailId, out mail))
            {
               mail =  MailManager.getMail(mailId);
            }
            switch (type)
            {
                case "inbox": return PartialView(mail);
                case "sent": return PartialView("sent", mail);
            }
            return PartialView(mail);
        }





        public ActionResult Mailbox()
        {
            var mail = new MailHeaderData();
            return View();
        }

        public JsonResult newMails(string userName)
        {
            var mails = MailCache.Current.newMails(userName);
            return Json(mails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult quickmail(string adress)
        {
            var mail = new MailHeaderData() { To = adress };
            return PartialView(mail);
        }

        public ActionResult Mails(string userName)
        {
            var usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            DateTime time = DateTime.Now;
            var mails = MailCache.Current.getMails(userName);
            foreach (var item in mails)
            {
                var user = usermanager.FindByName(item.From);
                item.avatar = user.Avatar;
                item.From = user.UserName;
                item.time = time - item.date;
            }
            return PartialView(mails);
        }

        public ActionResult GetsendedMail(string userName)
        {
            var mail = MailManager.getSendedMail(userName);
            return PartialView(mail);
        }

        public ActionResult Inbox(string userName)
        {
            var mail = MailManager.Inbox(userName);
            return PartialView(mail);
        }
    }
}