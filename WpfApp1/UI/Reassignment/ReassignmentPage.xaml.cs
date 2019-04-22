using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ClassroomAssignment.Model;
using ClassroomAssignment.Repo;
using ClassroomAssignment.Operations;
using ClassroomAssignment.Views.RoomSchedule;
using ClassroomAssignment.ViewModel;
using ClassroomAssignment.UI.Reassignment;
using ClassroomAssignment.Model.Repo;

namespace ClassroomAssignment.UI.Reassignment
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ReassignmentPage : Page
    {
        SaveBase saveWork;
        private ReassignmentViewModel viewModel;
        private ICourseRepository CourseRepo = CourseRepository.GetInstance();
        public string Type;
        public string Capacity;
        public IRoomRepository RoomRepo = RoomRepository.GetInstance();

        public ReassignmentPage(Course c, string capacity, string type)
        {
            saveWork = new SaveBase();

            LinkedReassignments node = new LinkedReassignments();
            Type = type;
            Capacity = capacity;

            var courses = from course in CourseRepo.Courses
                          where course.CourseName == c.CourseName && course.SectionNumber == c.SectionNumber
                          select course;

            foreach (Course course in courses)
            {
                course.RoomCapRequest = capacity;
            }

            c.RoomCapRequest = capacity;

            // initializes the head node
            node.steps = 0;
            node.courseSteps = c.CourseName;
            node.courseSections = c.SectionNumber;
            node.roomSteps = c.RoomAssignment.ToString();
            node.next = null;

            InitializeComponent();
            node = recursiveReassign(node, c);
            viewModel = new ReassignmentViewModel(node);
            DataContext = viewModel;

            if(viewModel.ReassignPath.Count() == 0)
            {
                PathDisplay.Text = "Couldn't reassign the course.\nPlease change the course time or manually reassign the room";
                RoomInfo.Text = "";
            }
        }

        // Still working on this
        public LinkedReassignments recursiveReassign(LinkedReassignments node, Course c)
        {
            int steps = node.steps + 1;
            LinkedReassignments traverser;
            AvailableRoomSearch availableRoomSearch = new AvailableRoomSearch(RoomRepo, CourseRepo);
            IEnumerable<Room> rooms = availableRoomSearch.AvailableRooms(c.MeetingDays, (TimeSpan)c.StartTime, (TimeSpan)c.EndTime, int.Parse(c.RoomCapRequest), Type);
            LinkedReassignments newnode;

            // If it takes more than three shuffles then that's too much and ends it
            if (steps > 3)
            {
                return null;
            }

            // If there are available rooms with regards to room type 
            else if (rooms.Count() > 0)
            {
                foreach (var room in rooms)
                {
                    traverser = node;
                    newnode = new LinkedReassignments();
                    newnode.steps = steps;
                    newnode.courseSections = node.courseSections + "," + c.SectionNumber;
                    newnode.courseSteps = node.courseSteps + "," + c.CourseName;
                    newnode.roomSteps = (node.roomSteps + "," + room.RoomName);
                    newnode.next = null;

                    while (traverser.next != null)
                    {
                        traverser = traverser.next;
                    }

                    traverser.next = newnode;
                }

                return node;
            }

            // Finds all the rooms that match, if a course is assigned to it takes that room, then puts that course through
            // the algorithm
            else
            {
                // Took this from AvailableRoomSearch
                // Pretty sure it gets all rooms from the roomrepository that matches the requirements
                // Then finds all the courses that are assigned to those rooms
                var coursesGroupedByRoom = from room in RoomRepo.Rooms
                                           where room.Capacity >= int.Parse(c.RoomCapRequest) && room.RoomType == Type
                                           join course in CourseRepo.Courses on room equals course.RoomAssignment into courseGroup
                                           select new { Room = room, Courses = courseGroup };

                foreach (var courseGroup in coursesGroupedByRoom)
                {
                    // If no available rooms are found find all the courses assigned
                    // to rooms that match the specifications and run the algorithm on them
                    List<Course> courses = courseGroup.Courses
                    .Where(x => x.HasRoomAssignment && x.MeetingDays.Intersect(c.MeetingDays).Count(z => true) != 0 && x.StartTime.HasValue && 
                    ((x.StartTime.Value <= c.EndTime && x.StartTime.Value >= c.StartTime) || (x.EndTime >= c.StartTime && x.EndTime <= c.EndTime)))
                    .OrderBy(x => x.StartTime.Value)
                    .ToList();

                    // Then we'll go through all the paths trying to shuffle here
                    for (int i = 0; i < courses.Count; i++)
                    {
                        newnode = new LinkedReassignments();
                        traverser = node;
                        newnode.steps = steps;
                        newnode.courseSections = node.courseSections + "," + c.SectionNumber + "," + courses[i].SectionNumber;
                        newnode.courseSteps = node.courseSteps + "," + c.CourseName + "," + courses[i].CourseName;
                        newnode.roomSteps = node.roomSteps + "," + courses[i].RoomAssignment.RoomName + "," + courses[i].RoomAssignment.RoomName;
                        newnode.next = null;
                        newnode = recursiveReassign(newnode, courses[i]);

                        // The first node is just this nodes info which isn't what we want
                        if (newnode != null)
                        {
                            if (newnode.next != null)
                            {
                                newnode = newnode.next;

                                while (traverser.next != null)
                                {
                                    traverser = traverser.next;
                                }

                                traverser.next = newnode;
                            }
                        }
                    }
                }

                return node;
            }
        }

        void CommitReassign(Object sender, RoutedEventArgs e)
        {
            string[] coursestoassign;
            string[] roomstoassign;
            string[] coursesections;
            LinkedReassignments node = ReassignPaths.SelectedItem as LinkedReassignments;

            // Splits the strings
            coursestoassign = node.courseSteps.Split(',');
            roomstoassign = node.roomSteps.Split(',');
            coursesections = node.courseSections.Split(',');

            for (int i = 0; i < roomstoassign.Length; i++)
            {
                if (node.steps == 0)
                {
                    MessageBox.Show("You can not reassign with this item!");
                    break;
                }

                else if(i != 0 && i % 2 == 1)
                {
                    var thecourse = from course in CourseRepo.Courses
                                  where course.CourseName == coursestoassign[i] && course.SectionNumber == coursesections[i]
                                  select course;

                    var theroom = from room in RoomRepo.Rooms
                                  where room.RoomName == roomstoassign[i]
                                  select room;

                    foreach (Course course in thecourse)
                    {
                        foreach(Room room in theroom)
                        {
                            System.Diagnostics.Debug.WriteLine("Course is: " + course.CourseName + " assigned to " + room.RoomName);
                            course.Room = room.RoomName;
                            course.RoomAssignment = room;
                        }
                    }
                }
            }

            MessageBox.Show("Reassigned rooms to courses");
            saveWork.SaveWork();
        }

        private void ReassignPaths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string display = "";
            string infoDisplay = "";
            string [] display1; // Contains the course names
            string [] display2; // Contains the room names
            string [] display3;  // Contains the course sections
            LinkedReassignments node = ReassignPaths.SelectedItem as LinkedReassignments;

            // Splits the strings
            display1 = node.courseSteps.Split(',');
            display2 = node.roomSteps.Split(',');
            display3 = node.courseSections.Split(',');

            for(int i = 0; i < display2.Length; i++)
            {
                if (node.steps == 0)
                {
                    display += display1[i] + " is assigned to " + display2[i] + "\n";
                    display += "If this is the only item you see the reassignment has failed";
                }

                else if(i != 0 && i % 2 == 1)
                {
                    display += display1[i]  + " will be assigned to " + display2[i] + "\n\n";
                }

                else
                {
                    display += display1[i] + " was assigned to " + display2[i] + "\n";

                    var thecourse = from course in CourseRepo.Courses
                                    where course.CourseName == display1[i] && course.SectionNumber == display3[i]
                                    select course;

                    foreach(Course course in thecourse)
                    {
                        infoDisplay += course.CourseName + " Section " + course.SectionNumber + "\n"  + course.MeetingPattern.ToString() + "\n\n"; 
                    }
                }
            }

            PathDisplay.Text = display;
            RoomInfo.Text = infoDisplay;
        }

        private void RoomScheduleControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
