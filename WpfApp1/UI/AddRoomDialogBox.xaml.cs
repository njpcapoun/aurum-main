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
            Room newRoom = new Room();
            //newRoom.RoomName = RoomName;
            
        }
    }
}
