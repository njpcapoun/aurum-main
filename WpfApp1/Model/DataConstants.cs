using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Model
{
    /// <summary>
    /// Initilize all information in spreadsheet
    /// </summary>
    public class DataConstants
    {
        /// <summary>
        /// Create constant string and initilize each variable for all headers
        /// </summary>
        public class Headers
        {
            public const string CLASS_ID = "CLSS ID";
            public const string SESSION_ID = "SIS ID";
            public const string TERM = "Term";
            public const string DEPARTMENT_CODE = "Department Code";
            public const string SUBJECT_CODE = "Subject Code";
            public const string CATALOG_NUMBER = "Catalog Number";
            public const string COURSE = "Course";
            public const string SECTION_NUM = "Section #";
            public const string COURSE_TITLE = "Course Title";
            public const string SECTION_TYPE = "Section Type";
            public const string TITLE_SLASH_TOPIC = "Title";

        }
        /// <summary>
        /// Initilizing each department string to constant string values.
        /// </summary>
        public class SubjectCode
        {
            public const string CSCI = "CSCI";
            public const string CSTE = "CSTE";
            public const string ISQA = "ISQA";
            public const string ITIN = "ITIN";
            public const string CIST = "CIST";
            public const string EMIT = "EMIT";
            public const string BMI = "BMI";
            public const string BIOI = "BIOI";
        }
        /// <summary>
        /// Initilize room options
        /// <example> This is for checking the class need a room, room location</example>
        /// </summary>
        public class RoomOptions
        {
            public const string GENERAL_ASSIGNMENT_ROOM = "General Assignment Room";
            public const string NO_ROOM_NEEDED = "No Room Needed";
            public const string ROOM_ASSIGNMENT_PENDING = "Room Assignment Pending";
            public const string PETER_KIEWIT_INSTITUTE_REGEX = @"Peter Kiewit Institute \d+";
            public const string PKI_REGEX = @"PKI (\d+)";
        }
        /// <summary>
        /// initilize type of Class
        /// <example> Class teaching type is it lecture,
        /// independent study or studio</example>
        /// </summary>
        public class SectionTypeOptions
        {
            public const string LECTURE = "Lecture";
            public const string INDEPENDENT_STUDY = "Independent Study";
            public const string STUDIO = "Studio";
        }
        /// <summary>
        /// use regex to check time and days, or does not meet which means does not need a room.
        /// </summary>
        public class MeetingPatternOptions
        {
            public const string TIME_PATTERN = @"(S|M|T|W|Th|F|Sa)+ (\d{1,2}(?::\d{1,2})?(?:am|pm|AM|PM))-(\d{1,2}(?::\d{1,2})?(?:am|pm|AM|PM))";
            public const string DOES_NOT_MEET = "Does Not Meet";
        }
        /// <summary>
        /// Initilize off campus and in person.
        /// </summary>
        public class InstructionMethods
        {
            public const string OFF_CAMPUS = "Off Campus";
            public const string IN_PERSON = "In Person";
        }
        /// <summary>
        /// Initilize active to constant value.
        /// </summary>
        public class StatusOptions
        {
            public const string ACTIVE = "Active";
        }
        /// <summary>
        /// Initilize regular academic session to constant string value.
        /// </summary>
        public class SessionOptions
        {
            public const string REGULAR_ACADEMIC_SESSION = "Regular Academic Session";
        }
        /// <summary>
        /// Campus Options, initilize UNO to constant string value.
        /// </summary>
        public class CampusOptions
        {
            public const string UNO = "UNO";
        }
    }
}
