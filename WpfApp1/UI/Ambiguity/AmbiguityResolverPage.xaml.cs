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

        public AmbiguityResolverPage()
        {
            InitializeComponent();
            RoomOptions = new List<Room>() { RoomRepository.NoRoom };
            RoomOptions.AddRange(RoomRepository.GetInstance().Rooms);

            var allCourses = CourseRepository.GetInstance().Courses;

            _ambiguousCourses = allCourses.ToList().FindAll(m => m.HasAmbiguousAssignment);

            CoursesDataGrid.ItemsSource = _ambiguousCourses;

        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var course in _ambiguousCourses)
            {
                course.HasAmbiguousAssignment = false;
            }

            NavigationService.Navigate(new Uri(@"UI/Main/MainPage.xaml", UriKind.Relative));
        }

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
