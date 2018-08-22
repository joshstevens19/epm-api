using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using epm_api.Models;
using epm_api.Services.Interfaces;

namespace epm_api.Services
{
    public class PackageService : IPackageService
    {
        private readonly IS3Service _s3Service;
        private readonly IAmazonS3 _client;

        public PackageService(IS3Service s3Service)
        {
            this._s3Service = s3Service;
            this._client = this._s3Service.GetClient();
        }

        /// <summary>
        /// Gets all the package files from s3
        /// </summary>
        /// <param name="packageName">The package name</param>
        /// <param name="version">The version requested</param>
        /// <returns>A read only collection of package files</returns>
        public async Task<IReadOnlyCollection<PackageFile>> GetPackageFiles(string packageName, string version)
        {
            IList<PackageFile> files = new List<PackageFile>();

            ListObjectsV2Response packageFilesResponse;

            string prefix = $"{packageName}/{version}/";

            do
            {
                ListObjectsV2Request requestPackages = new ListObjectsV2Request()
                {
                    BucketName = this._s3Service.GetBucketName(),
                    Prefix = prefix
                };

                packageFilesResponse = await this._client.ListObjectsV2Async(requestPackages);
                foreach (S3Object entry in packageFilesResponse.S3Objects)
                {
                    if (entry.Key != prefix)
                    {
                        GetObjectRequest request = new GetObjectRequest
                        {
                            BucketName = entry.BucketName,
                            Key = entry.Key
                        };

                        using (var response = await this._client.GetObjectAsync(request))
                        {
                            using (var responseStream = response.ResponseStream)
                            {
                                using (var reader = new StreamReader(responseStream))
                                {
                                    string name = this.ParsePackageFileName(prefix, entry.Key);
                                    string responseContent = reader.ReadToEnd();

                                    files.Add(new PackageFile(name, responseContent));
                                }
                            }
                        }
                    }
                }
            } while (packageFilesResponse.IsTruncated);

            return (IReadOnlyCollection<PackageFile>)files;
        }

        /// <summary>
        /// Parses the package location name into a proper file name 
        /// </summary>
        /// <param name="prefix">The s3 prefix the request has sent</param>
        /// <param name="key">The key of a entry returned from s3</param>
        /// <returns></returns>
        private string ParsePackageFileName(string prefix, string key)
        {
            return key.Replace(prefix, "");
        }
    }
}
