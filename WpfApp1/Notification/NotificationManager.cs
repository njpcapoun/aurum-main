using ClassroomAssignment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using ClassroomAssignment.Operations;
using ClassroomAssignment.Repo;

namespace ClassroomAssignment.Notification
{
    class NotificationManager
    {
        private static NotificationManager _instance;
        private static CourseRepository _courseRepository;
        public static AssignmentConflictDetector _courseConflictFinder { get; set; }

        public static event EventHandler<CourseConflictEventArgs> OnCourseConflict;

        
        private NotificationManager(CourseRepository courseRepository, AssignmentConflictDetector roomConflictFinder)
        {
            //_courseRepository = courseRepository;
            //_courseConflictFinder = roomConflictFinder;

            //_courseRepository.CourseModified += NotificationManager_CourseModified;
            //_courseRepository.CourseCollectionModified += _courseRepository_CourseCollectionModified;
            
        }

        private void _courseRepository_CourseCollectionModified(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<Conflict> conflicts = _courseConflictFinder.AllConflicts();

            OnCourseConflict?.Invoke(_instance, new CourseConflictEventArgs(conflicts));
        }

        private void NotificationManager_CourseModified(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            List<Conflict> conflicts = _courseConflictFinder.AllConflicts();

            OnCourseConflict?.Invoke(_instance, new CourseConflictEventArgs(conflicts));
        }

        /// <summary>
        /// returns instance of notifactionManager
        /// otherwise will throw an exception 
        /// </summary>
        public NotificationManager Instance
        {
            get
            {
                if (Instance == null) throw new Exception("Notification must be initialized before getting instance.");

                return _instance;
            }
        }

       
        
    }
}
