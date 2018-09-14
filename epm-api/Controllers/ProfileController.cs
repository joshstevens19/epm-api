using System.Linq;
using System.Threading.Tasks;
using epm_api.Dtos.Profile;
using epm_api.Extentions;
using epm_api.Models;
using epm_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace epm_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : AuthorizeApiController
    {
        private readonly IJwtService _jwtService;
        private readonly IProfileService _profileService;

        public ProfileController(IJwtService jwtService, IProfileService profileService)
        {
            this._jwtService = jwtService;
            this._profileService = profileService;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult Get()
        {
            UnpackedJwt unpackJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            return this.Ok((Profile)unpackJwt);
        }

        [HttpPut]
        [Produces("application/json")]
        public async Task<IActionResult> Put([FromBody] UpdateProfileDetailsRequestDto updateProfileDetailsRequestDto)
        {
            UnpackedJwt unpackJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            UsersEntity usersEntity = await this._profileService.UpdateDetails(updateProfileDetailsRequestDto.ToEntity(unpackJwt.Username));

            if (usersEntity == null)
            {
                this.BadRequest("Could not update user");
            }

            string jwtToken = this._jwtService.GenerateToken(usersEntity);
            
            return this.Ok(new { jwtToken });
        }
    }
}