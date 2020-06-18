using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using InfectedLibrary.Data;
using InfectedLibrary.Enums;
using InfectedLibrary.Models;

namespace InfectedLibrary
{
    public class Scenario
    {
        public List<Floor> Floors { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Messages { get; set; }
        public List<Log> Logs { get; set; }

        private List<Employee> _employee { get; set; }
        private const int _lunchtime = 12;

        public Scenario()
        {
            Floors = new List<Floor>();
            Logs = new List<Log>();
            _employee = new List<Employee>();
        }

        private void BuildScenario()
        {
            var employeeIdValidation = new HashSet<string>();
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

                // create employees and assign them to an office
                var employees = 0;
                var roomIndex = 0;
                while (employees < floor.EmployeesAssigned)
                {
                    var employee = EmployeeGenerator.NewEmployee();
                    if (!employeeIdValidation.Add(employee.Id)) continue;

                    employee.AssignedLocation = offices[roomIndex];
                    employee.CurrentLocation = employee.AssignedLocation;
                    employee.Location = Locations.Office;
                    if (!patientZero)
                    {
                        employee.Status = InfectionState.Infected;
                        patientZero = true;
                    }
                    _employee.Add(employee);

                    employees++;
                    roomIndex++;
                    if (roomIndex == offices.Count) roomIndex = 0;
                }

                floorNumber++;
            }
        }

        private void Infection(bool isLunchtime)
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());

