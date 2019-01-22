using ClassroomAssignment.Notification;
using ClassroomAssignment.Operations;
using ClassroomAssignment.Repo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using ClassroomAssignment.Model;
using static ClassroomAssignment.Model.Course;

namespace ClassroomAssignment.Repo
{
    [Serializable]
    class CourseRepository : ICourseRepository
    {
        public IEnumerable<Course> Courses { get; }

        private static CourseRepository _instance;
        private AssignmentConflictDetector roomConflictDetector;
        

        public event EventHandler<ChangeInConflictsEventArgs> ChangeInConflicts;

        public static CourseRepository GetInstance()
        {
            return _instance;
        }

        /// <summary>
        /// Handle course list exception if it is empty, 
        /// throw NullException.
        /// </summary>
        /// <param name="courses"></param>
        public static void InitInstance(ICollection<Course> courses)
        {
            if (courses == null) throw new ArgumentNullException();

            _instance = new CourseRepository(courses);
            _instance.roomConflictDetector = new AssignmentConflictDetector(_instance);
        }

        private CourseRepository(IEnumerable<Course> courses)
        {
            Courses = courses;
            
            foreach (var course in courses)
            {
                course.PropertyChanged += Course_PropertyChanged;
            }

            HandleChangeInCourseStates();
        }

        

        private void Course_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State") return;
            HandleChangeInConflicts();
            HandleChangeInCourseStates();
        }

        private void HandleChangeInConflicts()
        {
            var conflicts = GetConflicts();
            var eventArgs = new ChangeInConflictsEventArgs();
            eventArgs.Conflicts = conflicts;

            ChangeInConflicts?.Invoke(this, eventArgs);
        }

        private void HandleChangeInCourseStates()
        {
            var conflicts = GetConflicts();
            var coursesWithConflicts = new HashSet<Course>();

            foreach (var conflict in conflicts)
            {
                coursesWithConflicts.UnionWith(conflict.ConflictingCourses);
            }
            
            foreach (var c in Courses)
            {
                if (coursesWithConflicts.Contains(c)) c.State = CourseState.Conflicting;
                else if (c.NeedsRoom && c.HasRoomAssignment) c.State = CourseState.Assigned;
                else if (c.NeedsRoom) c.State = CourseState.Unassigned;
                else c.State = CourseState.NoRoomRequired;
            }
            
        }
        /// <summary>
        /// Get Conflicts list.
        /// </summary>
        /// <returns>AllConflicts</returns>
        public List<Conflict> GetConflicts()
        {
            return new AssignmentConflictDetector(this).AllConflicts();
        }

        /// <summary>
        /// Get conflicts involving courses.
        /// </summary>
        /// <param name="courses"></param>
        /// <returns> ConflictInvolvingCourses(course)</returns>
        public List<Conflict> GetConflictsInvolvingCourses(List<Course> courses)
        {
            return new AssignmentConflictDetector(this).ConflictsInvolvingCourses(courses);
        }


        public class ChangeInConflictsEventArgs : EventArgs
        {
            public IEnumerable<Conflict> Conflicts { get; set; }
        }

    }
}
