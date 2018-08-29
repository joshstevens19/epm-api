using epm_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace epm_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : BaseApiController
    {
        [HttpGet]
        [Route(template: "")]
        public IActionResult Get()
        {
            // check s3 is alive maybe!? do not want to do pointless chargable API calls though..
            return this.Ok();
        }
    }
}
