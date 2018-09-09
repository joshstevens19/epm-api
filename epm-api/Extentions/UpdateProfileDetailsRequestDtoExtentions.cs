using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using epm_api.Dtos;
using epm_api.Models;

namespace epm_api.Extentions
{
    public static class UpdateProfileDetailsRequestDtoExtentions
    {
        public static UsersEntity ToEntity(this UpdateProfileDetailsRequestDto dto, string username)
        {
            return new UsersEntity()
            {
                Username = username,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Introduction = dto.Introduction,
            };
        }

    }
}
