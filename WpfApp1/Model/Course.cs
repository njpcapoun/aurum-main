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
    [Serializable]
    public class Course : INotifyPropertyChanged
    {
        public Course()
        {
            _crossListedCourses = new ObservableCollection<Course>();
            _crossListedCourses.CollectionChanged += _crossListedCourses_CollectionChanged;
        }

        private void _crossListedCourses_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CrossListedCourses)));
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

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

        public bool NeedsRoom { get; set; }
        public List<DayOfWeek> MeetingDays { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public CourseState State { get; set; }

        public void AddCrossListedCourse(Course course)
        {
            _crossListedCourses.Add(course);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CrossListedCourses)));
        }

        public void RemoveCrossListedCourse(Course course)
        {
            _crossListedCourses.Remove(course);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CrossListedCourses)));
        }

        private ObservableCollection<Course> _crossListedCourses;
        public List<Course> CrossListedCourses
        {
            get => _crossListedCourses.ToList();
            private set { }
        }

        public int ClassID_AsInt => int.Parse(ClassID);

        public void SetAllDerivedProperties()
        {
            var roomName = this.QueryRoomAssignment().FirstOrDefault();
            RoomAssignment = RoomRepository.GetInstance().GetRoomWithName(roomName);
            NeedsRoom = this.QueryNeedsRoom();
            MeetingDays = this.QueryMeetingDays();
            StartTime = this.QueryStartTime();
            EndTime = this.QueryEndTime();
        }

        public Course ShallowCopy()
        {
            return (Course)this.MemberwiseClone();
        }

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

        public static bool operator ==(Course course1, Course course2)
        {
            return EqualityComparer<Course>.Default.Equals(course1, course2);
        }

        public static bool operator !=(Course course1, Course course2)
        {
            return !(course1 == course2);



        }
    }
}
