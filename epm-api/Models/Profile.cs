using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace epm_api.Models
{
    public class Profile
    {
        public Profile(string username, string firstName, string lastName, string introduction)
        {
            this.Username = username;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Introduction = introduction;
        }

        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Introduction { get; set; }
    }
}
