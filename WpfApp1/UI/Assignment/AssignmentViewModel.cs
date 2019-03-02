using ClassroomAssignment.Model;
using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.Operations;
using ClassroomAssignment.Repo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ClassroomAssignment.Model.Course;
using static ClassroomAssignment.Extension.CourseExtensions;
using System.Windows.Input;

namespace ClassroomAssignment.UI.Assignment
{
    /// <summary>
    /// 
    /// </summary>
    public class AssignmentViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Course> CoursesBeingAssigned { get; } = new ObservableCollection<Course>();
        public ObservableCollection<Room> AvailableRooms { get; } = new ObservableCollection<Room>();
        public ObservableCollection<Course> CoursesForSelectedRoom { get; } = new ObservableCollection<Course>();
        public ObservableCollection<ScheduleSlot> AvailableSlots { get; } = new ObservableCollection<ScheduleSlot>();


        private AvailableRoomSearch RoomSearch;
        private CourseRepository CourseRepo = CourseRepository.GetInstance();

        public event PropertyChangedEventHandler PropertyChanged;

        public List<Room> AllRooms { get; } = RoomRepository.GetInstance().Rooms;

        private Course _currentCourse;
        public Course CurrentCourse
        {
            get => _currentCourse;
            set
            {
                _currentCourse = value;
                if (_currentCourse != null)
                {
                    _currentCourse.PropertyChanged += _currentCourse_PropertyChanged;
                    OnCurrentCourseChanged();
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCourse)));
            }
        }

       

        private Room _currentRoom;
        public Room CurrentRoom
        {
            get => _currentRoom;
            set
            {
                Room room = value;
                if (room == null) return;
                _currentRoom = value;
                UpdateCoursesForCurrentRoom();
                UpdateAvailableSlotsForCurrentRoom();


                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentRoom)));

            }
        }

       

        private void _currentCourse_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnCurrentCourseChanged();
        }


        /// <summary>
        /// adds passed list of courses
        /// to room search view
        /// </summary>
        /// <param name="courses"></param>
        public AssignmentViewModel(IList<Course> courses)
        {
            foreach (var course in courses)
            {
                CoursesBeingAssigned.Add(course);
            }


            IRoomRepository roomRepository = RoomRepository.GetInstance();
            RoomSearch = new AvailableRoomSearch(roomRepository, CourseRepo);

            CurrentCourse = CoursesBeingAssigned.First();
        }

        private void OnCurrentCourseChanged()
        {
            UpdateCoursesForCurrentRoom();
            UpdateAvailableSlotsForCurrentRoom();
            AddAvailableRooms();
            AddConflictingCourses();
        }

        private void AddAvailableRooms()
        {
            AvailableRooms.Clear();

            var course = CurrentCourse;
            int capacity = int.MaxValue;
            bool result = int.TryParse(course.RoomCapRequest, out capacity);
            IEnumerable<Room> rooms = RoomSearch.
                 AvailableRooms(course.MeetingDays, course.StartTime.Value, course.EndTime.Value, capacity);

            foreach (var room in rooms.OrderBy(x => x.Capacity))
            {
                AvailableRooms.Add(room);
            }
        }

        /// <summary>
        /// searches for available rooms and updates 
        /// list of schedule slots
        /// </summary>
        public void UpdateAvailableSlotsForCurrentRoom()
        {
            var searchParameters = CurrentCourse.GetSearchParameters();

            List<ScheduleSlot> slots = RoomSearch.ScheduleSlotsAvailable(searchParameters);
            AvailableSlots.Clear();
            foreach (var slot in slots.FindAll(x => x.RoomAvailable == CurrentRoom))
            {
                AvailableSlots.Add(slot);
            }
        }

        /// <summary>
        /// if there are any changes to current room 
        /// schedule then the changes are updated
        /// </summary>
        public void UpdateCoursesForCurrentRoom()
        {
            if (CurrentRoom == null) return;
            var room = CurrentRoom;
            var courses = from course in CourseRepo.Courses
                          where course.RoomAssignment == room
                          select course;

            CoursesForSelectedRoom.Clear();
            foreach (Course course in courses)
            {
                CoursesForSelectedRoom.Add(course);
            }
        }

        

       /// <summary>
       /// remove uneeded rooms 
       /// </summary>
        public void RemoveStaleAvailableRooms()
        {
            while (AvailableRooms.Count != 0)
            {
                AvailableRooms.RemoveAt(0);
            }
        }

      

        /// <summary>
        /// When conflict is detected then
        /// conflicting courses are added to the current
        /// assignment view 
        /// </summary>
        public void AddConflictingCourses()
        {
            List<Conflict> conflicts = CourseRepo.GetConflictsInvolvingCourses(CoursesBeingAssigned.ToList());

            foreach (var conflict in conflicts)
            {
                foreach (var courseInConflict in conflict.ConflictingCourses)
                {
                    if (!CoursesBeingAssigned.Contains(courseInConflict))
                    {
                        CoursesBeingAssigned.Add(courseInConflict);
                    }
                }
            }
        }

    }
}
