 using ClassroomAssignment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;


namespace ClassroomAssignment.Operations
{
    /// <summary>
    /// This file shows all conflict courses, and print them on main view window.
    /// </summary>
    [Serializable]
    public class Conflict
    {
        public List<Course> ConflictingCourses { get; }

		/// <summary>
		/// Constructor for Conflict. Set the list of conflicting courses.
		/// </summary>
		/// <param name="conflictingCourses"></param>
        public Conflict(List<Course> conflictingCourses)
        {
            ConflictingCourses = conflictingCourses;
        }
        /// <summary>
        /// Print description of the conflict courses
        /// </summary>
        /// <return>A string of the description of the conflicting courses.</return>
        public string Description
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach (var course in ConflictingCourses)
                {
                    builder.Append(course.CourseName);
                    builder.Append(", ");
                }

                builder.Remove(builder.Length - 2, 2);
                builder.Append(" are in conflict in ");
                builder.Append(ConflictingCourses.First().RoomAssignment);
                return builder.ToString();
            }
        }
    }
}
