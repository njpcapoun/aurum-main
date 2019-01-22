using ClassroomAssignment.Model;
using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.TestModels
{
    class NonConflictingCourseRepo : ICourseRepository 
    {
        private List<Course> _courses = new List<Course>();

        public IEnumerable<Course> Courses => _courses;

        private Room room;

        public NonConflictingCourseRepo()
        {
            room = new Room();
            room.RoomName = "PKI 157";
            room.Capacity = 30;

            AddCourse1();
            AddCourse2();
            AddCourse3();
        }

        public void AddCourse1()
        {
            Course testCourse = new Course();
            testCourse.ClassID = "1";
            testCourse.SIS_ID = "12615";
            testCourse.TermCode = "1188";
            testCourse.DepartmentCode = "UNO-BIOI";
            testCourse.SubjectCode = "BIOI";
            testCourse.CatalogNumber = "1000";
            testCourse.CourseTitle = "BIOI 1000";
            testCourse.SectionNumber = "1";
            testCourse.MeetingPattern = "MW 1:30pm-2:45pm";
            testCourse.Instructor = "Bastola, Dhundy";
            testCourse.Room = "General Assignment Room";
            testCourse.Status = "Active";
            testCourse.Session = "Regular Academic Session";
            testCourse.Campus = "UNO";
            testCourse.InstructionMethod = "In Person";
            testCourse.Comments = "";
            testCourse.Notes = "PKI 157";
            testCourse.StartTime = new TimeSpan(13, 30, 0);
            testCourse.EndTime = new TimeSpan(14, 45, 0);

            // This is the "normalized" name for the room
            testCourse.RoomAssignment = room;
            testCourse.MeetingDays = new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Wednesday };

            _courses.Add(testCourse);
        }

        public void AddCourse2()
        {
            Course testCourse = new Course();
            testCourse.ClassID = "2";
            testCourse.SIS_ID = "12134";
            testCourse.TermCode = "1188";
            testCourse.DepartmentCode = "UNO-BIOI";
            testCourse.SubjectCode = "BIOI";
            testCourse.CatalogNumber = "1001";
            testCourse.CourseTitle = "BIOI 1001";
            testCourse.SectionNumber = "1";
            testCourse.MeetingPattern = "MW 1:45pm-3:00pm";
            testCourse.Instructor = "Dandy, Maley";
            testCourse.Room = "General Assignment Room";
            testCourse.Status = "Active";
            testCourse.Session = "Regular Academic Session";
            testCourse.Campus = "UNO";
            testCourse.InstructionMethod = "In Person";
            testCourse.Comments = "";
            testCourse.Notes = "PKI 158";
            testCourse.SetDerivedProperties();

            _courses.Add(testCourse);
        }

        public void AddCourse3()
        {
            Course testCourse = new Course();
            testCourse.ClassID = "3";
            testCourse.SIS_ID = "12134";
            testCourse.TermCode = "1188";
            testCourse.DepartmentCode = "UNO-BIOI";
            testCourse.SubjectCode = "BIOI";
            testCourse.CatalogNumber = "1001";
            testCourse.CourseTitle = "BIOI 1001";
            testCourse.SectionNumber = "1";
            testCourse.MeetingPattern = "MW 5:00pm-6:30pm";
            testCourse.Instructor = "Dandy, Maley";
            testCourse.Room = "General Assignment Room";
            testCourse.Status = "Active";
            testCourse.Session = "Regular Academic Session";
            testCourse.Campus = "UNO";
            testCourse.InstructionMethod = "In Person";
            testCourse.Comments = "";
            testCourse.Notes = "PKI 158";
            testCourse.SetDerivedProperties();

            _courses.Add(testCourse);
        }
    }
}
