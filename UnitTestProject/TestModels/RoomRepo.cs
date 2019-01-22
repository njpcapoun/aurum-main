using ClassroomAssignment.Model;
using ClassroomAssignment.Model.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.TestModels
{
    public class RoomRepo : IRoomRepository 
    {
        public List<Room> Rooms { get; }

        public RoomRepo()
        {
            Rooms = new List<Room>();


            Room room1 = new Room();

            room1.RoomName = "PKI 157";
            room1.Capacity = 40;
            Rooms.Add(room1);

            Room room2 = new Room();
            room2.RoomName = "PKI 158";
            room2.Capacity = 56;
            Rooms.Add(room2);
        }

        public string GetNormalizedRoomName(string roomName)
        {
            throw new NotImplementedException();
        }

        public Room GetRoomWithName(string roomName)
        {
            return Rooms.Find(x => x.RoomName == roomName);
        }
    }

    
}
