using System.Collections.Generic;
using System.Threading.Tasks;
using epm_api.Models;
using epm_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace epm_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        private readonly IS3Service _client;

        public PackagesController(IS3Service client)
        {
            this._client = client;
        }

        [HttpGet]
        [Route(template: "{packageName}")]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromRoute] string packageName)
        {
            string latestVersion = await this._client.GetLatestVersionOfPackge(packageName);

            IReadOnlyCollection<PackageFile> packageFiles = await this._client.GetPackageFiles(packageName, latestVersion);

            return this.Ok(packageFiles);
        }
    }
}
