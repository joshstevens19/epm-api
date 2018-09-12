using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using epm_api.Entities;
using epm_api.Models;
using epm_api.Services.Interfaces;

namespace epm_api.Services
{
    public class StarService : IStarService
    {
        private readonly IDynamoDbService _dynamoDbService;

        public StarService(IDynamoDbService dynamoDbService)
        {
            this._dynamoDbService = dynamoDbService;
        }

        public async Task StarPackage(string packageName, string jwtUsername)
        {
            UsersEntity user = await this._dynamoDbService.GetItemAsync<UsersEntity>(jwtUsername);

            if (user == null)
                throw new Exception("Can not find your user in the db, please try again later");

            if (user.Stars == null)
                user.Stars = new List<string>();

            if (!user.Stars.Contains(packageName))
            {
                user.Stars.Add(packageName);

                await this._dynamoDbService.PutItemAsync<UsersEntity>(user);
            }
        }

        public async Task UnstarPackage(string packageName, string jwtUsername)
        {
            UsersEntity user = await this._dynamoDbService.GetItemAsync<UsersEntity>(jwtUsername);

            if (user == null)
                throw new Exception("Can not find your user in the db, please try again later");

            if (user.Stars != null)
            {
                if (user.Stars.Contains(packageName))
                {
                    user.Stars = user.Stars.Where(s => s != packageName).ToList();

                    // db not updating at the moment for some reason!
                    await this._dynamoDbService.PutItemAsync<UsersEntity>(user);
                }
            }
        }

        public async Task<IReadOnlyList<string>> GetStarredProjects(string jwtUsername)
        {
            UsersEntity user = await this._dynamoDbService.GetItemAsync<UsersEntity>(jwtUsername);

            if (user == null)
                throw new Exception("Can not find your user in the db, please try again later");

            return user.Stars;
        }
    }
}
