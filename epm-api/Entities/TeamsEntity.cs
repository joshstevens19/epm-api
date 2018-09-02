using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace epm_api.Entities
{
    [DynamoDBTable("Teams")]
    public class TeamsEntity
    {
        [DynamoDBHashKey]
        public string TeamName { get; set; }
        public List<string> Users { get; set; }
        public List<string> AdminUsers { get; set; }
        public string Logo { get; set; }
        public List<string> Packages { get; set; }
        public bool Private { get; set; }
    }
}
