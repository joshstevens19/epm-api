using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace epm_api.Entities
{
    [DynamoDBTable("PackageDetails")]
    public class PackageDetailsEntity
    {
        [DynamoDBHashKey]
        public string PackageName { get; set; }
        public IList<string> Version { get; set; }
        public string Description { get; set; }
        public bool Private { get; set; }
        public string Team { get; set; }
        public string Owner { get; set; }
        public string LatestVersion { get; set; }
        public string GitHub { get; set; }
        public bool Deprecated { get; set; }
        public IList<string> AdminUsers { get; set; }
        public IList<string> Users { get; set; }
        public IList<string> Keywords { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
