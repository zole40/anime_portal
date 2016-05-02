using Bll.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Anime_Portal
{
    public class MailList
    {
        private LinkedList<MailHeaderData> mails = new LinkedList<MailHeaderData>();
        private static object syncRoot = new object();

        public string User { get; }

        public int Mail { get; private set; }

        public MailList(string user)
        {
            User = user;
            Mail = 0;
        }

        public void add(MailHeaderData mail)
        {
            lock (syncRoot)
            {
                if (mails.Count >= 5)
                {
                    mails.Remove(mails.Last());
                }
                mails.AddFirst(mail);
                Mail++;
            }
        }

        public void add(List<MailHeaderData> mail)
        {
            var date = DateTime.Now;
            lock (syncRoot)
            {
                foreach (var item in mail)
                {
                    item.time = date - item.date;
                    if (mails.Count >= 5)
                    {
                        mails.Remove(mails.Last());
                    }
                    mails.AddFirst(item);
                }
            }

        }

        public LinkedList<MailHeaderData> getMails()
        {
            Mail = 0;
            return mails;
        }

        public bool getMails(int? maild, out MailHeaderData mail)
        {
            foreach (var item in mails)
            { 
             if(item.Id == maild)
                {
                    mail = item;
                    return true;
                }
            }
            mail = null;
            return false;
        }
    }
}