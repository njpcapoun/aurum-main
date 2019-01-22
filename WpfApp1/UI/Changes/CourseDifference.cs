using ClassroomAssignment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.UI.Changes
{
    public class CourseDifference
    {
        public string DifferenceType { get; set; }
        public Course OriginalCourse { get; set; }
        public Course NewestCourse { get; set; }
    }

}
 