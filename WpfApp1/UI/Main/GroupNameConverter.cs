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
    /// as either assigned, unassigned, no room needed, or ambiguous 
    /// </summary>
    public class GroupNameConverter : IValueConverter
    {
        /// <summary>
        /// Assigns a state to each course
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>object</returns>
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
