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

        public string RoomName;
        public int Capacity;
        public string Details;
        public string type = RoomType.Lab;
        
        public AddRoomDialogBox(RoomRepository roomRepository, MainWindowViewModel ViewModel)
        {
            CopyRoom = new Room();
            RoomTypes = ViewModel.RoomTypes;
            roomRepo = roomRepository;
            InitializeComponent();
            DataContext = this;
        }

        public Room CopyRoom { get; set; }

        private List<PropertyInfo> propertiesChanged = new List<PropertyInfo>();

        private List<string> RoomTypes;
       
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
            int index = roomRepo.Rooms.Count;
            RoomName = enterRoomName.Text;
            Capacity = int.Parse(enterCapacity.Text);
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

    }
}
