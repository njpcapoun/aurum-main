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
        public string type;
        
        public AddRoomDialogBox(RoomRepository roomRepository)
        {
            roomRepo = roomRepository;
            InitializeComponent();
            CopyRoom = new Room();
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
            int index = roomRepo.Rooms.Count;
            RoomName = enterRoomName.Text;
            Capacity = int.Parse(enterCapacity.Text);
            Details = enterDetails.Text;
            //get type from rad buttons

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

        private void Handle_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            string name = rb.Name;
            switch(name)
            {
                case "Lab":
                    type = RoomType.Lab;
                    break;
                case "Lecture":
                    type = RoomType.Lecture;
                    break;
                case "Conference":
                    type = RoomType.Conference;
                    break;
                case "ITIN":
                    type = RoomType.Itin;
                    break;
                case "CYBER":
                    type = RoomType.Cyber;
                    break;
                default:    //This shouldn't occur
                    break;
            }
        }

    }
}
