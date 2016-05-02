using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bll.DTO
{
    public class MailHeaderData
    {
        public string From { get; set; }
        public string To { get; set; }

        [AllowHtml]
        public string Text { get; set; }
        public string Title { get; set; }
        public string avatar { get; set; }
        public DateTime date { get; set; }

        public TimeSpan time { get; set; }
        public int Id { get; set; }

    }
}
