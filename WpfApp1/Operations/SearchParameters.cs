using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Operations
{
    /// <summary>
    /// This file search parameters, calculate duration for courses.
    /// </summary>
    public struct SearchParameters
    {
        public IEnumerable<DayOfWeek> MeetingDays;
        public TimeSpan StartTime;
        public TimeSpan EndTime;
        public TimeSpan Duration;
        public int Capacity;
        /// <summary>
        /// Calculate duration for the class.
        /// </summary>
        /// <param name="meetingDays">Days of meeting for a course.</param>
        /// <param name="startTime">The start time of a course.</param>
        /// <param name="endTime">The end time of a course.</param>
        /// <param name="capacity">The capacity of a course.</param>
        /// <param name="duration">The time length of a course.</param>
        public SearchParameters(IEnumerable<DayOfWeek> meetingDays, TimeSpan startTime, TimeSpan endTime, int capacity = int.MaxValue, TimeSpan duration = new TimeSpan())
        {
            MeetingDays = meetingDays;
            StartTime = startTime;
            EndTime = endTime;
            Capacity = capacity;

            if (duration.TotalMinutes == 0) Duration = endTime - startTime;
            else Duration = duration;
        }


    }
}
