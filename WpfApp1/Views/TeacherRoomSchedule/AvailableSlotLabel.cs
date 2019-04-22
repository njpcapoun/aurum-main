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
    /// Sets visualization size for room schedule
    /// </summary>
    public class AvailableSlotLabel : TextBlock
    {
        /// <summary>
        /// Sets size and margin for available slots label
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
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

        private void SetBackground()
        {
            var color = Brushes.LightGreen.Color;
            color.A = 100;
            Background = new SolidColorBrush(color);
        }
        
    }
}
