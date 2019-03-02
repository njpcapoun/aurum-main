using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using ClassroomAssignment.Model;
using ClassroomAssignment.Repo;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using ClassroomAssignment.Properties;

namespace ClassroomAssignment
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        protected override void OnExit(ExitEventArgs e)
        {
            Save();
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
           
            base.OnStartup(e);
        }

        void Save()
        {
            SaveFileDialog saveFileDialog2 = new SaveFileDialog();
            saveFileDialog2.Filter = "Assignment File | *.agn";
            var fileName = "";

            if (saveFileDialog2.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog2.FileName;
            }


            try
            {
                List<Course> originalCourses = GetOriginalCourses();
                AppState appState = new AppState(originalCourses, GetUpToDateCourses());

                IFormatter formatter = new BinaryFormatter();
                Stream stream = File.Open(fileName, FileMode.Create, FileAccess.Write);

                formatter.Serialize(stream, appState);
                stream.Close();

            }
            catch (SerializationException a)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + a.Message);
            }

            catch(Exception)
            {

            }

        }

        private List<Course> GetOriginalCourses()
        {
            return System.Windows.Application.Current.Resources["originalCourses"] as List<Course>;
        }

        private List<Course> GetUpToDateCourses()
        {
            return CourseRepository.GetInstance().Courses.OrderBy(x => int.Parse(x.ClassID)).ToList();
        }

    }
}
