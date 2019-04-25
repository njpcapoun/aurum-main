using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;

namespace ClassroomAssignment.Model.Repo
{
	/// <summary>
	/// Collection of the rooms.
	/// </summary>
    [Serializable]
    public class RoomRepository : IRoomRepository
    {
        private static RoomRepository instance;

        public static Room NoRoom = new Room() { RoomName = "None", Capacity = 0 };

        //Get Rooms
        public List<Room> Rooms { get; private set; }

        /// <summary>
        /// Constructor for RoomRepository. Add allRooms to Rooms list. 
        /// </summary>
        public RoomRepository()
        {
            Rooms = AllRooms(); //adding all rooms to room list.
        }

		/// <summary>
		/// Initialize the room repository. Handle exception if room repo already initilized .
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
        /// Add Rooms to room list with their number, capacity, and details.
        /// And initilize these information into Rooms list.
        /// </summary>
        /// <returns>rooms</returns>
        public List<Room> AllRooms()
        {
            Console.WriteLine("Now Loading Room file...");
            List<Room> rooms = new List<Room>();
            var input = File.ReadAllText("RoomData.json");
            try
            {
                var roomArr = JsonConvert.DeserializeObject<List<Room>>(input);
                for (int x = 0; x < roomArr.Count; x++)
                {
                    rooms.Add(roomArr[x]);
                    Console.WriteLine(roomArr[x].RoomName);
                }
            }
            catch (JsonSerializationException e)
            {
                Console.WriteLine("ERROR: " + e);
            }

            return rooms;
        }

		/// <summary>
		/// Get room name, it is kinda room number.
		/// <example>PKI 153 is roomName.</example>
		/// </summary>
		/// <param name="roomName">The name of the room.</param>
		/// <returns>Rooms.Find(x => x.RoomName == roomName);</returns>
		public Room GetRoomWithName(string roomName)
        {
            return Rooms.Find(x => x.RoomName == roomName);
        }

		/// <summary>
		/// It replaces roomName to acronym format, if it is not.
		/// <example>Peter Kiewit Institude to PKI</example>
		/// </summary>
		/// <param name="roomName">The name of the room.</param>
		/// <returns>roomName.Replace("Peter Kiewit Institute", "PKI")</returns>
		public string GetNormalizedRoomName(string roomName)
        {
            // TODO: Placeholder implementation
            return roomName.Replace("Peter Kiewit Institute", "PKI");
        }

		/// <summary>
		/// Used to add a new room, triggered from "Add Room" button on "Edit Rooms" screen
		/// </summary>
		/// <param name="index">ID of the room</param>
		/// <param name="roomName">Name of the room</param>
		/// <param name="capacity">Capacity of the room</param>
		/// <param name="details">Details of the room</param>
		/// <param name="roomType">Type of room (lab, conference, distance, lecture, ITIN, CYBR).</param>
		/// <returns>False to prompt error for repeat room name; True for unique.</returns>
		public bool AddNewRoom(int index, string roomName, int capacity, string details, string roomType)
        {
            bool isNewRoom = true;

            Room newRoom = new Room() {
                Index = index,
                RoomName = roomName,
                Capacity = capacity,
                Details = details,
                RoomType = roomType
            };

            foreach (Room checkRoom in Rooms) {
                if (Room.Equals(checkRoom, newRoom))
                {   // Don't want to allow rooms of the same name
                    isNewRoom = false;
                }
            }
            if(isNewRoom)
            {  // Only add if a unique room name
                int newIndex = Rooms.Last().Index + 1;
                Rooms.Add(new Room() { Index = newIndex, RoomName = roomName, Capacity = capacity, Details = details , RoomType = roomType});
                Rooms.Sort(delegate (Room x, Room y)
                {
                    return x.RoomName.CompareTo(y.RoomName);
                });
                int z = 0;
                foreach ( Room room in Rooms)
                {
                    room.Index = z;
                    z++;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

		/// <summary>
		/// Used to remove a room from the "Edit Rooms" screen.
		/// </summary>
		/// <param name="room">A Room object</param>
		/// <returns>A bool for a notification on UI.</returns>
		public bool RemoveRoom(Room room)
        {
            return Rooms.Remove(room);
        }

		/// <summary>
		/// Save data to RoomData.json file.
		/// </summary>
        public void SaveData()
        {
            var output = JsonConvert.SerializeObject(Rooms);
            File.WriteAllText("RoomData.json", output);
        }
    }
}
