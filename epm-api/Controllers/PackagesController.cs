using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using epm_api.Common;
using epm_api.Dtos;
using epm_api.Models;
using epm_api.Services;
using epm_api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace epm_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackagesController : BaseApiController
    {
        private readonly IPackageService _packageService;
        private readonly IVersionService _versionService;

        public PackagesController(IPackageService packageService, IVersionService versionService)
        {
            this._packageService = packageService;
            this._versionService = versionService;
        }

        [HttpGet]
        [Route(template: "{packageName}")]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromRoute] string packageName)
        {
            string latestVersion = await this._versionService.GetLatestVersionOfPackgeAsync(packageName);

            PackageFiles packageFiles = await this._packageService.GetPackageFilesAsync(packageName, latestVersion);

            return this.Ok(packageFiles);
        }

        [HttpPost]
        [Authorize]
        [Route(template: "")]
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
                GitHub = (string)ethereumPmJson["github"],
                Private = (bool?)ethereumPmJson["private"] ?? false,
                Team = (string)ethereumPmJson["team"]
            };

            PackageFiles packageFiles = new PackageFiles
            (
                uploadPackageRequestDto.PackageVersion,
                uploadPackageRequestDto.PackageName,
                (IReadOnlyCollection<PackageFile>)files
            );

            try
            {
                await this._packageService.UploadPackageAsync(packageFiles, metaData);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex);
            }

            return this.Ok();
        }
    }
}
