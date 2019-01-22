using ClassroomAssignment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.Util
{
    class TestUtil
    {
        static Course CreateTestCourseWithoutDerivedProperties()
        {
            Course testCourse = new Course();
            testCourse.ClassID = "239";
            testCourse.SIS_ID = "12615";
            testCourse.TermCode = "1188";
            testCourse.DepartmentCode = "UNO-BIOI";
            testCourse.SubjectCode = "BIOI";
            testCourse.CatalogNumber = "1000";
            testCourse.CourseName = "BIOI 1000";
            testCourse.SectionNumber = "1";
            testCourse.CourseTitle = "INTRODUCTION TO BIOINFORMATICS";
            testCourse.SectionType = DataConstants.SectionTypeOptions.LECTURE;
            testCourse.Topic = string.Empty;
            testCourse.MeetingPattern = "MW 1:30pm-2:45pm";
            testCourse.Instructor = "Bastola, Dhundy";
            testCourse.Room = DataConstants.RoomOptions.GENERAL_ASSIGNMENT_ROOM;
            testCourse.Status = DataConstants.StatusOptions.ACTIVE;
            testCourse.Session = "Regular Academic Session";
            testCourse.Campus = DataConstants.CampusOptions.UNO;
            testCourse.InstructionMethod = DataConstants.InstructionMethods.IN_PERSON;
            testCourse.IntegerPartner = string.Empty;
            testCourse.Comments = "PKI 153";
            testCourse.Notes = "PKI 157";

            return testCourse;
        }

        public bool VerifyDerivedPropertiesCorrect(Course testCourse)
        {
            return true;
        }
    }
}
