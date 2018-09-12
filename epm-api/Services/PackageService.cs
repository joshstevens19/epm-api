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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="jwtUsername"></param>
        /// <param name="deprecate"></param>
        /// <returns></returns>
        public async Task UpdateDeprecateValueInPackage(string packageName,
                                                        string jwtUsername,
                                                        bool deprecate)
        {
            PackageDetailsEntity packageDetails =
                await this._dynamoDbService.GetItemAsync<PackageDetailsEntity>(packageName);

            if (packageDetails == null)
                throw new Exception("This package does not exist");

            // if the package is already deprecate do not
            // call anymore api logic
            if (packageDetails.Deprecated == deprecate)
                return;

            // is team
            if (!string.IsNullOrEmpty(packageDetails.Team))
            {
                TeamsEntity teamsEntity = await this._dynamoDbService.GetItemAsync<TeamsEntity>(packageDetails.Team);

                if (!teamsEntity.AdminUsers.Contains(jwtUsername))
                    throw new Exception("You are not allowed to mark this package as deprecated");

                packageDetails.Deprecated = deprecate;
            }
            // it is a normal user who owns the package
            else
            {
                if (!packageDetails.AdminUsers.Contains(jwtUsername))
                    throw new Exception("You are not allowed to mark this package as deprecated");
            }

            packageDetails.Deprecated = deprecate;
            await this._dynamoDbService.PutItemAsync<PackageDetailsEntity>(packageDetails);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="username"></param>
        /// <param name="jwtUsername"></param>
        /// <returns></returns>
        public async Task AddAdminUserToPackage(string packageName,
                                                string username,
                                                string jwtUsername)
        {
            UsersEntity userEntity = await this._dynamoDbService.GetItemAsync<UsersEntity>(username);

            if (userEntity == null)
                throw new Exception("This user does not exist");

            PackageDetailsEntity packageDetails =
                await this._dynamoDbService.GetItemAsync<PackageDetailsEntity>(packageName);

            if (packageDetails == null)
                throw new Exception("No package found");

            if (!string.IsNullOrEmpty(packageDetails.Team))
                throw new Exception("Please use the teams API method to add memebers to this package");

            if (packageDetails.AdminUsers.Contains(jwtUsername))
            {
                if (!packageDetails.AdminUsers.Contains(username))
                {
                    packageDetails.AdminUsers.Add(username);
                }

                // do nothing as already present in team :)
            }
            else
            {
                throw new Exception("Do not have permission to add a admin user to this package");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageFiles"></param>
        /// <param name="ethereumPmMetaData"></param>
        /// <param name="jwtUsername"></param>
        /// <returns></returns>
        public async Task UploadPackageAsync(PackageFiles packageFiles,
                                             EthereumPmMetaData ethereumPmMetaData,
                                             string jwtUsername)
        {
            PackageDetailsEntity packageDetails =
                await this._dynamoDbService.GetItemAsync<PackageDetailsEntity>(packageFiles.PackageName);

            // if it is null then its a brand new package 
            if (packageDetails == null)
            {

                // should be in a transaction which i will put in, as if one of these 
                // fails then i want to roll back all the data, i don't really want to
                // to insert then delete so will look at dynamodb to see if this rollback
                // logic exists 
                TeamsEntity teamsEntity = null;

                // if it is a package for a team
                if (!string.IsNullOrEmpty(ethereumPmMetaData.Team))
                {
                    teamsEntity = await this._dynamoDbService.GetItemAsync<TeamsEntity>(ethereumPmMetaData.Team);

                    if (teamsEntity == null) throw new Exception("Team does not exists");

                    packageDetails = new PackageDetailsEntity
                    {
                        PackageName = packageFiles.PackageName,
                        Version = new List<string> { packageFiles.Version },
                        Private = ethereumPmMetaData.Private,
                        Team = ethereumPmMetaData.Team,
                        GitHub = ethereumPmMetaData.GitHub,
                        Owner = ethereumPmMetaData.Team,
                        LatestVersion = packageFiles.Version,
                        Deprecated = false,
                    };
                }
                else
                {
                    packageDetails = new PackageDetailsEntity
                    {
                        PackageName = packageFiles.PackageName,
                        Version = new List<string> { packageFiles.Version },
                        Private = ethereumPmMetaData.Private,
                        Team = ethereumPmMetaData.Team,
                        GitHub = ethereumPmMetaData.GitHub,
                        Owner = jwtUsername,
                        LatestVersion = packageFiles.Version,
                        Deprecated = false,
                        AdminUsers = new List<string> { jwtUsername }
                    };
                }

                await this._dynamoDbService.PutItemAsync<PackageDetailsEntity>(packageDetails);

                if (teamsEntity != null)
                {
                    if (teamsEntity.Packages == null || teamsEntity.Packages.GetType() != typeof(List<string>))
                        teamsEntity.Packages = new List<string>();

                    teamsEntity.Packages.Add(packageFiles.PackageName);

                    await this._dynamoDbService.PutItemAsync<TeamsEntity>(teamsEntity);
                }
                else
                {
                    // as they have authenticated with the request the user should always exist 
                    // do not want to do a load on db to check each time
                    UsersEntity usersEntity = await this._dynamoDbService.GetItemAsync<UsersEntity>(jwtUsername);

                    if (usersEntity.Packages == null || usersEntity.Packages.GetType() != typeof(List<string>))
                        usersEntity.Packages = new List<string>();

                    usersEntity.Packages.Add(packageFiles.PackageName);

                    await this._dynamoDbService.PutItemAsync<UsersEntity>(usersEntity);
                }
            }
            else
            {
                if (!this.UpdatingPackageHigherVersionThenCurrent(packageDetails.LatestVersion, packageFiles.Version))
                {
                    throw new Exception("Your package version is not higher then the current one");
                }

                bool allowedToUpdatePackage =
                    await this.AllowedToUpdatePackageAsync(packageDetails.Team, packageDetails.AdminUsers, jwtUsername);

                if (!allowedToUpdatePackage)
                {
                    throw new Exception("You are not allowed to update this package");
                }

                packageDetails.Version.Add(packageFiles.Version);
                packageDetails.GitHub = ethereumPmMetaData.GitHub;
                packageDetails.LatestVersion = packageFiles.Version;

                await this._dynamoDbService.PutItemAsync<PackageDetailsEntity>(packageDetails);
            }

            // upload the files last, this means it all has been successfully inserted into 
            // the db
            string keyName = $"{packageFiles.PackageName}/{packageFiles.Version}";
            await this._s3Service.UploadFilesAsync(packageFiles.Files.ToS3Files(), keyName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastestPackageVersion"></param>
        /// <param name="newPackageVersion"></param>
        /// <returns></returns>
        private bool UpdatingPackageHigherVersionThenCurrent(string lastestPackageVersion,
                                                             string newPackageVersion)
        {
            // for now lets assume every version follows a structure like this
            // 1.0.0, 1.3.5, 6.7.5 - always 3 digits + 3 dots 
            // will support a few version i am thinking
            // > 1.0 - not sure if this makes any sense though 
            // > 1.0.0
            // > 1.0.0.0
            int packageVersion = int.Parse(lastestPackageVersion.Replace(".", ""));
            int updatedPackageVersion = int.Parse(newPackageVersion.Replace(".", ""));

            return updatedPackageVersion > packageVersion;
        }

        /// <summary>
        /// Checks if the user can update the package
        /// </summary>
        /// <param name="adminUsers"></param>
        /// <param name="jwtUsername"></param>
        /// <param name="team"></param>
        /// <returns>bool</returns>
        private async Task<bool> AllowedToUpdatePackageAsync(string team,
                                                             List<string> adminUsers,
                                                             string jwtUsername)
        {
            if (!string.IsNullOrEmpty(team))
            {
                TeamsEntity teamDetails = await this._dynamoDbService.GetItemAsync<TeamsEntity>(team);

                if (teamDetails == null) return false;

                if (teamDetails.AdminUsers.Contains(jwtUsername))
                {
                    return true;
                }
            }
            else if (adminUsers.Contains(jwtUsername))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets all the package files from s3
        /// </summary>
        /// <param name="packageName">The package name</param>
        /// <param name="version">The version requested</param>
        /// <param name="jwtUsername"></param>
        /// <returns>A read only collection of package files</returns>
        public async Task<PackageFiles> GetPackageFilesAsync(string packageName,
                                                             string version, 
                                                             string jwtUsername)
        {

            PackageDetailsEntity packageDetails =
                await this._dynamoDbService.GetItemAsync<PackageDetailsEntity>(packageName);

            if (packageDetails == null)
                throw new Exception("Package does not exist");

            // if it is a private package lets check this person can install it
            if (packageDetails.Private)
            {
                // is team
                if (!string.IsNullOrEmpty(packageDetails.Team))
                {
                    TeamsEntity teamsEntity = await this._dynamoDbService.GetItemAsync<TeamsEntity>(packageDetails.Team);

                    if (!teamsEntity.Users.Contains(jwtUsername))
                        throw new Exception("You are not allowed to install this package");
                }
                // it is a normal user who owns the package
                else
                {
                    if (!packageDetails.AdminUsers.Contains(jwtUsername))
                        throw new Exception("You are not allowed to install this package");
                }
            }

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
