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
    class StringToTimeSpanValidationRule : ValidationRule
    {
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
