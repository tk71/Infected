using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using InfectedLibrary.Models;
using InfectedLibrary.Data;
using System.Text;

namespace InfectedLibrary
{
    public class Scenario
    {
        public string Messages { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Building> Buildings { get; set; }
        public List<Log> Logs { get; set; }

        public Scenario()
        {
            Buildings = new List<Building>();
            Logs = new List<Log>();
        }

        public bool RunScenario()
        {
            Messages = string.Empty;
            var messages = new StringBuilder();
            Logs = new List<Log>();

            // make sure there is at least one building for the scenario; if not, use the default scenario
            if (Buildings.Count == 0)
            {
                messages.Append("Building count was zero. Default scenario was used." + Environment.NewLine);
                Buildings = Defaults.Scenario().Buildings;
            }

            // build the screnario
            BuildScenario();
            
            // if no start date, default to today
            if (StartDate == DateTime.MinValue)
            {
                StartDate = DateTime.Now;
                messages.Append("Start date was missing. " + StartDate.ToString("MM-dd-yyyy") + " was used." + Environment.NewLine);
            }

            // if no end date or end date is less than start date, default to 4 months in the future
            if (EndDate == DateTime.MinValue || EndDate.Date < StartDate.Date)
            {
                EndDate = StartDate.AddMonths(4);
                messages.Append("End date was missing or less than start date. " + EndDate.ToString("MM-dd-yyyy") + " was used." + Environment.NewLine);
            }

            // loop through days; Mon-Fri are workdays
            while (StartDate.Date != EndDate.Date)
            {
                if (StartDate.DayOfWeek == DayOfWeek.Saturday || StartDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    StartDate = StartDate.AddDays(1);
                    continue;
                }

                // start workday time at 8a
                var timeOfDay = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 8, 0, 0).TimeOfDay;
                
                while (timeOfDay.Hours < 16)
                {

                    // everyone has to make a log entry
                    MakeLogEntries(StartDate, timeOfDay);

                    timeOfDay += TimeSpan.FromHours(1);
                }

                StartDate = StartDate.AddDays(1);
            }

            // gather logs
            GatherLogs();

            Messages = messages.ToString();
            Debug.WriteLine(Messages);
            return true;
        }

        private void BuildScenario()
        {
            var patientZero = false;
            var buildingNumber = 1;
            foreach (var building in Buildings)
            {
                building.BuildingNumber = buildingNumber;

                var floorNumber = 1;
                foreach (var floor in building.Floors)
                {
                    floor.FloorNumber = floorNumber;

                    // create office rooms
                    var roomNumber = 1;
                    for (int i = 0; i < floor.OfficeRooms; i++)
                    {
                        floor.Rooms.Add(new Room()
                        {
                            RoomNumber = int.Parse((floorNumber * 100 + roomNumber).ToString()),
                            RoomType = RoomType.office
                        });
                        roomNumber++;
                    }

                    // create people and assign them to an office
                    var people = 0;
                    var roomIndex = 0;
                    while (people < floor.PeopleAssigned)
                    {
                        var person = PersonGenerator.NewPerson();
                        person.AssignedBuilding = building.BuildingNumber;
                        person.AssignedFloor = floor.FloorNumber;
                        person.AssignedRoom = floor.Rooms[roomIndex].RoomNumber;
                        if (!patientZero)
                        {
                            person.Status = InfectionState.Infected;
                            patientZero = true;
                        }
                        floor.Rooms[roomIndex].People.Add(person);

                        people++;
                        roomIndex++;
                        if (roomIndex == floor.Rooms.Count) roomIndex = 0;
                    }

                    // create meeting rooms
                    for (int i = 0; i < floor.MeetingRooms; i++)
                    {
                        floor.Rooms.Add(new Room()
                        {
                            RoomNumber = int.Parse((floorNumber * 100 + roomNumber).ToString()),
                            RoomType = RoomType.meeting
                        });
                        roomNumber++;
                    }

                    // create breakrooms
                    for (int i = 0; i < floor.Breakrooms; i++)
                    {
                        floor.Rooms.Add(new Room()
                        {
                            RoomNumber = int.Parse((floorNumber * 100 + roomNumber).ToString()),
                            RoomType = RoomType.breakroom
                        });
                        roomNumber++;
                    }

                    floorNumber++;
                }

                buildingNumber++;
            }
        }

        private void MakeLogEntries(DateTime dateTime, TimeSpan timeOfDay)
        {
            // from each person still in a building
            foreach (var building in Buildings)
            {
                foreach (var floor in building.Floors)
                {
                    foreach (var room in floor.Rooms)
                    {
                        foreach (var person in room.People)
                        {
                            person.LogEntry(dateTime, timeOfDay, building.BuildingNumber, floor.FloorNumber, room);
                        }
                    }
                }
            }

            // from anyone in the hospital
            Hospital.Rooms.ForEach(room => room.People.ForEach(person => person.LogEntry(dateTime, timeOfDay, 0, 0, room)));
        }

        private void GatherLogs()
        {
            // from each person still in a building
            foreach (var building in Buildings)
            {
                foreach (var floor in building.Floors)
                {
                    foreach (var room in floor.Rooms)
                    {
                        foreach (var person in room.People)
                        {
                            Logs.AddRange(person.Log);
                        }
                    }
                }
            }

            // from anyone in the hospital
            Hospital.Rooms.ForEach(room => room.People.ForEach(person => Logs.AddRange(person.Log)));
        }
    }
}