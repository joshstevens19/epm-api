using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using epm_api.Models;
using epm_api.Services.Interfaces;

namespace epm_api.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IDynamoDbService _dynamoDbService;
        public ProfileService(IDynamoDbService dynamoDbService)
        {
            this._dynamoDbService = dynamoDbService;
        }

        public async Task<UsersEntity> UpdateDetails(UsersEntity updatedUserEntity)
        {
            UsersEntity currentUserEntity = await this._dynamoDbService.GetItemAsync<UsersEntity>(updatedUserEntity.Username);

            if (currentUserEntity == null) return null;

            currentUserEntity.FirstName = updatedUserEntity.FirstName ?? currentUserEntity.FirstName;
            currentUserEntity.LastName = updatedUserEntity.LastName ?? currentUserEntity.LastName;
            currentUserEntity.Introduction = updatedUserEntity.Introduction ?? currentUserEntity.Introduction;
            currentUserEntity.UpdatedOn = DateTime.Now;

            try
            {
                return await this._dynamoDbService.PutItemAsync<UsersEntity>(currentUserEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
