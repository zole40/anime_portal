using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bll.Helper
{
    public static class UrlHepler
    {
        /// <summary>
        /// Returns the search engine friendly encoded version or the specified URL.
        /// </summary>
        /// <remarks>
        /// Removes common special characters, replaces accented characters and
        /// uses standard URL encoding to safely encode the remaining special characters.
        /// Returns "1" if all characters of the <paramref name="text"/> are lost during normalization.
        /// </remarks>
        /// <param name="text">The URL to encode.</param>
        /// <returns>The search engine friendly encoded version of the specified URL.</returns>
        public static string ToSeoFriendlyUrl(this string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;
            // Normalize the text using full canonical decomposition.
            text = text.Normalize(NormalizationForm.FormD);
            // Remove non-number and non-letter characters.
            var nonspace = new Regex("[^0-9A-Za-z ]");
            text = nonspace.Replace(text, String.Empty);
            // Replace space characters with "-", following Google's recommendation.
            text = text.Replace(' ', '-');
            // Remove trailing and leading dashes.
            string replaced = text.Trim('-');
            // Encode the remaining special characters with standard URL encoding.
            string ret = HttpUtility.UrlEncode(replaced);
            // Return the normalized text or "1" if the normalized text is empty.
            return String.IsNullOrEmpty(ret) ? "1" : ret;
        }
    }
}
