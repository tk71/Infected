using System;
using System.Collections.Generic;
using System.Text;

namespace InfectedLibrary.Models
{
    internal static class Hospital
    {
        public static List<Room> Rooms;

        static Hospital()
        {
            Refresh();
        }

        public static void Refresh()
        {
            Rooms = new List<Room>
            {
                new Room()
                {
                    RoomNumber = 1,
                    RoomType = RoomType.hospital
                },
                new Room()
                {
                    RoomNumber = 2,
                    RoomType = RoomType.hospital
                },
                new Room()
                {
                    RoomNumber = 3,
                    RoomType = RoomType.hospital
                },
                new Room()
                {
                    RoomNumber = 4,
                    RoomType = RoomType.hospital
                },
                new Room()
                {
                    RoomNumber = 5,
                    RoomType = RoomType.hospital
                }
            };
        }
    }
}
