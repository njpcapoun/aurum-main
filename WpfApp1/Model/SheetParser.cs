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
    /// This file sheet parser, and get input file and parse that information
    /// <example>Its read excel file and read each row and parse.</example>
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
        /// <param name="filePaths"></param>
        /// <param name="roomRepo"></param>
        /// <returns> courses </returns>
        public static List<Course> Parse(string[] filePaths, IRoomRepository roomRepo)
        {
            SheetParser.roomRepo = roomRepo;

            var courses = new List<Course>();
            
            foreach (string file in filePaths)
            {
                fileHasMoreRecords = true;
                var coursesFromFile = parseFile(file);
                courses.AddRange(coursesFromFile);//Add courses in course list
            }

            return courses;
        }
        /// <summary>
        /// parse the file, and get information
        /// <remark>skip the headers</remark>
        /// </summary>
        /// <param name="file"></param>
        /// <returns>courseForFile</returns>
        static List<Course> parseFile(string file)
        {
            var coursesForFile = new List<Course>();

            using (StreamReader fileStream = File.OpenText(file))
            {
                // Use CsvHelper library to read the file.
                var csvReader = new CsvHelper.CsvReader(fileStream);
                
                // configure csv reader
                csvReader.Configuration.HasHeaderRecord = false;
                csvReader.Configuration.RegisterClassMap<CourseClassMap>();

                skipHeaders(csvReader);
                csvReader.Read(); // read first header and skip it.
                while(fileHasMoreRecords)
                {
                    coursesForFile.AddRange(parseRecordsForCourse(csvReader));
                }
            }

            return coursesForFile;
        }

        private static List<Course> parseRecordsForCourse(CsvHelper.CsvReader reader)
        {

            // make sure not at header or end of file
            List<Course> courseList = new List<Course>();

            while((fileHasMoreRecords = reader.Read()) && courseHasMoreRecords(reader))
            {
                Course course = reader.GetRecord<Course>();
                course.SetAllDerivedProperties();
                courseList.Add(course); //add course to course list.
            }

            return courseList;
        }

        


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
        /// <param name="reader"></param>
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
