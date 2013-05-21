using System;

namespace Tfs2Gource.Extensions {
    public static class DateExtensions {

        private static readonly DateTime UnixBaseDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();

        /// <summary>
        /// method for converting a System.DateTime value to a UNIX Timestamp
        /// </summary>
        /// <param name="dateTime">date to convert</param>
        /// <returns></returns>
        public static long ToUnix(this DateTime dateTime) {
            //create Timespan by subtracting the value provided from
            //the Unix Epoch
            var span = (dateTime - UnixBaseDate);
            //return the total seconds (which is a UNIX timestamp)
            return (long)span.TotalSeconds;
        }
    }
}
