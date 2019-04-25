using ClassroomAssignment.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ClassroomAssignment.UI.Changes
{
	/// <summary>
	/// Highlight the course differences with a border highlight.
	/// </summary>
    public class CourseDifferenceToBorderBrushConverter : IMultiValueConverter
    {
		/// <summary>
		/// Highlight the changes made to a course with red.
		/// </summary>
		/// <param name="values">The course values for the datagrid.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns></returns>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var dataGrid = values[0] as DataGrid;
            var dataGridCell = values[1] as DataGridCell;
            var courseDiff = values[2] as CourseDifference;

            
            Brush NoChangeBrush = Brushes.Transparent;
            Brush ChangeBrush = Brushes.Red;

            var column = dataGrid.Columns[dataGridCell.Column.DisplayIndex];

            if (column is DataGridTextColumn && courseDiff != null)
            {
                var textColumn = column as DataGridTextColumn;
                var binding = textColumn.Binding as Binding;
                var propertyName = binding.Path.Path.Replace("NewestCourse.", "");

                var property = courseDiff.OriginalCourse.GetType().GetProperty(propertyName);
                if (property == null) return NoChangeBrush;

                var type = property.PropertyType;
                if (property.PropertyType == typeof(string))
                {
                    var oldValue = property.GetValue(courseDiff.OriginalCourse) as string;
                    var newValue = property.GetValue(courseDiff.NewestCourse) as string;

                    return oldValue == newValue ? NoChangeBrush : ChangeBrush;
                }
                else if (property.PropertyType == typeof(Room))
                {
                    var oldValue = property.GetValue(courseDiff.OriginalCourse) as Room;
                    var newValue = property.GetValue(courseDiff.NewestCourse) as Room;

                    return oldValue == newValue ? NoChangeBrush : ChangeBrush;
                }
            }

            return NoChangeBrush;
        }

		/// <summary>
		/// Undo the red border highlight.
		/// </summary>
		/// <param name="values">The course values for the datagrid.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns></returns>
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
