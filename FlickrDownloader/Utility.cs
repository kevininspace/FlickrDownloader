//-----------------------------------------------------------------------
// <copyright file="Utility.cs" company="Sondre Bjellås">
// This software is licensed as Microsoft Public License (Ms-PL).
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Text;

namespace FlickrDownloader
{
    /// <summary>
    /// Utility functions used for formatting and validation.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Appends a space before all capital letters in a sentence, except the first character.
        /// </summary>
        /// <param name="text">Enumeration or text value that is formated like "AllRightsReserved".</param>
        /// <returns>Input text with a space infront of all capital letters.</returns>
        public static string AddSpaceBetweenCapitalLetters(string text)
        {
            StringBuilder str = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                if (i > 0 && char.IsUpper(text[i]))
                {
                    str.Append(" ");
                }

                str.Append(text[i]);
            }

            return str.ToString();
        }

        /// <summary>
        /// Removes any illegal characters from the path. This is used to clean out any
        /// special characters from the user name.
        /// </summary>
        /// <param name="path">Local disk path.</param>
        /// <returns>Local disk path where illegal characters has been removed.</returns>
        public static string RemoveIllegalCharacters(string path)
        {
            string p = path;

            char[] chars = Path.GetInvalidFileNameChars();

            for (int i = 0; i < chars.Length; i++)
            {
                p = p.Replace(chars[i], ' ');
            }

            return p;
        }
    }
}