using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using epm_api.Entities;
using epm_api.Services.Interfaces;

namespace epm_api.Services
{
    public class VersionService : IVersionService
    {
        private readonly IDynamoDbService _dynamoDbService;

        public VersionService(IDynamoDbService dynamoDbService)
        {
            this._dynamoDbService = dynamoDbService;
        }

        /// <summary>
        /// Gets the latest version of a package
        /// </summary>
        /// <param name="packageName">The package name</param>
        /// <returns></returns>
        public async Task<string> GetLatestVersionOfPackgeAsync(string packageName)
        {
            PackageDetailsEntity packageDetails =
                await this._dynamoDbService.GetItemAsync<PackageDetailsEntity>(packageName);

            return packageDetails?.LatestVersion;
        }
    }
}
