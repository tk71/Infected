using System;
using System.Collections.Generic;

namespace InfectedLibrary.Models
{
    public class Log
    {
        public DateTime Created { get; set; }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string CurrentRoom { get; set; }
        public string CurrentRoomType { get; set; }
        public string Status { get; set; }
        public List<Contact> Contacts { get; set; }

        public Log()
        {
            Contacts = new List<Contact>();
        }
    }
}
