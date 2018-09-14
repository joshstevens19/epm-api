using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace epm_api.Dtos.Profile
{
    public class UpdateProfileDetailsRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Introduction { get; set; }
    }
}
