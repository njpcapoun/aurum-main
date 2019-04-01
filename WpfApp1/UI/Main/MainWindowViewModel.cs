using ClassroomAssignment.Model;
using ClassroomAssignment.Repo;
using ClassroomAssignment.Visual;
using ClassroomAssignment.Notification;
using ClassroomAssignment.Operations;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using ClassroomAssignment.Model.Repo;

namespace ClassroomAssignment.UI.Main
{
    /// <summary>
    /// Main Window
    /// </summary>
    [Serializable]
    public class MainWindowViewModel: INotifyPropertyChanged
    {
        // Assign tab
        private ObservableCollection<Course> _courses;
        public ObservableCollection<Course> Courses
        {
            get { return _courses; }
            set
            {
                _courses = value;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Courses)));
            }
        }
        public ObservableCollection<Conflict> Conflicts { get; } = new ObservableCollection<Conflict>();

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
        private Room _editableRoom;
        public Room EditableRoom
        {
            get => _editableRoom;
            set
            {
                _editableRoom = value;
                SetDataForEditableRoom();
            }
        }
        public ObservableCollection<Room> RoomList { get; set; }

        public BindingList<Room> AllRooms { get; set; }
        public ObservableCollection<Course> CoursesForCurrentRoom { get; private set; }

        private CourseRepository CourseRepo;
        public RoomRepository RoomRepo;

        public event PropertyChangedEventHandler PropertyChanged;

        private MainPage Page;

        /// <summary>
        /// initializes main window
        /// </summary>
        public MainWindowViewModel(MainPage page)
        {
            this.Page = page;
            CourseRepo = CourseRepository.GetInstance();
            RoomRepo = RoomRepository.GetInstance();

            Courses = new ObservableCollection<Course>(CourseRepo.Courses);
            
            foreach (var conflict in CourseRepo.GetConflicts())
            {
                Conflicts.Add(conflict);
            }

            CourseRepo.ChangeInConflicts += CourseRepo_ChangeInConflicts;

            AllRooms = convertToBindingList(RoomRepo.Rooms);
            CurrentRoom = AllRooms.FirstOrDefault();
            EditableRoom = AllRooms.FirstOrDefault();
        }

        private void CourseRepo_ChangeInConflicts(object sender, CourseRepository.ChangeInConflictsEventArgs e)
        {
            Conflicts.Clear();
            foreach (var conflict in e.Conflicts)
            {
                Conflicts.Add(conflict);
            }
           
        }

        private void SetCoursesForCurrentRoom()
        {
            CoursesForCurrentRoom = new ObservableCollection<Course>(CourseRepo.Courses.Where(x => x.NeedsRoom && x.RoomAssignment?.Equals(CurrentRoom) == true));
        }

        private void SetDataForEditableRoom()
        {
            RoomList = new ObservableCollection<Room>();
            IEnumerator<Room> enumerator = AllRooms.GetEnumerator();
            while(enumerator.MoveNext())
            {
                RoomList.Add(enumerator.Current);
            }
            Page.saveChanges.IsEnabled = true;
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

        public void UpdateRoomList()
        {
            AllRooms = convertToBindingList(RoomRepo.Rooms);
            CurrentRoom = AllRooms.FirstOrDefault();
            EditableRoom = AllRooms.FirstOrDefault();
            RoomRepo.SaveData();
        }

    }
    
}
