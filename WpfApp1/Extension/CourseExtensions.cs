using ClassroomAssignment.Model;
using ClassroomAssignment.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Extension
{
	/// <summary>
	/// Extension methods for course object.
	/// </summary>
    public static class CourseExtensions
    {
		/// <summary>
		/// Extension method that creates a SearchParameters object for the <paramref name="course"/> used for the available rooms search.
		/// </summary>
		/// <param name="course">A course object</param>
		/// <returns>searchParameters</returns>
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
