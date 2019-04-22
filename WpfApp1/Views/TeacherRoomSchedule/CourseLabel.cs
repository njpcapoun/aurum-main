using ClassroomAssignment.Model;
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
    /// Sets label sizes for visualization of room assignment
    /// view 
    /// </summary>
    class CourseLabel : TextBlock
    {
        private readonly Course _boundCourse;
        public Course BoundCourse
        { get => _boundCourse; }

        /// <summary>
        /// sets margins and size for course label
        /// </summary>
        /// <param name="course"></param>
        public CourseLabel(Course course)
        {
            _boundCourse = course;
            TextWrapping = TextWrapping.Wrap;
            Margin = new Thickness(5, 0, 5, 0);
            Text = course.ToString();
        }

       
    }
}
