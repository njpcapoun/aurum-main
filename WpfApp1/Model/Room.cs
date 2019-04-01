using ClassroomAssignment.Model.Utils;
using ClassroomAssignment.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static ClassroomAssignment.Model.DataConstants;

namespace ClassroomAssignment.Model
{

    public class RoomType
    {
        public const string Lab = "Lab";
        public const string Lecture = "Lecture";
        public const string Conference = "Conference";
        public const string Itin = "ITIN";
        public const string Cyber = "CYBER";
    }

    [Serializable]
    public class Room
    {
        /// <summary>
        /// Getter and setter for RoomName and Capacity of the room.
        /// </summary>
        public int Index { get; set; }
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public string Details { get; set; }
        public string RoomType { get; set; }

        public override bool Equals(object obj)
        {
            // I want to make this just check room name if possible
            var room = obj as Room;
            var result = room != null &&
                   RoomName == room.RoomName &&
                   Capacity == room.Capacity; //&&
                   //Details.Equals(room.Details) == true;
                    //&& RoomType.Equals(room.RoomType) == true;
            return result;
        }

        public override int GetHashCode()
        {
            var hashCode = 1430268434;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RoomName);
            hashCode = hashCode * -1521134295 + Capacity.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return RoomName; // RoomNumber.
        }

        public static bool operator ==(Room room1, Room room2)
        {
            return EqualityComparer<Room>.Default.Equals(room1, room2);
        }

        public static bool operator !=(Room room1, Room room2)
        {
            return !(room1 == room2);
        }
    }
}
