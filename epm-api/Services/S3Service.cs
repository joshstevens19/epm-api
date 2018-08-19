using Amazon.S3;
using System;
using System.Collections.Generic;
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
        /// <param name="packageName"></param>
        /// <returns></returns>
        public async Task<string> GetLatestVersionOfPackge(string packageName)
        {
            ListObjectsRequest request = new ListObjectsRequest
            {
                BucketName = "ethereumpackagemanager",
                Prefix = packageName + "/latestversion"
            };

            S3Object versionDetails = (await this._client.ListObjectsAsync(request)).S3Objects.FirstOrDefault();

            return versionDetails?.Key.Split('@')[1];
        }

        public async Task<IReadOnlyCollection<PackageFile>> GetPackageFilesDetails(string packageName, string version)
        {
            IList<PackageFile> files = new List<PackageFile>();

            ListObjectsRequest request = new ListObjectsRequest
            {
                BucketName = "ethereumpackagemanager",
                Prefix = packageName + "/" + version
            };

            ListObjectsResponse response;

            string s3Endpoint = "https://s3.amazonaws.com/ethereumpackagemanager/";

            do
            {
                // Get a list of objects
                response = await this._client.ListObjectsAsync(request);
                foreach (S3Object file in response.S3Objects)
                {
                    string locationInPackage = file.Key.Replace(packageName + "/" + version + "/", "");

                    if (locationInPackage.Length > 0)
                    {
                        Uri fileUrl = new Uri(s3Endpoint + file.Key);
                        files.Add(new PackageFile(fileUrl, locationInPackage));
                    }
                }

                // Set the marker property
                request.Marker = response.NextMarker;
            } while (response.IsTruncated);

            return (IReadOnlyCollection<PackageFile>)files;
        }
    }
}
