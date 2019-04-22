using ClassroomAssignment.Model;
using ClassroomAssignment.Repo;
using ClassroomAssignment.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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

namespace ClassroomAssignment.Operations
{
    class SaveBase
    {
        // Save as code. Always brings up the dialog box then saves the chosen file as where the autosave should go
        public void SaveAs()
        {
            if (GetUpToDateCourses() != null)
            {
                SaveFileDialog saveFileDialog2 = new SaveFileDialog();
                saveFileDialog2.Filter = "Assignment File | *.agn";

                if (saveFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    var fileName = saveFileDialog2.FileName;
                    Properties.Settings.Default["SavePath"] = fileName;
                    Properties.Settings.Default.Save();

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
                }

            }

        }


        // Same as save as, but checks if theres a path to autosave to
        public void SaveWork()
        {
            if (GetUpToDateCourses() != null) {
                SaveFileDialog saveFileDialog2 = new SaveFileDialog();
                saveFileDialog2.Filter = "Assignment File | *.agn";
                var fileName = "";

                if (Properties.Settings.Default["SavePath"] != null || (string)Properties.Settings.Default["SavePath"] != "default")
                {
                    fileName = (string)Properties.Settings.Default["SavePath"];
                }
                else if (saveFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    fileName = saveFileDialog2.FileName;
                    Properties.Settings.Default["SavePath"] = fileName;
                    Properties.Settings.Default.Save();
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
            }
        }

        private List<Course> GetOriginalCourses()
        {
            try
            {
                return System.Windows.Application.Current.Resources["originalCourses"] as List<Course>;
            }

            catch(Exception e)
            {
                return null;
            }
        }

        private List<Course> GetUpToDateCourses()
        {
            try
            {
                return CourseRepository.GetInstance().Courses.OrderBy(x => int.Parse(x.ClassID)).ToList();
            }

            catch(Exception e)
            {
                return null;
            }
        }

    }
}

