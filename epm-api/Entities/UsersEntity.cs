using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace epm_api.Models
{
    [DynamoDBTable("Users")]
    public class UsersEntity
    {
        [DynamoDBHashKey]
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Uri ProfilePicture { get; set; }
        public string Introduction { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public List<string> Teams { get; set; }
        public List<string> TeamsOwner { get; set; }
        public List<string> Packages { get; set; }
        public List<string> Stars { get; set; }
        public bool Blocked { get; set; }
    }
}
