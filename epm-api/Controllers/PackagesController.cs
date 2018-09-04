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
        [Authorize]
        [Route(template: "")]
        public IActionResult Post([FromBody] UploadPackageRequestDto uploadPackageRequestDto)
        {
            return this.Ok(uploadPackageRequestDto);
        }
    }
}
