using ClassroomAssignment.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ClassroomAssignment.Extension;

namespace ClassroomAssignment.UI.Main
{
    /// <summary>
    /// Takes groups of courses and converts their type 
    /// as either assigned, unassigned, conflicting, no room needed, or ambiguous 
    /// </summary>
    public class GroupNameConverter : IValueConverter
    {
        /// <summary>
        /// Assigns a state to each course
        /// </summary>
		/// <param name="values">The course state values to be convertted.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The course's converted state.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Course.CourseState? state = value as Course.CourseState?;

            if (!state.HasValue) return string.Empty;

            switch(state.Value)
            {
                case Course.CourseState.Ambiguous:
                    return Course.CourseState.Ambiguous.GetDescription();
                case Course.CourseState.Assigned:
                    return Course.CourseState.Assigned.GetDescription();
                case Course.CourseState.Unassigned:
                    return Course.CourseState.Unassigned.GetDescription();
                case Course.CourseState.NoRoomRequired:
                    return Course.CourseState.NoRoomRequired.GetDescription();
                case Course.CourseState.Conflicting:
                    return Course.CourseState.Conflicting.GetDescription();
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Undo the course state conversion
        /// </summary>
		/// <param name="values">The course state values to be converted back.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
        /// <returns>new NotImplementedException</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
