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
        private ReassignmentViewModel viewModel;
        private ICourseRepository CourseRepo = CourseRepository.GetInstance();
        public string Type;
        public string Capacity;
        public IRoomRepository RoomRepo = RoomRepository.GetInstance();

        public ReassignmentPage(Course c, string capacity, string type)
        {
            LinkedReassignments node = new LinkedReassignments();
            Type = type;
            Capacity = capacity;

            // initializes the head node
            node.steps = 0;
            node.courseSteps = c.CourseName;
            node.roomSteps = c.RoomAssignment.ToString();

            InitializeComponent();
            viewModel = new ReassignmentViewModel(recursiveReassign(node, c));
            DataContext = viewModel;

            var courses = from course in CourseRepo.Courses
                          where course.CourseName == c.CourseName
                          select course;

            foreach (Course course in courses)
            {
                course.RoomCapRequest = capacity;
            }

        }

        // Still working on this
        public LinkedReassignments recursiveReassign(LinkedReassignments node, Course c)
        {
            int steps = node.steps + 1;
            AvailableRoomSearch availableRoomSearch = new AvailableRoomSearch(RoomRepo, CourseRepo);
            IEnumerable<Room> rooms = availableRoomSearch.AvailableRooms(c.MeetingDays, (TimeSpan)c.StartTime, (TimeSpan)c.EndTime, int.Parse(c.RoomCapRequest), Type);
            LinkedReassignments newnode = new LinkedReassignments();

            // If it takes more than three shuffles then that's too much and ends it
            if (steps > 3)
            {
                return null;
            }
            
            // If there are available rooms with regards to room type 
            else if (rooms != null)
            {
                foreach(var room in rooms)
                {
                    newnode.steps = steps;
                    newnode.courseSteps = node.courseSteps + "," + c.CourseName;
                    newnode.roomSteps = (node.roomSteps + "," + room.RoomName);
                    node.listAppend(newnode);
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
                    // I'm not sure how to fix this query
                    // I'm trying to get a query that gets all the courses
                    // Assigned to the rooms and match the meeting times
                    List<Course> courses = courseGroup.Courses
                    .Where(x => x.HasRoomAssignment && x.MeetingDays.Intersect(c.MeetingDays).Count(z => true) != 0 && x.StartTime.HasValue && !(x.StartTime.Value >= c.EndTime || x.EndTime <= c.StartTime))
                    .OrderBy(x => x.StartTime.Value)
                    .ToList();

                    // Then we'll go through all the paths trying to shuffle here
                    for (int i = 0; i < courses.Count; i++)
                    { 
                        newnode.steps = steps;
                        newnode.courseSteps = node.courseSteps;
                        newnode.roomSteps = node.roomSteps + "," + courseGroup.Room.RoomName;
                        node.listAppend(recursiveReassign(newnode, courses[i]));
                    }
                }
            }

            return node;
        }

        public void CommitReassign(Object sender, RoutedEventArgs e)
        {

        }

        private void ReassignPaths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string display = "";
            string [] display1;
            string [] display2;
            LinkedReassignments node = ReassignPaths.SelectedItem as LinkedReassignments;

            // Hopefully this will be how it works
            display1 = node.courseSteps.Split(',');
            display2 = node.roomSteps.Split(',');

            for(int i = 0; i < display2.Length; i+= 2)
            {
                display += display1[i] + " assigned to" + display2[i] + "\n";
                display += display[i + 1] + " will be assigned to " + display2[i + 1] + "\n";
            }

            PathDisplay.Text = display;
        }
    }
}
