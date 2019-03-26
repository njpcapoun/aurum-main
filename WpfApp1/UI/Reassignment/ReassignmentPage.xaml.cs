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
        public ReassignmentPage(Course c, string capacity, string type)
        {
            InitializeComponent();
            viewModel = new ReassignmentViewModel(c);
            recursiveReassign(c);
        }

        // Not sure what the return type should be
        public LinkedReassignments recursiveReassign(Course c)
        {
         
        }
    }
}
