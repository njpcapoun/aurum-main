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
                        if (originalCourses[i] != newCourses[j])
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

        private bool CoursesAreSame(Course a, Course b)
        {
            if (a.RoomAssignment == null || b.RoomAssignment == null) return a.RoomAssignment == b.RoomAssignment;
            else return a.RoomAssignment.Equals(b.RoomAssignment);
        }



    }
}

