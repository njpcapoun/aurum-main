using ClassroomAssignment.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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

namespace ClassroomAssignment.UI.Edit
{
    /// <summary>
    /// Interaction logic for CourseEditPage.xaml
    /// </summary>
    public partial class CourseEditPage : Page, INotifyPropertyChanged
    {
        private Course originalCourse;

        private Course _copyCourse;
		/// <summary>
		/// Getter and setter for copy of the course bieng editted.
		/// </summary>
        public Course CopyCourse
        {
            get => _copyCourse;
            set
            {
                _copyCourse = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CopyCourse)));
            }
        }

        private List<PropertyInfo> propertiesChanged = new List<PropertyInfo>();

        public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Constructor for CourseEditPage. Initialize the course that is being editted.
		/// </summary>
		/// <param name="course">The course being editted.</param>
        public CourseEditPage(Course course)
        {
            InitializeComponent();
            originalCourse = course;
            CopyCourse = course.ShallowCopy();
            CopyCourse.PropertyChanged += CopyCourse_PropertyChanged;
            DataContext = CopyCourse;
        }

		/// <summary>
		/// Get the properties that have been changed of the course being editted.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void CopyCourse_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Type type = originalCourse.GetType();
            PropertyInfo propertyInfo = type.GetProperty(e.PropertyName);
            if (propertyInfo.SetMethod != null)
            {
                propertiesChanged.Add(propertyInfo);
            }
        }

		/// <summary>
		/// Save the changes made to the editted course when save button is clicked.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool hasError = false;
            if (!CourseDetail.ValidMeetingPattern)
            {
                MeetingPatternWarningTextBlock.Visibility = Visibility.Visible;
                hasError = true;
            }
            if(!CourseDetail.ValidRoomCapRequest)
            {
                RoomCapWarningTextBlock.Visibility = Visibility.Visible;
                hasError = true;
            }

            if (hasError) return;

            foreach (var property in propertiesChanged)
            {
                var newValue = property.GetValue(CopyCourse);
                property.SetValue(originalCourse, newValue);
            }

            originalCourse.MeetingDays = CopyCourse.QueryMeetingDays();
            originalCourse.StartTime = CopyCourse.QueryStartTime();
            originalCourse.EndTime = CopyCourse.QueryEndTime();

            NavigationService.GoBack();
        }

		/// <summary>
		/// Go back to the main page if the cancel button has been clicked.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
