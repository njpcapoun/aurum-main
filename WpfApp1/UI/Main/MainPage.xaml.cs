using ClassroomAssignment.Model;
using ClassroomAssignment.Operations;
using ClassroomAssignment.Repo;
using ClassroomAssignment.UI.Assignment;
using ClassroomAssignment.UI.Reassignment;
using ClassroomAssignment.UI.Changes;
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
using Microsoft.VisualBasic;

namespace ClassroomAssignment.UI.Main
{
      /// <summary>
      /// Interaction logic for MainPage.xaml
      /// </summary>
      public partial class MainPage : Page
      {
          public MainWindowViewModel ViewModel { get; set; }

          private Dictionary<Course, Course> CrossListedToMain = new Dictionary<Course, Course>();

          public Room SelectedRoom { get; set; }

          public int index { get; set; }

          private RoomRepository roomRepo;

        SaveBase saveWork = new SaveBase();

        Regex regex;

          public MainPage()
          {
              InitializeComponent();
              ViewModel = new MainWindowViewModel(this);
              roomRepo = ViewModel.RoomRepo;
              DataContext = ViewModel;
              this.Loaded += MainPage_Loaded;
          }

          private void MainPage_Loaded(object sender, RoutedEventArgs e)
          {
              while (NavigationService.RemoveBackEntry() != null);
          }

          private void SaveAs(object sender, EventArgs e)
          {
            saveWork.SaveAs();
          }

          private void Menu_Save(object sender, EventArgs e)
          {
            saveWork.SaveWork();
          }

          private List<Course> GetOriginalCourses()
          {
              return App.Current.Resources["originalCourses"] as List<Course>;
          }

          /*
           * Moves to the Changes page, file menu item
           */
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

          /*
           * Sends selected courses to AssignmentPage to be assigned, right click menu
           */
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
        
          // When you click reassign it brings up two popups
          // Asking for the capacity and type of the room
          // Then it goes to the reassignment page
          private void Reassignment_Click(object sender, RoutedEventArgs e)
          {
            Course c = CoursesDataGrid.SelectedItem as Course;
            ReassignmentPopUp reassignmentPopUp = new ReassignmentPopUp(c); 
          }

          /*
           * Export the files to CSV format, file menu item
           */
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

          /*
           *  Sets which options are available for a course on right click
           */
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

          /*
           * For the Edit Course option on right click
           */
          private void CoursesMenuItem_Click(object sender, RoutedEventArgs e)
          {
              var course = CoursesDataGrid.SelectedItem as Course;
              if (course == null) return;

              var editPage = new CourseEditPage(course);
              NavigationService.Navigate(editPage);
          }

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

          /*
           * Set Course as no assignment needed on click
           */
          private void NoAssignmentNeededMenuItem_Click(object sender, RoutedEventArgs e)
          {
              var courses = CoursesDataGrid.SelectedItems;

              foreach (Course course in courses)
              {
                  course.NeedsRoom = false;
              }
          }

          /*
           * Crosslists selected courses on click
           */
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
                  mainCourse.AddCrossListedCourse(course);
                  CrossListedToMain[course] = mainCourse;
              }
          }

          private void NewCourseMenuItem_Click(object sender, RoutedEventArgs e)
          {
              var dialog = new AddCourseDialogBox();
              dialog.Show();
          }

          /*
           * Move course to assignment needed on click
           */
          private void AssignmentNeeded_Click(object sender, RoutedEventArgs e)
          {
              foreach (Course course in CoursesDataGrid.SelectedItems)
              {
                  course.NeedsRoom = true;
                  if (CrossListedToMain.ContainsKey(course))
                  {
                      CrossListedToMain[course].RemoveCrossListedCourse(course);
                      CrossListedToMain.Remove(course);
                  }
              }
          }

          /*
           * Removes crosslisted courses from no assignment needed section, and removes the crosslist
           */
          private void RemoveCrossListedCourseMenuItem_Click(object sender, RoutedEventArgs e)
          {
              var contextMenu = (sender as System.Windows.Controls.MenuItem).Parent as System.Windows.Controls.ContextMenu;
              var crossListedCourse = (contextMenu.PlacementTarget as System.Windows.Controls.ComboBox).SelectedItem as Course;
              var mainCourse = CoursesDataGrid.SelectedItem as Course;

              if (mainCourse == null) return;

              mainCourse.RemoveCrossListedCourse(crossListedCourse);
              crossListedCourse.NeedsRoom = crossListedCourse.QueryNeedsRoom();
              CrossListedToMain[crossListedCourse].RemoveCrossListedCourse(crossListedCourse);
              CrossListedToMain.Remove(crossListedCourse);
          }

          private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
          {

          }

		/* 
		 * Unassign an assigned course if "Unassign" is clicked in context menu.
		 */
		private void Unassign_Click(object sender, RoutedEventArgs e)
		{
			foreach (Course course in CoursesDataGrid.SelectedItems)
			{
				course.NeedsRoom = true; // course.QueryNeedsRoom();???
				course.RoomAssignment = null;
				/*if (CrossListedToMain.ContainsKey(course))
				{
					CrossListedToMain[course].RemoveCrossListedCourse(course);
					CrossListedToMain.Remove(course);
				}*/
			}
		}

		private void ListBox_Selected(object sender, RoutedEventArgs e)
          {
              ViewModel.EditableRoom = (Room)listBox.SelectedItem;
          }

          private void NewButton_Click(object sender, RoutedEventArgs e)
          {
              var dialog = new AddRoomDialogBox(roomRepo);
              dialog.Show();
          }

          private void RemoveButton_Click(object sender, RoutedEventArgs e)
          {
              // confirmation pop-up
          }
          
           /*
           * Highlight text entered in search bar
           */
          private void CourseSearch_TextChanged(object sender, TextChangedEventArgs e)
          {
			FindItem(CoursesDataGrid);
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
      }
  }
