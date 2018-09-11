using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using epm_api.Entities;
using epm_api.Models;
using epm_api.Services.Interfaces;

namespace epm_api.Services
{
    public class TeamService : ITeamService
    {
        private readonly IDynamoDbService _dynamoDbService;
        public TeamService(IDynamoDbService dynamoDbService)
        {
            this._dynamoDbService = dynamoDbService;
        }

        /// <summary>
        /// Create a team
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="isPrivate"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<TeamsEntity> CreateTeam(string teamName, bool isPrivate, string username)
        {
            // need to check if team name already exists - will do that later
            TeamsEntity teamEntity = new TeamsEntity()
            {
                TeamName = teamName,
                TeamOwner = username,
                AdminUsers = new List<string>() { username },
                Users = new List<string>() { username },
                Private = isPrivate,
                Packages = new List<string>(),
                Deprecated = false,
            };

            return await this._dynamoDbService.PutItemAsync<TeamsEntity>(teamEntity);
        }

        /// <summary>
        /// Add user to a team
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="newUser"></param>
        /// <param name="isAdmin"></param>
        /// <param name="adminUser"></param>
        /// <returns></returns>
        public async Task<TeamsEntity> AddUser(string teamName, string newUser, bool isAdmin, string adminUser)
        {
            UsersEntity userEntity = await this._dynamoDbService.GetItemAsync<UsersEntity>(newUser);

            if (userEntity == null) throw new Exception("Username is not a valid user");

            TeamsEntity teamsEntity = await this._dynamoDbService.GetItemAsync<TeamsEntity>(teamName);

            if (teamsEntity == null) throw new Exception("No team found");

            if (teamsEntity.AdminUsers.Contains(adminUser))
            {
                if (isAdmin && !teamsEntity.AdminUsers.Contains(newUser))
                {
                    teamsEntity.AdminUsers.Add(newUser);
                }

                if (!teamsEntity.Users.Contains(newUser))
                {
                    teamsEntity.Users.Add(newUser);
                }

                return await this._dynamoDbService.PutItemAsync<TeamsEntity>(teamsEntity);
            }
            else
            {
                throw new Exception("Not allowed to update this team");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="username"></param>
        /// <param name="jwtUsername"></param>
        /// <returns></returns>
        public async Task<TeamsEntity> RemoveUser(string teamName, string username, string jwtUsername)
        {
            TeamsEntity teamsEntity = await this._dynamoDbService.GetItemAsync<TeamsEntity>(teamName);

            if (teamsEntity == null) throw new Exception("No team found");

            if (!teamsEntity.AdminUsers.Contains(jwtUsername))
                throw new Exception("Do not have permission to remove a user");

            if (teamsEntity.TeamOwner == username)
                throw new Exception("Can not remove the owner of the team");

            if (!teamsEntity.AdminUsers.Contains(username) && !teamsEntity.Users.Contains(username))
                throw new Exception("Username does not exist in the team");

            teamsEntity.AdminUsers = teamsEntity.AdminUsers.Where(u => u != username).ToList();
            teamsEntity.Users = teamsEntity.Users.Where(u => u != username).ToList();

            return await this._dynamoDbService.PutItemAsync<TeamsEntity>(teamsEntity);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="username"></param>
        /// <param name="jwtUsername"></param>
        /// <returns></returns>
        public async Task<TeamsEntity> RemoveAdminUser(string teamName, string username, string jwtUsername)
        {
            TeamsEntity teamsEntity = await this._dynamoDbService.GetItemAsync<TeamsEntity>(teamName);

            if (teamsEntity == null) throw new Exception("No team found");

            if (!teamsEntity.AdminUsers.Contains(jwtUsername))
                throw new Exception("Do not have permission to remove a user");

            if (teamsEntity.TeamOwner == username)
                throw new Exception("Can not remove the owner of the team");

            if (!teamsEntity.Users.Contains(username))
                throw new Exception("Username does not exist in the team");

            teamsEntity.Users = teamsEntity.Users.Where(u => u != username).ToList();

            return await this._dynamoDbService.PutItemAsync<TeamsEntity>(teamsEntity);
        }

        /// <summary>
        /// Transfer the owner of the team to another user
        /// </summary>
        /// <param name="teamName">The team name</param>
        /// <param name="newOwner">The new owner username</param>
        /// <param name="jwtUsername">The JWT request username</param>
        /// <returns></returns>
        public async Task<TeamsEntity> TransferOwner(string teamName, string newOwner, string jwtUsername)
        {
            TeamsEntity teamsEntity = await this._dynamoDbService.GetItemAsync<TeamsEntity>(teamName);

            if (teamsEntity == null) throw new Exception("No team found");

            if (teamsEntity.TeamOwner != jwtUsername)
                throw new Exception("Can only transfer owner of the team if you are the current owner");

            UsersEntity userEntity = await this._dynamoDbService.GetItemAsync<UsersEntity>(newOwner);

            if (userEntity == null) throw new Exception("Username is not a valid user");

            teamsEntity.TeamOwner = newOwner;
            
            if (!teamsEntity.AdminUsers.Contains(newOwner)) teamsEntity.AdminUsers.Add(newOwner);
            if (!teamsEntity.Users.Contains(newOwner)) teamsEntity.Users.Add(newOwner);

            return await this._dynamoDbService.PutItemAsync<TeamsEntity>(teamsEntity);
        }

        //public async Task<IReadOnlyCollection<TeamsEntity>> AllTeams(string username)
        //{
        //     TeamsEntity 
        //}
    }
}
