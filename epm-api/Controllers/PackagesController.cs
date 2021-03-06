﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using epm_api.Common;
using epm_api.Dtos.EpmPackages;
using epm_api.Models;
using epm_api.Packages.Dtos.EpmPackages;
using epm_api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace epm_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackagesController : BaseApiController
    {
        private readonly IPackageService _packageService;
        private readonly IVersionService _versionService;
        private readonly IJwtService _jwtService;

        public PackagesController(IPackageService packageService, IVersionService versionService, IJwtService jwtService)
        {
            this._packageService = packageService;
            this._versionService = versionService;
            this._jwtService = jwtService;
        }

        [HttpGet]
        [Route(template: "{packageName}")]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromRoute] string packageName)
        {
            string latestVersion = await this._versionService.GetLatestVersionOfPackgeAsync(packageName);

            string jwtUsername = string.Empty;

            if (User != null)
            {
                UnpackedJwt unpackedJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());
                jwtUsername = unpackedJwt.Username;
            }

            PackageFiles packageFiles = await this._packageService.GetPackageFilesAsync(packageName, latestVersion, jwtUsername);

            return this.Ok(packageFiles);
        }

        [HttpGet]
        [Route(template: "{packageName}/{version}")]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromRoute] string packageName, [FromRoute] string version)
        {
            string jwtUsername = string.Empty;

            if (User != null)
            {
                UnpackedJwt unpackedJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());
                jwtUsername = unpackedJwt.Username;
            }

            PackageFiles packageFiles = await this._packageService.GetPackageFilesAsync(packageName, version, jwtUsername);

            return this.Ok(packageFiles);
        }

        [HttpPost]
        [Authorize]
        [Route(template: "unpublish")]
        public async Task<IActionResult> Post([FromBody] UnpublishPackageRequestDtos unpublishPackageRequestDto)
        {
            UnpackedJwt unpackJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            try
            {
                await this._packageService.UnpublishPackage(unpublishPackageRequestDto.PackageName, unpackJwt.Username);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.ToString());
            }

            return this.Ok();
        }

        [HttpPost]
        [Authorize]
        [Route(template: "deprecate")]
        public async Task<IActionResult> Post([FromBody] DeprecatePackageRequestDto deprecatePackageRequestDto)
        {
            UnpackedJwt unpackedJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());


            await this._packageService.UpdateDeprecateValueInPackage(deprecatePackageRequestDto.PackageName,
                                                                     unpackedJwt.Username, 
                                                                     true);

            return this.Ok();
        }

        [HttpPost]
        [Authorize]
        [Route(template: "undeprecate")]
        public async Task<IActionResult> UndeprecatePost([FromBody] DeprecatePackageRequestDto undeprecatePackageRequestDto)
        {
            UnpackedJwt unpackedJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());


            await this._packageService.UpdateDeprecateValueInPackage(undeprecatePackageRequestDto.PackageName,
                                                                     unpackedJwt.Username,
                                                                     false);

            return this.Ok();
        }

        [HttpPost]
        [Authorize]
        [Route(template: "admin")]
        public async Task<IActionResult> Post([FromBody] AddAdminToPackageRequestDto addAdminToPackageRequestDto)
        {
            UnpackedJwt unpackedJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            await this._packageService.AddAdminUserToPackage(addAdminToPackageRequestDto.PackageName,
                                                             addAdminToPackageRequestDto.Username,
                                                             unpackedJwt.Username);

            return this.Ok();
        }

        [HttpDelete]
        [Authorize]
        [Route(template: "admin")]
        public async Task<IActionResult> Delete([FromBody] RemoveAdminToPackageRequestDto removeAdminToPackageRequestDto)
        {
            UnpackedJwt unpackedJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            await this._packageService.RemoveAdminPermissonFromUserForPackage(removeAdminToPackageRequestDto.PackageName,
                                                                              removeAdminToPackageRequestDto.Username,
                                                                              unpackedJwt.Username);

            return this.Ok();
        }

        [HttpPost]
        [Authorize]
        [Route(template: "user")]
        public async Task<IActionResult> Post([FromBody] AddUserToPackageRequestDto addUserToPackageRequestDto)
        {
            UnpackedJwt unpackedJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            await this._packageService.AddUserToPackage(addUserToPackageRequestDto.PackageName,
                                                        addUserToPackageRequestDto.Username,
                                                        unpackedJwt.Username);

            return this.Ok();
        }

        [HttpDelete]
        [Authorize]
        [Route(template: "user")]
        public async Task<IActionResult> Delete([FromBody] RemoveUserToPackageRequestDto removeUserToPackageRequestDto)
        {
            UnpackedJwt unpackedJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            await this._packageService.AddUserToPackage(removeUserToPackageRequestDto.PackageName,
                                                        removeUserToPackageRequestDto.Username,
                                                        unpackedJwt.Username);

            return this.Ok();
        }

        [HttpPost]
        [Authorize]
        [Route(template: "transfer")]
        public async Task<IActionResult> Post([FromBody] TransferOwnershipOfPackageRequestDto transferOwnershipOfPackageRequestDto)
        {
            UnpackedJwt unpackedJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            await this._packageService.TransferPackageOwner(transferOwnershipOfPackageRequestDto.PackageName,
                                                            transferOwnershipOfPackageRequestDto.Username,
                                                            unpackedJwt.Username);

            return this.Ok();
        }

        [HttpGet]
        [Authorize]
        [Route(template: "admin/{packageName}")]
        public async Task<IActionResult> GetAdminUsers([FromRoute] string packageName)
        {
            UnpackedJwt unpackedJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            IReadOnlyList<string> adminUsers = await this._packageService.GetAdminUsers(packageName, unpackedJwt.Username);

            return this.Ok(new { users = adminUsers });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] UploadPackageRequestDto uploadPackageRequestDto)
        {
            IList<PackageFile> files = uploadPackageRequestDto.PackageFiles
                                                              .Select(file => new PackageFile(file.FileName, file.FileContent))
                                                              .ToList();

            // put a hard limit on the amount of files per package for now
            if (files.Count > 99)
            {
                return this.BadRequest("can not have more then 100 items in a package");
            }

            PackageFile ethereumPm = files.FirstOrDefault(f => f.FileName == Constants.EthereumPmJson);
            if (ethereumPm == null)
            {
                return this.BadRequest("To upload a package you need a `ethereum-pm.json` file");
            }

            JObject ethereumPmJson;

            try
            {
                ethereumPmJson = JObject.Parse(ethereumPm.FileContent);
            }
            catch (Exception)
            {
                return this.BadRequest("Invalid JSON - `ethereum-pm.json`");
            }

            EthereumPmMetaData metaData = new EthereumPmMetaData()
            {
                Description = (string)ethereumPmJson["description"],
                GitHub = (string)ethereumPmJson["github"],
                Private = (bool?)ethereumPmJson["private"] ?? false,
                Team = (string)ethereumPmJson["team"],
                Keywords = (IList<string>)ethereumPmJson["keywords"]
            };

            PackageFiles packageFiles = new PackageFiles
            (
                uploadPackageRequestDto.PackageVersion,
                uploadPackageRequestDto.PackageName,
                (IReadOnlyCollection<PackageFile>)files
            );

            try
            {
                UnpackedJwt unpackedJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

                await this._packageService.UploadPackageAsync(packageFiles, metaData, unpackedJwt.Username);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex);
            }

            return this.Ok();
        }
    }
}
