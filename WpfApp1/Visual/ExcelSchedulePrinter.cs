using ClassroomAssignment.Model.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using static ClassroomAssignment.Model.ClassScheduleTemplate;
using System.Collections.Specialized;
using System.Collections;
using NPOI.HSSF.UserModel;
using System.IO;
using ClassroomAssignment.Extension;
using ClassroomAssignment.Model;
using ClassroomAssignment.Repo;

namespace ClassroomAssignment.Visual
{
    /// <summary>
    /// Printer and setup for the room or teacher schedules in an Excel workbook.
    /// </summary>
    class ExcelSchedulePrinter : ISchedulePrinter
    {
        static Tuple<int, int> RoomNameLocation = Tuple.Create<int, int>(2, 0);
        static Tuple<int, int> RoomCapacityLocation = Tuple.Create<int, int>(2, 1);
        static Tuple<int,int> TimeHeaderLocation = Tuple.Create<int, int>(4, 1);
        static int CellSpanPerTimeInterval = 2;
        static int startTimeLocationRow = 6;

        static TimeSpan StartTime = new TimeSpan(7, 30, 0);
        static TimeSpan EndTime = new TimeSpan(22, 0, 0);
        static TimeSpan TimeInterval = new TimeSpan(0, 30, 0);
        
        static Dictionary<TimeSpan, int> TimeMap = new Dictionary<TimeSpan, int>();
        static Dictionary<DayOfWeek, int> DayMap = new Dictionary<DayOfWeek, int>();

        private string _outputFile;

        private IWorkbook _workbook;

        private ISheet _scheduleTemplate;

        private ClassScheduleTemplate Template = new ClassScheduleTemplate();

		/// <summary>
		/// Constructor for ExcelSchedulePrinter. Initialize the parameters.
		/// </summary>
		/// <param name="outputFile">The name and location of the output file.</param>
		/// <param name="workbook">A Microsoft Excel workbook.</param>
        public ExcelSchedulePrinter(string outputFile, IWorkbook workbook)
        {
            _outputFile = outputFile;
            _workbook = workbook;
            _scheduleTemplate = _workbook.GetSheet(ClassScheduleTemplate.SCHEDULE_TEMPLATE_NAME);
        }

		/// <summary>
		/// Constructor for ExcelSchedulePrinter. Initialize the times and days of the week for the schedules. 
		/// </summary>
        static ExcelSchedulePrinter()
        {
            // initialize TimeMap, maps times to row location
            TimeMap.Add(StartTime, startTimeLocationRow);
            var currTime = StartTime;
            var currRow = startTimeLocationRow - 1;
            while(currTime.CompareTo(EndTime) < 0)
            {
                currTime = currTime.Add(TimeInterval);
                currRow += CellSpanPerTimeInterval;
                TimeMap.Add(currTime, currRow);
            }

            // initialize DayMap: Maps days to column locations
            int i = 2;
            DayMap.Add(DayOfWeek.Monday, i++);
            DayMap.Add(DayOfWeek.Tuesday, i++);
            DayMap.Add(DayOfWeek.Wednesday, i++);
            DayMap.Add(DayOfWeek.Thursday, i++);
            DayMap.Add(DayOfWeek.Friday, i++);
            DayMap.Add(DayOfWeek.Saturday, i++);

        }
        
        /// <summary>
        /// Writes course schedules for each room in the excel file.
        /// </summary>
        /// <param name="courseRepo">The collection of courses.</param>
        /// <param name="roomRepo">The collection of rooms.</param>
        public void Print(ICourseRepository courseRepo, IRoomRepository roomRepo)
        {
            List<Course> courses = courseRepo.Courses.ToList();

            var coursesInRoom = from course in courses
                                where course.State == Course.CourseState.Assigned && course.MeetingDays != null
                                group course by course.RoomAssignment;


            foreach (var courseGroup in coursesInRoom)
            {
                Room room = courseGroup.Key;
                ISheet sheet = _workbook.CloneSheet(_workbook.GetSheetIndex(_scheduleTemplate));
                var sheetIndex = _workbook.GetSheetIndex(sheet);
                _workbook.SetSheetName(sheetIndex, room.RoomName + " " +room.RoomType);
                _workbook.SetSheetHidden(sheetIndex, SheetState.Visible);

                ICell cell = sheet.GetRow(RoomNameLocation.Item1).GetCell(RoomNameLocation.Item2);
                cell.SetCellValue(room.RoomName);

                cell = sheet.GetRow(RoomCapacityLocation.Item1).GetCell(RoomCapacityLocation.Item2);
                cell.SetCellValue(string.Format("Cap: {0}", room.Capacity));

                PrintCourses(sheet, courseGroup.ToList());
                printLegend(sheet);
            }

            _workbook.SortWorksheets();
            _workbook.SetActiveSheet(0);

            _workbook.WriteToFile(_outputFile);
        }

		/// <summary>
		/// Prints legend of the depertments by side of the schedule for a room.
		/// </summary>
		/// <param name="sheet">A Microsoft Excel sheet.</param>
        private void printLegend(ISheet sheet)
        {
            CellReference cellReference = new CellReference("J5");
            int rowIndex = cellReference.Row;
            int cellIndex = cellReference.Col;

            Dictionary<string, short> subjectColorMap =  Template.GetSubjectColorMap();
            foreach(var entry in subjectColorMap)
            {
                IRow row = sheet.GetRow(rowIndex);
                ICell cell = row.GetCell(cellIndex);
                cell.CellStyle = Template.GetCellStyle(_workbook, (string) entry.Key);
                cell.SetCellValue((string) entry.Key);
                rowIndex++;
            }
        }

