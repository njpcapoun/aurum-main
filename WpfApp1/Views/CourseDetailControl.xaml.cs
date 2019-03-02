﻿using ClassroomAssignment.Model;
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
    public partial class CourseDetailControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty CourseShownProperty = DependencyProperty.Register("CourseShown", typeof(Course), typeof(CourseDetailControl));

        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> ValidTimes { get; }
        public bool ValidMeetingPattern { get; private set; } = true;
        public bool ValidRoomCapRequest { get; private set; } = true;

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
        

        public CourseDetailControl()
        {
            InitializeComponent();

            ValidTimes = GetValidTimes();
            StartTimeOptions.ItemsSource = ValidTimes;
            EndTimeOptions.ItemsSource = ValidTimes;
        }

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

        private void DayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var meetingPattern = string.Format("{0} {1}-{2}", GetMeetingDays(), StartTimeOptions.SelectedItem, EndTimeOptions.SelectedItem);
            ScheduleTextBox.Text = meetingPattern;
            (DataContext as Course).MeetingPattern = meetingPattern;
            ValidateMeetingPattern();
        }

        private void ValidateMeetingPattern()
        {
            if (string.IsNullOrEmpty(GetMeetingDays()) || StartTimeOptions.SelectedItem == null || EndTimeOptions.SelectedItem == null)
            {
                ValidMeetingPattern = false;
                return;
            }

            ValidMeetingPattern = DateTime.Parse(StartTimeOptions.SelectedItem as String) < DateTime.Parse(EndTimeOptions.SelectedItem as String);
        }


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

        private void TimeOptions_Selected(object sender, RoutedEventArgs e)
        {
            var meetingPattern = string.Format("{0} {1}-{2}", GetMeetingDays(), StartTimeOptions.SelectedItem, EndTimeOptions.SelectedItem);
            ScheduleTextBox.Text = meetingPattern;
            (DataContext as Course).MeetingPattern = meetingPattern;
            ValidateMeetingPattern();
        }

        private void RmCapRequestTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int i = 0;
            ValidRoomCapRequest = int.TryParse(RmCapRequestTextBox.Text, out i);
        }
    }
}
