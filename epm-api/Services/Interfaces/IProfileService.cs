using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using epm_api.Models;

namespace epm_api.Services.Interfaces
{
    public interface IProfileService
    {
        Task<UsersEntity> UpdateDetails(UsersEntity updatedUserEntity);
    }
}
