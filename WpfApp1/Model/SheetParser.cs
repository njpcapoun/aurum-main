using ClassroomAssignment.Model.Repo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassroomAssignment.Model
{
    /// <summary>
    /// The file sheet parser. Reads each row of input csv files and parses their information
    /// </summary>
    public sealed class SheetParser
    {
        //Last row of header.
        /// <remark>
        /// In client spreadsheet, header is at row 3.
        /// So thats why it start from 3 to read the file.
        /// </remark>
        const int LAST_ROW_OF_HEADER = 3;
        static bool fileHasMoreRecords = true;
        private static IRoomRepository roomRepo;
     
        /// <summary>
        /// Get room repositoy and create course list, 
        /// and add these courses in courses list.
        /// </summary>
        /// <param name="filePaths">THe file paths of the csv files.</param>
        /// <param name="roomRepo">The collection of rooms.</param>
        /// <returns>The list of courses from a csv file</returns>
        public static List<Course> Parse(string filePath, IRoomRepository roomRepo)
        {
            var fileCourseList = new List<Course>();
            SheetParser.roomRepo = roomRepo;

            //var courses = new List<Course>();

            //foreach (string file in filePaths)
            //{
            fileHasMoreRecords = true;
                var coursesFromFile = parseFile(filePath);
                fileCourseList.AddRange(coursesFromFile);//Add courses in course list
            //}

            return fileCourseList;
        }
        /// <summary>
        /// parse the file, and get information
        /// <remark>skip the headers</remark>
        /// </summary>
        /// <param name="file">The name of the csv file.</param>
        /// <returns>courseForFile</returns>
        static List<Course> parseFile(string file)
        {
            var coursesForFile = new List<Course>();
            var fileName = new DirectoryInfo(file).Name;

            try
            {
                using (StreamReader fileStream = File.OpenText(file))
                {
                    // Use CsvHelper library to read the file.
                    var csvReader = new CsvHelper.CsvReader(fileStream);

                    // configure csv reader
                    csvReader.Configuration.HasHeaderRecord = false;
                    csvReader.Configuration.RegisterClassMap<CourseClassMap>();
                    csvReader.Configuration.MissingFieldFound = null;

                    skipHeaders(csvReader);
                    csvReader.Read(); // read first header and skip it.
                    while (fileHasMoreRecords)
                    {
                        coursesForFile.AddRange(parseRecordsForCourse(csvReader));
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("file " + fileName + " was not able to load\n" + "Exception: " + e);
            }

            //Console.WriteLine(coursesForFile);
            return coursesForFile;
        }

		/// <summary>
		/// Sets the parameterss of the courses and adds them to the courses list.
		/// </summary>
		/// <param name="reader">The CsvReader.</param>
		/// <returns>courseList</returns>
        private static List<Course> parseRecordsForCourse(CsvHelper.CsvReader reader)
        {
            // make sure not at header or end of file
            List<Course> courseList = new List<Course>();
			Regex rgx = new Regex(@"(\S*,.*?)\s*\(\d*\)");
			Match match;
			string NewInstructor;

			while ((fileHasMoreRecords = reader.Read()) && courseHasMoreRecords(reader))
            {
                Course course = reader.GetRecord<Course>();
                course.SetAllDerivedProperties();
				NewInstructor = "";
				match = rgx.Match(course.Instructor);
				while (match.Success)
				{
					NewInstructor += match.Groups[1].Value + "; ";
					match = match.NextMatch();
				}
				if (NewInstructor != "")
				{
					course.Instructor = NewInstructor.Substring(0, NewInstructor.Length - 2);
				}

				courseList.Add(course); //add course to course list.
            }

            return courseList;
        }

		/// <summary>
		/// Checks if there are more courses need to be read from the files.
		/// </summary>
		/// <param name="reader">The CsvReader</param>
		/// <returns>True if there are more courses left. False otherwise</returns>
        static bool courseHasMoreRecords(CsvHelper.CsvReader reader)
        {
            string courseHeader = reader.GetField(0);
            string firstFieldOfRecord = reader.GetField(1);
            bool hasRecordsLeft = string.IsNullOrEmpty(courseHeader) && !string.IsNullOrEmpty(firstFieldOfRecord);
            return hasRecordsLeft;
        }
        /// <summary>
        /// Skip the headers from input file
        /// </summary>
        /// <param name="reader">The CsvReader</param>
        static void skipHeaders(CsvHelper.CsvReader reader)
        { 
            ///<value> LastRowOfHeader is 3</value>
            for (int i = 0; i < LAST_ROW_OF_HEADER; i++)
            {
                reader.Read();
            }

        }
    }
}
