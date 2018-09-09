using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace epm_api.Models
{
    public class UnpackedJwt : Profile
    {
        public UnpackedJwt(string username, string firstName, string lastName, string introduction) 
                        : base(username, firstName, lastName, introduction)
        { }
    }
}
