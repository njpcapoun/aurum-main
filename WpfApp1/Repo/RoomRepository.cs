using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ClassroomAssignment.Model.Repo
{
    [Serializable]
    public class RoomRepository : IRoomRepository
    {
        private static RoomRepository instance;

        public static Room NoRoom = new Room() { RoomName = "None", Capacity = 0 };

        private List<Room> _rooms;
        //Get Rooms
        public List<Room> Rooms { get; private set; }
        /// <summary>
        /// Add allRooms to Rooms list. 
        /// </summary>
        public RoomRepository()
        {

            Rooms = AllRooms(); //adding all rooms to room list.
        }
        /// <summary>
        /// Exception Handler for that room repo already initilized .
        /// </summary>
        public static void InitInstance()
        {
            if (instance != null) throw new InvalidOperationException("Room Repo already initialized");
            instance = new RoomRepository();
        }
        /// <summary>
        /// Exception handler for Room repo not yet initialized.
        /// </summary>
        /// <returns>instance</returns>
        public static RoomRepository GetInstance()
        {
            return instance ?? throw new InvalidOperationException("Room Repo not yet intialized");
           
        }    
        /// <summary>
        /// Its add Rooms to room list with their number and capacity.
        /// And initilize these information into Rooms list.
        /// </summary>
        /// <returns>rooms</returns>
        public List<Room> AllRooms()
        {
            List<Room> rooms = new List<Room>();

            rooms.Add(new Room() { RoomName = "PKI 153", Capacity = 40 });
            rooms.Add(new Room() { RoomName = "PKI 155", Capacity = 45 });
            rooms.Add(new Room() { RoomName = "PKI 157", Capacity = 24 });
            rooms.Add(new Room() { RoomName = "PKI 160", Capacity = 44 });
            rooms.Add(new Room() { RoomName = "PKI 161", Capacity = 30 });
            rooms.Add(new Room() { RoomName = "PKI 164", Capacity = 56 });
            rooms.Add(new Room() { RoomName = "PKI 252", Capacity = 58 });
            rooms.Add(new Room() { RoomName = "PKI 256", Capacity = 40 });
            rooms.Add(new Room() { RoomName = "PKI 259", Capacity = 20 });
            rooms.Add(new Room() { RoomName = "PKI 260", Capacity = 40 });
            rooms.Add(new Room() { RoomName = "PKI 261", Capacity = 24 });
            rooms.Add(new Room() { RoomName = "PKI 263", Capacity = 48 });
            rooms.Add(new Room() { RoomName = "PKI 269", Capacity = 30 });
            rooms.Add(new Room() { RoomName = "PKI 270", Capacity = 16 });
            rooms.Add(new Room() { RoomName = "PKI 274", Capacity = 30 });
            rooms.Add(new Room() { RoomName = "PKI 276", Capacity = 35 });
            rooms.Add(new Room() { RoomName = "PKI 278", Capacity = 35 });
            rooms.Add(new Room() { RoomName = "PKI 279", Capacity = 30 });
            rooms.Add(new Room() { RoomName = "PKI 361", Capacity = 35 });


            return rooms;
        }
        /// <summary>
        /// Get room name, it is kinda room number.
        /// <example>PKI 153 is roomName.</example>
        /// </summary>
        /// <param name="roomName"></param>
        /// <returns>Rooms with their name</returns>
        public Room GetRoomWithName(string roomName)
        {
            return Rooms.Find(x => x.RoomName == roomName);
        }
        /// <summary>
        /// It replaces roomName to acronym format, if it is not.
        /// <example>Peter Kiewit Institude to PKI</example>
        /// </summary>
        /// <param name="roomName"></param>
        /// <returns>RoomName replace</returns>
        public string GetNormalizedRoomName(string roomName)
        {
            // TODO: Placeholder implementation
            return roomName.Replace("Peter Kiewit Institute", "PKI");
        }

    }
}
