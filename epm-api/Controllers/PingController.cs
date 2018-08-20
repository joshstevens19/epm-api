using epm_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace epm_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly IS3Service _client;

        public PingController(IS3Service client)
        {
            this._client = client;
        }

        [HttpGet]
        [Route(template: "")]
        public IActionResult Get()
        {
            // check s3 is alive maybe!? do not want to do pointless chargable API calls though..
            return this.Ok();
        }
    }
}
