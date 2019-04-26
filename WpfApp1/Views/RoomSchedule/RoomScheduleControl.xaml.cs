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

namespace ClassroomAssignment.Views.RoomSchedule
{
    /// <summary>
    /// Interaction logic for RoomScheduleControl.xaml
    /// </summary>
    public partial class RoomScheduleControl : UserControl
    {

        private const int COLUMN_WIDTH = 60;
        private const int TIME_DURATION_UNIT_IN_MINUTES = 5;
        private static readonly TimeSpan FIRST_TIME_SLOT = new TimeSpan(7, 0, 0);
        private static readonly TimeSpan LAST_TIME_SLOT = new TimeSpan(22, 0, 0);
        private const DayOfWeek FIRST_DAY_OF_SCHEDULE = DayOfWeek.Sunday;
        private const DayOfWeek LAST_DAY_OF_SCHEDULE = DayOfWeek.Saturday;

        private ScheduleGridLayout gridLayout;
        static List<SolidColorBrush> BackgroundColors;
        private Dictionary<string, SolidColorBrush> colorMap = new Dictionary<string, SolidColorBrush>();
        private int currentColorIndex = 0;

        #region Dependency Properties
        // private readonly DependencyProperty _roomScheduledProperty;
        public static readonly DependencyProperty RoomScheduledProperty = DependencyProperty.Register("RoomScheduled", typeof(Room), typeof(RoomScheduleControl), new PropertyMetadata(new PropertyChangedCallback(OnRoomScheduledChange)));

        /// <summary>
        /// Set up the text format of a room's information.
        /// </summary>
        /// <param name="d">Represents an object that participates in the dependency property system.</param>
        /// <param name="e">Provides data for various dependency property changed events.</param>
        private static void OnRoomScheduledChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RoomScheduleControl;
            control.RoomNameTextBlock.Text = control.RoomScheduled.RoomName;
            control.RoomCapacityTextBlock.Text = string.Format("Capacity: {0}", control.RoomScheduled.Capacity.ToString());
        }

        /// <summary>
        /// Getter and setter for the room's schedule.
        /// </summary>
        [Bindable(true)]
        public Room RoomScheduled
        {
            get { return GetValue(RoomScheduledProperty) as Room; }

            set { SetValue(RoomScheduledProperty, value); }
        }


        private ObservableCollection<Course> _coursesForRoom;

        /// <summary>
        /// Getter and setter for the courses of a room.
        /// </summary>
        public ObservableCollection<Course> CoursesForRoom
        {
            get => (ObservableCollection<Course>)GetValue(CoursesForRoomProperty);
            set
            {
                SetValue(CoursesForRoomProperty, value);
            }
        }

        public static readonly DependencyProperty CoursesForRoomProperty = DependencyProperty.Register("CoursesForRoom", typeof(ObservableCollection<Course>), typeof(RoomScheduleControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(CoursesForRoomChanged)));

        /// <summary>
        /// Handle the event when the courses of a room have changed.
        /// </summary>
        /// <param name="d">Represents an object that participates in the dependency property system.</param>
        /// <param name="e">Provides data for various dependency property changed events.</param>
        private static void CoursesForRoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RoomScheduleControl;
            control.RemoveStaleAvailableItems();
            control.ShowAvailableSlots();
            control.SetCoursesForRoom(control.CoursesForRoom);

