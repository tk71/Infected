using System;
using System.Collections.Generic;
using System.Diagnostics;
using InfectedLibrary.Models;
using InfectedLibrary.Data;
using System.Text;
using System.Linq;

namespace InfectedLibrary
{
    public class Scenario
    {
        public List<Floor> Floors { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Messages { get; set; }
        public List<Log> Logs { get; set; }

        private List<Person> _employee { get; set; }

        public Scenario()
        {
            Floors = new List<Floor>();
            Logs = new List<Log>();
            _employee = new List<Person>();
        }

        private void BuildScenario()
        {
            var employees = new HashSet<string>();
            var patientZero = false;
            
            var floorNumber = 1;
            foreach (var floor in Floors)
            {
                // create a variable to hold office numbers and personnel allocations
                var offices = new List<string>();
                for (int i = 0; i < floor.OfficeRooms; i++)
                {
                    offices.Add((floorNumber * 100 + i).ToString());
                }

                // create people and assign them to an office
                var people = 0;
                var roomIndex = 0;
                while (people < floor.PeopleAssigned)
                {
                    var person = PersonGenerator.NewPerson();
                    if (!employees.Add(person.Id)) continue;

                    person.AssignedRoom = offices[roomIndex];
                    person.CurrentRoom = person.AssignedRoom;
                    person.CurrentRoomType = RoomType.Office;
                    if (!patientZero)
                    {
                        person.Status = InfectionState.Infected;
                        patientZero = true;
                    }
                    _employee.Add(person);

                    people++;
                    roomIndex++;
                    if (roomIndex == offices.Count) roomIndex = 0;
                }

                floorNumber++;
            }
        }

        private void Migration(bool isLunchtime)
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());

            var floorNumber = 1;
            foreach (var floor in Floors)
            {
                if (isLunchtime)
                {
                    var rooms = new List<string>();

                    float breakroomUsage = 0.0f;
                    for (int i = 0; i < floor.Breakrooms; i++)
                    {
                        rooms.Add("Breakroom " + floorNumber + ((char)(i + 65)).ToString());
                        breakroomUsage += floor.PeopleAssigned;
                    }
                    breakroomUsage *= .25f; // 25% of employees will use breakrooms

                    var people = 0;
                    var roomIndex = 0;
                    while (people < breakroomUsage)
                    {
                        var person = _employee[rnd.Next(0, _employee.Count)];
                        if (person.CurrentRoomType == RoomType.Breakroom || person.CurrentRoomType == RoomType.Hospital || 
                            person.CurrentRoomType == RoomType.Testing) continue;

                        person.CurrentRoom = rooms[roomIndex];
                        person.CurrentRoomType = RoomType.Breakroom;

                        people++;
                        roomIndex++;
                        if (roomIndex == rooms.Count) roomIndex = 0;
                    }
                }
                else
                {
                    var rooms = new List<Tuple<string, int>>();
                    for (int i = 0; i < floor.MeetingRooms; i++)
                    {
                        rooms.Add(new Tuple<string, int>("Meeting Room " + floorNumber + ((char)(i + 65)).ToString(), rnd.Next(4, 9)));
                    }

                    foreach (var room in rooms)
                    {
                        var people = 0;
                        while (people < room.Item2)
                        {
                            var person = _employee[rnd.Next(0, _employee.Count)];
                            if (person.CurrentRoomType == RoomType.Meeting || person.CurrentRoomType == RoomType.Hospital ||
                                person.CurrentRoomType == RoomType.Testing) continue;

                            person.CurrentRoom = room.Item1;
                            person.CurrentRoomType = RoomType.Meeting;

                            people++;
                        }
                    }
                }

                floorNumber++;
            }
        }

        private void RecordLogEntries(DateTime dateTime, TimeSpan timeOfDay)
        {
            foreach (var person in _employee)
            {
                // people in the hospital or in-testing do not need to make a log entry; if lunch only people in break rooms need to make a log entry
                if (person.CurrentRoomType == RoomType.Hospital || person.CurrentRoomType == RoomType.Testing || 
                    (timeOfDay.Hours == 12 && person.CurrentRoomType != RoomType.Breakroom)) continue;

                string roomType;
                switch (person.CurrentRoomType)
                {
                    case RoomType.Breakroom:
                        roomType = "Breakroom";
                        break;
                    case RoomType.Hospital:
                        roomType = "Hospital";
                        break;
                    case RoomType.Meeting:
                        roomType = "Meeting";
                        break;
                    case RoomType.Office:
                        roomType = "Office";
                        break;
                    case RoomType.Testing:
                        roomType = "Testing";
                        break;
                    default:
                        roomType = string.Empty;
                        break;
                }

                string status;
                switch (person.Status)
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

                var log = new Log()
                {
                    Created = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, timeOfDay.Hours, 0, 0),
                    Id = person.Id,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    Sex = person.Sex, 
                    CurrentRoom = person.CurrentRoom,
                    CurrentRoomType = roomType,
                    Status = status
                };

                // contacts will be employees in the same room
                foreach (var contact in _employee)
                {
                    if (person.Id != contact.Id && person.CurrentRoom == contact.CurrentRoom) log.Contacts.Add(new Contact()
                    {
                        Id = contact.Id, 
                        FirstName = contact.FirstName, 
                        LastName = contact.LastName
                    });
                }

                Logs.Add(log);
            }
        }

        public bool RunScenario()
        {
            var messages = new StringBuilder();
            
            Messages = string.Empty;
            Logs.Clear();
            _employee.Clear();

            // make sure there is at least one floor for the scenario; if not, use the default scenario
            if (Floors.Count == 0)
            {
                messages.Append("Floor count was zero. Default scenario was used." + Environment.NewLine);
                Floors = Defaults.Floors();
            }

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

            // build the screnario
            BuildScenario();

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

                // if anyone is symptomatic, check to see if they decide to get tested
                //

                // if anyone was being test, check to see if they go back to work or into hospital
                //

                // anyone that has been in the hospital 5 days can go back to work and is considered immune
                //

                // this loop is where the work day processing happens
                while (timeOfDay.Hours < 17)
                {
                    // migrate people to breakrooms or meeting rooms
                    Migration(timeOfDay.Hours == 12);

                    // check to see whether anyone becomes infected
                    //

                    // record logs
                    RecordLogEntries(StartDate, timeOfDay);

                    // put migrated people back in thier offices
                    _employee.Where(item => item.CurrentRoomType == RoomType.Breakroom || item.CurrentRoomType == RoomType.Meeting).ToList().
                        ForEach(item =>
                        {
                            item.CurrentRoom = item.AssignedRoom;
                            item.CurrentRoomType = RoomType.Office;
                        });

                    timeOfDay += TimeSpan.FromHours(1);
                }

                // anyone in the hospital 1-4 days gets credit for a day
                //

                StartDate = StartDate.AddDays(1);
            }

            Messages = messages.ToString();
            Debug.WriteLine(Messages);
            return true;
        }
    }
}