        private Dictionary<Room, List<Course>> getRoomNameToCoursesMap(IEnumerable<Course> courses) 
        {

            Dictionary<Room, List<Course>> roomCourseMap = new Dictionary<Room, List<Course>>();

            foreach (Course course in courses)
            {
                if (roomCourseMap.ContainsKey(course.RoomAssignment))
                {
                    roomCourseMap[course.RoomAssignment].Add(course);
                }
                else
                {
                    roomCourseMap.Add(course.RoomAssignment, new List<Course>() { course });
                }
            }

            return roomCourseMap;
        }

		/// <summary>
		/// Prints the courses in the schedules.
		/// </summary>
		/// <param name="sheet">A Microsoft Exceel sheet</param>
		/// <param name="courses">The list of courses.</param>
        private void PrintCourses(ISheet sheet, List<Course> courses)
        {
            int otherStartRow = 5;
            int otherEndRow = 9;
            int otherColumn = 11;
            foreach (Course course in courses)
            {
                foreach (DayOfWeek meetingDay in course.MeetingDays)
                {
                    if (meetingDay == DayOfWeek.Sunday) continue;
                    int column = DayMap[meetingDay];
                    int startRow = GetRowForTime(course.StartTime.Value);
                    int endRow = GetRowForTime(course.EndTime.Value);

                    
                    //Get cell
                    var row = sheet.GetRow(startRow);
                    var cell = row.GetCell(column);

                    // Style cell
                    cell.CellStyle = Template.GetCellStyle(_workbook, course.SubjectCode);

                    var cellValue = getCourseLabel(course);
                    cell.SetCellValue(cellValue);
                    sheet.AutoSizeColumn(column, true);

                    CellRangeAddress cellRange = new CellRangeAddress(startRow , endRow, column, column);
                    var regionIndex = sheet.AddMergedRegion(cellRange);
                }

                if (course.MeetingPattern.Equals("Does Not Meet"))
                {
                    //Get cell
                    var row = sheet.GetRow(otherStartRow);
                    var cell = row.GetCell(otherColumn);

                    // Style cell
                    cell.CellStyle = Template.GetCellStyle(_workbook, course.SubjectCode);

                    var cellValue = getCourseLabel(course);
                    cell.SetCellValue(cellValue);
                    sheet.AutoSizeColumn(otherColumn, true);

                    CellRangeAddress cellRange = new CellRangeAddress(otherStartRow, otherEndRow, otherColumn, otherColumn);
                    var regionIndex = sheet.AddMergedRegion(cellRange);
                    
                    otherStartRow += 6;
                    otherEndRow += 6;
                }
            }
        }

		/// <summary>
		/// Text format for a cell containing a course.
		/// </summary>
		/// <param name="course">A course object.</param>
		/// <returns>The text format of a course</returns>
        private string getCourseLabel(Course course)
        {
            return course.CourseName 
                + Environment.NewLine
                + string.Format("Sect. {0}", course.SectionNumber)
                + Environment.NewLine
                + course.Instructor 
                + Environment.NewLine 
                + course.MeetingPattern
                + Environment.NewLine
                + course.RoomAssignment;
        }

		/// <summary>
		/// Set the times for the schedule.
		/// </summary>
		/// <param name="time">A time interval</param>
		/// <returns>TimeMap[time] or  TimeMap[new TimeSpan(time.Hours, minutes, 0)]</returns>
		private int GetRowForTime(TimeSpan time)
        {
            int minutes = time.Minutes;
            if (minutes % 30 == 0)
            {
                return TimeMap[time];
            }
            else
            {
                minutes = (minutes / 30) * 30;
                return TimeMap[new TimeSpan(time.Hours, minutes, 0)];
            }
        }

		/// <summary>
		/// Write the schedule of an instructor to an Excel sheet.
		/// </summary>
		/// <param name="teacherCourseSchedule">Schedule of a teacher's courses.</param>
		/// <param name="roomRepo">The collection of rooms.</param>
		/// <param name="teacherName">The name of an instructor</param>
        public void PrintSchedule(ITeacherScheduleRepository teacherCourseSchedule, IRoomRepository roomRepo, string teacherName)
        {
            List<Course> courses = teacherCourseSchedule.Courses.ToList();

            var coursesInRoom = from course in courses
                                where (course.State == Course.CourseState.Assigned && course.MeetingDays != null) ||
                                      (course.State != Course.CourseState.Assigned || course.MeetingDays == null) ||
                                      (course.State != Course.CourseState.Assigned || course.MeetingDays != null)
                                group course by new { name = course.Instructor, roomNameAssignment = course.RoomAssignment };

            ISheet sheet = _workbook.CloneSheet(_workbook.GetSheetIndex(_scheduleTemplate));

            foreach (var courseGroup in coursesInRoom)
            {
                Room room = courseGroup.Key.roomNameAssignment;
                var sheetIndex = _workbook.GetSheetIndex(sheet);

                _workbook.SetSheetName(sheetIndex, teacherName);
                _workbook.SetSheetHidden(sheetIndex, SheetState.Visible);

                PrintCourses(sheet, courseGroup.ToList());
                printLegend(sheet);
            }

            _workbook.SortWorksheets();
            _workbook.SetActiveSheet(0);

            _workbook.WriteToFile(_outputFile);
        }
    }

    
}
