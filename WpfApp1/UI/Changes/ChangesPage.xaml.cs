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
        public ChangesPage()
        {
            InitializeComponent();
            List<Course> originalCourses = GetOriginalCourses();
            List<Course> upToDateCourses = GetUpToDateCourses();
            List<CourseDifference> courseDifferences = GetDifferences(originalCourses, upToDateCourses);
            DataContext = courseDifferences;
        }

        private List<Course> GetOriginalCourses()
        {
            return App.Current.Resources["originalCourses"] as List<Course>;
        }

        private List<Course> GetUpToDateCourses()
        {
            return CourseRepository.GetInstance().Courses.OrderBy(x => int.Parse(x.ClassID)).ToList();
        }

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

        // Method to set the differences using the info from getDifferences
        // Mainly to make future additions to the changes page easier
        private List<CourseDifference> setDifferences(Course i, Course j, string type, List<CourseDifference> diff, CourseDifference difference)
        {
            difference.DifferenceType = type;
            difference.OriginalCourse = i;
            difference.NewestCourse = j;
            diff.Add(difference);
            return diff;
        }

        private bool CoursesAreSame(Course a, Course b)
        {
            if (a.RoomAssignment == null || b.RoomAssignment == null) return a.RoomAssignment == b.RoomAssignment;
            else return a.RoomAssignment.Equals(b.RoomAssignment);
        }



    }
}

