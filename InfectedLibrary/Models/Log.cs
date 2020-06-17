using System;
using System.Collections.Generic;
using System.Text;

namespace InfectedLibrary.Models
{
    public class Log
    {
        public DateTime Created { get; set; }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Building { get; set; }
        public int Floor { get; set; }
        public int Room { get; set; }
        public string RoomType { get; set; }
        public string Status { get; set; }
        public List<Contact> Contacts { get; set; }
    }
}
