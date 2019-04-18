using ClassroomAssignment.Model;
using ClassroomAssignment.UI.Reassignment;
using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.Operations;
using ClassroomAssignment.Repo;
using ClassroomAssignment.Views.RoomSchedule;
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

namespace ClassroomAssignment.UI.Reassignment 
{
    
    public class ReassignmentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<LinkedReassignments> ReassignPath { get; } = new ObservableCollection<LinkedReassignments>();
        private CourseRepository CourseRepo;
        private RoomRepository RoomRepo;
        // Info tab
        private Room _currentRoom;
        public Room CurrentRoom
        {
            get => _currentRoom;
            set
            {
                _currentRoom = value;
                SetCoursesForCurrentRoom();
            }
        }

        public BindingList<Room> AllRooms { get; set; }
        public ObservableCollection<Course> CoursesForCurrentRoom { get; private set; }

        public ReassignmentViewModel(LinkedReassignments node)
        {
            CourseRepo = CourseRepository.GetInstance();
            RoomRepo = RoomRepository.GetInstance();

            LinkedReassignments traverser = node;

            while (traverser.next != null)
            {
                traverser = traverser.next;
                ReassignPath.Add(traverser);
            }

            AllRooms = convertToBindingList(RoomRepo.Rooms);
            CurrentRoom = AllRooms.FirstOrDefault();
        }

        private void SetCoursesForCurrentRoom()
        {
            CoursesForCurrentRoom = new ObservableCollection<Course>(CourseRepo.Courses.Where(x => x.NeedsRoom && x.RoomAssignment?.Equals(CurrentRoom) == true));
        }

        private BindingList<Room> convertToBindingList(List<Room> rooms)
        {
            BindingList<Room> bindRooms = new BindingList<Room>();
            foreach (Room r in rooms)
            {
                bindRooms.Add(r);
            }
            return bindRooms;
        }
    }
}
