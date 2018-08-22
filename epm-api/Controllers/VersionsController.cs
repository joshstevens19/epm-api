using System.Threading.Tasks;
using epm_api.Services;
using epm_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace epm_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionsController : ControllerBase
    {
        private readonly IVersionService _versionService;

        public VersionsController(IVersionService versionService)
        {
           this._versionService = versionService;
        }

        [HttpGet]
        [Route(template: "{packageName}/latest")]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromRoute] string packageName)
        {
            string latestVersion = await this._versionService.GetLatestVersionOfPackge(packageName);

            if (latestVersion == null)
            {
                return this.NotFound();
            }

            return this.Ok(new { latestVersion });
        }
    }
}
