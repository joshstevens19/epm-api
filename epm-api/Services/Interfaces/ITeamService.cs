using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using epm_api.Entities;

namespace epm_api.Services.Interfaces
{
    public interface ITeamService
    {
        Task<TeamsEntity> CreateTeam(string teamName, bool isPrivate, string username);
        Task<TeamsEntity> AddUser(string teamName, string newUser, bool isAdmin, string adminUser);
        Task<TeamsEntity> RemoveUser(string teamName, string username, string jwtUsername);
        Task<TeamsEntity> RemoveAdminUser(string teamName, string username, string jwtUsername);
        Task<TeamsEntity> TransferOwner(string teamName, string newOwner, string jwtUsername);
    }
}
