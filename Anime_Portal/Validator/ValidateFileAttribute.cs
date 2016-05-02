using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anime_Portal.Validator
{
    public class ValidateFileAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;
            if (file == null)
            {
                return true;
            }


            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                   if(img.RawFormat.Equals(ImageFormat.Bmp) || img.RawFormat.Equals(ImageFormat.Jpeg) || img.RawFormat.Equals(ImageFormat.Gif) || img.RawFormat.Equals(ImageFormat.Png))
                    {
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }
    }
}