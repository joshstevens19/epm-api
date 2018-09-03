using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model.Internal.MarshallTransformations;
using epm_api.Dtos;
using epm_api.Enums;
using epm_api.Models;
using epm_api.Services.Interfaces;

namespace epm_api.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IJwtService _jwtService;
        private readonly IDynamoDbService _dynamoDbService;
        public AuthenticationService(IJwtService jwtService, IDynamoDbService dynamoDbService)
        {
            this._jwtService = jwtService;
            this._dynamoDbService = dynamoDbService;
        }

        /// <summary>
        /// Login the user in if the username and password are correct
        /// </summary>
        /// <param name="username">Users username</param>
        /// <param name="password">Users password</param>
        /// <returns></returns>
        public async Task<UsersEntity> LoginAsync(string username, string password)
        {
            // may use context instead from dynamodb service makes more sense
            // will change later on when have time
            UsersEntity user = await this._dynamoDbService.GetItemAsync<UsersEntity>(username);

            if (user == null || user.Password != password)
            {
                return null;
            }

            return user;
        }

        /// <summary>
        /// This registers the user 
        /// </summary>
        /// <param name="userEntity">The user details</param>
        /// <returns></returns>
        public async Task<UsersEntity> RegisterAsync(UsersEntity userEntity)
        {
            try
            {
                // need to do some checks to make sure user does not already exists 
                // use expressions
                userEntity = await this._dynamoDbService.PutItemAsync<UsersEntity>(userEntity);
            }
            catch (Exception)
            {
                return null;
            }

            return userEntity;
        }
    }
}

