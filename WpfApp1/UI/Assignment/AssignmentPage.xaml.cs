using ClassroomAssignment.Model;
using ClassroomAssignment.Repo;
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

        void onClickSave(object sender, RoutedEventArgs e)
        {
            save();
        }

        void save()
        {
            SaveFileDialog saveFileDialog2 = new SaveFileDialog();
            saveFileDialog2.Filter = "Assignment File | *.agn";

            if (saveFileDialog2.ShowDialog() == DialogResult.OK)
            {
                var fileName = saveFileDialog2.FileName;

                try
                {
                    List<Course> originalCourses = GetOriginalCourses();
                    AppState appState = new AppState(originalCourses, GetUpToDateCourses());

                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = File.Open(fileName, FileMode.Create, FileAccess.Write);

                    formatter.Serialize(stream, appState);
                    stream.Close();

                }
                catch (SerializationException a)
                {
                    Console.WriteLine("Failed to deserialize. Reason: " + a.Message);
                }

            }
        }

        private List<Course> GetOriginalCourses()
        {
            return App.Current.Resources["originalCourses"] as List<Course>;
        }

        private List<Course> GetUpToDateCourses()
        {
            return CourseRepository.GetInstance().Courses.OrderBy(x => int.Parse(x.ClassID)).ToList();
        }

    }

}

