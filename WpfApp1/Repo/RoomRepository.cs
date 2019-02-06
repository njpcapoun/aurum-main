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
    [Serializable]
    public class RoomRepository : IRoomRepository
    {
        private static RoomRepository instance;

        public static Room NoRoom = new Room() { RoomName = "None", Capacity = 0 };

        //Get Rooms
        public List<Room> Rooms { get; private set; }

        /// <summary>
        /// Add allRooms to Rooms list. 
        /// </summary>
        public RoomRepository()
        {
            Rooms = AllRooms(); //adding all rooms to room list.
        }
        /*  
         *  <summary>
         *   Exception Handler for that room repo already initilized .
         *  </summary> 
         */

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
        /// Its add Rooms to room list with their number, capacity, and details.
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

        // Used to add a new room, triggered from "Add Room" button on "Edit Rooms" screen
        public void AddNewRoom(string roomName, int capacity, string details)
        {
            Rooms.Add(new Room() { RoomName = roomName, Capacity = capacity, Details = details });
        }
    }

    public class RoomList
    {
        public List<Room> Data { get; set; }

        public List<Room> getData()
        {
            return this.Data;
        }
    }
}