            foreach (var employee in _employee)
            {
                // only employees in an office, breakroom or meeting and haver a status of well can be infected
                if (employee.Location == Locations.Hospital || employee.Location == Locations.Testing ||
                    employee.Status != InfectionState.Well) continue;

                if (isLunchtime)
                {
                    // only check breakrooms
                    if (employee.Location == Locations.Breakroom)
                    {
                        _employee.Where(contact => contact.CurrentLocation == employee.CurrentLocation && contact.Infected).ToList().
                            ForEach(contact =>
                            {
                                if (rnd.Next(0, 101) * 0.01f < employee.ChanceOfInfection) employee.Status = InfectionState.Infected;
                            });
                    }
                }
                else
                {
                    _employee.Where(contact => contact.CurrentLocation == employee.CurrentLocation && contact.Infected).ToList().
                        ForEach(contact =>
                        {
                            if (rnd.Next(0, 101) * 0.01f < employee.ChanceOfInfection) employee.Status = InfectionState.Infected;
                        });
                }
            }
        }

        private void Migration(bool isLunchtime)
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());
            var eligibleEmployees = _employee.Where(employee => employee.Location == Locations.Office).ToList();

            var floorNumber = 1;
            foreach (var floor in Floors)
            {
                if (isLunchtime)
                {
                    if (floor.Breakrooms == 0) continue;

                    var rooms = new List<string>();
                    for (int i = 0; i < floor.Breakrooms; i++)
                    {
                        rooms.Add("Breakroom " + floorNumber + ((char)(i + 65)).ToString());
                    }

                    var breakroomUsage = eligibleEmployees.Count * 0.25f;// 25% of eligible employees will use breakrooms

                    var employees = 0;
                    var roomIndex = 0;
                    while (employees < breakroomUsage)
                    {
                        var employee = eligibleEmployees[rnd.Next(0, eligibleEmployees.Count)];
                        if (employee.Location == Locations.Breakroom) continue;

                        employee.CurrentLocation = rooms[roomIndex];
                        employee.Location = Locations.Breakroom;

                        eligibleEmployees = eligibleEmployees.Where(eligible => eligible.Location == Locations.Office).ToList();
                        if (eligibleEmployees.Count == 0) break;

                        employees++;
                        roomIndex++;
                        if (roomIndex == rooms.Count) roomIndex = 0;
                    }
                }
                else
                {
                    if (floor.MeetingRooms == 0) continue;

                    var rooms = new List<Tuple<string, int>>();
                    for (int i = 0; i < floor.MeetingRooms; i++)
                    {
                        rooms.Add(new Tuple<string, int>("Meeting Room " + floorNumber + ((char)(i + 65)).ToString(), rnd.Next(4, 9)));
                    }

                    foreach (var room in rooms)
                    {
                        var employees = 0;
                        while (employees < room.Item2)
                        {
                            var employee = eligibleEmployees[rnd.Next(0, eligibleEmployees.Count)];
                            if (employee.Location == Locations.Meeting) continue;

                            employee.CurrentLocation = room.Item1;
                            employee.Location = Locations.Meeting;

                            eligibleEmployees = eligibleEmployees.Where(eligible => eligible.Location == Locations.Office).ToList();
                            if (eligibleEmployees.Count == 0) break;

                            employees++;
                        }
                    }
                }

                floorNumber++;
            }
        }

        private void RecordLogEntries(DateTime dateTime, TimeSpan timeOfDay)
        {
            foreach (var employee in _employee)
            {
                // employees in the hospital or in-testing do not need to make a log entry; if lunch, only employees in a break room need to make a log entry
                if (employee.Location == Locations.Hospital || employee.Location == Locations.Testing || 
                    (timeOfDay.Hours == _lunchtime && employee.Location != Locations.Breakroom)) continue;

                string locationType;
                switch (employee.Location)
                {
                    case Locations.Breakroom:
                        locationType = "Breakroom";
                        break;
                    case Locations.Hospital:
                        locationType = "Hospital";
                        break;
                    case Locations.Meeting:
                        locationType = "Meeting";
                        break;
                    case Locations.Office:
                        locationType = "Office";
                        break;
                    case Locations.Testing:
                        locationType = "Testing";
                        break;
                    default:
                        locationType = string.Empty;
                        break;
                }

                string status;
                switch (employee.Status)
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
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Sex = employee.Sex,
                    CurrentLocation = employee.CurrentLocation,
                    CurrentLocationType = locationType,
                    Status = status
                };

                // contacts will be employees in the same room
                foreach (var contact in _employee)
                {
                    if (employee.Id != contact.Id && employee.CurrentLocation == contact.CurrentLocation) log.Contacts.Add(new Contact()
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
            var rnd = new Random(Guid.NewGuid().GetHashCode());

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
                // if anyone is symptomatic, check to see if they decide to get tested
                _employee.Where(employee => employee.Location != Locations.Hospital && employee.Location != Locations.Testing && employee.Status == InfectionState.Symptomatic).ToList().
                    ForEach(employee => 
                    {
                        if (rnd.Next(0, 101) * 0.01f <= 0.35f)
                        {
                            employee.Location = Locations.Testing;
                            employee.TreatmentCount = 1;
                        }
                    });

                // if anyone was being tested and is at day 5 of testing, check to see if they go back to work or the hospital
                _employee.Where(employee => employee.Location == Locations.Testing && employee.TreatmentCount == 3).ToList().
                    ForEach(employee =>
                    {
                        if (employee.Status != InfectionState.Immune || employee.Status != InfectionState.Well)
                        {
                            employee.Location = Locations.Hospital;
                            employee.TreatmentCount = 1;
                        }
                        else
                        {
                            employee.Location = Locations.Office;
                        }
                    });

                // if anyone was in the hospital 5 days, they can go back to work and is considered immune
                _employee.Where(employee => employee.Location == Locations.Hospital &&  employee.TreatmentCount == 5).ToList().
                    ForEach(employee =>
                    {
                        employee.CurrentLocation = employee.AssignedLocation;
                        employee.Location = Locations.Office;
                        employee.Status = InfectionState.Immune;
                    });

                // start workday time at 8a
                var timeOfDay = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 8, 0, 0).TimeOfDay;

                if (StartDate.DayOfWeek != DayOfWeek.Saturday && StartDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    // this loop is where the workday processing happens
                    while (timeOfDay.Hours < 17)
                    {
                        // migrate employees to breakrooms or meeting rooms
                        Migration(timeOfDay.Hours == _lunchtime);

                        // check to see whether anyone becomes infected
                        Infection(timeOfDay.Hours == _lunchtime);

                        // record logs
                        RecordLogEntries(StartDate, timeOfDay);

                        var well = _employee.Where(employee => employee.Status == InfectionState.Well).ToList();
                        var infected = _employee.Where(employee => employee.Status == InfectionState.Infected).ToList();
                        var incubating = _employee.Where(employee => employee.Status == InfectionState.Incubation).ToList();
                        var symptomatic = _employee.Where(employee => employee.Status == InfectionState.Symptomatic).ToList();
                        var immune = _employee.Where(employee => employee.Status == InfectionState.Immune).ToList();
                        var office = _employee.Where(employee => employee.Location == Locations.Office).ToList();
                        var breakroom = _employee.Where(employee => employee.Location == Locations.Breakroom).ToList();
                        var meeting = _employee.Where(employee => employee.Location == Locations.Meeting).ToList();
                        var testing = _employee.Where(employee => employee.Location == Locations.Testing).ToList();
                        var hospital = _employee.Where(employee => employee.Location == Locations.Hospital).ToList();
                        Debug.WriteLine(StartDate.ToString("MM-dd-yyyy") + " " + timeOfDay.ToString() + " " +
                            " well: " + well.Count.ToString().PadLeft(3, '0') + " infected: " + infected.Count.ToString().PadLeft(3, '0') +
                            " incubating: " + incubating.Count.ToString().PadLeft(3, '0') + " symptomatic: " + symptomatic.Count.ToString().PadLeft(3, '0') +
                            " immune: " + immune.Count.ToString().PadLeft(3, '0') + " office: " + office.Count.ToString().PadLeft(3, '0') +
                            " breakroom: " + breakroom.Count.ToString().PadLeft(3, '0') + " meeting: " + meeting.Count.ToString().PadLeft(3, '0') +
                            " testing: " + testing.Count.ToString().PadLeft(3, '0') + " hospital: " + hospital.Count.ToString().PadLeft(3, '0'));

                        // put migrated employees back in thier offices
                        _employee.Where(employee => employee.Location == Locations.Breakroom || employee.Location == Locations.Meeting).ToList().
                            ForEach(employee =>
                            {
                                employee.CurrentLocation = employee.AssignedLocation;
                                employee.Location = Locations.Office;
                            });

                        timeOfDay += TimeSpan.FromHours(1);
                    }
                }

                // anyone being tested gets credit for a day
                _employee.Where(employee => employee.Location == Locations.Testing).ToList().
                    ForEach(employee => employee.TreatmentCount++);

                // anyone in the hospital gets credit for a day
                _employee.Where(employee => employee.Location == Locations.Hospital).ToList().
                    ForEach(employee => employee.TreatmentCount++);

                // anyone sick gets credit for a day and might progess to the next stage
                _employee.Where(employee => employee.Infected).ToList().
                    ForEach(employee => employee.InfectionProgression());

                StartDate = StartDate.AddDays(1);
            }

            Messages = messages.ToString();
            Debug.WriteLine(Messages);
            return true;
        }
    }
}