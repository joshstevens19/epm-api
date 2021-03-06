﻿using System;
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

                if (!packageDetails.Users.Contains(username))
                {
                    packageDetails.Users.Add(username);
                }

                await this._dynamoDbService.PutItemAsync<PackageDetailsEntity>(packageDetails);
            }
            else
            {
                throw new Exception("Do not have permission to add a admin user to this package");
            }
        }

        /// <summary>
        /// Add a user to a package - normally used for private packages you want to share with friends
        /// </summary>
        /// <param name="packageName">The package name</param>
        /// <param name="username">The username</param>
        /// <param name="jwtUsername">The jwt username</param>
        /// <returns></returns>
        public async Task AddUserToPackage(string packageName,
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
                if (!packageDetails.Users.Contains(username))
                {
                    packageDetails.Users.Add(username);
                }

                await this._dynamoDbService.PutItemAsync<PackageDetailsEntity>(packageDetails);
            }
            else
            {
                throw new Exception("Do not have permission to add a user to this package");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageName">The package name</param>
        /// <param name="username">The username</param>
        /// <param name="jwtUsername">The jwt username</param>
        /// <returns></returns>
        public async Task RemoveUserFromPackage(string packageName,
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

            if (packageDetails.Owner == username)
                throw new Exception("Can not remove owner from package, please transfer the owner first");

            if (packageDetails.AdminUsers.Contains(jwtUsername))
            {
                packageDetails.AdminUsers = packageDetails.AdminUsers.Where(a => a != username).ToList();
                packageDetails.Users = packageDetails.Users.Where(a => a != username).ToList();

                await this._dynamoDbService.PutItemAsync<PackageDetailsEntity>(packageDetails);
            }
            else
            {
                throw new Exception("Do not have permission to add a user to this package");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageName">The package name</param>
        /// <param name="username">The username</param>
        /// <param name="jwtUsername">The jwt username</param>
        /// <returns></returns>
        public async Task RemoveAdminPermissonFromUserForPackage(string packageName,
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

            if (packageDetails.Owner == username)
                throw new Exception("Can not remove owner from package, please transfer the owner first");

            if (packageDetails.AdminUsers.Contains(jwtUsername))
            {
                packageDetails.AdminUsers = packageDetails.AdminUsers.Where(a => a != username).ToList();
            
                await this._dynamoDbService.PutItemAsync<PackageDetailsEntity>(packageDetails);
            }
            else
            {
                throw new Exception("Do not have permission to add a user to this package");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageName">The package name</param>
        /// <param name="username">The username</param>
        /// <param name="jwtUsername">The jwt username</param>
        /// <returns></returns>
        public async Task TransferPackageOwner(string packageName,
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
                throw new Exception("Please use the teams API method to transfer ownership for this package");

            if (packageDetails.Owner == username)
                throw new Exception("Already owner of this project");

            if (packageDetails.Owner == jwtUsername)
            {
                packageDetails.Owner = username;

                await this._dynamoDbService.PutItemAsync<PackageDetailsEntity>(packageDetails);
            }
            else
            {
                throw new Exception("Do not have permission to add a user to this package");
            }
        }

        /// <summary>
        /// Returns the list of admin users
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="username"></param>
        /// <param name="jwtUsername"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<string>> GetAdminUsers(string packageName,
                                                               string jwtUsername)
        {
            UsersEntity userEntity = await this._dynamoDbService.GetItemAsync<UsersEntity>(jwtUsername);

            if (userEntity == null)
                throw new Exception("This user does not exist");

            PackageDetailsEntity packageDetails =
                await this._dynamoDbService.GetItemAsync<PackageDetailsEntity>(packageName);

            if (packageDetails == null)
                throw new Exception("No package found");

            if (!string.IsNullOrEmpty(packageDetails.Team))
                throw new Exception("Please use the teams API method to get list of admin memebers");

            if (!packageDetails.AdminUsers.Contains(jwtUsername))
                throw new Exception("You are not allowed to get a list of all admin users");

            return (IReadOnlyList<string>)packageDetails.AdminUsers;
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
                        Description = ethereumPmMetaData.Description,
                        Keywords = ethereumPmMetaData.Keywords,
                        Private = ethereumPmMetaData.Private,
                        Team = ethereumPmMetaData.Team,
                        GitHub = ethereumPmMetaData.GitHub,
                        Owner = ethereumPmMetaData.Team,
                        LatestVersion = packageFiles.Version,
                        Deprecated = false,
                        CreatedOn = DateTime.UtcNow
                    };
                }
                else
                {
                    packageDetails = new PackageDetailsEntity
                    {
                        PackageName = packageFiles.PackageName,
                        Version = new List<string> { packageFiles.Version },
                        Description = ethereumPmMetaData.Description,
                        Keywords = ethereumPmMetaData.Keywords,
                        Private = ethereumPmMetaData.Private,
                        Team = ethereumPmMetaData.Team,
                        GitHub = ethereumPmMetaData.GitHub,
                        Owner = jwtUsername,
                        LatestVersion = packageFiles.Version,
                        Deprecated = false,
                        AdminUsers = new List<string> { jwtUsername },
                        Users = new List<string> { jwtUsername },
                        CreatedOn = DateTime.UtcNow
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
                packageDetails.Description = ethereumPmMetaData.Description;
                packageDetails.Keywords = ethereumPmMetaData.Keywords;

                await this._dynamoDbService.PutItemAsync<PackageDetailsEntity>(packageDetails);
            }

            // upload the files last, this means it all has been successfully inserted into 
            // the db
            string keyName = $"{packageFiles.PackageName}/{packageFiles.Version}";
            await this._s3Service.UploadFilesAsync(packageFiles.Files.ToS3Files(), keyName);
        }

        /// <summary>
        /// This unpublishs a package if it was created before 72 hours ago
        /// </summary>
        /// <param name="packageName">The package name</param>
        /// <param name="jwtUsername">The JWT username</param>
        /// <returns></returns>
        public async Task UnpublishPackage(string packageName,
                                           string jwtUsername)
        {
            UsersEntity user = await this._dynamoDbService.GetItemAsync<UsersEntity>(jwtUsername);
            TeamsEntity team = null;

            PackageDetailsEntity packageDetails = await this._dynamoDbService.GetItemAsync<PackageDetailsEntity>(packageName);

            if (packageDetails == null)
                throw new Exception("Package does not exist");

            if (packageDetails.Team != null)
            {
                team = await this._dynamoDbService.GetItemAsync<TeamsEntity>(packageDetails.Team);
                if (!team.AdminUsers.Contains(jwtUsername))
                    throw new Exception("You are not allowed to unpublish this package");
            }
            else
            {

                if (!packageDetails.AdminUsers.Contains(jwtUsername))
                    throw new Exception("You are not allowed to unpublish this package");
            }

            DateTime unpublishedMaxDate = packageDetails.CreatedOn.AddDays(3);

            if (unpublishedMaxDate > DateTime.UtcNow)
                throw new Exception("You can only unpublish a package 72 hours after it has been created. Please mark it as deprecated");

            await this._dynamoDbService.DeleteItemAsync<PackageDetailsEntity>(packageName);
            if (team != null)
            {
                team.Packages = team.Packages.Where(p => p != packageName).ToList();
                await this._dynamoDbService.PutItemAsync<TeamsEntity>(team);
            }
            else
            {
                user.Packages = user.Packages.Where(p => p != packageName).ToList();
                await this._dynamoDbService.PutItemAsync<UsersEntity>(user);
            }
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
                                                             IList<string> adminUsers,
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
                    if (!packageDetails.Users.Contains(jwtUsername))
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
