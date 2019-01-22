using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassroomAssignment.Utils
{
    /// <summary>
    /// Class to standardize time after parsing
    /// </summary>
    public class TimeUtil
    {
        /// <summary>
        /// Uses regex pattern to standardize 
        /// time from parsed data, also checks to see if
        /// military time is used and converts it to standard time
        /// </summary>
        /// <param name="timeStr"></param>
        /// <returns>TimeSpan</returns>
        public static TimeSpan StringToTimeSpan(string timeStr)
        {
            Regex timePattern = new Regex(@"(\d{1,2})(?::)?(\d{1,2})?(am|pm)");
            Match match = timePattern.Match(timeStr);

            if (!match.Success)
            {
                throw new FormatException("Invalid time string argument provided.");
            }

            string hrString = match.Groups[1].Value;
            var amPm = match.Groups[3].Value;

            int hr;
            if (amPm.Equals("am"))
            {
                hr = militaryHr(hrString, false);
            }
            else
            {
                hr = militaryHr(hrString, true);
            }

            string minStr = match.Groups[2].Value;
            int min = string.IsNullOrEmpty(minStr) ? 0 : int.Parse(minStr);

            return new TimeSpan(hr, min, 0);
        }

        private static int militaryHr(string timeStr, bool pm)
        {
            if (timeStr.Equals("12"))
            {
                return pm ? 12 : 0;
            }
            else
            {
                int hr = int.Parse(timeStr);

                return pm ? hr + 12 : hr;
            }
        }
    }
}
