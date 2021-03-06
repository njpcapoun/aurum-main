﻿using ClassroomAssignment.Model;
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
using System.Text.RegularExpressions;

namespace ClassroomAssignment.UI.Main
{
    /// <summary>
    /// View model for the main page.
    /// </summary>
    [Serializable]
    public class MainWindowViewModel: INotifyPropertyChanged
    {
        // Assign tab
        private ObservableCollection<Course> _courses;

        /// <summary>
        /// Getter and setter for the list of courses.
        /// </summary>
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
        public string _currentTeacher;

        /// <summary>
        /// Getter and setter for the current teacher.
        /// </summary>
        public string CurrentTeacher
        {
            get => _currentTeacher;
            set
            {
                _currentTeacher = value;
            }
        }

        /// <summary>
        /// Getter and setter for the current room.
        /// </summary>
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

        /// <summary>
        /// Getter and setter for the editable room.
        /// </summary>
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
        public ObservableCollection<Course> CoursesForCurrentTeacher { get; set; }

        public CourseRepository CourseRepo;
        public RoomRepository RoomRepo;

        public event PropertyChangedEventHandler PropertyChanged;

        private MainPage Page;

        public List<String> RoomTypes { get; set; }

        /// <summary>
        /// Constructor for MainWindowViewModel. Initializes the main page along with courses and rooms.
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

            RoomTypes = new List<string>();
            RoomTypes.Add(RoomType.Lab);
            RoomTypes.Add(RoomType.Lecture);
            RoomTypes.Add(RoomType.Conference);
            RoomTypes.Add(RoomType.Itin);
            RoomTypes.Add(RoomType.Cyber);
            RoomTypes.Add(RoomType.Distance);

            AllRooms = convertToBindingList(RoomRepo.Rooms);
            CurrentRoom = AllRooms.FirstOrDefault();
            EditableRoom = AllRooms.FirstOrDefault();
        }

        /// <summary>
        /// Handle any changes to the conflicts lists
        /// </summary>
        /// <param name="sender">A reference to the control/object that raised the event.</param>
        /// <param name="e">State information and event data associated with a ChangeInConflicts event.</param>
        private void CourseRepo_ChangeInConflicts(object sender, CourseRepository.ChangeInConflictsEventArgs e)
        {
            Conflicts.Clear();
            foreach (var conflict in e.Conflicts)
            {
                Conflicts.Add(conflict);
            }
           
        }

        /// <summary>
        /// Set the courses for the current room.
        /// </summary>
        private void SetCoursesForCurrentRoom()
        {
            CoursesForCurrentRoom = new ObservableCollection<Course>(CourseRepo.Courses.Where(x => x.NeedsRoom && x.RoomAssignment?.Equals(CurrentRoom) == true));
        }

        /// <summary>
        /// Set data for the editable room.
        /// </summary>
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

        /// <summary>
        /// Convert the list of rooms to a binding list for data binding.
        /// </summary>
        /// <param name="rooms">The list of rooms</param>
        /// <returns>The binding list of rooms.</returns>
        private BindingList<Room> convertToBindingList(List<Room> rooms)
        {
            BindingList<Room> bindRooms = new BindingList<Room>();
            foreach (Room r in rooms)
            {
                bindRooms.Add(r);
            }
            return bindRooms;
        }

        /// <summary>
        /// Update the rooms when they are edited.
        /// </summary>
        public void UpdateRoomList()
        {
            AllRooms = convertToBindingList(RoomRepo.Rooms);
            CurrentRoom = AllRooms.FirstOrDefault();
            EditableRoom = AllRooms.FirstOrDefault();
            RoomRepo.SaveData();
        }
    }
    
}
