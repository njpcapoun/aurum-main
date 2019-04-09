using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.UI.Reassignment
{
    public class LinkedReassignments : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int steps { get; set; }
        public string courseSteps { get; set; }
        public string roomSteps { get; set; }

        public LinkedReassignments next { get; set; }

       
    }
}
