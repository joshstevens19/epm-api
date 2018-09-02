using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace epm_api.Enums
{
    public enum DynamoDbTablesEnum
    {
        [DisplayName("PackageDetails")]
        PackageDetails = 0,
        [DisplayName("Teams")]
        Teams = 1,
        [DisplayName("Users")]
        Users = 2
    }
}
