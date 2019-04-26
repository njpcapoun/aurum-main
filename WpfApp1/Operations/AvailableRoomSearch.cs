using ClassroomAssignment.Model;
using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.Notification;
using ClassroomAssignment.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Operations
{
    /// <summary>
    /// See if there is any available rooms based on course's time duration.
    /// </summary>
    public class AvailableRoomSearch
    {

        private IRoomRepository roomRepository;
        private ICourseRepository courseRepository;

        /// <summary>
        /// Initilize roomRepo and courseRepo and throw new exceptions to handle it.
        /// </summary>
        /// <param name="roomRepo">The collection of rooms</param>
        /// <param name="courseRepo">The collection of courses</param>
        public AvailableRoomSearch(IRoomRepository roomRepo, ICourseRepository courseRepo)
        {
            roomRepository = roomRepo ?? throw new ArgumentNullException();
            courseRepository = courseRepo ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// Find course meeting days, startTime, endTime, capacity.
        /// create searchParameters list, and initilize these values.
        /// </summary>
        /// <param name="meetingDays">Meetings days of a course</param>
        /// <param name="startTime">The start time of a course</param>
        /// <param name="endTime">The end time of a course.</param>
        /// <param name="minCapacity">The minimum capacity for a course.</param>
        /// <returns>ScheduleSlotsAvailable</returns>
        public IEnumerable<Room> AvailableRooms(List<DayOfWeek> meetingDays, TimeSpan startTime, TimeSpan endTime, int minCapacity)
        {
            SearchParameters searchParameters = new SearchParameters();
            searchParameters.MeetingDays = meetingDays;
            searchParameters.StartTime = startTime;
            searchParameters.EndTime = endTime;
            searchParameters.Capacity = minCapacity;
            searchParameters.Duration = endTime - startTime;

            return ScheduleSlotsAvailable(searchParameters).ConvertAll(x => x.RoomAvailable).Distinct();
        }

		/// <summary>
		/// Overloaded method of available rooms to set SearchParametersWithType
		/// </summary>
		/// <param name="meetingDays">Meetings days of a course</param>
		/// <param name="startTime">The start time of a course</param>
		/// <param name="endTime">The end time of a course.</param>
		/// <param name="minCapacity">The minimum capacity for a course.</param>
		/// <param name="type">The type of room</param>
		/// <returns></returns>
		public IEnumerable<Room> AvailableRooms(List<DayOfWeek> meetingDays, TimeSpan startTime, TimeSpan endTime, int minCapacity, string type)
        {
            SearchParametersWithType searchParameters = new SearchParametersWithType();
            searchParameters.MeetingDays = meetingDays;
            searchParameters.StartTime = startTime;
            searchParameters.EndTime = endTime;
            searchParameters.Capacity = minCapacity;
            searchParameters.Duration = endTime - startTime;
            searchParameters.Type = type;

            return ScheduleSlotsAvailable(searchParameters).ConvertAll(x => x.RoomAvailable).Distinct();
        }

		/// <summary>
		/// ScheduleSlotsAvailable method, check any room available at course duraction.
		/// </summary>
		/// <param name="searchParameters">The parameters for the available room search.</param>
		/// <returns>The available schedule slots</returns>
		public List<ScheduleSlot> ScheduleSlotsAvailable(SearchParameters searchParameters)
        {


            var coursesGroupedByRoom = from room in roomRepository.Rooms
                                       where room.Capacity >= searchParameters.Capacity
                                       join course in courseRepository.Courses on room equals course.RoomAssignment into courseGroup
                                       select new { Room = room, Courses = courseGroup };


            List<ScheduleSlot> availableSlots = new List<ScheduleSlot>();
            foreach (var courseGroup in coursesGroupedByRoom)
            {

                List<Course> courses = courseGroup.Courses
                    .Where(x => x.NeedsRoom  && x.MeetingDays.Intersect(searchParameters.MeetingDays).Count(z => true) != 0 && x.StartTime.HasValue && !(x.StartTime.Value >= searchParameters.EndTime || x.EndTime <= searchParameters.StartTime))
                    .OrderBy(x => x.StartTime.Value)
                    .ToList();
                //Check there is no courses in that slot.
                if (courses.Count == 0)
                {
                    availableSlots.Add(
                        new ScheduleSlot()
                        {
                            RoomAvailable = courseGroup.Room,
                            StartTime = searchParameters.StartTime,
                            EndTime = searchParameters.EndTime,
                            MeetingDays = searchParameters.MeetingDays.AsEnumerable()
                        });

                    continue;
                }
                //Calculate duraction of the course.
                if (courses[0].StartTime - searchParameters.StartTime >= searchParameters.Duration)
                {
                    availableSlots.Add(
                        new ScheduleSlot()
                        {
                            RoomAvailable = courseGroup.Room,
                            StartTime = searchParameters.StartTime,
                            EndTime = courses[0].StartTime.Value,
                            MeetingDays = searchParameters.MeetingDays.AsEnumerable()
                        });
                }

                for (int i = 0; i < courses.Count - 1; i++)
                {
                    if (courses[i + 1].StartTime - courses[i].EndTime >= searchParameters.Duration)
                    {
                        availableSlots.Add(
                            new ScheduleSlot()
                            {
                                RoomAvailable = courseGroup.Room,
                                StartTime = courses[i].EndTime.Value,
                                EndTime = courses[i+1].StartTime.Value,
                                MeetingDays = searchParameters.MeetingDays.AsEnumerable()
                            });
                    }
                }

                if (searchParameters.EndTime - courses.Last().EndTime.Value  >= searchParameters.Duration)
                {
                    availableSlots.Add(
                        new ScheduleSlot()
                        {
                            RoomAvailable = courseGroup.Room,
                            StartTime = courses.Last().EndTime.Value,
                            EndTime = searchParameters.EndTime,
                            MeetingDays = searchParameters.MeetingDays.AsEnumerable()
                        });
                }
            }

            
            return availableSlots;
        }

		/// <summary>
		/// Overloaded method of ScheduleSlotsAvailable to search for rooms with regards to type
		/// </summary>
		/// <param name="searchParameters">The parameters for the available room search.</param>
		/// <returns>The available schedule slots</returns>
		public List<ScheduleSlot> ScheduleSlotsAvailable(SearchParametersWithType searchParameters)
        {


            var coursesGroupedByRoom = from room in roomRepository.Rooms
                                       where room.Capacity >= searchParameters.Capacity && room.RoomType == searchParameters.Type
                                       join course in courseRepository.Courses on room equals course.RoomAssignment into courseGroup
                                       select new { Room = room, Courses = courseGroup };


            List<ScheduleSlot> availableSlots = new List<ScheduleSlot>();
            foreach (var courseGroup in coursesGroupedByRoom)
            {

                List<Course> courses = courseGroup.Courses
                    .Where(x => x.NeedsRoom && x.MeetingDays.Intersect(searchParameters.MeetingDays).Count(z => true) != 0 && x.StartTime.HasValue && !(x.StartTime.Value >= searchParameters.EndTime || x.EndTime <= searchParameters.StartTime))
                    .OrderBy(x => x.StartTime.Value)
                    .ToList();
                //Check there is no courses in that slot.
                if (courses.Count == 0)
                {
                    availableSlots.Add(
                        new ScheduleSlot()
                        {
                            RoomAvailable = courseGroup.Room,
                            StartTime = searchParameters.StartTime,
                            EndTime = searchParameters.EndTime,
                            MeetingDays = searchParameters.MeetingDays.AsEnumerable()
                        });

                    continue;
                }
                //Calculate duraction of the course.
                if (courses[0].StartTime - searchParameters.StartTime >= searchParameters.Duration)
                {
                    availableSlots.Add(
                        new ScheduleSlot()
                        {
                            RoomAvailable = courseGroup.Room,
                            StartTime = searchParameters.StartTime,
                            EndTime = courses[0].StartTime.Value,
                            MeetingDays = searchParameters.MeetingDays.AsEnumerable()
                        });
                }

                for (int i = 0; i < courses.Count - 1; i++)
                {
                    if (courses[i + 1].StartTime - courses[i].EndTime >= searchParameters.Duration)
                    {
                        availableSlots.Add(
                            new ScheduleSlot()
                            {
                                RoomAvailable = courseGroup.Room,
                                StartTime = courses[i].EndTime.Value,
                                EndTime = courses[i + 1].StartTime.Value,
                                MeetingDays = searchParameters.MeetingDays.AsEnumerable()
                            });
                    }
                }

                if (searchParameters.EndTime - courses.Last().EndTime.Value >= searchParameters.Duration)
                {
                    availableSlots.Add(
                        new ScheduleSlot()
                        {
                            RoomAvailable = courseGroup.Room,
                            StartTime = courses.Last().EndTime.Value,
                            EndTime = searchParameters.EndTime,
                            MeetingDays = searchParameters.MeetingDays.AsEnumerable()
                        });
                }
            }


            return availableSlots;
        }

		/// <summary>
		/// Checks if courses conflict based on time.
		/// </summary>
		/// <param name="courseGroup">The group of courses with potential conflicts.</param>
		/// <param name="startTime">Start time of the courses.</param>
		/// <param name="endTime">End time of the courses.</param>
		/// <returns>True if conflicts exist. False otherwise.</returns>
        private bool CoursesConflictWithTime(IGrouping<string, Course> courseGroup, TimeSpan startTime, TimeSpan endTime)
        {
            bool hasConflict = true;
            // Check if there is any time conflict between two courses.
            foreach (var course in courseGroup)
            {
                if (course.EndTime < startTime)
                {
                    hasConflict = false;
                }
                else if (course.StartTime > endTime)
                {
                    hasConflict = false;
                }
            }

            return hasConflict; //if hasConflict is true then these 2 courses conflict.
        }

    }
}
