using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Model.Repo
{
    /// <summary>
    /// Get rooms list and room with their name.
    /// </summary>
    public interface IRoomRepository
    {
        string GetNormalizedRoomName(string roomName);
        List<Room> Rooms { get; } 
        Room GetRoomWithName(string roomName); //Room Name
    }
}
