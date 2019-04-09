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

namespace ClassroomAssignment.UI.Reassignment
{
    /// <summary>
    /// Interaction logic for ReassignmentPopUp.xaml
    /// </summary>
    public partial class ReassignmentPopUp : Window
    {
        // Add a list of strings to use for the combobox itemsource
        Course C;
        ReassignmentPage reassignmentPage;
        public List<string> types;
        public ReassignmentPopUp(Course c)
        {
            C = c;
            InitializeComponent();
            types = new List<string> {"Lab", "Lecture", "Conference", "ITIN", "CYBER"};
            DataContext = this;
            TypeBox.ItemsSource = types;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string capacity = roomCap.Text;
            if (int.TryParse(capacity, out int i) == false)
            {
                System.Windows.MessageBox.Show("Invalid capacity");
            }
            else
            {
                string type = TypeBox.SelectedItem.ToString();
                reassignmentPage = new ReassignmentPage(C, capacity, type);
                this.Close();
            }
        }

        public ReassignmentPage getRP()
        {
            return reassignmentPage; 
        }

        public void Cancel_Click(object sender, RoutedEventArgs e)
        {
            reassignmentPage = null;
            this.Close();
        }
    }
}
