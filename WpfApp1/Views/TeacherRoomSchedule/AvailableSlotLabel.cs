using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClassroomAssignment.Views.TeacherRoomSchedule
{
    /// <summary>
    /// Sets the size and color for availble slot label
    /// </summary>
    public class AvailableSlotLabel : TextBlock
    {
        /// <summary>
        /// Constructor for AvailableSlotLabel. Sets up attributes for the available slot labels.
        /// </summary>
        /// <param name="startTime">The start time for the available slot.</param>
        /// <param name="endTime">The end time for the available slot.</param>
        public AvailableSlotLabel(TimeSpan startTime, TimeSpan endTime)
        {
            var start = new DateTime().Add(startTime);
            var end = new DateTime().Add(endTime);
            Margin = new Thickness(5, 0, 5, 0);
            Text = string.Format("{0}{1}{2:t}-{3:t}", "Available", Environment.NewLine, start, end);
            TextAlignment = TextAlignment.Center;
            Padding = new Thickness(5);

            SetBackground();
        }

        /// <summary>
        /// Set the background for the available schedule slot.
        /// </summary>
        private void SetBackground()
        {
            var color = Brushes.LightGreen.Color;
            color.A = 100;
            Background = new SolidColorBrush(color);
        }
        
    }
}
