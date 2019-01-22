using ClassroomAssignment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment
{
    [Serializable]
    public class AppState
    {
        public List<Course> OriginalCourses { get; }
        public List<Course> CurrentCourses { get; }

        public AppState(List<Course> original, List<Course> current)
        {
            OriginalCourses = original;
            CurrentCourses = current;
        }
        
    }
}
