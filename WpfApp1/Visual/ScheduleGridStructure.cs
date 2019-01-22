using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Visual
{
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

        private void InitTimeToRowMap()
        {
            var column = 2;
            for (DayOfWeek day = FirstDayOfSchedule; day <= LastDayOfSchedule; day++)
            {
                dayToColumnMap.Add(day, column++);
            }
        }

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
        /// 
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public int SpanForDurationInMinutes(int duration)
        {
            var span = duration / (int) TimeUnitInMinutes.TotalMinutes;

            return span >= 1 ? span : 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
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

        public int GetColumnForDay(DayOfWeek day)
        {
            if (dayToColumnMap.ContainsKey(day)) return dayToColumnMap[day];
            else return -1;
        }

        public IEnumerable<TimeSpan> TimeSlotsInSchedule()
        {
            var current = FirstTimeSlot;
            while (current <= LastTimeSlot)
            {
                yield return current;
                current = current + TimeUnitInMinutes;
            }
        }

        public IEnumerable<DayOfWeek> DaysOfWeekInGrid()
        {
            for (DayOfWeek day = FirstDayOfSchedule; day <= LastDayOfSchedule; day++)
            {
                yield return day;
            }
        }

    }
}
