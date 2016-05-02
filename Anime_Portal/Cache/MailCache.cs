using Bll.DTO;
using Bll.Manager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Anime_Portal.Cache
{
    public class MailCache
    {
        private ConcurrentBag<MailList> mails = new ConcurrentBag<MailList>();

        private static volatile MailCache current;
        private static object syncRoot = new object();
        private MailCache() { }
        public static MailCache Current
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                        {
                            current = new MailCache();
                        }
                    }
                }
                return current;
            }
        }

        public bool getMail(string UserName,int? mailId,out MailHeaderData mail)
        {
            var list = mails.ToList();

            foreach (var item in list)
            {
                if (item.User.Equals(UserName))
                {
                    return item.getMails(mailId, out mail);
                }
            }
            mail = null;
            return false;
        }

        public void add(MailHeaderData mail)
        {
            var list = mails.ToList();
            foreach (var item in list)
            {
                if (item.User.Equals(mail.To))
                {
                    item.add(mail);
                    return;
                }
            }
        }

        public int newMails(string userName)
        {
            var list = mails.ToList();
            foreach (var item in list)
            {
                if (item.User.Equals(userName))
                {
                    return item.Mail;
                }
            }
            return 0;
        }

        public void login(string username)
        {
            var maillist = new MailList(username);
            maillist.add(MailManager.lastMails(username));
            mails.Add(maillist);
        }

        public LinkedList<MailHeaderData> getMails(string userName)
        {
            var list = mails.ToList();
            foreach (var item in list)
            {
                if (item.User.Equals(userName))
                {
                    return item.getMails();
                }
            }
            return new LinkedList<MailHeaderData>();
        }
    }
}
