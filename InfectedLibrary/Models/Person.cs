using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using InfectedLibrary.Models;

namespace InfectedLibrary.Models
{
    public class Person
    {
        private InfectionState status;

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public int AssignedBuilding { get; set; }
        public int AssignedFloor { get; set; }
        public int AssignedRoom { get; set; }
        public float InfectionRate { get; set; }
        public int DaysSick { get; set; }
        public int InfectionTime { get; set; }
        public int IncubationTime { get; set; }
        public int SymptomaticTime { get; set; }
        public List<Log> Log { get; set; }
        public InfectionState Status { 
            get { return status; }
            set {
                status = value;

                Random rnd;
                switch (status)
                {
                    case InfectionState.Well:
                        InfectionRate = 0.02f;
                        break;
                    case InfectionState.Infected:
                        DaysSick = 1;
                        InfectionTime = 3;
                        break;
                    case InfectionState.Incubation:
                        rnd = new Random(Guid.NewGuid().GetHashCode());
                        IncubationTime = rnd.Next(3, 7);
                        break;
                    case InfectionState.Symptomatic:
                        rnd = new Random(Guid.NewGuid().GetHashCode());
                        SymptomaticTime = rnd.Next(6, 11);
                        break;
                    case InfectionState.Immune:
                        break;
                    default:
                        break;
                }
            } 
        }
        
        public Person()
        {
            Log = new List<Log>();
        }

        public void LogEntry(DateTime dateTime, TimeSpan timeOfDay, int buildingNumber, int floorNumber, Room room)
        {
            string roomType;
            switch (room.RoomType)
            {
                case RoomType.office:
                    roomType = "Office";
                    break;
                case RoomType.breakroom:
                    roomType = "Breakroom";
                    break;
                case RoomType.meeting:
                    roomType = "Meeting";
                    break;
                case RoomType.hospital:
                    roomType = "Hospital";
                    break;
                default:
                    roomType = string.Empty;
                    break;
            }

            string status;
            switch (Status)
            {
                case InfectionState.Well:
                    status = "Well";
                    break;
                case InfectionState.Infected:
                    status = "Infected";
                    break;
                case InfectionState.Incubation:
                    status = "Incubation";
                    break;
                case InfectionState.Symptomatic:
                    status = "Symptomatic";
                    break;
                case InfectionState.Immune:
                    status = "Immune";
                    break;
                default:
                    status = string.Empty;
                    break;
            }

            var contacts = new List<Contact>();
            foreach (var contact in room.People)
            {
                if (contact.Id != Id) contacts.Add(new Contact() {
                    Id = contact.Id,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName
                });
            }

            Log.Add(new Log()
            {
                Created = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, timeOfDay.Hours, 0, 0),
                Id = Id,
                FirstName = FirstName,
                LastName = LastName,
                Building = buildingNumber,
                Floor = floorNumber,
                Room = room.RoomNumber,
                RoomType = roomType,
                Status = status,
                Contacts = contacts
            });
        }
    }
}
