using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using epm_api.Dtos;
using epm_api.Models;
using epm_api.Services;
using epm_api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [Route(template: "")]
        public async Task<IActionResult> Post([FromBody] UploadPackageRequestDto uploadPackageRequestDto)
        {
            IList<PackageFile> files = new List<PackageFile>();

            foreach (var file in uploadPackageRequestDto.PackageFiles)
            {
                files.Add(new PackageFile(file.FileName, file.FileContent));
            }

            PackageFiles packageFiles = new PackageFiles
            (
                uploadPackageRequestDto.PackageVersion,
                uploadPackageRequestDto.PackageName,
                (IReadOnlyCollection<PackageFile>)files
            );

            try
            {
                await this._packageService.UploadPackageAsync(packageFiles);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex);
            }

            return this.Ok();
        }
    }
}
