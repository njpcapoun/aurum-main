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
	/// <summary>
	/// View model for the available room search.
	/// </summary>
    class SearchViewModel
    {
        public ObservableCollection<ScheduleSlot> AvailableSlots { get; } = new ObservableCollection<ScheduleSlot>();

        private AvailableRoomSearch roomSearch;

		/// <summary>
		/// Constructor for SearchViewModel. Get instances of room and course repositories.
		/// </summary>
        public SearchViewModel()
        {
            roomSearch = new AvailableRoomSearch(RoomRepository.GetInstance(), CourseRepository.GetInstance());
        }

		/// <summary>
		/// Search for available slots for a room.
		/// </summary>
		/// <param name="searchParameters">The search parameters for a course to fina n available room.</param>
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
