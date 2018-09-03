﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using epm_api.Models;

namespace epm_api.Dtos.Extentions
{
    public static class RegisterRequestDtoExtentions
    {
        public static UsersEntity ToEntity(this RegisterRequestDto dto)
        {
            return new UsersEntity()
            {
                Blocked = false,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                EmailAddress = dto.EmailAddress,
                FirstName = dto.FirstName,
                Introduction = dto.Introduction,
                LastName = dto.LastName,
                Packages = new List<string>(),
                Password = dto.Password,
                ProfilePicture = string.Empty, // this will be populated but later
                Stars = new List<string>(),
                Teams = new List<string>(),
                TeamsOwner = new List<string>()
            };
        }
    }
}
