using ClassroomAssignment.Model;
using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.Operations;
using ClassroomAssignment.ViewModel;
using ClassroomAssignment.Visual;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

namespace ClassroomAssignment.Views.TeacherRoomSchedule
{
    /// <summary>
    /// Interaction logic for TeacherRoomScheduleControl.xaml
    /// </summary>
    public partial class TeacherRoomScheduleControl : UserControl
    {

        private const int T_COLUM_WIDTH = 60;
        private const int T_TIME_DURATION_UNIT_IN_MINUTES = 5;
        private static readonly TimeSpan T_FIRST_TIME_SLOT = new TimeSpan(7, 0, 0);
        private static readonly TimeSpan T_LAST_TIME_SLOT = new TimeSpan(22, 0, 0);
        private const DayOfWeek T_FIRST_DAY_OF_SCHEDULE = DayOfWeek.Sunday;
        private const DayOfWeek T_LAST_DAY_OF_SCHEDULE = DayOfWeek.Saturday;

        private ScheduleGridLayout gridLayout_t;
        static List<SolidColorBrush> BackgroundColors_t;
        private Dictionary<string, SolidColorBrush> colorMap_t = new Dictionary<string, SolidColorBrush>();
        private int currentColorIndex_t = 0;

        #region Dependency Properties
        // private readonly DependencyProperty _roomScheduledProperty;
        public static readonly DependencyProperty RoomScheduledProperty_t = DependencyProperty.Register("RoomScheduled_T", typeof(Room), typeof(TeacherRoomScheduleControl), new PropertyMetadata(new PropertyChangedCallback(OnRoomScheduledChangeT)));

        /// <summary>
        /// Set up the text format of a teacher's room's information.
        /// </summary>
        /// <param name="d">Represents an object that participates in the dependency property system.</param>
        /// <param name="e">Provides data for various property changed events.</param>
        private static void OnRoomScheduledChangeT(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TeacherRoomScheduleControl;
            //control.RoomNameTextBlock.Text = control.RoomScheduled_T.RoomName;
            //control.RoomCapacityTextBlock.Text = string.Format("Capacity: {0}", control.RoomScheduled.Capacity.ToString());
            //control.TeacherNameTextBlock.Text = string.Format("Teacher: ");
            //control.TeacherNameTextBlock.Text = string.Format("Teacher: test" + control.RoomScheduled_T.Details);
        }

        /// <summary>
        /// Getter and setter for the teacher's scheduled rooms.
        /// </summary>
        [Bindable(true)]
        public Room RoomScheduled_T
        {
            get { return GetValue(RoomScheduledProperty_t) as Room; }

            set { SetValue(RoomScheduledProperty_t, value); }
        }

        public string selectedTeacher;
        private ObservableCollection<Course> _coursesForTeacher;

        /// <summary>
        /// Getter and setter for the teacher's courses.
        /// </summary>
        public ObservableCollection<Course> CoursesForTeacher
        {
            get => (ObservableCollection<Course>)GetValue(CoursesForRoomPropertyT);
            set
            {
                SetValue(CoursesForRoomPropertyT, value);
            }
        }

        public static readonly DependencyProperty CoursesForRoomPropertyT = DependencyProperty.Register("CoursesForTeacher", typeof(ObservableCollection<Course>), typeof(TeacherRoomScheduleControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(CoursesForRoomChangedT)));

        /// <summary>
        /// Handle the event when the teacher's courses for a room have changed.
        /// </summary>
        /// <param name="d">Represents an object that participates in the dependency property system.</param>
        /// <param name="e">Provides data for various dependency property changed events.</param>
        private static void CoursesForRoomChangedT(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TeacherRoomScheduleControl;
            control.RemoveStaleAvailableItemsT();
            control.ShowAvailableSlotsT();
            control.SetCoursesForRoomT(control.CoursesForTeacher);

            if (e.OldValue != null) (e.OldValue as ObservableCollection<Course>).CollectionChanged -= control.CoursesForRoom_CollectionChanged_T;
            if (e.NewValue != null) (e.NewValue as ObservableCollection<Course>).CollectionChanged += control.CoursesForRoom_CollectionChanged_T;
        }

        /// <summary>
        /// Occurs when the teacher's courses for a room have changed.
        /// </summary>
        /// <param name="sender">A reference to the control/object that raised the event.</param>
        /// <param name="e">State information and event data associated with a NotifyCollectionChanged event.</param>
        private void CoursesForRoom_CollectionChanged_T(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                RemoveStaleCourseLabelsT();
                
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                SetCoursesForRoomT(CoursesForTeacher);
            }
        }

