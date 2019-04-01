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

        public ReassignmentPage(Course c, string capacity, string type)
        {
            LinkedReassignments node = new LinkedReassignments();
            node.steps = 0;
            node.courseSteps = c.CourseName;

            InitializeComponent();
            viewModel = new ReassignmentViewModel(recursiveReassign(node));
            DataContext = viewModel;
        }

        // Still working on this
        public LinkedReassignments recursiveReassign(LinkedReassignments node)
        {
            return node;
        }

        public void CommitReassign(Object sender, RoutedEventArgs e)
        {

        }
    }
}
