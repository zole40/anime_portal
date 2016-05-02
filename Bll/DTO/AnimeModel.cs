using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Bll.DTO.AnimeModel
{
   public class AnimeHeaderData
    {
        [Required]
        public string Name { get; set; }
        public string FriendlyUrl { get; set; }
        public string PictureUrl { get; set; }
        public double Rating { get; set; }
        [Required]
        public string Category { get; set; }
        public string Uploader { get; set; }
        public bool Edit { get; set; }
        public bool Creator { get; set; }

        public bool Request { get; set; }
        [AllowHtml]
        public  string Description { get; set; }

        public bool Favorite { get; set; }

        public bool Sess { get; set; }
    }

   public class AnimeManagement
    {
        [Display(Name ="Anime")]
        public string AnimeName { get; set; }
        [Display(Name = "Username")]
        public string UserName { get; set; }
        public string friendlyUrl { get; set; }
        [Display(Name = "Status")]
        public int Edit { get; set; }
        public string UserId { get; set; }
    }


    public class AnimeHeaderIndex
    {
        public string Name { get; set; }
        public string FriendlyUrl { get; set; }
        public string PictureUrl { get; set; }
        public double Rating { get; set; }
        public string Category { get; set; }
    }

    public class AnimeHeader
    {
        public int Id { get; set; }
        public string friendlyUrl { get; set; }
        public string uploader { get; set; }
    }
}
