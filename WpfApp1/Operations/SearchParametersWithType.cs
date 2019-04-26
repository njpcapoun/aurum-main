using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Operations
{
    /// <summary>
    /// The file search parameters including room type, calculate and set the time durations for the courses.
    /// </summary>
    public struct SearchParametersWithType
    {
        public IEnumerable<DayOfWeek> MeetingDays;
        public TimeSpan StartTime;
        public TimeSpan EndTime;
        public TimeSpan Duration;
        public int Capacity;
        public string Type;

		/// <summary>
		/// Constructor for SearchParametersWithType. Calculate and set the time duration of a course.
		/// </summary>
		/// <param name="meetingDays">Days of meeting for a course.</param>
		/// <param name="roomtype">The type of room.</param>
		/// <param name="startTime">The start time of a course.</param>
		/// <param name="endTime">The end time of a course.</param>
		/// <param name="capacity">The capacity of a course.</param>
		/// <param name="duration">The time length of a course.</param>
		public SearchParametersWithType(IEnumerable<DayOfWeek> meetingDays, string roomtype, TimeSpan startTime, TimeSpan endTime, int capacity = int.MaxValue, TimeSpan duration = new TimeSpan())
        {
            MeetingDays = meetingDays;
            StartTime = startTime;
            EndTime = endTime;
            Capacity = capacity;
            Type = roomtype;

            if (duration.TotalMinutes == 0) Duration = endTime - startTime;
            else Duration = duration;
        }
    }
}
