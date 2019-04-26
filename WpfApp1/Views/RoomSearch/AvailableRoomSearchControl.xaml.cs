using ClassroomAssignment.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace ClassroomAssignment.Views.RoomSearch
{
    /// <summary>
    /// Interaction logic for AvailableRoomSearch.xaml
    /// </summary>
    public partial class AvailableRoomSearchControl : UserControl
    {
        private int Capacity;
        private TimeSpan Duration;
        private TimeSpan StartTime;
        private TimeSpan EndTime;
        private List<DayOfWeek> MeetingDays;

        /// <summary>
        /// Constructor for AvailbleRoomSearchControl. Initialize the components.
        /// </summary>
        public AvailableRoomSearchControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Getter for checking if the search parameters are valid.
        /// </summary>
        public bool ParametersValid
        {
            get
            {
                return ValidateInput();
            }
        }

        /// <summary>
        /// Getter for the search parameters for an available room.
        /// </summary>
        public SearchParameters SearchParameters
        {
            get
            {
                var searchParameters = new SearchParameters();
                searchParameters.Capacity = Capacity;
                searchParameters.Duration = Duration;
                searchParameters.MeetingDays = MeetingDays;
                searchParameters.StartTime = StartTime;
                searchParameters.EndTime = EndTime;

                return searchParameters;
            }
        }

        /// <summary>
        /// Validate the search parameters.
        /// </summary>
        /// <returns>True if all the search parameters are valid. False otherwise.</returns>
        private bool ValidateInput()
        {
            return MeetingDaysValid()
                && DurationValid()
                && CapacityValid()
                && StartTimeValid()
                && EndTimeValid();
        }

        /// <summary>
        /// Validate and add the available meeting days for a course to be assigned.
        /// </summary>
        /// <returns>The list of meetings days if it is not equal to zero.</returns>
        private bool MeetingDaysValid()
        {
            List<DayOfWeek> meetingDays = new List<DayOfWeek>();

            if (MondayCheckBox.IsChecked == true) meetingDays.Add(DayOfWeek.Monday);
            if (TuesdayCheckBox.IsChecked == true) meetingDays.Add(DayOfWeek.Tuesday);
            if (WednesdayCheckBox.IsChecked == true) meetingDays.Add(DayOfWeek.Wednesday);
            if (ThursdayCheckBox.IsChecked == true) meetingDays.Add(DayOfWeek.Thursday);
            if (FridayCheckBox.IsChecked == true) meetingDays.Add(DayOfWeek.Friday);

            MeetingDays = meetingDays;

            return MeetingDays.Count != 0;
        }

        /// <summary>
        /// Validate the time duration of a course to be assigned.
        /// </summary>
        /// <returns>True if it's valid. False otherwise.</returns>
        private bool DurationValid()
        {
            var duration = DurationTextBox.Text;
            var regex = new Regex(@"\s*\d{1,2}:\d{2}\s*");
            var match = regex.Match(duration);

            if (match.Success)
            {
                Duration = TimeSpan.Parse(duration);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Validate the capacity of a course to be assigned
        /// </summary>
        /// <returns>True if it's valid. False otherwise.</returns>
        private bool CapacityValid()
        {
            int capacity = -1;
            if (int.TryParse(CapacityTextBox.Text, out capacity))
            {
                Capacity = capacity;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Validate the start time of a course to be assigned.
        /// </summary>
        /// <returns>True if it's valid. False otherwise.</returns>
        private bool StartTimeValid()
        {
            var startTime = StartTimeTextBox.Text;
            var regex = new Regex(@"\s*\d{1,2}:\d{2}\s*(am|AM|pm|PM)\s*");
            var match = regex.Match(startTime);

            if (match.Success)
            {
                StartTime = DateTime.Parse(startTime).TimeOfDay;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Validate the end time of a course to be assigned.
        /// </summary>
        /// <returns>True if it's valid. False otherwise.</returns>
        private bool EndTimeValid()
        {
            var endTime = EndTimeTextBox.Text;
            var regex = new Regex(@"\s*\d{1,2}:\d{2}\s*(am|AM|pm|PM)\s*");
            var match = regex.Match(endTime);

            if (match.Success)
            {
                EndTime = DateTime.Parse(endTime).TimeOfDay;
                return true;
            }
            else return false;
        }
    }
}
