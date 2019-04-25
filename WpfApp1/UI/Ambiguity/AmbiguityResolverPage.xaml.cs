using ClassroomAssignment.Model;
using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.Repo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClassroomAssignment.UI.Ambiguity
{
    /// <summary>
    /// Interaction logic for AmbiguityResolverPage.xaml
    /// </summary>
    public partial class AmbiguityResolverPage : Page, INotifyPropertyChanged
    {
        private List<Course> _ambiguousCourses;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<Room> RoomOptions { get; }

		/// <summary>
		/// Constructor for AmbiguityResolverPage. Get instances of the room and course repositories.
		/// </summary>
        public AmbiguityResolverPage()
        {
            InitializeComponent();
			Application.Current.MainWindow.WindowState = WindowState.Maximized;
			RoomOptions = new List<Room>() { RoomRepository.NoRoom };
            RoomOptions.AddRange(RoomRepository.GetInstance().Rooms);

            var allCourses = CourseRepository.GetInstance().Courses;

            _ambiguousCourses = allCourses.ToList().FindAll(m => m.HasAmbiguousAssignment);

            CoursesDataGrid.ItemsSource = _ambiguousCourses;

        }

		/// <summary>
		/// Navigate to the main page after resolving ambigiuous assignments.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var course in _ambiguousCourses)
            {
                course.HasAmbiguousAssignment = false;
            }

            NavigationService.Navigate(new Uri(@"UI/Main/MainPage.xaml", UriKind.Relative));
        }

		/// <summary>
		/// Handle the room selection of a course. Null room assignment if None is selected. The selected room otherwise.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a SelectionChanged event.</param>
		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var course = (sender as ComboBox).DataContext as Course;
            var selectedRoom = (sender as ComboBox).SelectedItem as Room;
            
            if (selectedRoom == RoomRepository.NoRoom)
            {
                course.RoomAssignment = null;
            }
            else
            {
                course.RoomAssignment = selectedRoom;
            }
        }
    }

}
