using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using epm_api.Entities;
using epm_api.Extentions;
using epm_api.Models;
using epm_api.Services.Interfaces;

namespace epm_api.Services
{
    public class PackageService : IPackageService
    {
        private readonly IS3Service _s3Service;
        private readonly IAmazonS3 _s3client;
        private readonly IDynamoDbService _dynamoDbService;

        public PackageService(IS3Service s3Service, IDynamoDbService dynamoDbService)
        {
            // s3 logic 
            this._s3Service = s3Service;
            this._s3client = this._s3Service.GetClient();

            // dynamodb logic 
            this._dynamoDbService = dynamoDbService;
        }

        // REFACTOR THIS LOGIC LETS JUST GET SOMETHING UPLOADING
        // FOR NOW
        public async Task UploadPackageAsync(PackageFiles packageFiles,
                                             EthereumPmMetaData ethereumPmMetaData,
                                             string jwtUsername)
        {
            // 1 - (if installed already) check the package user can upload this 
            // 2 - (if installed already) check that the version is higher then already installed 
            // 3 - (hard limits for the amount of files for now) say 500
            // 4 - upload files to AWS s3 
            // 5 - insert package data into dynamodb 
            // 6 - done i think :) 
            PackageDetailsEntity packageDetails =
                await this._dynamoDbService.GetItemAsync<PackageDetailsEntity>(packageFiles.PackageName);

            if ((await this.ValidateUpdatingPackageAsync(packageFiles.Version, packageDetails, jwtUsername)))
            {
                string keyName = $"{packageFiles.PackageName}/{packageFiles.Version}";
                await this._s3Service.UploadFilesAsync(packageFiles.Files.ToS3Files(), keyName);

                PackageDetailsEntity packageEntity = new PackageDetailsEntity();

                // if it is null then its a brand new package 
                if (packageDetails == null)
                {
                    packageEntity.PackageName = packageFiles.PackageName;
                    packageEntity.Version = new List<string> { packageFiles.Version };
                    packageEntity.Private = ethereumPmMetaData.Private;
                    packageEntity.Team = ethereumPmMetaData.Team;
                    packageEntity.GitHub = ethereumPmMetaData.GitHub;
                    packageEntity.Owner = jwtUsername;
                    packageEntity.LatestVersion = packageFiles.Version;

                    await this._dynamoDbService.PutItemAsync<PackageDetailsEntity>(packageEntity);
                }
                else
                {
                    packageDetails.Version.Add(packageFiles.Version);
                    packageDetails.GitHub = ethereumPmMetaData.GitHub;
                    packageDetails.LatestVersion = packageFiles.Version;

                    await this._dynamoDbService.PutItemAsync<PackageDetailsEntity>(packageDetails);
                }
            }
            else
            {
                throw new Exception("Not allowed to update file");
            }
        }

        private async Task<bool> ValidateUpdatingPackageAsync(string newPackageVersion, 
                                                              PackageDetailsEntity packageDetails,
                                                              string jwtUsername)
        {
            if (packageDetails != null)
            {
                return (this.UpdatingPackageHigherVersionThenCurrent(packageDetails.LatestVersion, newPackageVersion) &&
                        await this.AllowedToUpdatePackageAsync(packageDetails, jwtUsername));
            }

            return true;
        }

        private bool UpdatingPackageHigherVersionThenCurrent(string lastestPackageVersion,
                                                             string newPackageVersion)
        {
            // for now lets assume every version follows a structure like this
            // 1.0.0, 1.3.5, 6.7.5 - always 3 digits + 3 dots 
            int packageVersion = int.Parse(lastestPackageVersion.Replace(".", ""));
            int updatedPackageVersion = int.Parse(newPackageVersion.Replace(".", ""));

            return updatedPackageVersion > packageVersion;
        }

        /// <summary>
        /// Checks if the user can update the package
        /// </summary>
        /// <param name="packageDetails">The package details entity</param>
        /// <returns>bool</returns>
        private async Task<bool> AllowedToUpdatePackageAsync(PackageDetailsEntity packageDetails,
                                                             string jwtUsername)
        {
            // check if they own this package
            if (packageDetails.Owner == jwtUsername)
            {
                return true;
            }
            // if it is not owned by this user go and check the teams data 
            else
            {
                TeamsEntity teamDetails =
                    await this._dynamoDbService.GetItemAsync<TeamsEntity>(packageDetails.Team);

                if (teamDetails.AdminUsers.Contains(jwtUsername))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets all the package files from s3
        /// </summary>
        /// <param name="packageName">The package name</param>
        /// <param name="version">The version requested</param>
        /// <returns>A read only collection of package files</returns>
        public async Task<PackageFiles> GetPackageFilesAsync(string packageName, string version)
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

                packageFilesResponse = await this._s3client.ListObjectsV2Async(requestPackages);
                foreach (S3Object entry in packageFilesResponse.S3Objects)
                {
                    if (entry.Key != prefix)
                    {
                        GetObjectRequest request = new GetObjectRequest
                        {
                            BucketName = entry.BucketName,
                            Key = entry.Key
                        };

                        using (var response = await this._s3client.GetObjectAsync(request))
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

            return new PackageFiles(version, packageName, (IReadOnlyCollection<PackageFile>)files);
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
