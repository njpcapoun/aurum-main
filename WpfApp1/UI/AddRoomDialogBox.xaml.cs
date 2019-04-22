using ClassroomAssignment.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Shapes;
using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.UI.Main;

namespace ClassroomAssignment.UI
{
    /// <summary>
    /// Interaction logic for AddRoomDialogBox.xaml
    /// </summary>
    public partial class AddRoomDialogBox : Window
    {

        private RoomRepository roomRepo;

        public string RoomName = "";
        public int Capacity = 0;
        public string Details = "";
        public string type = RoomType.Lab;
        
        public AddRoomDialogBox(RoomRepository roomRepository, MainWindowViewModel ViewModel)
        {
            CopyRoom = new Room();
            roomRepo = roomRepository;
            InitializeComponent();
            DataContext = this;
        }

        public Room CopyRoom { get; set; }

        private List<PropertyInfo> propertiesChanged = new List<PropertyInfo>();
       
        private void CopyRoom_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Type type = CopyRoom.GetType();
            PropertyInfo propertyInfo = type.GetProperty(e.PropertyName);
            if (propertyInfo.SetMethod != null)
            {
                propertiesChanged.Add(propertyInfo);
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            int temp;
            int index = roomRepo.Rooms.Count;
            if (enterRoomName.Text != "")
            {
                RoomName = enterRoomName.Text;
            }
            else
            {
                nameError.Visibility = Visibility.Visible;
                return;
            }

            if (Int32.TryParse(enterCapacity.Text, out temp))
            {
                Capacity = temp;
            }
            else
            {
                capacityError.Content = "The capacity must be an integer.";
                capacityError.Visibility = Visibility.Visible;
                return;
            }
            Details = enterDetails.Text;
            type = (string)enterType.SelectedItem;

            bool success = roomRepo.AddNewRoom(index, RoomName, Capacity, Details, type);
            if(success)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("The submitted room has been added", "SUCCESS!", System.Windows.MessageBoxButton.OK);
                this.Close();
            }
            else
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("The submitted Room Name is already a room in use", "ERROR: Room Already Exists", System.Windows.MessageBoxButton.OK);
            }
        }

        private void EnterType_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> RoomTypes = new List<string>();
            RoomTypes.Add(RoomType.Lab);
            RoomTypes.Add(RoomType.Lecture);
            RoomTypes.Add(RoomType.Conference);
            RoomTypes.Add(RoomType.Itin);
            RoomTypes.Add(RoomType.Cyber);
            RoomTypes.Add(RoomType.Distance);
            var combo = sender as ComboBox;
            combo.ItemsSource = RoomTypes;
            combo.SelectedIndex = 0;
        }

        private void EnterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = sender as ComboBox;
            type = selectedItem.SelectedItem as string;
        }
    }

}


