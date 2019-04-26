using ClassroomAssignment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Operations
{

    /// <summary>
    /// Schedule Slots, create methods for RoomAvailable,
    /// meetingDays,StartTime and EndTime.
    /// </summary>
    public class ScheduleSlot
    {
        public Room RoomAvailable { get; set; }
        public IEnumerable<DayOfWeek> MeetingDays { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

		/// <summary>
		/// Converts the start time to the hh:mm tt format
		/// </summary>
		public string FormattedStartTime
        {
            get => new DateTime().Add(StartTime).ToString("hh:mm tt");
        }

		/// <summary>
		/// Converts the end time to the hh:mm tt format
		/// </summary>
		public string FormattedEndTime
        {
            get => new DateTime().Add(EndTime).ToString("hh:mm tt");
        }
    }
}
