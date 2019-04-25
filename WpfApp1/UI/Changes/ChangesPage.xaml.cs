using ClassroomAssignment.Model;
using ClassroomAssignment.Repo;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClassroomAssignment.UI.Changes
{
    /// <summary>
    /// Interaction logic for ChangesPage.xaml
    /// </summary>
    public partial class ChangesPage : Page
    {
		/// <summary>
		/// Constructor for ChangesPage. Initialalize the original and updated courses and their differences.
		/// </summary>
        public ChangesPage()
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
            return App.Current.Resources["originalCourses"] as List<Course>;
        }

		/// <summary>
		/// Get the updated courses which have had changes made to them during the session.
		/// </summary>
		/// <returns>The list of updated courses.</returns>
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
                        var difference = new CourseDifference();

                        // Checks if the needsRoom variable has been changed from true to false
                        bool v = (originalCourses[i].NeedsRoom == true && newCourses[j].NeedsRoom == false);
                        if (v)
                        {
                            setDifferences(originalCourses[i], newCourses[j], "No Assignment Needed", differences, difference);
                        }

                        // Otherwise check if another change has been made
                        if (originalCourses[i] != newCourses[j])
                        {
                            setDifferences(originalCourses[i], newCourses[j], "Modified", differences, difference);
                        }
                    }

                    // TODO: Finish Implementing differences
                }
            }

            return differences;
        }

		/// <summary>
		/// Method to set the differences using the info from getDifferences.
		/// Mainly to make future additions to the changes page easier
		/// </summary>
		/// <param name="i">The original course.</param>
		/// <param name="j">The update course</param>
		/// <param name="type">The type of difference between the two courses</param>
		/// <param name="diff">The list of differences</param>
		/// <param name="difference">The difference to be added to the list of differences</param>
		/// <returns>The list of differences</returns>
        private List<CourseDifference> setDifferences(Course i, Course j, string type, List<CourseDifference> diff, CourseDifference difference)
        {
            difference.DifferenceType = type;
            difference.OriginalCourse = i;
            difference.NewestCourse = j;
            diff.Add(difference);
            return diff;
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

