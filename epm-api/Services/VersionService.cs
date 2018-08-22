using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using epm_api.Services.Interfaces;

namespace epm_api.Services
{
    public class VersionService : IVersionService
    {
        private readonly IS3Service _s3Service;
        private readonly IAmazonS3 _client;

        public VersionService(IS3Service s3Service)
        {
            this._s3Service = s3Service;
            this._client = this._s3Service.GetClient();
        }

        /// <summary>
        /// Gets the latest version of a package
        /// </summary>
        /// <param name="packageName">The package name</param>
        /// <returns></returns>
        public async Task<string> GetLatestVersionOfPackge(string packageName)
        {
            ListObjectsRequest request = new ListObjectsRequest
            {
                BucketName = this._s3Service.GetBucketName(),
                Prefix = packageName + "/latestversion"
            };

            S3Object versionDetails = (await this._client.ListObjectsAsync(request)).S3Objects.FirstOrDefault();

            return versionDetails?.Key.Split('@')[1];
        }
    }
}
