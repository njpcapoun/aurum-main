using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Visual
{
    public class ScheduleVisualization
    {
        private ICourseRepository CourseRepo;
        private IRoomRepository RoomRepo;
        private ISchedulePrinter Printer;
        private ITeacherScheduleRepository TeacherSchedule;
        private string TeacherName;

        public ScheduleVisualization(ICourseRepository courseRepo, IRoomRepository roomRepo, ISchedulePrinter printer, string teacherName, ITeacherScheduleRepository teacherSchedule)
        {
            CourseRepo = courseRepo;
            RoomRepo = roomRepo;
            Printer = printer;
            TeacherName = teacherName;
            TeacherSchedule = teacherSchedule;
        }

        public void PrintSchedule()
        {
            Printer.Print(CourseRepo, RoomRepo);
        }

        public void PrintTeacherSchedule()
        {
            Printer.PrintSchedule(TeacherSchedule, RoomRepo, TeacherName);
        }
    }
}
