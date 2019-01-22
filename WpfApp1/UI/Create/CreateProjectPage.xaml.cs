using ClassroomAssignment.Model;
using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.Repo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClassroomAssignment.UI.Create
{
    /// <summary>
    /// Interaction logic for CreateProjectPage.xaml
    /// </summary>
    public partial class CreateProjectPage : Page
    {
        public CreateProjectPage()
        {
            InitializeComponent();
            RoomRepository.InitInstance();
        }


        private void NewProjectButton_Click(object sender, RoutedEventArgs e)
        {

            string[] docLocations = GetSheetPaths();
            if (docLocations == null) return;

            List<Course> courses = null;
            try
            {
                courses = SheetParser.Parse(docLocations, RoomRepository.GetInstance());
            }
            catch { }

            if (courses == null || courses.Count == 0)
            {
                OnNewProjectCreationError();
                return;
            }

            //InitCrossListedCourses(courses);

            MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, courses);
            stream.Seek(0, SeekOrigin.Begin);

            var copy = formatter.Deserialize(stream) as List<Course>;
            App.Current.Resources["originalCourses"] = copy;

            CourseRepository.InitInstance(courses);


            NextPage(courses);

        }

        private void OnNewProjectCreationError()
        {
            ProjectCreationErrorTextBlock.Text = "Unable able to use selected folder to create new project.";
        }

        private string[] GetSheetPaths()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            var result = folderBrowser.ShowDialog();

            string[] docLocations = null;
            if (result == DialogResult.OK)
            {
                var pathToDocs = folderBrowser.SelectedPath;
                docLocations = Directory.GetFiles(pathToDocs);
            }

            return docLocations;
        }

        private void ExistingProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var filePath = GetFilePath();
            if (filePath == null) return;
            var appState = GetAppState(filePath);

            if (appState.CurrentCourses == null || appState.OriginalCourses == null)
            {
                OnExistingProjectError();
            }
            else
            {
                App.Current.Resources["originalCourses"] = appState.OriginalCourses;
                CourseRepository.InitInstance(appState.CurrentCourses);
                NextPage(appState.CurrentCourses);
            }
           
        }

        private void OnExistingProjectError()
        {
            ProjectCreationErrorTextBlock.Text = "Selected file was not valid.";
        }

        private void NextPage(List<Course> courses)
        {
            if (courses.FindAll(m => m.HasAmbiguousAssignment).Count > 0)
            {
                NavigationService.Navigate(new Uri(@"UI/Ambiguity/AmbiguityResolverPage.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri(@"UI/Main/MainPage.xaml", UriKind.Relative));
            }
        }

        private AppState GetAppState(string filePath)
        {
            AppState appState = null;
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    IFormatter formatter = new BinaryFormatter();
                    appState = formatter.Deserialize(stream) as AppState;
                }
            }
            catch (Exception e)
            {

            }

            return appState;
        }

        private string GetFilePath()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Assignment File | *.agn";
            var result = dialog.ShowDialog();

            return result == DialogResult.OK ? dialog.FileName : null;
        }

        private void InitCrossListedCourses(List<Course> courses)
        {
            HashSet<Course> MainCourses = new HashSet<Course>();

            bool isMainCourse = true;
            foreach (var course in courses)
            {
                if (!string.IsNullOrEmpty(course.CrossListings))
                {
                    List<Course> crossListedCourses = new List<Course>();

                    var regex = new Regex(@"\s([A-Z]+)\s(\d+)-(\d+)");
                    var matches = regex.Matches(course.CrossListings);

                    for (int i = 0; i < matches.Count; i++)
                    {
                        var subjectCode = matches[i].Groups[1].Value;
                        var catalogNumber = matches[i].Groups[2].Value.TrimStart(new char[] { '0' });
                        var sectionNumber = matches[i].Groups[3].Value.TrimStart(new char[] { '0' });
                        var c = courses.Find(x => x.SubjectCode == subjectCode && x.CatalogNumber == catalogNumber && x.SectionNumber == sectionNumber);

                        if (c != null)
                        {
                            crossListedCourses.Add(c);
                            if (MainCourses.Contains(c)) isMainCourse = false;
                        }
                    }

                    if (isMainCourse) MainCourses.Add(course);
                    else course.NeedsRoom = false;
                    
                    foreach (var crossListed in crossListedCourses)
                    {
                        course.AddCrossListedCourse(crossListed);
                    }
                }


            }
        }


    }
}

