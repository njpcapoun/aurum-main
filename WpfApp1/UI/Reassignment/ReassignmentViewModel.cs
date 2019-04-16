using ClassroomAssignment.Model;
using ClassroomAssignment.UI.Reassignment;
using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.Operations;
using ClassroomAssignment.Repo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ClassroomAssignment.Model.Course;
using static ClassroomAssignment.Extension.CourseExtensions;
using System.Windows.Input;

namespace ClassroomAssignment.UI.Reassignment 
{
    
    public class ReassignmentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<LinkedReassignments> ReassignPath { get; } = new ObservableCollection<LinkedReassignments>();
      
        public ReassignmentViewModel(LinkedReassignments node)
        {
            LinkedReassignments traverser = node;
            ReassignPath.Add(traverser);

            while (traverser.next != null)
            {
                traverser = traverser.next;
                ReassignPath.Add(traverser);
            }
        }
    }
}
