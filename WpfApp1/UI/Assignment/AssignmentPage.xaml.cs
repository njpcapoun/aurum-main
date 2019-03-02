using ClassroomAssignment.Model;
using ClassroomAssignment.Repo;
using ClassroomAssignment.Operations;
using ClassroomAssignment.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClassroomAssignment.UI.Assignment
{
    /// <summary>
    /// Interaction logic for AssignmentPage.xaml
    /// </summary>
    public partial class AssignmentPage : Page
    {
        private AssignmentViewModel viewModel;

        SaveBase saveWork = new SaveBase();

        public AssignmentPage(List<Course> courses)
        {

            InitializeComponent();
            viewModel = new AssignmentViewModel(courses);

            DataContext = viewModel;
            
            AvailableRoomsListView.ItemsSource = viewModel.AvailableRooms;
            RoomSchedule.RoomScheduled = viewModel.CurrentRoom;
            RoomSchedule.CoursesForRoom = viewModel.CoursesForSelectedRoom;
            RoomSchedule.AvailableSlots = viewModel.AvailableSlots;

            
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null) return;
            window.SizeToContent = SizeToContent.Width;
        }

        void OnClickSave(object sender, RoutedEventArgs e)
        {
            saveWork.SaveWork();
            System.Windows.Forms.MessageBox.Show("Saved!");
        }

    }

}

