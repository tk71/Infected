using System;
using System.Collections.Generic;
using System.Text;
using InfectedLibrary.Models;

namespace InfectedLibrary
{
    public static class Defaults
    {
        public static Scenario Scenario()
        {
            // scenario default is 1 building with 5 floors
            var scenario = new Scenario();
            var building = new Building();

            int assigned;
            int offices;
            int breakRooms;
            int meetingRooms;

            for (int i = 0; i < 5; i++)
            {
                switch (i)
                {
                    case 0:
                        assigned = 30;
                        offices = 2;
                        breakRooms = 0;
                        meetingRooms = 0;
                        break;
                    case 1:
                        assigned = 150;
                        offices = 12;
                        breakRooms = 1;
                        meetingRooms = 2;
                        break;
                    case 2:
                        assigned = 150;
                        offices = 15;
                        breakRooms = 1;
                        meetingRooms = 2;
                        break;
                    case 3:
                        assigned = 150;
                        offices = 5;
                        breakRooms = 1;
                        meetingRooms = 3;
                        break;
                    default:
                        assigned = 50;
                        offices = 50;
                        breakRooms = 1;
                        meetingRooms = 2;
                        break;
                }

                building.Floors.Add(new Floor()
                {
                    FloorNumber = i + 1,
                    PeopleAssigned = assigned,
                    OfficeRooms = offices,
                    Breakrooms = breakRooms,
                    MeetingRooms = meetingRooms
                });
            }

            scenario.Buildings.Add(building);
            return scenario;
        }
    }
}
