﻿using ClassroomAssignment.Model;
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
            List<string> badFiles = new List<string>();
            if (docLocations == null) return;
            var courses = new List<Course>();
            var tempList = new List<Course>();
            courses = null;

            try
            {
                foreach (string file in docLocations)
                {
                    var fileName = new DirectoryInfo(file).Name;
                    tempList = SheetParser.Parse(file, RoomRepository.GetInstance());

                    if (tempList == null || tempList.Count == 0)
                    {
                        badFiles.Add(fileName);
                        Console.WriteLine(fileName);
                        tempList = null;
                    }
                    else if (tempList != null && courses == null)
                    {
                        courses = tempList;
                        tempList = null;
                    }
                    else if (tempList != null && courses != null)
                    {
                        courses.AddRange(tempList);
                        tempList = null;
                    }
                }
            } catch (NullReferenceException nre)
            {
                Console.Out.WriteLine("NRE: " + nre);
            }

            string[] output = null;

            if (badFiles.Count > 0)
            {
                output = badFiles.Select(i => i.ToString()).ToArray();
            }

            //try
            //{
            //    courses = SheetParser.Parse(docLocations, RoomRepository.GetInstance(), courses);
            //}
            //catch { }

            if (output != null || courses.Count == 0 || courses == null)
            {
                OnNewProjectCreationError(string.Join(", ", output));
                return;
            }

            InitCrossListedCourses(courses);

            MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, courses);
            stream.Seek(0, SeekOrigin.Begin);

            var copy = formatter.Deserialize(stream) as List<Course>;
            App.Current.Resources["originalCourses"] = copy;

            CourseRepository.InitInstance(courses);

            NextPage(courses);

        }

        private void OnNewProjectCreationError(string filenames)
        {
            ProjectCreationErrorTextBlock.Text = "The following files could not be opened because they were improperly formatted.\n\t\t" + filenames;
        }

        private string[] GetSheetPaths()
        {
            
            
       

            // string to store the default path
            string defaultPath = "none";

            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.RootFolder = Environment.SpecialFolder.Desktop;

            // If it exists read the default folder path
            if (Properties.Settings.Default["FilePath"] != null || (string) Properties.Settings.Default["FilePath"] != "default")
            {
                defaultPath = (string) Properties.Settings.Default["FilePath"];

                // Pretty sure this sets a default selected path
                folderBrowser.SelectedPath = defaultPath;
            }

            // Visual studio recommended using the actual type instead of var for this
            DialogResult result = folderBrowser.ShowDialog();

            // This gets the path they select
            defaultPath = folderBrowser.SelectedPath;


            // Saves the selected path into properties
            Properties.Settings.Default["FilePath"] = defaultPath;
            Properties.Settings.Default.Save();

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
            

            // string to store the default path
            string defaultPath = "none";

            OpenFileDialog dialog = new OpenFileDialog();

            // If it exists read the default folder path
            if (Properties.Settings.Default["FilePath"] != null || (string)Properties.Settings.Default["FilePath"] != "default")
            {
                defaultPath = (string) Properties.Settings.Default["FilePath"];

                // Sets the initial directory
                dialog.InitialDirectory = defaultPath;
            }

            
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

