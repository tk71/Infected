using System;
using System.Collections.Generic;
using System.Text;

namespace InfectedLibrary.Models
{
    public class Room
    {
        public int RoomNumber { get; set; }
        public RoomType RoomType { get; set; }
        public List<Person> People { get; set; }

        public Room()
        {
            People = new List<Person>();
        }
    }
}
