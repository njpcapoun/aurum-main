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

        public int steps;
        public string courseSteps;
        public string roomSteps;

        public LinkedReassignments next { get; set; }

       
    }
}
