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
	/// <summary>
	/// The type of room (Lab, Lecture, Conference, ITIN, CYBER, Distance).
	/// </summary>
    public class RoomType
    {
        public const string Lab = "Lab";
        public const string Lecture = "Lecture";
        public const string Conference = "Conference";
        public const string Itin = "ITIN";
        public const string Cyber = "CYBER";
        public const string Distance = "Distance";
    }

	/// <summary>
	/// A classroom represented as an object model.
	/// </summary>
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

		/// <summary>
		/// Equals method for a room.
		/// </summary>
		/// <param name="obj">A room object.</param>
		/// <returns>True if all parameters match for passed room. False otherwise.</returns>
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

		/// <summary>
		/// Getter for room's hashcode.
		/// </summary>
		/// <returns>hashCode</returns>
        public override int GetHashCode()
        {
            var hashCode = 1430268434;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RoomName);
            hashCode = hashCode * -1521134295 + Capacity.GetHashCode();
            return hashCode;
        }

		/// <summary>
		/// String representing the room object.
		/// </summary>
		/// <returns>A string that represents the room object.</returns>
		public override string ToString()
        {
            return RoomName; // RoomNumber.
        }

		/// <summary>
		/// Checks of the two rooms are equal to each other.
		/// </summary>
		/// <param name="course1">The first room object.</param>
		/// <param name="course2">The second room object.</param>
		/// <returns>True of the two passed rooms are equal. False otherwise</returns>
		public static bool operator ==(Room room1, Room room2)
        {
            return EqualityComparer<Room>.Default.Equals(room1, room2);
        }

		/// <summary>
		/// Checks of the two rooms are not equal to each other.
		/// </summary>
		/// <param name="course1">The first room object.</param>
		/// <param name="course2">The second room object.</param>
		/// <returns>True of the two passed rooms are not equal. False otherwise</returns>
		public static bool operator !=(Room room1, Room room2)
        {
            return !(room1 == room2);
        }
    }
}
