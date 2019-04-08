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
    public struct SearchParametersWithType
    {
        public IEnumerable<DayOfWeek> MeetingDays;
        public TimeSpan StartTime;
        public TimeSpan EndTime;
        public TimeSpan Duration;
        public int Capacity;
        public string Type;
        /// <summary>
        /// Calculate duration for the class.
        /// </summary>
        /// <param name="meetingDays"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="capacity"></param>
        /// <param name="duration"></param>
        public SearchParametersWithType(IEnumerable<DayOfWeek> meetingDays, TimeSpan startTime, TimeSpan endTime, int capacity = int.MaxValue, TimeSpan duration = new TimeSpan(), string roomtype)
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