            if (e.OldValue != null) (e.OldValue as ObservableCollection<Course>).CollectionChanged -= control.CoursesForRoom_CollectionChanged;
            if (e.NewValue != null) (e.NewValue as ObservableCollection<Course>).CollectionChanged += control.CoursesForRoom_CollectionChanged;
        }

        /// <summary>
        /// Occurs when the courses of a room have changed.
        /// </summary>
        /// <param name="sender">A reference to the control/object that raised the event.</param>
        /// <param name="e">State information and event data associated with a NotifyCollectionChanged event.</param>
        private void CoursesForRoom_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                RemoveStaleCourseLabels();
                
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                SetCoursesForRoom(CoursesForRoom);
            }
        }

        public static readonly DependencyProperty AvailableSlotsProperty = DependencyProperty.Register(nameof(AvailableSlots), typeof(ObservableCollection<ScheduleSlot>), typeof(RoomScheduleControl), new PropertyMetadata(OnAvailableSlotChange));

        private ObservableCollection<ScheduleSlot> _availableSlots;

        /// <summary>
        /// Getter and setter for the available slots
        /// </summary>
        public ObservableCollection<ScheduleSlot> AvailableSlots
        {
            get => _availableSlots;
            set
            {
                _availableSlots = value;
            }
        }

        /// <summary>
        /// Occurs when the available slots for a room have changed.
        /// </summary>
        /// <param name="d">Represents an object that participates in the dependency property system.</param>
        /// <param name="e">Provides data for various dependency property changed events.</param>
        private static void OnAvailableSlotChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var roomScheduleControl = (RoomScheduleControl)d;
            if (roomScheduleControl.AvailableSlots == null) return;
            roomScheduleControl.AvailableSlots.CollectionChanged += roomScheduleControl.AvailableSlots_CollectionChanged;
        }

        /// <summary>
        /// Occurs when the collection of availble slots have changed
        /// </summary>
        /// <param name="sender">A reference to the control/object that raised the event.</param>
        /// <param name="e">State information and event data associated with a NotifyCollectionChanged event.</param>
        private void AvailableSlots_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                ShowAvailableSlots();
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                RemoveStaleAvailableItems();
            }
        }

        #endregion

        /// <summary>
        /// Constructor for RoomScheduleControl. Set the colors for the room schedule.
        /// </summary>
        static RoomScheduleControl()
        {
            var colors = new List<SolidColorBrush>()
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

            BackgroundColors = colors.ConvertAll(x =>
            {
                var color = x.Color;
                color.A = 200;
                return new SolidColorBrush(color);
            });

        }

        /// <summary>
        /// Constructor for RoomScheduleControl. Set the course time slots for the room schedule.
        /// </summary>
        public RoomScheduleControl()
        {
            InitializeComponent();

            gridLayout = new ScheduleGridLayout(
                FIRST_TIME_SLOT,
                LAST_TIME_SLOT,
                FIRST_DAY_OF_SCHEDULE,
                LAST_DAY_OF_SCHEDULE,
                TIME_DURATION_UNIT_IN_MINUTES
                );

            SetupScheduleGrid();            
        }

        #region Setup

        /// <summary>
        /// Set up the schedule grid.
        /// </summary>
        private void SetupScheduleGrid()
        {
            SetupStructure();
            PopulateHeaders();
        }

        /// <summary>
        /// Set up the structure of the schedule grid.
        /// </summary>
        private void SetupStructure()
        {
            AddTimeColumn();
            AddEmptyColumn();
            AddDayOfWeekColumns();
            AddEmptyColumn();

            AddDayOfWeekRow();
            AddTimeRows();
            AddBorders();
        }

        /// <summary>
        /// Fill in the headers for the time rows and days of the week columns for the schedule grid.
        /// </summary>
        private void PopulateHeaders()
        {
            SetupTimeRowHeaders();
            SetupDayOfWeekColumnHeaders();
        }

        /// <summary>
        /// Add in the time rows to the schedule grid.
        /// </summary>
        private void AddTimeRows()
        {
            foreach (var slot in gridLayout.TimeSlotsInSchedule())
            {
                ScheduleGrid.RowDefinitions.Add(new RowDefinition());
            }
        }

        /// <summary>
        /// Add in the days of the weeks to the rows of the schedule grid.
        /// </summary>
        private void AddDayOfWeekRow()
        {
            ScheduleGrid.RowDefinitions.Add(new RowDefinition());
        }

        /// <summary>
        /// Add the days of the weeks for the columns of the schedule grid.
        /// </summary>
        private void AddDayOfWeekColumns()
        {
            var column = ScheduleGrid.ColumnDefinitions.Count;
            foreach (var day in gridLayout.DaysOfWeekInGrid())
            {
                var columnDef = new ColumnDefinition();
                columnDef.Width = new GridLength(1, GridUnitType.Star);
                ScheduleGrid.ColumnDefinitions.Add(columnDef);
            }
        }

        /// <summary>
        /// Add the times to the columns of the schedule grid.
        /// </summary>
        private void AddTimeColumn()
        {
            ScheduleGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        /// <summary>
        /// Add an empty column to the schedule grid.
        /// </summary>
        private void AddEmptyColumn()
        {
            var columnDef = new ColumnDefinition();
            columnDef.Width = new GridLength(COLUMN_WIDTH, GridUnitType.Pixel);
            ScheduleGrid.ColumnDefinitions.Add(columnDef);
        }

        /// <summary>
        /// Set up the headers for the time rows.
        /// </summary>
        private void SetupTimeRowHeaders()
        {
            foreach (var slot in gridLayout.TimeSlotsInSchedule())
            {
                var textblock = GetTextBlockForTime(slot);
                ScheduleGrid.Children.Add(textblock);
                Grid.SetColumn(textblock, 0);
                Grid.SetRow(textblock, gridLayout.GetRowForTime(slot));
            }
        }

        /// <summary>
        /// Set up the headers for the days of the week columns.
        /// </summary>
        private void SetupDayOfWeekColumnHeaders()
        {
            const int firstRow = 0;
            for (DayOfWeek day = FIRST_DAY_OF_SCHEDULE; day <= LAST_DAY_OF_SCHEDULE; day++)
            {
                var textBlock = GetTextBlockForDay(day);
                ScheduleGrid.Children.Add(textBlock);
                Grid.SetRow(textBlock, firstRow);
                Grid.SetColumn(textBlock, gridLayout.GetColumnForDay(day));
            }
        }

        /// <summary>
        /// Get the text block for the day of the week.
        /// </summary>
        /// <param name="day">A day of the week</param>
        /// <returns>A textblock containing the day of htw eek</returns>
        private TextBlock GetTextBlockForDay(DayOfWeek day)
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
        private TextBlock GetTextBlockForTime(TimeSpan time)
        {
            var textblock = new TextBlock();
            textblock.Text = new DateTime().Add(time).ToString("h tt");
            if (time.Minutes != 0) textblock.Visibility = Visibility.Hidden;

            return textblock;
        }

        /// <summary>
        /// Add the borders to the schedule grid.
        /// </summary>
        private void AddBorders()
        {
            AddDayOfWeekColumnBorders();
            AddHrlyRowBorders();
        }

        /// <summary>
        /// Add column borders to the days of the week.
        /// </summary>
        private void AddDayOfWeekColumnBorders()
        {

            foreach (var day in gridLayout.DaysOfWeekInGrid())
            {
                Border border = new Border();
                if (day == FIRST_DAY_OF_SCHEDULE)
                {
                    border.Style = FindResource("LeftMostColumnBorder") as Style;
                }
                else
                {
                    border.Style = FindResource("ColumnBorder") as Style;
                }

                ScheduleGrid.Children.Add(border);
                Grid.SetColumn(border, gridLayout.GetColumnForDay(day));
                Grid.SetRow(border, 1);
                Grid.SetRowSpan(border, ScheduleGrid.RowDefinitions.Count - 1);
            }
        }

        /// <summary>
        /// Add row borders for the hourly rows.
        /// </summary>
        private void AddHrlyRowBorders()
        {
            var thickness = new Thickness(0, 1, 0, 0);

            foreach (var time in gridLayout.TimeSlotsInSchedule())
            {
                if (time.Minutes == 0)
                {
                    var border = new Border();
                    border.BorderThickness = thickness;
                    border.BorderBrush = Brushes.Gray;
                    ScheduleGrid.Children.Add(border);

                    Grid.SetRow(border,gridLayout.GetRowForTime(time));
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
        private void SetCoursesForRoom(IEnumerable<Course> courses)
        {
            if (courses == null) return;

            RemoveStaleCourseLabels();

            foreach (var course in courses)
            {
                foreach (var day in course.MeetingDays)
                {
                    var textBlock = GetCourseLabel(day, course);
                    textBlock.Background = GetBackgroundColorForCourse(course);
                    textBlock.TextAlignment = TextAlignment.Center;
                    textBlock.Text = course.ToString();
                    Grid.SetRowSpan(textBlock, RowSpanForCourse(course));
                }
            }
        }

        /// <summary>
        /// Remove the stale available items from the schedule grid.
        /// </summary>
        private void RemoveStaleAvailableItems()
        {
            List<TextBlock> staleAvailableItems = new List<TextBlock>();
            foreach (var child in ScheduleGrid.Children)
            {
                AvailableSlotLabel textBlock;
                if ((textBlock = child as AvailableSlotLabel) != null) staleAvailableItems.Add(textBlock);
              
            }

            foreach (var staleItem in staleAvailableItems) ScheduleGrid.Children.Remove(staleItem);
        }

        /// <summary>
        /// Show the available schedule slots where the course can be assigned.
        /// </summary>
        private void ShowAvailableSlots()
        {
            if (AvailableSlots == null) return;

            foreach (var slot in AvailableSlots)
            {
                foreach (var day in slot.MeetingDays)
                {
                    int row = gridLayout.GetRowForTime(slot.StartTime);
                    int span = gridLayout.SpanForDurationInMinutes((int)(slot.EndTime - slot.StartTime).TotalMinutes);

                    var availableSlot = new AvailableSlotLabel(slot.StartTime, slot.EndTime);

                    ScheduleGrid.Children.Add(availableSlot);
                    Grid.SetRow(availableSlot, gridLayout.GetRowForTime(slot.StartTime));
                    Grid.SetColumn(availableSlot, gridLayout.GetColumnForDay(day));
                    Grid.SetRowSpan(availableSlot, span);
                }
            }
        }

        #endregion

        #region Private Helper Methods
        /// <summary>
        /// Set the background colors for the courses based on subject code.
        /// </summary>
        /// <param name="course">A course object.</param>
        /// <returns>The background color assigned to the course's subject code.</returns>
        private SolidColorBrush GetBackgroundColorForCourse(Course course)
        {
            if (colorMap.ContainsKey(course.SubjectCode))
            {
                return colorMap[course.SubjectCode];
            }
            else if (BackgroundColors.Count == currentColorIndex + 1)
            {
                return BackgroundColors.Last();
            }
            else
            {
                colorMap[course.SubjectCode] = BackgroundColors[currentColorIndex];
                currentColorIndex++;
                return colorMap[course.SubjectCode];
            }
        }

        /// <summary>
        /// Remove the stale course labels from the schedule grid.
        /// </summary>
        private void RemoveStaleCourseLabels()
        {
            var staleLabels = new List<CourseLabel>();
            foreach (var child in ScheduleGrid.Children)
            {
                CourseLabel availableSlot = child as CourseLabel;
                if (availableSlot != null)
                {
                    staleLabels.Add(availableSlot);
                }
            }

            foreach (var staleLabel in staleLabels)
            {
                ScheduleGrid.Children.Remove(staleLabel);
            }
        }

        /// <summary>
        /// Get the labels of a course.
        /// </summary>
        /// <param name="day">The day of the week.</param>
        /// <param name="course">A course object.</param>
        /// <returns>The label of a course.</returns>
        private CourseLabel GetCourseLabel(DayOfWeek day, Course course)
        {
            var courseLabel = new CourseLabel(course);
            
            ScheduleGrid.Children.Add(courseLabel);
            Grid.SetRow(courseLabel, gridLayout.GetRowForTime(course.StartTime.Value));
            Grid.SetColumn(courseLabel, gridLayout.GetColumnForDay(day));

            return courseLabel;
        }

        /// <summary>
        /// Set up the row span length for a course based on its time length.
        /// </summary>
        /// <param name="course">A course object</param>
        /// <returns>The time span of a course.</returns>
        private int RowSpanForCourse(Course course)
        {
            TimeSpan courseLength = course.EndTime.Value - course.StartTime.Value;

            return gridLayout.SpanForDurationInMinutes((int) courseLength.TotalMinutes);
        }

        #endregion

       
    }
}
