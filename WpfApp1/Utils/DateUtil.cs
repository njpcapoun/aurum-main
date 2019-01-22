using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ClassroomAssignment.Model.Utils
{
    /// <summary>
    /// Class to keep consistent day information 
    /// </summary>
    public class DateUtil
    {
        private static Dictionary<string, DayOfWeek> DayNameMatcher = new Dictionary<string, DayOfWeek>();


        /// <summary>
        /// Adds abbreviations for
        /// each day of the week
        /// </summary>
        static DateUtil()
        {
            DayNameMatcher.Add("Sn", DayOfWeek.Sunday);
            DayNameMatcher.Add("M", DayOfWeek.Monday);
            DayNameMatcher.Add("T", DayOfWeek.Tuesday);
            DayNameMatcher.Add("W", DayOfWeek.Wednesday);
            DayNameMatcher.Add("Th", DayOfWeek.Thursday);
            DayNameMatcher.Add("F", DayOfWeek.Friday);
            DayNameMatcher.Add("Sa", DayOfWeek.Saturday);

           
        }


        /// <summary>
        /// Matches day of the week to corresponding abbreviation
        /// and shortens it to abbreviation  
        /// </summary>
        /// <param name="dayAbbreviation"></param>
        /// <returns>string</returns>
        public static string ShortToLongDayName(string dayAbbreviation)
        {
            try
            {
                return DayNameMatcher[dayAbbreviation].ToString();
            }
            catch (Exception e)
            {
                //LogUtil.Debug(e.Message);
                return null;
            }
        }

  

        /// <summary>
        /// returns match of day to abbreviation 
        /// </summary>
        /// <param name="dayAbbreviation"></param>
        /// <returns>DayOfWeek</returns>
        public static DayOfWeek AbbreviationToDayOfWeek(string dayAbbreviation)
        {
            return DayNameMatcher[dayAbbreviation];
        }
    }
}
