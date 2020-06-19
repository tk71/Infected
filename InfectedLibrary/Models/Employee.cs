﻿using System;
using InfectedLibrary.Enums;

namespace InfectedLibrary.Models
{
    internal class Employee
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string AssignedLocation { get; set; }
        public string CurrentLocation { get; set; }
        public Locations Location { get; set; }
        public float ChanceOfInfection { get; set; }
        public float InfectedPercent { get; set; }
        public int InfectionTime { get; set; }
        public int IncubationTime { get; set; }
        public int SymptomaticTime { get; set; }
        public int InfectionCount { get; set; }
        public int TreatmentCount { get; set; }
        private InfectionState status;
        public InfectionState Status { 
            get { return status; }
            set {
                status = value;

                Random rnd;
                switch (status)
                {
                    case InfectionState.Well:
                        break;
                    case InfectionState.Infected:
                        InfectionCount = 1;
                        InfectionTime = 3;
                        break;
                    case InfectionState.Incubation:
                        InfectionCount = 1;
                        rnd = new Random(Guid.NewGuid().GetHashCode());
                        IncubationTime = rnd.Next(3, 8);
                        break;
                    case InfectionState.Symptomatic:
                        InfectionCount = 1;
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
        public bool Infected
        {
            get { return (status != InfectionState.Well && status != InfectionState.Immune); }
        }

        public void InfectionProgression()
        {
            InfectionCount++;
            switch (Status)
            {
                case InfectionState.Well:
                    break;
                case InfectionState.Infected:
                    if (InfectionCount == InfectionTime) Status = InfectionState.Incubation;
                    break;
                case InfectionState.Incubation:
                    if (InfectionCount == IncubationTime) Status = InfectionState.Symptomatic;
                    break;
                case InfectionState.Symptomatic:
                    if (InfectionCount == SymptomaticTime) Status = InfectionState.Immune;
                    break;
                case InfectionState.Immune:
                    break;
                default:
                    break;
            }
        }
    }
}
