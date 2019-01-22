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

        public CourseEditPage(Course course)
        {
            InitializeComponent();
            originalCourse = course;
            CopyCourse = course.ShallowCopy();
            CopyCourse.PropertyChanged += CopyCourse_PropertyChanged;
            DataContext = CopyCourse;
        }

        private void CopyCourse_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Type type = originalCourse.GetType();
            PropertyInfo propertyInfo = type.GetProperty(e.PropertyName);
            if (propertyInfo.SetMethod != null)
            {
                propertiesChanged.Add(propertyInfo);
            }
        }

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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
