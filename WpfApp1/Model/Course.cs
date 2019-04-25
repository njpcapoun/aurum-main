using ClassroomAssignment.Model.Repo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ClassroomAssignment.Model.CourseQueryRules;

namespace ClassroomAssignment.Model
{
	/// <summary>
	/// A course represented as an object model.
	/// </summary>
    [Serializable]
    public class Course : INotifyPropertyChanged
    {
		/// <summary>
		/// Constructor for Course. Sets its crosslistings.
		/// </summary>
        public Course()
        {
            _crossListedCourses = new ObservableCollection<Course>();
            _crossListedCourses.CollectionChanged += _crossListedCourses_CollectionChanged;
        }

		/// <summary>
		/// Handle changes of a course's crosslistings.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a CollectionChanged event.</param>
		private void _crossListedCourses_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CrossListedCourses)));
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// The different states for a course (conflicting, ambigiuous assignment, unassigned, assigned, no assignment required).
		/// </summary>
        public enum CourseState
        {
            [Description("Conflicting")]
            Conflicting,
            [Description("Ambigious Assignment Courses")]
            Ambiguous,
            [Description("Unassigned Courses")]
            Unassigned,
            [Description("Assigned Courses")]
            Assigned,
            [Description("No Assignment Required")]
            NoRoomRequired
        };

        #region Query Properties

        public bool HasRoomAssignment => RoomAssignment != null;

        private bool? _hasAmbiguousAssignment;
        public bool HasAmbiguousAssignment
        {
            get
            {
                if (_hasAmbiguousAssignment.HasValue) return _hasAmbiguousAssignment.Value;
                else return RoomAssignment != null && this.QueryHasAmbiguousAssignment();
            }
            set
            {
                _hasAmbiguousAssignment = value;
            }
        }

        #endregion
       // public string pad { get; set; }
        public string ClassID { get; set; }

        public string SIS_ID { get; set; }

        public string Term { get; set; }

        public string TermCode { get; set; }

        public string DepartmentCode { get; set; }

        public string SubjectCode { get; set; }

        public string CatalogNumber { get; set; }


        /// <summary>
        /// Property maps to the "Course" column of the deparment spreadsheet.
        /// </summary>
        public string CourseName { get; set; }

        public string SectionNumber { get; set; }

        public string CourseTitle { get; set; }


        public string SectionType { get; set; }



        /// <summary>
        /// Property maps to the "Title/Topic" column of the department spreadsheet.
        /// </summary>
        public string Topic { get; set; }


        public string MeetingPattern { get; set; }


        public string Instructor { get; set; }


        public string Room { get; set; }


        public string Status { get; set; }


        public string Session { get; set; }


        public string Campus { get; set; }


        public string InstructionMethod { get; set; }


        public string IntegerPartner { get; set; }


        public String Schedule { get; set; }


        public string Consent { get; set; }


        public string CreditHrsMin { get; set; }


        public string CreditHrs { get; set; }

        public String GradeMode { get; set; }


        public string Attributes { get; set; }


        public string RoomAttributes { get; set; }


        public string Enrollment { get; set; }


        public string MaximumEnrollment { get; set; }


        public string PriorEnrollment { get; set; }


        public string ProjectedEnrollment { get; set; }


        public string WaitCap { get; set; }


        public string RoomCapRequest { get; set; }


        public string CrossListings { get; set; }


        public string LinkTo { get; set; }

        public string Comments { get; set; }

        public string Notes { get; set; }

        private Room _roomAssignment;
        public string _currentTeacher;

		/// <summary>
		/// Getter and setter for course's room assignment.
		/// </summary>
        public Room RoomAssignment
        {
            get => _roomAssignment;
            set
            {
                _roomAssignment = value;
                
                foreach (var course in CrossListedCourses)
                {
                    course.RoomAssignment = _roomAssignment;
                }

            }
        }

		/// <summary>
		/// Getter and setter for course's instructor.
		/// </summary>
        public string CurrentTeacherInfo
        {
            get => Instructor;
            set
            {
                _currentTeacher = value;

                foreach (var course in CrossListedCourses)
                {
                    course.CurrentTeacherInfo = _currentTeacher;
                }

            }
        }

        public bool NeedsRoom { get; set; }
        public List<DayOfWeek> MeetingDays { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public CourseState State { get; set; }

		/// <summary>
		/// Adds a crosslisted course to the main course.
		/// </summary>
		/// <param name="course">A course object.</param>
        public void AddCrossListedCourse(Course course)
        {
            _crossListedCourses.Add(course);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CrossListedCourses)));
        }

		/// <summary>
		/// Removes a crosslisted course from its main course.
		/// </summary>
		/// <param name="course">A course object</param>
        public void RemoveCrossListedCourse(Course course)
        {
            _crossListedCourses.Remove(course);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CrossListedCourses)));
        }

        private ObservableCollection<Course> _crossListedCourses;

		/// <summary>
		/// Getter and setter for a course's crosslistings.
		/// </summary>
        public List<Course> CrossListedCourses
        {
            get => _crossListedCourses.ToList();
            private set { }
        }

        public int ClassID_AsInt => int.Parse(ClassID);

		/// <summary>
		/// Sets the properties associated with assignments and meetings.
		/// </summary>
        public void SetAllDerivedProperties()
        {
            var roomName = this.QueryRoomAssignment().FirstOrDefault();
            RoomAssignment = RoomRepository.GetInstance().GetRoomWithName(roomName);
            NeedsRoom = this.QueryNeedsRoom();
            MeetingDays = this.QueryMeetingDays();
            StartTime = this.QueryStartTime();
            EndTime = this.QueryEndTime();
        }

		/// <summary>
		/// Creates shallows copy of a course.
		/// </summary>
		/// <returns>(Course)this.MemberwiseClone()</returns>
		public Course ShallowCopy()
        {
            return (Course)this.MemberwiseClone();
        }

		/// <summary>
		/// String representing the course object.
		/// </summary>
		/// <returns>A string that represents the course object.</returns>
		public override string ToString()
        {
            var stringBuilder = new StringBuilder()
                    .Append(CourseName)
                    .AppendLine()
                    .AppendFormat("Sect. {0}", SectionNumber)
                    .AppendLine();
            var instructors = Instructor.Split(new char[] { ';' });

            foreach (var instructor in instructors)
            {
                stringBuilder.Append(instructor);
                stringBuilder.AppendLine();
            }

            stringBuilder.Append(MeetingPattern);

            return stringBuilder.ToString();
        }

		/// <summary>
		/// Equals method for a course.
		/// </summary>
		/// <param name="obj">A course object.</param>
		/// <returns>True of all parameters match for passed course; False otherwise</returns>
        public override bool Equals(object obj)
        {
            var course = obj as Course;
            return course != null &&
                   ClassID == course.ClassID &&
                   SIS_ID == course.SIS_ID &&
                   Term == course.Term &&
                   TermCode == course.TermCode &&
                   DepartmentCode == course.DepartmentCode &&
                   SubjectCode == course.SubjectCode &&
                   CatalogNumber == course.CatalogNumber &&
                   CourseName == course.CourseName &&
                   SectionNumber == course.SectionNumber &&
                   CourseTitle == course.CourseTitle &&
                   SectionType == course.SectionType &&
                   Topic == course.Topic &&
                   MeetingPattern == course.MeetingPattern &&
                   Instructor == course.Instructor &&
                   Room == course.Room &&
                   Status == course.Status &&
                   Session == course.Session &&
                   Campus == course.Campus &&
                   InstructionMethod == course.InstructionMethod &&
                   IntegerPartner == course.IntegerPartner &&
                   Schedule == course.Schedule &&
                   Consent == course.Consent &&
                   CreditHrsMin == course.CreditHrsMin &&
                   CreditHrs == course.CreditHrs &&
                   GradeMode == course.GradeMode &&
                   Attributes == course.Attributes &&
                   RoomAttributes == course.RoomAttributes &&
                   Enrollment == course.Enrollment &&
                   MaximumEnrollment == course.MaximumEnrollment &&
                   PriorEnrollment == course.PriorEnrollment &&
                   ProjectedEnrollment == course.ProjectedEnrollment &&
                   WaitCap == course.WaitCap &&
                   RoomCapRequest == course.RoomCapRequest &&
                   CrossListings == course.CrossListings &&
                   LinkTo == course.LinkTo &&
                   Comments == course.Comments &&
                   Notes == course.Notes &&
                   EqualityComparer<Room>.Default.Equals(RoomAssignment, course.RoomAssignment);
        }

		/// <summary>
		/// Checks of the two courses are equal to each other.
		/// </summary>
		/// <param name="course1">The first course object.</param>
		/// <param name="course2">The second course object.</param>
		/// <returns>True of the two passed courses are equal. False otherwise</returns>
        public static bool operator ==(Course course1, Course course2)
        {
            return EqualityComparer<Course>.Default.Equals(course1, course2);
        }

		/// <summary>
		/// Checks of the two courses passed are not equal to each other.
		/// </summary>
		/// <param name="course1">The first course object.</param>
		/// <param name="course2">The second course object.</param>
		/// <returns>True of the two passed courses are not equal. False otherwise</returns>
		public static bool operator !=(Course course1, Course course2)
        {
            return !(course1 == course2);
        }
    }
}
