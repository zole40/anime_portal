using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bll.DTO.VideoModel
{
   public class VideoHeader 
    {
      
        public int Number { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public int Id { get; set; }
    }

    public class VideoDataHeader
    {
        public int  Id { get; set; }
        [Required]
        public int Number { get; set; }
        public string Name { get; set; }
        public string VideoUrl { get; set; }
        public string Uploader { get; set; }
        [Required]
        public int Session { get; set; }
        public string Anime { get; set; }
        public string AnimeFriendlyUrl { get; set; }
        public int SessionId { get; set; }
        public string AnimePictureUrl { get; set; }
        public bool Editor { get; set; }
        public DateTime Upload { get; set; }
    }
}
