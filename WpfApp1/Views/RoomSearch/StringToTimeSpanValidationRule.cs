using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ClassroomAssignment.Views.RoomSearch
{
    /// <summary>
    /// Validates if the time is in the correct format.
    /// </summary>
    class StringToTimeSpanValidationRule : ValidationRule
    {
        /// <summary>
        /// Override the ValidateRule Validate method.
        /// </summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>Return valid Validation result if the time matches the correct format.False otherwise and print the error.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var regex = new Regex(@"\s*\d{2}:\d{2}\s*");
            var match = regex.Match(value.ToString());
            if (match.Success)
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, "Must be in following format: 01:30");
            }
            
        }
    }
}
