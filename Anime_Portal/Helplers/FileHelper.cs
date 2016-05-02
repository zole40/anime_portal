using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace Anime_Portal.Helplers
{
    public class FileHelper
    {

        public static Image ResizeImage(Image img, int width, int height)
        {
            Bitmap b = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage((Image)b))
            {
                g.DrawImage(img, 0, 0, width, height);
            }

            return (Image)b;
        }

        public static string GetFileName(string userName, HttpPostedFileBase picture)
        {
            if (picture != null && picture.ContentLength > 0)
            {
                // TODO: Validáció, hogy tényleg kép-e?
                var pictureFileName = Path.GetFileName(picture.FileName);
                return userName + "_" + pictureFileName;
            }
            return null;
        }
    }

}