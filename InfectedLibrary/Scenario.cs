using System;
using System.Collections.Generic;
using System.Diagnostics;
using InfectedLibrary.Models;
using InfectedLibrary.Data;
using System.Text;

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

        public bool RunScenario()
        {
            var messages = new StringBuilder();
            var rnd = new Random(Guid.NewGuid().GetHashCode());

            Messages = string.Empty;
            Logs.Clear();
            _employee.Clear();
            
            // make sure there is at least one building for the scenario; if not, use the default scenario
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
                    // check for lunch
                    //
                    // migrate people to meeting rooms or breakrooms
                    //
                    // check to see whether anyone becomes infected
                    //

                    // record logs
                    RecordLogEntries(StartDate, timeOfDay);
                    
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

        private void BuildScenario()
        {
            var employees = new HashSet<string>();
            var patientZero = false;
            
            var floorNumber = 1;
            foreach (var floor in Floors)
            {
                // create a variable to hold office numbers and personnel allocations
                var offices = new List<Tuple<string, int>>();
                for (int i = 0; i < floor.OfficeRooms; i++)
                {
                    offices.Add(new Tuple<string, int>((floorNumber * 100 + i).ToString(), 0));
                }

                // create people and assign them to an office
                var people = 0;
                var roomIndex = 0;
                while (people < floor.PeopleAssigned)
                {
                    var person = PersonGenerator.NewPerson();
                    if (!employees.Add(person.Id)) continue;

                    person.AssignedRoom = offices[roomIndex].Item1;
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
    }
}