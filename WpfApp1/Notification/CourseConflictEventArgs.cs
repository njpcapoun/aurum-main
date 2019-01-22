using ClassroomAssignment.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Notification
{
    class CourseConflictEventArgs : EventArgs
    {
        /// <summary>
        /// get method for conflicts
        /// </summary>
        public List<Conflict> Conflicts { get; }

        /// <summary>
        /// set method for conflicts
        /// </summary>
        /// <param name="conflicts"></param>
        public CourseConflictEventArgs(List<Conflict> conflicts)
        {
            Conflicts = conflicts;
        }
    }
}
