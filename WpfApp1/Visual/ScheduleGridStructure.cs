using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Visual
{
	/// <summary>
	/// Sets layout of the schedule grid.
	/// </summary>
    class ScheduleGridLayout
    {
        private readonly TimeSpan _firstTimeSlot;
        public TimeSpan FirstTimeSlot => _firstTimeSlot;

        private readonly TimeSpan _lastTimeSlot;
        public TimeSpan LastTimeSlot => _lastTimeSlot;

        private readonly DayOfWeek _firstDayOfSchedule;
        public DayOfWeek FirstDayOfSchedule => _firstDayOfSchedule;

        private readonly DayOfWeek _lastDayOfSchedule;
        public DayOfWeek LastDayOfSchedule => _lastDayOfSchedule;

        private readonly TimeSpan _timeUnitInMinutes;
        public TimeSpan TimeUnitInMinutes => _timeUnitInMinutes;

        private Dictionary<TimeSpan, int> timeToRowMap = new Dictionary<TimeSpan, int>();
        private Dictionary<DayOfWeek, int> dayToColumnMap = new Dictionary<DayOfWeek, int>();

		/// <summary>
		/// Constructor for ScheduleGridLayout. Set passed parameters and initialize time rows and day columns.
		/// </summary>
		/// <param name="firstTimeSlot">The first time slot in the schedule.</param>
		/// <param name="lastTimeSlot">The last time slot in the schedule.</param>
		/// <param name="firstDayOfSchedule">The first day of the schedule.</param>
		/// <param name="lastDayOfSchedule">The last day of the schedule.</param>
		/// <param name="timeUnitInMinutes">The times in minutes.</param>
        public ScheduleGridLayout(TimeSpan firstTimeSlot, TimeSpan lastTimeSlot, DayOfWeek firstDayOfSchedule, DayOfWeek lastDayOfSchedule, int timeUnitInMinutes)
        {
            _firstTimeSlot = firstTimeSlot;
            _lastTimeSlot = lastTimeSlot;
            _firstDayOfSchedule = firstDayOfSchedule;
            _lastDayOfSchedule = lastDayOfSchedule;
            _timeUnitInMinutes = new TimeSpan(0, timeUnitInMinutes, 0);

            InitTimeToRowMap();
            InitDayToColumnMap();
        }

		/// <summary>
		/// Initialize the list of times in the rows.
		/// </summary>
        private void InitTimeToRowMap()
        {
            var column = 2;
            for (DayOfWeek day = FirstDayOfSchedule; day <= LastDayOfSchedule; day++)
            {
                dayToColumnMap.Add(day, column++);
            }
        }

		/// <summary>
		/// Initialize the days of weeks in the columns.
		/// </summary>
        private void InitDayToColumnMap()
        {
            var currentTime = FirstTimeSlot;

            int row = 1;
            while (currentTime <= LastTimeSlot)
            {
                timeToRowMap.Add(currentTime, row++);
                currentTime = currentTime + TimeUnitInMinutes;
            }
        }

		/// <summary>
		/// Set the TimeSpan duration in minutes.
		/// </summary>
		/// <param name="duration"></param>
		/// <returns>span >= 1 ? span : 1</returns>
		public int SpanForDurationInMinutes(int duration)
        {
            var span = duration / (int) TimeUnitInMinutes.TotalMinutes;

            return span >= 1 ? span : 1;
        }

		/// <summary>
		/// Set all the times for the rows.
		/// </summary>
		/// <param name="time">A time interval</param>
		/// <returns>timeToRowMap[timeKey] or -1</returns>
		public int GetRowForTime(TimeSpan time)
        {
            if (timeToRowMap.ContainsKey(time)) return timeToRowMap[time];

            var first = time - TimeUnitInMinutes;
            var last = time + TimeUnitInMinutes;
            foreach (var timeKey in timeToRowMap.Keys)
            {
                if (timeKey >= first && timeKey < last) return timeToRowMap[timeKey];
            }

            return -1;
        }

		/// <summary>
		/// Sets all the days of the weeks for the columns.
		/// </summary>
		/// <param name="day">Day of the week.</param>
		/// <returns>dayToColumnMap[day] or -1</returns>
		public int GetColumnForDay(DayOfWeek day)
        {
            if (dayToColumnMap.ContainsKey(day)) return dayToColumnMap[day];
            else return -1;
        }
		
		/// <summary>
		/// Increment the time slots in the schedule.
		/// </summary>
		/// <returns>current</returns>
        public IEnumerable<TimeSpan> TimeSlotsInSchedule()
        {
            var current = FirstTimeSlot;
            while (current <= LastTimeSlot)
            {
                yield return current;
                current = current + TimeUnitInMinutes;
            }
        }

		/// <summary>
		/// Increment the days of the week in the schedule.
		/// </summary>
		/// <returns></returns>
        public IEnumerable<DayOfWeek> DaysOfWeekInGrid()
        {
            for (DayOfWeek day = FirstDayOfSchedule; day <= LastDayOfSchedule; day++)
            {
                yield return day;
            }
        }

    }
}
