using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Visual
{
	/// <summary>
	/// Interface for the Excel Schedule Printer.
	/// </summary>
    public interface ISchedulePrinter
    {
        void Print(ICourseRepository courseRepo, IRoomRepository roomRepo);
        void PrintSchedule(ITeacherScheduleRepository courseRepo, IRoomRepository roomRepo, string teacherName);
    }
}
