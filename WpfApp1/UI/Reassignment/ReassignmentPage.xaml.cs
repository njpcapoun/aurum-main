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

namespace ClassroomAssignment.UI.Reassignment
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ReassignmentPage : Page
    {
        private ReassignmentViewModel viewModel;
        private CourseRepository CourseRepo = CourseRepository.GetInstance();

        public ReassignmentPage(Course c, string capacity, string type)
        {
            LinkedReassignments node = new LinkedReassignments();
            
            // These are for testing and display only, when we implement
            // The algorithm the nodes will contain instructions for various
            // Reassignment Paths
            node.steps = 0;
            node.courseSteps = c.CourseName;
            node.roomSteps = c.RoomAssignment.ToString();

            InitializeComponent();
            viewModel = new ReassignmentViewModel(recursiveReassign(node));
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
        public LinkedReassignments recursiveReassign(LinkedReassignments node)
        {
            return node;
        }

        public void CommitReassign(Object sender, RoutedEventArgs e)
        {

        }

        private void ReassignPaths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string display;
            LinkedReassignments node = ReassignPaths.SelectedItem as LinkedReassignments;

            // This is for testing, in the future the instructions will be seperated by a delimiter
            // And be split and then displayed properly
            display = node.courseSteps;
            display += " is assigned to room ";
            display += node.roomSteps;

            PathDisplay.Text = display;
        }
    }
}
