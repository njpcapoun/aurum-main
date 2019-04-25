using ClassroomAssignment.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Repo
{
    /// <summary>
    /// Getter for teacher schedules.
    /// </summary>
    public interface ITeacherScheduleRepository
    {
        IEnumerable<Course> Courses { get; }
    }
}
