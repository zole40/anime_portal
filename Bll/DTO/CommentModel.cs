using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bll.DTO.CommentModel
{
        public class CommentCreateData
        {
            public string UserId { get; set; }
            public int Type { get; set; }

            [AllowHtml]
            public string Text { get; set; }
            public int? CommentId { get; set; }
        }

    public class CommentData
    {
        public string UserPictureUrl { get; set; }
        [AllowHtml]
        public string Text { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }

    }

}
