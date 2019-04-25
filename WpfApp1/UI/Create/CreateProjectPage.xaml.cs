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
		/// <summary>
		/// Constructor for CreateProjectPage. Initualize the rooms.
		/// </summary>
		public CreateProjectPage()
		{
			InitializeComponent();
			RoomRepository.InitInstance();
		}

		/// <summary>
		/// Create a new project on click. Read all the courses from the csv files.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">>State information and event data associated with a routed event.</param>
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
			}
			catch (NullReferenceException nre)
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

			MemoryStream stream = new MemoryStream();
			IFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, courses);
			stream.Seek(0, SeekOrigin.Begin);

			var copy = formatter.Deserialize(stream) as List<Course>;
			App.Current.Resources["originalCourses"] = copy;

			CourseRepository.InitInstance(courses);

			NextPage(courses);
		}

		/// <summary>
		/// Print the list of files that have not been read due to errors.
		/// </summary>
		/// <param name="filenames">The list of file that caused the error</param>
		private void OnNewProjectCreationError(string filenames)
		{
			ProjectCreationErrorTextBlock.Text = "The following files could not be opened because they were improperly formatted.\n\t\t" + filenames;
		}

		/// <summary>
		/// Get the paths of the files read.
		/// </summary>
		/// <returns>The string array of paths of the csv files.</returns>
		private string[] GetSheetPaths()
		{
			// string to store the default path
			string defaultPath = "none";

			FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
			folderBrowser.RootFolder = Environment.SpecialFolder.Desktop;

			// If it exists read the default folder path
			if (Properties.Settings.Default["FilePath"] != null || (string)Properties.Settings.Default["FilePath"] != "default")
			{
				defaultPath = (string)Properties.Settings.Default["FilePath"];

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

		/// <summary>
		/// Open up an existing agn file on click.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">>State information and event data associated with a routed event.</param>
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

		/// <summary>
		/// Prints the error when a file read is not a csv file.
		/// </summary>
		private void OnExistingProjectError()
		{
			ProjectCreationErrorTextBlock.Text = "Selected file was not valid.";
		}

		/// <summary>
		/// Navigate to the ambiguity page if there are ambiguous assignments. The main page otherwise.
		/// </summary>
		/// <param name="courses">The list of courses.</param>
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

		/// <summary>
		/// Get the state of the app.
		/// </summary>
		/// <param name="filePath">The path of the existing agn/assignment file.</param>
		/// <returns>The app state.</returns>
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

		/// <summary>
		/// Get the file path of the agn/assignment file.
		/// </summary>
		/// <returns>The string of the agn/assignment file's directory. Null otherwise.</returns>
		private string GetFilePath()
		{
			// string to store the default path
			string defaultPath = "none";

			OpenFileDialog dialog = new OpenFileDialog();

			// If it exists read the default folder path
			if (Properties.Settings.Default["FilePath"] != null || (string)Properties.Settings.Default["FilePath"] != "default")
			{
				defaultPath = (string)Properties.Settings.Default["FilePath"];

				// Sets the initial directory
				dialog.InitialDirectory = defaultPath;
			}


			dialog.Filter = "Assignment File | *.agn";
			var result = dialog.ShowDialog();

			return result == DialogResult.OK ? dialog.FileName : null;
		}
	}
}
