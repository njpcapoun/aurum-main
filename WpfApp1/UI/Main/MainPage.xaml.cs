using ClassroomAssignment.Model;
using ClassroomAssignment.Operations;
using ClassroomAssignment.Repo;
using ClassroomAssignment.UI.Assignment;
using ClassroomAssignment.UI.Changes;
using ClassroomAssignment.UI.Reassignment;
using ClassroomAssignment.UI.Edit;
using ClassroomAssignment.ViewModel;
using ClassroomAssignment.Visual;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ClassroomAssignment.Model.Repo;
using System.ComponentModel;

namespace ClassroomAssignment.UI.Main
{
	/// <summary>
	/// Interaction logic for MainPage.xaml
	/// </summary>
	public partial class MainPage : Page
	{
		public MainWindowViewModel ViewModel { get; set; }

		public Room SelectedRoom { get; set; }

		public int index { get; set; }

		private RoomRepository roomRepo;

		SaveBase saveWork = new SaveBase();

		Regex regex;

		int count = 0;

		/// <summary>
		/// Load and initialize the Main Page.
		/// </summary>
		public MainPage()
		{
			InitializeComponent();
			System.Windows.Application.Current.MainWindow.WindowState = WindowState.Maximized;
			ViewModel = new MainWindowViewModel(this);
			roomRepo = ViewModel.RoomRepo;
			DataContext = ViewModel;
			this.Loaded += MainPage_Loaded;
		}

		/// <summary>
		/// Loads the main page when navigated to.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">>State information and event data associated with a routed event.</param>
		private void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			while (NavigationService.RemoveBackEntry() != null) ;
		}

		/// <summary>
		/// Save work as a different .agn file from the file menu item.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">>State information and event data.</param>
		private void SaveAs(object sender, EventArgs e)
		{
			saveWork.SaveAs();
		}

		/// <summary>
		/// Save all work to most recently opened or created assignment/.agn file from the file menu item.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">>State information and event data.</param>
		private void Menu_Save(object sender, EventArgs e)
		{
			saveWork.SaveWork();
		}

		/// <summary>
		/// Get and return the list of the original courses.
		/// </summary>
		/// <returns>App.Current.Resources["originalCourses"] as List<Course></returns>
		private List<Course> GetOriginalCourses()
		{
			return App.Current.Resources["originalCourses"] as List<Course>;
		}

