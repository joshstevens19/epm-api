using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using epm_api.Enums;
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

        public async Task<bool> LoginAsync(string username, string password)
        {
            // may use context instead from dynamodb service makes more sense
            // will change later on when have time
            Document doc = await this._dynamoDbService.GetItemAsync(DynamoDbTablesEnum.Users, username);

            // no user found
            if (doc == null) return false;

            return doc["Password"] == password;
        }
    }
}

