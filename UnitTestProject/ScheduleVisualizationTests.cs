using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ClassroomAssignment.Model;
using System.Collections.Generic;
using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.Visual;
using ClassroomAssignment.Repo;

namespace UnitTestProject
{
    [TestClass]
    public class ScheduleVisualizationTests
    {
        Mock<ISchedulePrinter> PrinterMock = new Mock<ISchedulePrinter>();
        Mock<IRoomRepository> RoomRepoMock = new Mock<IRoomRepository>();
        Mock<ICourseRepository> CourseRepoMock = new Mock<ICourseRepository>(); 
        string Term = "Fall 2018";
        List<Course> Courses;

        [TestInitialize]
        public void Initialize()
        {
            Course testCourse = new Course();
            testCourse.ClassID = "239";
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
            testCourse.Notes = "PKI 157";

            // This is the "normalized" name for the room
            var room = new Room();
            room.RoomName = "PKI 157";
            room.Capacity = 30;

            testCourse.RoomAssignment = room;
            testCourse.MeetingDays = new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Wednesday };

            Courses = new List<Course>();
            Courses.Add(testCourse);
            CourseRepoMock.Setup(x => x.Courses).Returns(Courses);
        }

        [TestMethod]
        public void PrintCalled_Test()
        {
            
            ScheduleVisualization scheduleVisualization = new ScheduleVisualization(CourseRepoMock.Object, RoomRepoMock.Object, PrinterMock.Object);
            scheduleVisualization.PrintSchedule();
            
            PrinterMock.Verify(x => x.Print(CourseRepoMock.Object, RoomRepoMock.Object));
        }
    }
}
