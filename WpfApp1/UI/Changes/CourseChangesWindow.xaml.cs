﻿using ClassroomAssignment.Model;
using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.Repo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClassroomAssignment.UI.Changes
{
    /// <summary>
    /// Interaction logic for CourseChangesWindow.xaml
    /// </summary>
    public partial class CourseChangesWindow : Window
    {
		/// <summary>
		/// Constructor for CourseChangesWindow. Initialize the original and updated courses and course differences.
		/// </summary>
        public CourseChangesWindow()
        {
            InitializeComponent();
            List<Course> originalCourses = GetOriginalCourses();
            List<Course> upToDateCourses = GetUpToDateCourses();
            List<CourseDifference> courseDifferences = GetDifferences(originalCourses, upToDateCourses);
            DataContext = courseDifferences;
        }

		/// <summary>
		/// Get the original courses before the changes.
		/// </summary>
		/// <returns>The list of original courses.</returns>
		private List<Course> GetOriginalCourses()
        {
            string fileName = "original.bin";
            IFormatter formatter = new BinaryFormatter();
            Stream stream = File.OpenRead(fileName);

            List<Course> originalCourses = formatter.Deserialize(stream) as List<Course>;
            originalCourses = originalCourses.OrderBy(x => int.Parse(x.ClassID)).ToList();
            stream.Close();

            return originalCourses;
        }

		/// <summary>
		/// Get the updated courses after changes were made to them during the session.
		/// </summary>
		/// <returns>THe list of updated courses.</returns>
        private List<Course> GetUpToDateCourses()
        {
            return CourseRepository.GetInstance().Courses.OrderBy(x => int.Parse(x.ClassID)).ToList();
        }

		/// <summary>
		/// Get the differences between the original and updated courses.
		/// </summary>
		/// <param name="originalCourses">The original courses before changes were made to them.</param>
		/// <param name="newCourses">The updated courses which have had changes made during the session.</param>
		/// <returns>The differences between the original and new courses.</returns>
		private List<CourseDifference> GetDifferences(List<Course> originalCourses, List<Course> newCourses)
        {
            List<CourseDifference> differences = new List<CourseDifference>();
            for (int i = 0; i < originalCourses.Count; i++)
            {
                for (int j = 0; j < newCourses.Count; j++)
                {
                    if (originalCourses[i].ClassID_AsInt == newCourses[j].ClassID_AsInt)
                    {
                        if (!CoursesAreSame(originalCourses[i], newCourses[j]))
                        {
                            var difference = new CourseDifference();

                            difference.DifferenceType = "Modified";
                            difference.OriginalCourse = originalCourses[i];
                            difference.NewestCourse = newCourses[j];
                            differences.Add(difference);
                        }
                    }

                   // TODO: Finish Implementing differences
                }
            }

            return differences;
        }

		/// <summary>
		/// Checks of the two courses have the same room assignments.
		/// </summary>
		/// <param name="a">The first course to be compared.</param>
		/// <param name="b">The second course to be compared.</param>
		/// <returns>True if the two courses have the same room assignemtns. False otherwise.</returns>
		private bool CoursesAreSame(Course a, Course b)
        {
            if (a.RoomAssignment == null || b.RoomAssignment == null) return a.RoomAssignment == b.RoomAssignment;
            else return a.RoomAssignment.Equals(b.RoomAssignment);
        }

       
        
    }
}
