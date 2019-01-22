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

namespace ClassroomAssignment.UI
{
    /// <summary>
    /// Interaction logic for AddCourseDialogBox.xaml
    /// </summary>
    public partial class AddCourseDialogBox : Window
    {

        public List<String> SectionOptions { get; } = new List<string>()
        {
            DataConstants.SectionTypeOptions.INDEPENDENT_STUDY,
            DataConstants.SectionTypeOptions.LECTURE,
            DataConstants.SectionTypeOptions.STUDIO
        };

        public List<string> CampusOptions { get; } = new List<string>()
        {
            DataConstants.CampusOptions.UNO
        };

        public List<string> InstructionMethods { get; } = new List<string>()
        {
            DataConstants.InstructionMethods.IN_PERSON,
            DataConstants.InstructionMethods.OFF_CAMPUS
        };
        
        public AddCourseDialogBox()
        {
            InitializeComponent();
            CopyCourse = new Course();
        }

        public Course CopyCourse { get; set; }

        private List<PropertyInfo> propertiesChanged = new List<PropertyInfo>();

       
        private void CopyCourse_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Type type = CopyCourse.GetType();
            PropertyInfo propertyInfo = type.GetProperty(e.PropertyName);
            if (propertyInfo.SetMethod != null)
            {
                propertiesChanged.Add(propertyInfo);
            }
        }

        
    }
}