		/// <summary>
		/// Automatically crosslists all the courses based on their crosslistings field from the file menu.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void AutomaticCrosslisting(object sender, EventArgs e)
		{
			HashSet<Course> MainCourses = new HashSet<Course>();
			bool isMainCourse = true;
			var courses = CoursesDataGrid.ItemsSource;

			foreach (Course course in courses)
			{
				isMainCourse = true;
				List<Course> crossListedCourses = new List<Course>();

				HashSet<string> subjectCodes = new HashSet<string>();
				HashSet<string> catalogNumbers = new HashSet<string>();
				HashSet<string> sectionNumbers = new HashSet<string>();

				if (course.CrossListings.ToUpper().Contains("ALSO"))
				{
					var regex = new Regex(@"([A-Z]+)\s*(\d+)[- ]+(\d+)");
					var matches = regex.Matches(course.CrossListings);

					for (int i = 0; i < matches.Count; i++)
					{
						var subjectCode = matches[i].Groups[1].Value;
						subjectCodes.Add(subjectCode);
						var catalogNumber = matches[i].Groups[2].Value;
						catalogNumbers.Add(catalogNumber);
						var catalogNumber2 = catalogNumber.TrimStart(new char[] { '0' });
						catalogNumbers.Add(catalogNumber2);
						var sectionNumber = matches[i].Groups[3].Value;
						sectionNumbers.Add(sectionNumber);
						var sectionNumber2 = sectionNumber.TrimStart(new char[] { '0' });
						sectionNumbers.Add(sectionNumber2);
					}

					foreach (Course y in courses)
					{
						if (course == y)
							continue;
						if (subjectCodes.Contains(y.SubjectCode) &&
							catalogNumbers.Contains(y.CatalogNumber) &&
							sectionNumbers.Contains(y.SectionNumber))
						{
							crossListedCourses.Add(y);
							if (MainCourses.Contains(y))
								isMainCourse = false;
						}
					}

					if (isMainCourse)
					{
						MainCourses.Add(course);
						foreach (Course crossListed in crossListedCourses)
						{
							crossListed.NeedsRoom = false;
							if (!course.CrossListedCourses.Contains(crossListed))
							{
								course.AddCrossListedCourse(crossListed);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Moves to the Changes page from the file menu.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data.</param>
		private void Menu_Changes(object sender, EventArgs e)
		{
			NavigationService.Navigate(new ChangesPage());
		}

		/*
           * Unimplemented, possibly meant to have a view for just crosslisted classes?
        */
		private void ConflictsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			//var conflict = ConflictsListView.SelectedItem as Conflict;
			//if (conflict != null)
			//{
			//    var assignmentPage = new AssignmentPage(conflict.ConflictingCourses);
			//    NavigationService.Navigate(assignmentPage);
			//}
		}

		/// <summary>
		/// Sends selected courses to AssignmentPage to be assigned from right click context menu
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void AssignMenuItem_Click(object sender, RoutedEventArgs e)
		{
			List<Course> courses = new List<Course>();

			IList selectedItems = CoursesDataGrid.SelectedItems;
			foreach (Course c in selectedItems)
			{
				courses.Add(c);
			}

			var assignmentPage = new AssignmentPage(courses);
			NavigationService.Navigate(assignmentPage);
		}

		/// <summary>
		/// Navigates to the reassignment screen for a selected course from the right click context menu
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void ReassignMenuItem_Click(object sender, RoutedEventArgs e)
		{

			Course c = CoursesDataGrid.SelectedItem as Course;
			ReassignmentPopUp popUp = new ReassignmentPopUp(c);
			popUp.ShowDialog();
			ReassignmentPage reassignmentPage = popUp.getRP();
			NavigationService.Navigate(reassignmentPage);
		}

		/// <summary>
		/// Export the files to CSV format from the file menu.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void Export_Click(object sender, RoutedEventArgs e)
		{
			if (ViewModel.Conflicts.Count != 0)
			{
				string message = "Exporting to Excel while there are conflicts may result in incorrect output. Do you wish to continue with the export?";
				string caption = "Export to Excel";
				MessageBoxImage icon = MessageBoxImage.Warning;
				MessageBoxButton button = MessageBoxButton.YesNo;
				MessageBoxResult result = System.Windows.MessageBox.Show(message, caption, button, icon);

				if (result == MessageBoxResult.No) return;
			}

			Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
			saveFileDialog.Filter = "Excel Worksheets|*.xls";
			if (saveFileDialog.ShowDialog() == true)
			{
				var fileName = saveFileDialog.FileName;
				var templateFile = System.IO.Path.Combine(Environment.CurrentDirectory, "ClassroomGridTemplate.xls");
				using (var fileStream = File.OpenRead(templateFile))
				{
					IWorkbook workbook = new HSSFWorkbook(fileStream);
					workbook.RemoveSheetAt(workbook.GetSheetIndex("Sheet1"));

					workbook.MissingCellPolicy = MissingCellPolicy.CREATE_NULL_AS_BLANK;
					ExcelSchedulePrinter printer = new ExcelSchedulePrinter(fileName, workbook);
					ICourseRepository courseRepository = CourseRepository.GetInstance();

					new ScheduleVisualization(courseRepository, null, printer).PrintSchedule();
				}
			}
		}

		/// <summary>
		/// Sets which options are available for a course for the right click context menu.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void CoursesContextMenu_Opened(object sender, RoutedEventArgs e)
		{
			var course = CoursesDataGrid.SelectedItem as Course;

			bool containNoRoomNeeded = false;
			foreach (Course c in CoursesDataGrid.SelectedItems)
			{
				if (!c.NeedsRoom) containNoRoomNeeded = true;
			}

			if (course == null) CoursesContextMenu.IsEnabled = false;
			else CoursesContextMenu.IsEnabled = true;

			if (containNoRoomNeeded) AssignMenuItem.IsEnabled = false;
			else AssignMenuItem.IsEnabled = true;

			if (containNoRoomNeeded) NoAssignmentNeededMenuItem.IsEnabled = false;
			else NoAssignmentNeededMenuItem.IsEnabled = true;

			if (CoursesDataGrid.SelectedItems.Count < 2) CrossListMenuItem.IsEnabled = false;
			else CrossListMenuItem.IsEnabled = true;

			if (CoursesDataGrid.SelectedItems.Count > 1) CoursesMenuItem.IsEnabled = false;
			else CoursesMenuItem.IsEnabled = true;

			int i;
			if (!course.NeedsRoom && course.QueryMeetingDays().Count != 0 && course.QueryStartTime() != null && course.QueryEndTime() != null && int.TryParse(course.RoomCapRequest, out i)) AssignmentNeeded.Visibility = Visibility.Visible;
			else AssignmentNeeded.Visibility = Visibility.Collapsed;

			if (course.HasRoomAssignment) Unassign.Visibility = Visibility.Visible;
			else Unassign.Visibility = Visibility.Collapsed;
		}

		/// <summary>
		/// Navigate to the edit course screen for a selected course from the right click context menu.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void CoursesMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var course = CoursesDataGrid.SelectedItem as Course;
			if (course == null) return;

			var editPage = new CourseEditPage(course);
			NavigationService.Navigate(editPage);
		}

		/// <summary>
		/// Navigate to a crosslisted course that is selected in its main course's crosslistings menu from the right click context menu.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void GoToCourseMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var contextMenu = (sender as System.Windows.Controls.MenuItem).Parent as System.Windows.Controls.ContextMenu;
			var course = (contextMenu.PlacementTarget as System.Windows.Controls.ComboBox).SelectedItem as Course;

			if (course != null)
			{
				CoursesDataGrid.SelectedItem = course;
				CoursesDataGrid.ScrollIntoView(course);
				CoursesDataGrid.Focus();
			}
		}

		/// <summary>
		/// Set Course as no assignment needed from right click context menu.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void NoAssignmentNeededMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var courses = CoursesDataGrid.SelectedItems;

			foreach (Course course in courses)
			{
				course.NeedsRoom = false;
			}
		}

		/// <summary>
		/// Crosslists selected courses from the right click context menu. Courses are crosslisted to the course which has been first selecte.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void CrossListMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var courses = CoursesDataGrid.SelectedItems;

			var mainCourse = courses[0] as Course;
			foreach (Course course in courses)
			{
				if (course.NeedsRoom)
				{
					mainCourse = course;
					break;
				}
			}

			foreach (Course course in courses)
			{
				if (course == mainCourse) continue;

				course.NeedsRoom = false;
				if (!mainCourse.CrossListedCourses.Contains(course))
					mainCourse.AddCrossListedCourse(course);
				//CrossListedToMain[course] = mainCourse;
			}
		}

		private void NewCourseMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new AddCourseDialogBox();
			dialog.Show();
		}

