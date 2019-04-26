using ClassroomAssignment.Model;
using ClassroomAssignment.Model.Utils;
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

namespace ClassroomAssignment.Views
{
    /// <summary>
    /// Interaction logic for CourseDetailControl.xaml
    /// </summary>
    public partial class CourseDetailLocked: UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty CourseShownProperty = DependencyProperty.Register("CourseShown", typeof(Course), typeof(CourseDetailLocked));

        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> ValidTimes { get; }
        public bool ValidMeetingPattern { get; private set; } = true;
        public bool ValidRoomCapRequest { get; private set; } = true;

        /// <summary>
        /// Getter and setter for the course details being displayed.
        /// </summary>
        public Course CourseShown
        {
            get
            {
                return (Course)GetValue(CourseShownProperty);
            }

            set
            {
                SetValue(CourseShownProperty, value);
            }
        }

        /// <summary>
        /// Constructor for CourseDetailLocked. Set the valid times for the start and end time options.
        /// </summary>
        public CourseDetailLocked()
        {
            InitializeComponent();

            ValidTimes = GetValidTimes();
            StartTimeOptions.ItemsSource = ValidTimes;
            EndTimeOptions.ItemsSource = ValidTimes;
        }

        /// <summary>
        /// Set up the list of times for the start and end times.
        /// </summary>
        /// <returns>The list of valid times the course can be assigned.</returns>
        private List<string> GetValidTimes()
        {
            var validTimes = new List<string>();
            TimeSpan lastTime = new TimeSpan(22, 0, 0);
            TimeSpan currentTime = new TimeSpan(7, 0, 0);
            TimeSpan diff = new TimeSpan(0, 5, 0);
            while (currentTime <= lastTime)
            {
                validTimes.Add((new DateTime()).Add(currentTime).ToString("hh:mmtt"));
                currentTime += diff;
            }

            return validTimes;
        }

        /// <summary>
        /// The check boxes for the days of the week the course is on.
        /// </summary>
        /// <param name="sender">A reference to the control/object that raised the event.</param>
        /// <param name="e">State information and event data associated with a routed event.</param>
        private void DayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var meetingPattern = string.Format("{0} {1}-{2}", GetMeetingDays(), StartTimeOptions.SelectedItem, EndTimeOptions.SelectedItem);
            ScheduleTextBox.Text = meetingPattern;
            (DataContext as Course).MeetingPattern = meetingPattern;
            ValidateMeetingPattern();
        }

        /// <summary>
        /// Validate the meeting times and days for the course.
        /// </summary>
        private void ValidateMeetingPattern()
        {
            if (string.IsNullOrEmpty(GetMeetingDays()) || StartTimeOptions.SelectedItem == null || EndTimeOptions.SelectedItem == null)
            {
                ValidMeetingPattern = false;
                return;
            }

            ValidMeetingPattern = DateTime.Parse(StartTimeOptions.SelectedItem as String) < DateTime.Parse(EndTimeOptions.SelectedItem as String);
        }

        /// <summary>
        /// Get the meeting days for the course.
        /// </summary>
        /// <returns>String of the meetings days for a course.</returns>
        private string GetMeetingDays()
        {
            List<string> meetingDays = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            if (MondayCheckBox.IsChecked == true) stringBuilder.Append(MondayCheckBox.Content as string);
            if (TuesdayCheckBox.IsChecked == true) stringBuilder.Append(TuesdayCheckBox.Content as string);
            if (WednesdayCheckBox.IsChecked == true) stringBuilder.Append(WednesdayCheckBox.Content as string);
            if (ThursdayCheckBox.IsChecked == true) stringBuilder.Append(ThursdayCheckBox.Content as string);
            if (FridayCheckBox.IsChecked == true) stringBuilder.Append(FridayCheckBox.Content as string);
            if (SaturdayCheckBox.IsChecked == true) stringBuilder.Append(SaturdayCheckBox.Content as string);

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Handle the event when the time option combo boxes have changed.
        /// </summary>
        /// <param name="sender">A reference to the control/object that raised the event.</param>
        /// <param name="e">State information and event data associated with a routed event.</param>
        private void TimeOptions_Selected(object sender, RoutedEventArgs e)
        {
            var meetingPattern = string.Format("{0} {1}-{2}", GetMeetingDays(), StartTimeOptions.SelectedItem, EndTimeOptions.SelectedItem);
            ScheduleTextBox.Text = meetingPattern;
            (DataContext as Course).MeetingPattern = meetingPattern;
            ValidateMeetingPattern();
        }

        /// <summary>
        /// Handle the event when the room capacity request text box has changed.
        /// </summary>
        /// <param name="sender">A reference to the control/object that raised the event.</param>
        /// <param name="e">State information and event data associated with a TextChanged event.</param>
        private void RmCapRequestTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int i = 0;
            ValidRoomCapRequest = int.TryParse(RmCapRequestTextBox.Text, out i);
        }
    }
}
