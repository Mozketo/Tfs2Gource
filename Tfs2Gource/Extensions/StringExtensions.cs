using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Tfs2Gource.Extensions
{
    public static class StringExtensions
    {
        public static string GetValidFileName(this string name)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidReStr = string.Format(@"[{0}]+", invalidChars);
            return Regex.Replace(name, invalidReStr, "_");
        }

        public static byte[] GetBytes(this string str) {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static bool HasValue(this string s) {
            return !String.IsNullOrEmpty(s);
        }
    }
}
