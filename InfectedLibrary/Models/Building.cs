using System;
using System.Collections.Generic;
using System.Text;

namespace InfectedLibrary.Models
{
    public class Building
    {
        public int BuildingNumber { get; set; }
        public List<Floor> Floors { get; set; }

        public Building()
        {
            Floors = new List<Floor>();
        }
    }
}
