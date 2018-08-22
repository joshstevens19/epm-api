using Amazon.S3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;
using epm_api.Models;

namespace epm_api.Services
{
    // clear this logic up so no hard coded strings ;) ....... lazy josh
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _client;
        private readonly string _bucketNames = "ethereumpackagemanager";

        // will keep injection in for now - will remove if it stays like this forever 
        public S3Service(IAmazonS3 client)
        {
            // this._client = client.Config;
            // as aws seem to not bring my region back even though it is in the config
            // we have to create a new instance of amazons3client :(
            this._client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
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
                BucketName = this._bucketNames,
                Prefix = packageName + "/latestversion"
            };

            S3Object versionDetails = (await this._client.ListObjectsAsync(request)).S3Objects.FirstOrDefault();

            return versionDetails?.Key.Split('@')[1];
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
                    BucketName = this._bucketNames,
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