		/// <summary>
		/// Moves course(s) to assignment needed on click.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void AssignmentNeeded_Click(object sender, RoutedEventArgs e)
		{
			foreach (Course course in CoursesDataGrid.SelectedItems)
			{
				course.NeedsRoom = true;
			}
		}

		/// <summary>
		/// Removes a crosslisted course from a main course when selected in the drop down menu.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void RemoveCrossListedCourseMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var contextMenu = (sender as System.Windows.Controls.MenuItem).Parent as System.Windows.Controls.ContextMenu;
			var crossListedCourse = (contextMenu.PlacementTarget as System.Windows.Controls.ComboBox).SelectedItem as Course;
			var mainCourse = CoursesDataGrid.SelectedItem as Course;

			if (mainCourse == null) return;

			if (crossListedCourse != null)
			{
				mainCourse.RemoveCrossListedCourse(crossListedCourse);
				crossListedCourse.NeedsRoom = crossListedCourse.QueryNeedsRoom();
			}
		}

		/// <summary>
		/// Visibility control for the search where it is only visible if the assign tab is selected.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TabItem thisTab = (TabItem)MainTab.SelectedItem;
			if (thisTab != null)
			{
				if (thisTab.Name.Equals("AssignTabItem"))
				{
					CourseSearch.Visibility = Visibility.Visible;
					EnterSearch.Visibility = Visibility.Visible;
					if (!CourseSearch.Text.Equals(""))
					{
						Matches.Visibility = Visibility.Visible;
					}
				}
				else
				{
					CourseSearch.Visibility = Visibility.Hidden;
					EnterSearch.Visibility = Visibility.Hidden;
					Matches.Visibility = Visibility.Hidden;
				}
			}
		}

		/// <summary>
		/// Unassigns selected course(s) by removing its unassigned course from right click context menu.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void Unassign_Click(object sender, RoutedEventArgs e)
		{
			foreach (Course course in CoursesDataGrid.SelectedItems)
			{
				course.NeedsRoom = true;
				course.RoomAssignment = null;
			}
		}

		/// <summary>
		/// Opens the add room dialog box when the Add Room button is selected.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void NewButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new AddRoomDialogBox(roomRepo, ViewModel);
			dialog.Closing += new System.ComponentModel.CancelEventHandler(Dialog_Closing);
			dialog.Show();
		}

		/// <summary>
		/// Update the room list if add room has been canclled
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a cancelled event.</param>
		private void Dialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			ViewModel.UpdateRoomList();
		}

		/// <summary>
		/// Deletes a room only if it has no assignments.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void RemoveButton_Click(object sender, RoutedEventArgs e)
		{
			bool status = false;
			bool hasRooms = false;

			if (ViewModel.CoursesForCurrentRoom.GetEnumerator().MoveNext())
			{
				hasRooms = true;
			}

			if (hasRooms == true)
			{
				MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("The selected room must have no assignments before removing ", "ERROR: Room Still has Assignments", System.Windows.MessageBoxButton.OK);
				// Unsure if need to handle the OK click
			}
			else
			{
				MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Removing the Selected Room. Please Confirm ", "Confirm Room Removal", System.Windows.MessageBoxButton.OKCancel);
				if (messageBoxResult == MessageBoxResult.OK)
				{
					status = ViewModel.RoomRepo.RemoveRoom(ViewModel.EditableRoom);
					if (status == true)
					{
						ViewModel.UpdateRoomList();
						messageBoxResult = System.Windows.MessageBox.Show("Successful removal of selected room ", "Removal Succeeded", System.Windows.MessageBoxButton.OK);
					}
					else
					{
						messageBoxResult = System.Windows.MessageBox.Show("An error occurred in the removal, please find the problem and try again ", "Removal Unsucessful", System.Windows.MessageBoxButton.OK);
					}
				}
			}

		}

		/// <summary>
		/// Searchs for all the matches for the text entered in the search bar. 
		/// </summary>
		/// <param name="obj">The text entered in the search bas as a DependencyObjct</param>
		public void FindItem(DependencyObject obj)
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				System.Windows.Controls.DataGridCell dg = obj as System.Windows.Controls.DataGridCell;
				if (dg != null)
				{
					HighlightText(dg);
				}
				FindItem(VisualTreeHelper.GetChild(obj as DependencyObject, i));
			}
		}

		/// <summary>
		/// Highlight the text that has been entered in the search bar and increment # of matches.
		/// </summary>
		/// <param name="itx">The text entered in the search box as an object</param>
		private void HighlightText(Object itx)
		{
			if (itx != null)
			{
				if (itx is TextBlock)
				{
					regex = new Regex("(" + CourseSearch.Text + ")", RegexOptions.IgnoreCase);
					TextBlock tb = itx as TextBlock;
					if (CourseSearch.Text.Length == 0)
					{
						string str = tb.Text;
						tb.Inlines.Clear();
						tb.Inlines.Add(str);
						return;
					}
					string[] substr = regex.Split(tb.Text);
					tb.Inlines.Clear();
					foreach (var item in substr)
					{
						if (regex.Match(item).Success)
						{
							Run runx = new Run(item);
							runx.Background = Brushes.GreenYellow;
							tb.Inlines.Add(runx);
							count++;
						}
						else
						{
							tb.Inlines.Add(item);
						}
					}
					return;
				}
				else
				{
					for (int i = 0; i < VisualTreeHelper.GetChildrenCount(itx as DependencyObject); i++)
					{
						HighlightText(VisualTreeHelper.GetChild(itx as DependencyObject, i));
					}
				}
			}
		}

		/// <summary>
		/// Save changes for a room that has been edited
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void SaveChanges_Click(object sender, RoutedEventArgs e)
		{
			int temp;
			bool error = false;
			Room editedRoom = new Room();
			editedRoom.RoomName = ViewModel.EditableRoom.RoomName;
			if (Int32.TryParse(capacityText.Text, out temp))
			{
				editedRoom.Capacity = int.Parse(capacityText.Text);
			}
			else
			{
				capacityError.Visibility = Visibility.Visible;
				error = true;
			}
			editedRoom.Details = detailsText.Text;
			editedRoom.RoomType = (string)RoomTypeBox.SelectedItem;
			editedRoom.Index = ViewModel.EditableRoom.Index;

			if (!error)
			{
				ViewModel.CurrentRoom = editedRoom;
				//saveChanges.IsEnabled = false;

				ObservableCollection<Course> coursesNeedEditing = ViewModel.CoursesForCurrentRoom;
				foreach (Course course in coursesNeedEditing)
				{
					course.RoomAssignment = editedRoom;
				}
				ViewModel.UpdateRoomList();
				MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Successfully edited the data of selected room ", "Edit Succeeded", System.Windows.MessageBoxButton.OK);
				capacityError.Visibility = Visibility.Hidden;
			}
		}

		private string GetRoomType()
		{
			return RoomType.Lab;
		}

		/// <summary>
		/// Opens up a room that has been selected in the edit rooms tab.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a SelectionChanged event.</param>
		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			capacityError.Visibility = Visibility.Hidden;
			ViewModel.CurrentRoom = ViewModel.EditableRoom;
		}

		/// <summary>
		/// Search for text entered in search box when search button has been clicked.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
		private void EnterSearch_Click(object sender, RoutedEventArgs e)
		{
			FindItem(CoursesDataGrid);
      SetMatches();
		}
            
          public void FindItem(DependencyObject obj)
          {
               	for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
               	{
               		System.Windows.Controls.DataGridCell dg = obj as System.Windows.Controls.DataGridCell;
               		if (dg != null)
               		{
               			HighlightText(dg);
               		}
               		FindItem(VisualTreeHelper.GetChild(obj as DependencyObject, i));
               	}
          }

          private void Expander_Expanded(object sender, RoutedEventArgs e)
          {
              var window = Window.GetWindow(this);
              if (window == null) return;
              window.SizeToContent = SizeToContent.Width;
          }

        private void HighlightText(Object itx)
          {
               	if (itx != null)
               	{
               		if (itx is TextBlock)
               		{
               			regex = new Regex("(" + CourseSearch.Text + ")", RegexOptions.IgnoreCase);
               			TextBlock tb = itx as TextBlock;
       					if (CourseSearch.Text.Length == 0)
               			{
               				string str = tb.Text;
               				tb.Inlines.Clear();
               				tb.Inlines.Add(str);
               			    return;
               			}
               			string[] substr = regex.Split(tb.Text);
               			tb.Inlines.Clear();
               			foreach (var item in substr)
               			{
               				if (regex.Match(item).Success)
               				{
               					Run runx = new Run(item);
               					runx.Background = Brushes.GreenYellow;
               					tb.Inlines.Add(runx);
               				}
               				else
               				{
               					tb.Inlines.Add(item);
               				}
               			}
               			return;
               		}
               		else
               		{
               			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(itx as DependencyObject); i++)
               			{
               				HighlightText(VisualTreeHelper.GetChild(itx as DependencyObject, i));
               			}
               		}
               	}
           }

        private void Teacher_Search_Click(object sender, RoutedEventArgs e)
        {
            string[] multi = {""};
            string delim = "; ";
            List<string> listOfTeachers;
            listOfTeachers = new List<string>();
            List<string> tempDuplicates;
            tempDuplicates = new List<string>();
            List<string> listOfDistinctTeachers;
            listOfDistinctTeachers = new List<string>();
            output = new List<string>();

            ViewModel.CurrentTeacher = SuggestedTeacherlistBox.SelectedItems.ToString();

            Regex rgx = new Regex(@"(\S*,.*?)\s*\(\d*\)");
            Match match;
            string NewInstructor;
            NewInstructor = TeacherSearch.Text;
            ObservableCollection<Course> Courses = new ObservableCollection<Course>(ViewModel.CourseRepo.Courses);

            /*This for loop removes the (ID) at the end of each teacher's name, I only included it for testing on my branch*/
            /*Comment it out or remove it when merging back into the master*/
            foreach (var course in Courses)
            {
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
            }

            /*Loop through the list of teachers and return the possible suggestions that match the input*/
            foreach (var course3 in Courses)
            {
                if (course3.Instructor.Contains(';') == false)
                {
                    listOfTeachers.Add(course3.Instructor);
                }
            }

            /*Collect all of the teachers names in the courses with multiple teachers*/
            foreach (var course4 in Courses)
            {
                if (course4.Instructor.Contains(';') == true)
                {
                    multi = Regex.Split(course4.Instructor, delim);
                    foreach (var d in multi)
                    {
                        tempDuplicates.Add(d);
                    }
                }
            }

            /*Sort and get all distinct teachers*/
            tempDuplicates.Sort();
            tempDuplicates = tempDuplicates.Distinct().ToList();

            /*If the global list of unique teachers doesn't contain the teacher then add it to the list*/
            /*This logic deals with the courses that have multiple teachers listed in the record*/
            foreach (var u in tempDuplicates)
            {
                if (listOfTeachers.Contains(u) == false)
                {
                    listOfTeachers.Add(u);
                }
            }

            listOfTeachers.Sort();
            listOfDistinctTeachers = listOfTeachers.Distinct().ToList(); //The distinct list of teachers in CourseRepo

            foreach (var v in listOfDistinctTeachers)
            {
                if (v.Contains(TeacherSearch.Text))
                {
                    output.Add(v);
                }
            }

            if (TeacherSearch.Text.Equals("") == true)
            {
                output = new List<string>();
                SuggestedTeacherlistBox.ItemsSource = output; // Dispay auto-suggested teacehrs
            }
            else
            {
                SuggestedTeacherlistBox.ItemsSource = output; // Dispay auto-suggested teacehrs
            }
        }

        private void Teacher_ListBox_Selected(object sender, SelectionChangedEventArgs e)
        {
            ObservableCollection<Course> Courses = new ObservableCollection<Course>(ViewModel.CourseRepo.Courses);

            try
            {
                /*This loop builds the list of courses that the user wanted to look up in the search bar (by teacher name)*/
                foreach (var course2 in Courses)
                {
                    /*On initial loadup of the page, there is no search term so skip it*/
                    if (TeacherSearch.Text == null)
                    {
                        continue;
                    }
                    /*If the instructer = Staff but the search term != Staff then skip it*/
                    else if (course2.Instructor.Equals("Staff") && SuggestedTeacherlistBox.SelectedItems.Contains("Staff") == false)
                    {
                        continue;
                    }
                    /* If the instructor = Staff and the search term == Staff then add it to the list*/
                    //else if (course2.Instructor.Equals("Staff") && SuggestedTeacherlistBox.SelectedItems.Contains("Staff") == true)
                    //{
                    //    CoursesForCurrentTeacher = new ObservableCollection<Course>(Courses.Where(x => x.CurrentTeacherInfo?.Equals(SuggestedTeacherlistBox.SelectedItem) == true));
                    //}
                    /*In this case, the Instructor name contains the search term so add it to the list*/
                    else if (course2.Instructor.Equals(SuggestedTeacherlistBox.SelectedValue.ToString()) == true)
                    {
                        CoursesForCurrentTeacher = new ObservableCollection<Course>(Courses.Where(x => x.CurrentTeacherInfo?.Equals(SuggestedTeacherlistBox.SelectedItem) == true));
                    }
                    /*If the course has multiple teachers then use the clicked event to get the courses with that insructor*/
                    else if (course2.Instructor.Contains(SuggestedTeacherlistBox.SelectedValue.ToString()))
                    {
                        CoursesForCurrentTeacher = new ObservableCollection<Course>(Courses.Where(x => x.CurrentTeacherInfo?.Equals(course2.Instructor) == true));
                    }
                }
            }
            catch (NullReferenceException nre)
            {
                /*If nothing has been selected and the user deletes their search query then create a blank lsit of classes to display underneath the suggested teachers box*/
                CoursesForCurrentTeacher = new ObservableCollection<Course>();
            }

            TeacherlistBox.ItemsSource = CoursesForCurrentTeacher;
            //ViewModel.CoursesForCurrentTeacher = CoursesForCurrentTeacher;

            //ICollection<Course> teacherCourseList = (ICollection<Course>)CoursesForCurrentTeacher;
            //CourseRepository.InitInstance(CoursesForCurrentTeacher);
        }

        private void RoomSchedule_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Teacher_Export_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Conflicts.Count != 0)
            {
                string message = "Exporting to Excel while there are conflicts may result in incorrect output. Do you wish to continue with the export?";
                string caption = "Export to Excel";
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxResult result = System.Windows.MessageBox.Show(message, caption, button, icon);

                if (result == MessageBoxResult.No) return;
            }

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Excel Worksheets|*.xls";
            if (saveFileDialog.ShowDialog() == true)
            {
                var fileName = saveFileDialog.FileName;
                var templateFile = System.IO.Path.Combine(Environment.CurrentDirectory, "ClassroomGridTemplate.xls");
                using (var fileStream = File.OpenRead(templateFile))
                {
                    IWorkbook workbook = new HSSFWorkbook(fileStream);
                    workbook.RemoveSheetAt(workbook.GetSheetIndex("Sheet1"));

                    workbook.MissingCellPolicy = MissingCellPolicy.CREATE_NULL_AS_BLANK;
                    ExcelSchedulePrinter printer = new ExcelSchedulePrinter(fileName, workbook);
                    ICourseRepository courseRepository = CourseRepository.GetInstance();
                    //ITeacherScheduleRepository courseSchedule = TeacherScheduleRepository.GetInstance();
                    TeacherScheduleRepository.InitInstance(CoursesForCurrentTeacher);
                    ITeacherScheduleRepository courseSchedule = TeacherScheduleRepository.GetInstance();
                    //ICourseRepository export_courses = (ICourseRepository)CoursesForCurrentTeacher;

                    new ScheduleVisualization(courseRepository, null, printer, SuggestedTeacherlistBox.SelectedValue.ToString(), courseSchedule).PrintTeacherSchedule();
                    //new ScheduleVisualization(export_courses, null, printer).PrintSchedule();
                }
            }
        }
    }

		/// <summary>
		/// Search for text entered in search box when enter key has been pressed.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with KeyUp and KeyDown routed events, as well as related attached and Preview events.</param>
		private void CourseSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				FindItem(CoursesDataGrid);
				SetMatches();
			}
		}

		/// <summary>
		/// Set the color and text for the # of matches found from search.
		/// </summary>
		private void SetMatches()
		{
			Matches.Visibility = Visibility.Visible;
			Matches.Content = "Number of Matches: " + count;
			if (count == 0)
			{
				Matches.Foreground = Brushes.Red;
			}
			else
			{
				Matches.Foreground = Brushes.Green;
			}
			count = 0;
		}
	}
}
