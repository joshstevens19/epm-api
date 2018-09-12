using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using epm_api.Dtos.Stars;
using epm_api.Models;
using epm_api.Services;
using epm_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace epm_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StarsController : AuthorizeApiController
    {
        private readonly IJwtService _jwtService;
        private readonly IStarService _starService;
        public StarsController(IStarService starService, IJwtService jwtService)
        {
            this._starService = starService;
            this._jwtService = jwtService;
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> Get()
        {
            UnpackedJwt unpackJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            IReadOnlyList<string> starredPackages = await this._starService.GetStarredProjects(unpackJwt.Username);

            return this.Ok(new { packages = starredPackages });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StarProjectRequestDto starProjectRequestDto)
        {
            UnpackedJwt unpackJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            await this._starService.StarPackage(starProjectRequestDto.PackageName, unpackJwt.Username);

            return this.Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] UnstarProjectRequestDto unstarProjectRequestDto)
        {
            UnpackedJwt unpackJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            await this._starService.UnstarPackage(unstarProjectRequestDto.PackageName, unpackJwt.Username);

            return this.Ok();
        }
    }
}