        public static readonly DependencyProperty AvailableSlotsPropertyT = DependencyProperty.Register(nameof(AvailableSlotsT), typeof(ObservableCollection<ScheduleSlot>), typeof(TeacherRoomScheduleControl), new PropertyMetadata(OnAvailableSlotChangeT));

        private ObservableCollection<ScheduleSlot> _availableSlotsT;

        /// <summary>
        /// Getter and setter for the available slots
        /// </summary>
        public ObservableCollection<ScheduleSlot> AvailableSlotsT
        {
            get => _availableSlotsT;
            set
            {
                _availableSlotsT = value;
            }
        }

        /// <summary>
        /// Occurs when the available slots for a room assigned to a teacher have changed.
        /// </summary>
        /// <param name="d">Represents an object that participates in the dependency property system.</param>
        /// <param name="e">Provides data for various dependency property changed events.</param>
        private static void OnAvailableSlotChangeT(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var roomScheduleControl = (TeacherRoomScheduleControl)d;
            if (roomScheduleControl.AvailableSlotsT == null) return;
            roomScheduleControl.AvailableSlotsT.CollectionChanged += roomScheduleControl.AvailableSlots_CollectionChangedT;
        }

        /// <summary>
        /// Occurs when the collection of availble slots have changed
        /// </summary>
        /// <param name="sender">A reference to the control/object that raised the event.</param>
        /// <param name="e">State information and event data associated with a NotifyCollectionChanged event.</param>
        private void AvailableSlots_CollectionChangedT(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                ShowAvailableSlotsT();
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                RemoveStaleAvailableItemsT();
            }
        }

        #endregion

        /// <summary>
        /// Constructor for TeacherRoomScheduleControl. Set the colors for the room schedule.
        /// </summary>
        static TeacherRoomScheduleControl()
        {
            var colors_t = new List<SolidColorBrush>()
            {
                Brushes.YellowGreen,
                Brushes.LightBlue,
                Brushes.Beige,
                Brushes.Honeydew,
                Brushes.Cornsilk,
                Brushes.Blue,
                Brushes.Crimson,
                Brushes.Yellow,
                Brushes.Pink,
                Brushes.Purple
            };

            BackgroundColors_t = colors_t.ConvertAll(x =>
            {
                var color_t = x.Color;
                color_t.A = 200;
                return new SolidColorBrush(color_t);
            });

        }

        /// <summary>
        /// Constructor for RoomScheduleControl. Set the course time slots for the teacher room schedule.
        /// </summary>
        public TeacherRoomScheduleControl()
        {
            InitializeComponent();

            gridLayout_t = new ScheduleGridLayout(
                T_FIRST_TIME_SLOT,
                T_LAST_TIME_SLOT,
                T_FIRST_DAY_OF_SCHEDULE,
                T_LAST_DAY_OF_SCHEDULE,
                T_TIME_DURATION_UNIT_IN_MINUTES
                );

            SetupScheduleGridT();            
        }

        #region Setup

        /// <summary>
        /// Set up the schedule grid for the teacher view.
        /// </summary>
        private void SetupScheduleGridT()
        {
            SetupStructureT();
            PopulateHeadersT();
        }

        /// <summary>
        /// Set up the structure of the schedule grid for the teacher view.
        /// </summary>
        private void SetupStructureT()
        {
            AddTimeColumnT();
            AddEmptyColumnT();
            AddDayOfWeekColumnsT();
            AddEmptyColumnT();

            AddDayOfWeekRowT();
            AddTimeRowsT();
            AddBordersT();
        }

        /// <summary>
        /// Fill in the headers for the time rows and days of the week columns for the schedule grid.
        /// </summary>
        private void PopulateHeadersT()
        {
            SetupTimeRowHeadersT();
            SetupDayOfWeekColumnHeadersT();
        }

        /// <summary>
        /// Add in the time rows to the schedule grid.
        /// </summary>
        private void AddTimeRowsT()
        {
            foreach (var slot in gridLayout_t.TimeSlotsInSchedule())
            {
                ScheduleGrid_T.RowDefinitions.Add(new RowDefinition());
            }
        }

        /// <summary>
        /// Add in the days of the weeks to the rows of the schedule grid.
        /// </summary>
        private void AddDayOfWeekRowT()
        {
            ScheduleGrid_T.RowDefinitions.Add(new RowDefinition());
        }

        /// <summary>
        /// Add the days of the weeks for the columns of the schedule grid.
        /// </summary>
        private void AddDayOfWeekColumnsT()
        {
            var column = ScheduleGrid_T.ColumnDefinitions.Count;
            foreach (var day in gridLayout_t.DaysOfWeekInGrid())
            {
                var columnDef = new ColumnDefinition();
                columnDef.Width = new GridLength(1, GridUnitType.Star);
                ScheduleGrid_T.ColumnDefinitions.Add(columnDef);
            }
        }

        /// <summary>
        /// Add the times to the columns of the schedule grid.
        /// </summary>
        private void AddTimeColumnT()
        {
            ScheduleGrid_T.ColumnDefinitions.Add(new ColumnDefinition());
        }

        /// <summary>
        /// Add an empty column to the schedule grid.
        /// </summary>
        private void AddEmptyColumnT()
        {
            var columnDef = new ColumnDefinition();
            columnDef.Width = new GridLength(T_COLUM_WIDTH, GridUnitType.Pixel);
            ScheduleGrid_T.ColumnDefinitions.Add(columnDef);
        }

        /// <summary>
        /// Set up the headers for the time rows.
        /// </summary>
        private void SetupTimeRowHeadersT()
        {
            foreach (var slot in gridLayout_t.TimeSlotsInSchedule())
            {
                var textblock = GetTextBlockForTimeT(slot);
                ScheduleGrid_T.Children.Add(textblock);
                Grid.SetColumn(textblock, 0);
                Grid.SetRow(textblock, gridLayout_t.GetRowForTime(slot));
            }
        }

        /// <summary>
        /// Set up the headers for the days of the week columns.
        /// </summary>
        private void SetupDayOfWeekColumnHeadersT()
        {
            const int firstRow = 0;
            for (DayOfWeek day = T_FIRST_DAY_OF_SCHEDULE; day <= T_LAST_DAY_OF_SCHEDULE; day++)
            {
                var textBlock = GetTextBlockForDayT(day);
                ScheduleGrid_T.Children.Add(textBlock);
                Grid.SetRow(textBlock, firstRow);
                Grid.SetColumn(textBlock, gridLayout_t.GetColumnForDay(day));
            }
        }

        /// <summary>
        /// Get the text block for the day of the week.
        /// </summary>
        /// <param name="day">A day of the week</param>
        /// <returns>A textblock containing the day of htw eek</returns>
        private TextBlock GetTextBlockForDayT(DayOfWeek day)
        {
            var textBlock = new TextBlock();
            textBlock.Style = FindResource("DayOfWeekHeaderStyle") as Style;
            textBlock.Text = day.ToString();

            return textBlock;
        }

        /// <summary>
        /// Get the text block for the time.
        /// </summary>
        /// <param name="time">A time interval</param>
        /// <returns>A textblock containing the time</returns>
        private TextBlock GetTextBlockForTimeT(TimeSpan time)
        {
            var textblock = new TextBlock();
            textblock.Text = new DateTime().Add(time).ToString("h tt");
            if (time.Minutes != 0) textblock.Visibility = Visibility.Hidden;

            return textblock;
        }

        /// <summary>
        /// Add the borders to the schedule grid.
        /// </summary>
        private void AddBordersT()
        {
            AddDayOfWeekColumnBordersT();
            AddHrlyRowBordersT();
        }

        /// <summary>
        /// Add column borders to the days of the week.
        /// </summary>
        private void AddDayOfWeekColumnBordersT()
        {

            foreach (var day in gridLayout_t.DaysOfWeekInGrid())
            {
                Border border = new Border();
                if (day == T_FIRST_DAY_OF_SCHEDULE)
                {
                    border.Style = FindResource("LeftMostColumnBorder") as Style;
                }
                else
                {
                    border.Style = FindResource("ColumnBorder") as Style;
                }

                ScheduleGrid_T.Children.Add(border);
                Grid.SetColumn(border, gridLayout_t.GetColumnForDay(day));
                Grid.SetRow(border, 1);
                Grid.SetRowSpan(border, ScheduleGrid_T.RowDefinitions.Count - 1);
            }
        }

        /// <summary>
        /// Add row borders for the hourly rows.
        /// </summary>
        private void AddHrlyRowBordersT()
        {
            var thickness = new Thickness(0, 1, 0, 0);

            foreach (var time in gridLayout_t.TimeSlotsInSchedule())
            {
                if (time.Minutes == 0)
                {
                    var border = new Border();
                    border.BorderThickness = thickness;
                    border.BorderBrush = Brushes.Gray;
                    ScheduleGrid_T.Children.Add(border);

                    Grid.SetRow(border,gridLayout_t.GetRowForTime(time));
                    Grid.SetColumnSpan(border, 10);
                }
            }
        }

        #endregion


        #region Private Methods
        /// <summary>
        /// Setup the courses for a room on the schedule grid.
        /// </summary>
        /// <param name="courses">The enumerator containing the courses.</param>
        private void SetCoursesForRoomT(IEnumerable<Course> courses)
        {
            if (courses == null) return;

            RemoveStaleCourseLabelsT();

            foreach (var course in courses)
            {
                foreach (var day in course.MeetingDays)
                {
                    var textBlock = GetCourseLabelT(day, course);
                    textBlock.Background = GetBackgroundColorForCourseT(course);
                    textBlock.TextAlignment = TextAlignment.Center;
                    textBlock.Text = course.ToString();
                    Grid.SetRowSpan(textBlock, RowSpanForCourseT(course));
                }
            }
        }

        /// <summary>
        /// Remove the stale available items from the schedule grid.
        /// </summary>
        private void RemoveStaleAvailableItemsT()
        {
            List<TextBlock> staleAvailableItems = new List<TextBlock>();
            foreach (var child in ScheduleGrid_T.Children)
            {
                AvailableSlotLabel textBlock;
                if ((textBlock = child as AvailableSlotLabel) != null) staleAvailableItems.Add(textBlock);
              
            }

            foreach (var staleItem in staleAvailableItems) ScheduleGrid_T.Children.Remove(staleItem);
        }

        /// <summary>
        /// Show the available schedule slots where the course can be assigned.
        /// </summary>
        private void ShowAvailableSlotsT()
        {
            if (AvailableSlotsT == null) return;

            foreach (var slot in AvailableSlotsT)
            {
                foreach (var day in slot.MeetingDays)
                {
                    int row = gridLayout_t.GetRowForTime(slot.StartTime);
                    int span = gridLayout_t.SpanForDurationInMinutes((int)(slot.EndTime - slot.StartTime).TotalMinutes);

                    var availableSlot = new AvailableSlotLabel(slot.StartTime, slot.EndTime);

                    ScheduleGrid_T.Children.Add(availableSlot);
                    Grid.SetRow(availableSlot, gridLayout_t.GetRowForTime(slot.StartTime));
                    Grid.SetColumn(availableSlot, gridLayout_t.GetColumnForDay(day));
                    Grid.SetRowSpan(availableSlot, span);
                }
            }
        }

        #endregion

        #region Private Helper Methods
        /// Set the background colors for the courses based on subject code.
        /// </summary>
        /// <param name="course">A course object.</param>
        /// <returns>The background color assigned to the course's subject code.</returns>
        private SolidColorBrush GetBackgroundColorForCourseT(Course course)
        {
            if (colorMap_t.ContainsKey(course.SubjectCode))
            {
                return colorMap_t[course.SubjectCode];
            }
            else if (BackgroundColors_t.Count == currentColorIndex_t + 1)
            {
                return BackgroundColors_t.Last();
            }
            else
            {
                colorMap_t[course.SubjectCode] = BackgroundColors_t[currentColorIndex_t];
                currentColorIndex_t++;
                return colorMap_t[course.SubjectCode];
            }
        }

        /// <summary>
        /// Remove the stale course labels from the schedule grid.
        /// </summary>
        private void RemoveStaleCourseLabelsT()
        {
            var staleLabels = new List<CourseLabel>();
            foreach (var child in ScheduleGrid_T.Children)
            {
                CourseLabel availableSlot = child as CourseLabel;
                if (availableSlot != null)
                {
                    staleLabels.Add(availableSlot);
                }
            }

            foreach (var staleLabel in staleLabels)
            {
                ScheduleGrid_T.Children.Remove(staleLabel);
            }
        }

        /// <summary>
        /// Get the labels of a course.
        /// </summary>
        /// <param name="day">The day of the week.</param>
        /// <param name="course">A course object.</param>
        /// <returns>The label of a course.</returns>
        private CourseLabel GetCourseLabelT(DayOfWeek day, Course course)
        {
            var courseLabel = new CourseLabel(course);
            
            ScheduleGrid_T.Children.Add(courseLabel);
            Grid.SetRow(courseLabel, gridLayout_t.GetRowForTime(course.StartTime.Value));
            Grid.SetColumn(courseLabel, gridLayout_t.GetColumnForDay(day));

            return courseLabel;
        }

        /// <summary>
        /// Set up the row span length for a course based on its time length.
        /// </summary>
        /// <param name="course">A course object</param>
        /// <returns>The time span of a course.</returns>
        private int RowSpanForCourseT(Course course)
        {
            TimeSpan courseLength = course.EndTime.Value - course.StartTime.Value;

            return gridLayout_t.SpanForDurationInMinutes((int) courseLength.TotalMinutes);
        }

        #endregion

       
    }
}
