using System;

namespace InfectedLibrary.Models
{
    internal class Person
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string AssignedRoom { get; set; }
        public string CurrentRoom { get; set; }
        public RoomType CurrentRoomType { get; set; }
        public float ChanceOfInfection { get; set; }
        public int InfectionTime { get; set; }
        public int IncubationTime { get; set; }
        public int SymptomaticTime { get; set; }
        public int DaysSick { get; set; }
        private InfectionState status;
        public InfectionState Status { 
            get { return status; }
            set {
                status = value;

                Random rnd;
                switch (status)
                {
                    case InfectionState.Well:
                        ChanceOfInfection = 0.02f;
                        break;
                    case InfectionState.Infected:
                        DaysSick = 1;
                        InfectionTime = 3;
                        break;
                    case InfectionState.Incubation:
                        rnd = new Random(Guid.NewGuid().GetHashCode());
                        IncubationTime = rnd.Next(3, 8);
                        break;
                    case InfectionState.Symptomatic:
                        rnd = new Random(Guid.NewGuid().GetHashCode());
                        SymptomaticTime = rnd.Next(6, 12);
                        break;
                    case InfectionState.Immune:
                        break;
                    default:
                        break;
                }
            } 
        }
    }
}
