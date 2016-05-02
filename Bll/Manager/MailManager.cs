using Bll.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bll.Manager
{
    public class MailManager
    {
        public static bool send(string From, string To, string title, string text, out MailHeaderData mail)
        {
            if (From.Equals(""))
            {
                mail = null;
                return false;
            }
            if (To.Equals(""))
            {
                send("admin", From, "Sikertelen", "Az üzenetet nem sikerült kézbesíteni : " + text, out mail);
                return true;
            }

            using (var context = new AnimePortalEntities())
            {
                try
                {
                    var mas = new Message()
                    {
                        Sender = From,
                        Text = text,
                        Adress = To,
                        Title = title,
                        createdTime = DateTime.Now
                    };

                    context.Message.Add(mas);
                    context.SaveChanges();
                    mail = new MailHeaderData()
                    {
                        From = From,
                        To = To,
                        Title = title,
                        Text = text,
                        date = mas.createdTime,
                        Id = mas.Id
                    };
                    return true;
                }
                catch
                {
                    mail = null;
                    return false;
                }
            }
        }

        public static MailHeaderData getMail(int? mailId)
        {
            using (var context = new AnimePortalEntities())
            {
                var mails = (from mail in context.Message
                             where mail.Id == mailId
                             select new MailHeaderData
                             {
                                 To = mail.Adress,
                                 Text = mail.Text,
                                 Title = mail.Title,
                                 date = mail.createdTime,
                                 Id = mail.Id,
                                 From = mail.Sender
                             }).SingleOrDefault();
                return mails;
            }
        }

        public static List<MailHeaderData> getSendedMail(string userName)
        {
            using (var context = new AnimePortalEntities())
            {
                var mails = (from mail in context.Message
                             where mail.Sender.Equals(userName)
                             select new MailHeaderData
                             {
                                 To = mail.Adress,
                                 Text = mail.Text,
                                 Title = mail.Title,
                                 date = mail.createdTime,
                                 Id = mail.Id
                             }).ToList();
                return mails;
            }
        }

        public static List<MailHeaderData> Inbox(string userName)
        {
            using (var context = new AnimePortalEntities())
            {
                var mails = (from mail in context.Message
                             where mail.Adress.Equals(userName)
                             orderby mail.createdTime ascending
                             select new MailHeaderData
                             {
                                 From = mail.Sender,
                                 Text = mail.Text,
                                 Title = mail.Title,
                                 date = mail.createdTime,
                                 Id = mail.Id
                             }).ToList();
                return mails;
            }
        }

        public static List<MailHeaderData> lastMails(string userName)
        {

            using (var context = new AnimePortalEntities())
            {
                var mails = (from mail in context.Message
                             where mail.Adress.Equals(userName)
                             orderby mail.createdTime ascending
                             select new MailHeaderData
                             {
                                 From = mail.Sender,
                                 Text = mail.Text,
                                 Title = mail.Title,
                                 date = mail.createdTime,
                                 Id = mail.Id
                             }).Take(5).ToList();
                return mails;
            }
        }

        public static void send(string v1, object from, string v2, object p, out object mail)
        {
            throw new NotImplementedException();
        }
    }
}
