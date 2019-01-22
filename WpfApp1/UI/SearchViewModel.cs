using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.Operations;
using ClassroomAssignment.Repo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.ViewModel
{
    class SearchViewModel
    {
        public ObservableCollection<ScheduleSlot> AvailableSlots { get; } = new ObservableCollection<ScheduleSlot>();

        private AvailableRoomSearch roomSearch;

        public SearchViewModel()
        {
            roomSearch = new AvailableRoomSearch(RoomRepository.GetInstance(), CourseRepository.GetInstance());
        }

        public void Search(SearchParameters searchParameters)
        {
            List<ScheduleSlot> slots = roomSearch.ScheduleSlotsAvailable(searchParameters);
            AvailableSlots.Clear();

            foreach (var slot in slots)
            {
                AvailableSlots.Add(slot);
            }
        }
    }
}
