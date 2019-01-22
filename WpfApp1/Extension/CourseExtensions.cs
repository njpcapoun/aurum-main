using ClassroomAssignment.Model;
using ClassroomAssignment.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Extension
{
    public static class CourseExtensions
    {
        /// <summary>
        /// Extension method that creates a SearchParameters object for the <paramref name="course"/>
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        public static SearchParameters GetSearchParameters(this Course course)
        {
            var searchParameters = new SearchParameters();
            searchParameters.MeetingDays = course.MeetingDays;
            searchParameters.StartTime = course.StartTime.Value;
            searchParameters.EndTime = course.EndTime.Value;
            searchParameters.Capacity = int.Parse(course.RoomCapRequest);
            searchParameters.Duration = course.EndTime.Value - course.StartTime.Value;

            return searchParameters;
        }


    }
}
