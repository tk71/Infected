using System;
using System.Collections.Generic;
using System.Text;

namespace InfectedLibrary.Models
{
    public class Floor
    {
        public int FloorNumber { get; set; }
        public int PeopleAssigned { get; set; }
        public int OfficeRooms { get; set; }
        public int Breakrooms { get; set; }
        public int MeetingRooms { get; set; }
        public List<Room> Rooms { get; set; }

        public Floor()
        {
            Rooms = new List<Room>();
        }
    }
}
