using ClassroomAssignment.Model;
using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Operations
{
    /// <summary>
    /// The file conflict detector. Check if there are room conflicts based on the time and day.
    /// </summary>
    public class AssignmentConflictDetectorSchedule
    {
        //private ICourseRepository courseRepository;
        private ITeacherScheduleRepository CourseSchedule;
        
		/// <summary>
        /// Initilize courseRepo to courseRepository.
        /// </summary>
        /// <param name="courseRepo">The collection of courses</param>
        public AssignmentConflictDetectorSchedule(ITeacherScheduleRepository courseSchedule)
        {
            CourseSchedule = courseSchedule;
        }
        /// <summary>
        /// Return all time conflict courses. 
        /// Check class assignment time and see if any other courses use the same room at that time.
        /// </summary>
        /// <returns>The list of conflicts</returns>
        public List<Conflict> AllConflicts()
        {
            var courseGroupByRoom = from course in CourseSchedule.Courses
                                    where course.NeedsRoom && course.HasRoomAssignment
                                    group course by course.RoomAssignment;
            //Create conflicts list 
            List<Conflict> conflicts = new List<Conflict>();
            foreach (var roomGroup in courseGroupByRoom)
            {
                List<Course> courses = roomGroup.ToList();

                List<int> indicesUsed = new List<int>();
                for (int i = 0; i < courses.Count; i++)
                {
                    List<Course> conflictingCourses = new List<Course>();
                    for (int j = i+1; j < courses.Count && !indicesUsed.Contains(j); j++)
                    {
                        bool conflictExists = ConflictBetweenCourses(courses[i], courses[j]);
                        if (conflictExists)
                        {
                            conflictingCourses.Add(courses[j]);
                            indicesUsed.Add(j);
                        }
                    }
                    //if conflictingCourses more than 0. 
                    //Then add this course to conflicts list.
                    if (conflictingCourses.Count != 0)
                    {
                        conflictingCourses.Add(courses[i]);
                        conflicts.Add(new Conflict(conflictingCourses));
                    }
                }
            }

            return conflicts;
        }

		/// <summary>
		/// Check if the two courses in the parameters have conflicts.
		/// </summary>
		/// <param name="courseA">The first course object.</param>
		/// <param name="courseB">The second course object</param>
		/// <returns>True if conflict exists. False otherwise.</returns>
		private bool ConflictBetweenCourses(Course courseA, Course courseB)
        {
            bool candidate = courseA.MeetingDays?.Any(x => courseB.MeetingDays?.Contains(x) == true) == true;
            if (!candidate) return false;

            bool hasConflict = true;
            if (courseA.EndTime <= courseB.StartTime) hasConflict = false;
            else if (courseB.EndTime <= courseA.StartTime) hasConflict = false;

            return hasConflict;
        }


        /// <summary>
        /// Finds conflicts involving the <paramref name="courses"/> and the rest of the courses in the CourseRepo
        /// </summary>
        /// <param name="courses">The list of courses</param>
        /// <returns>List of all the conflicts</returns>
        public List<Conflict> ConflictsInvolvingCourses(List<Course> courses)
        {
            List<Conflict> allConflicts = AllConflicts();
            return allConflicts.FindAll(x => x.ConflictingCourses.Intersect(courses).FirstOrDefault() != null ? true : false);
        }


        /// <summary>
        /// Return conflicts solely among the <paramref name="courses"/>
        /// </summary>
        /// <param name="courses">The list of courses.</param>
        /// <returns>The new conflict list</returns>
        public List<Conflict> ConflictsAmongCourses(List<Course> courses)
        {
            return new List<Conflict>();
        }

    }
}
