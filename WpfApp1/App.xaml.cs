using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using ClassroomAssignment.Model;
using ClassroomAssignment.Repo;
using ClassroomAssignment.Operations;
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
        /// <summary>
        /// Auto-save work on exit.
        /// </summary>
        /// <param name="e">Contains the arguments for the Exit event.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            SaveBase saveWork = new SaveBase();
            saveWork.SaveWork();
            base.OnExit(e);
        }

        /// <summary>
        /// Load the app.
        /// </summary>
        /// <param name="e">Contains the arguments for the Startup event.</param>
        protected override void OnStartup(StartupEventArgs e)
        {

            base.OnStartup(e);
        }

    }
}
