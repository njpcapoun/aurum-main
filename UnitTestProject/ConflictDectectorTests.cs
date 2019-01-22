using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.Notification;
using ClassroomAssignment.Operations;
using ClassroomAssignment.Repo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestProject.TestModels;

namespace UnitTestProject
{
    [TestClass]
    public class ConflictDetectorTests
    {
        private static ICourseRepository conflictingCourseRepo;
        private static ICourseRepository nonConflictingCourseRepo;
        
        [ClassInitialize]
        static public void Initialize(TestContext testContext)
        {
            conflictingCourseRepo = new ConflictingCourseRepo();
            nonConflictingCourseRepo = new NonConflictingCourseRepo();
        }

        [TestMethod]
        public void OnConflict_AllConflicts_ReturnsCorrectConflict()
        {
            AssignmentConflictDetector detector = new AssignmentConflictDetector(conflictingCourseRepo);
            List<Conflict> conflicts = detector.AllConflicts();

            Assert.IsTrue(conflicts.Count == 1);
            var conflictingCourses = conflicts.First().ConflictingCourses;

            Assert.AreEqual<int>(2, conflictingCourses.Count);
            Assert.IsTrue(conflictingCourses.Exists(x => x.ClassID == "240"));
            Assert.IsTrue(conflictingCourses.Exists(x => x.ClassID == "241"));
        }

        [TestMethod]
        public void OnNoConflict_DetectorReturnEmptyList()
        {
            AssignmentConflictDetector detector = new AssignmentConflictDetector(nonConflictingCourseRepo);
            List<Conflict> conflicts = detector.AllConflicts();

            Assert.IsTrue(conflicts.Count == 0);
        }





    }
}
