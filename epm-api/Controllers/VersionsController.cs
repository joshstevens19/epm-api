using System.Threading.Tasks;
using epm_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace epm_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionsController : ControllerBase
    {
        private readonly IS3Service _client;

        public VersionsController(IS3Service client)
        {
           this._client = client;
        }

        [HttpGet]
        [Route(template: "{packageName}/latest")]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromRoute] string packageName)
        {
            string latestVersion = await this._client.GetLatestVersionOfPackge(packageName);

            if (latestVersion == null)
            {
                return this.NotFound();
            }

            return this.Ok(new { latestVersion });
        }
    }
}
