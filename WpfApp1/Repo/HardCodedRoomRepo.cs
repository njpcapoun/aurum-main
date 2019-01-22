using ClassroomAssignment.Model;
using ClassroomAssignment.Model.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Repo
{
    public class HardCodedRoomRepo : IRoomRepository
    {
       
        public List<Room> Rooms { get; }
        public string term = "Spring 2018";
        


        public HardCodedRoomRepo()
        {
            this.Rooms = new List<Room>();

            Room myRooms = new Room();

            myRooms.roomName = "PKI 153";
            myRooms.maxCapcity = 40;
            Rooms.Add(myRooms);

            

            myRooms.roomName = "PKI 155";
            myRooms.maxCapcity = 45;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 157";
            myRooms.maxCapcity = 24;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 160";
            myRooms.maxCapcity = 44;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 161";
            myRooms.maxCapcity = 30;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 164";
            myRooms.maxCapcity = 56;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 252";
            myRooms.maxCapcity = 58;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 256";
            myRooms.maxCapcity = 40;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 259";
            myRooms.maxCapcity = 20;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 260";
            myRooms.maxCapcity = 40;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 261";
            myRooms.maxCapcity = 56;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 263";
            myRooms.maxCapcity = 48;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 269";
            myRooms.maxCapcity = 30;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 270";
            myRooms.maxCapcity = 16;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 274";
            myRooms.maxCapcity = 30;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 276";
            myRooms.maxCapcity = 35;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 278";
            myRooms.maxCapcity = 35;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 279";
            myRooms.maxCapcity = 30;
            Rooms.Add(myRooms);

            myRooms.roomName = "PKI 361";
            myRooms.maxCapcity = 35;
            Rooms.Add(myRooms);
        }

      

        public string GetNormalizedRoomName(string roomName)
        {
            
            throw new NotImplementedException();
        }
    }
}
