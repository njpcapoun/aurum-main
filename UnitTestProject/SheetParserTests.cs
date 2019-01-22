using ClassroomAssignment.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClassroomAssignment.Model.Repo;

namespace UnitTestProject
{

    [TestClass]
    public class SheetParserTests
    {
        const int NUMBER_OF_PARSED_COURSES = 21;
        static List<Course> courseList;
        static Course firstCourseRecord;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var debugLocation = Assembly.GetExecutingAssembly().Location;
            var debugFolder = Path.GetDirectoryName(debugLocation);
            var testFolder = Path.Combine(debugFolder, "testData");
            var files = Directory.GetFiles(testFolder);

            RoomRepository.InitInstance();
            courseList = SheetParser.Parse(files, RoomRepository.GetInstance());
            firstCourseRecord = courseList.First();
        }

        [TestMethod]
        public void TestNumberOfCoursesParsed()
        {
            int actualCount = courseList.Count;
            Assert.AreEqual<int>(NUMBER_OF_PARSED_COURSES, actualCount);
        }

        [TestMethod]
        public void TestReadCoursePropertiesCorrect()
        {
            Course course = courseList.First();
            Assert.AreEqual<string>("3511", course.ClassID);
            Assert.AreEqual<string>("BMI 8020", course.CourseName);
            Assert.AreEqual<string>("In Person", course.InstructionMethod);
            Assert.AreEqual<string>("Graded", course.GradeMode);
            Assert.AreEqual<string>("0", course.Enrollment);
            Assert.AreEqual<string>("30", course.MaximumEnrollment);
            Assert.AreEqual<string>("30", course.RoomCapRequest);
        }

        [TestMethod]
        public void TestMeetingDays_FirstRecord()
        {
            Assert.AreEqual<int>(1, firstCourseRecord.MeetingDays.Count);
            Assert.IsTrue(firstCourseRecord.MeetingDays.Contains(DayOfWeek.Monday));
        }

        [TestMethod]
        public void TestStartTime_FirstRecord()
        {
            TimeSpan startTime = firstCourseRecord.StartTime.Value;
            Assert.AreEqual<int>(17, startTime.Hours);
            Assert.AreEqual<int>(30, startTime.Minutes);
        }

        [TestMethod]
        public void TestEndTime_FirstRecord()
        {
            TimeSpan endTime = firstCourseRecord.EndTime.Value;
            Assert.AreEqual<int>(20, endTime.Hours);
            Assert.AreEqual<int>(10, endTime.Minutes);
        }

        [TestMethod]
        public void TestNumberOfCoursesThatNeedRoom()
        {
            var coursesThatNeedRoom = courseList.FindAll(x => x.NeedsRoom);
            int actual = coursesThatNeedRoom.Count;
            Assert.AreEqual<int>(11, actual);
        }

        [TestMethod]
        public void TestNumberOfUnassignedCourses()
        {
            int expectedNum = 4; // counts the course assigned to MAM 121 in ITIN && counts abiguous courses that need room
            var unassignedCourses = courseList.FindAll(m => m.NeedsRoom && !m.AlreadyAssignedRoom);
            int actualNum = unassignedCourses.Count();
            Assert.AreEqual<int>(expectedNum, actualNum);
        }

        [TestMethod]
        public void TestNumberOfAmbiguousCourses()
        {
            int expectedNum = 1;  // does not count the course assigned to MAM 121 in ITIN
            int actualNum = courseList.FindAll(m => m.AmbiguousState).Count;
            Assert.AreEqual<int>(expectedNum, actualNum);
        }

    }
}
