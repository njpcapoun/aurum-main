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
	/// Prints the room or teacher schedules to an Excel workbook.
	/// </summary>
    public class ScheduleVisualization
    {
        private ICourseRepository CourseRepo;
        private IRoomRepository RoomRepo;
        private ISchedulePrinter Printer;
        private ITeacherScheduleRepository TeacherSchedule;
        private string TeacherName;

		/// <summary>
		/// Constructor for ScheduleVisualization. Sets passed parameters.
		/// </summary>
		/// <param name="courseRepo">The collection of courses.</param>
		/// <param name="roomRepo">The collection of rooms.</param>
		/// <param name="printer">Printer for the Excel schedules.</param>
		/// <param name="teacherName">Name of a teacher.</param>
		/// <param name="teacherSchedule">Collection of teachers' schedules.</param>
        public ScheduleVisualization(ICourseRepository courseRepo, IRoomRepository roomRepo, ISchedulePrinter printer, string teacherName, ITeacherScheduleRepository teacherSchedule)
        {
            CourseRepo = courseRepo;
            RoomRepo = roomRepo;
            Printer = printer;
            TeacherName = teacherName;
            TeacherSchedule = teacherSchedule;
        }

		/// <summary>
		/// Prints the schedules of the rooms with their courses.
		/// </summary>
        public void PrintSchedule()
        {
            Printer.Print(CourseRepo, RoomRepo);
        }

		/// <summary>
		/// Prints the teacher's schedule.
		/// </summary>
        public void PrintTeacherSchedule()
        {
            Printer.PrintSchedule(TeacherSchedule, RoomRepo, TeacherName);
        }
    }
}
