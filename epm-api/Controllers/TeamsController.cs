using System.Linq;
using System.Net;
using System.Threading.Tasks;
using epm_api.Dtos.Teams;
using epm_api.Models;
using epm_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace epm_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : AuthorizeApiController
    {
        private readonly IJwtService _jwtService;
        private readonly ITeamService _teamService;

        public TeamsController(IJwtService jwtService, ITeamService teamService)
        {
            this._jwtService = jwtService;
            this._teamService = teamService;
        }

        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody] CreateTeamRequestDto createTeamRequestDto)
        {
            UnpackedJwt unpackJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            await this._teamService.CreateTeam(createTeamRequestDto.TeamName,
                createTeamRequestDto.Private,
                unpackJwt.Username);

            return this.Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            UnpackedJwt unpackJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            // do the logic to get the teams this user is part off
            // split up by admin and normal user 

            return this.Ok();
        }

        [HttpPost]
        [Route(template: "users")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> AddUserToTeamPost([FromBody] AddUserToTeamRequestDto addUserToTeamRequestDto)
        {
            UnpackedJwt unpackJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            await this._teamService.AddUser(addUserToTeamRequestDto.TeamName,
                addUserToTeamRequestDto.NewUser,
                addUserToTeamRequestDto.IsAdmin,
                unpackJwt.Username);

            return this.Ok();
        }

        [HttpPost]
        [Route(template: "transfer")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> TransferOwnerPost([FromBody] TeamTransferOwnerDto teamTransferOwnerDto)
        {
            UnpackedJwt unpackJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            await this._teamService.TransferOwner(teamTransferOwnerDto.TeamName,
                teamTransferOwnerDto.NewOwner,
                unpackJwt.Username);

            return this.Ok();
        }

        [HttpDelete]
        [Route(template: "users")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> Delete([FromBody] DeleteUserFromTeamRequestDto deleteUserFromTeamRequestDto)
        {
            UnpackedJwt unpackJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            await this._teamService.RemoveUser(
                deleteUserFromTeamRequestDto.TeamName,
                deleteUserFromTeamRequestDto.DeleteUsername,
                unpackJwt.Username);

            return this.Ok();
        }

        [HttpDelete]
        [Route(template: "users/admin")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> Delete([FromBody] RevokeAdminPermissionFromUserForTeamRequestDto revokeAdminPermissionFromUserForTeamRequestDto)
        {
            UnpackedJwt unpackJwt = this._jwtService.UnpackJwtClaimsToProfile(User.Claims.ToList());

            await this._teamService.RemoveUser(
                revokeAdminPermissionFromUserForTeamRequestDto.TeamName,
                revokeAdminPermissionFromUserForTeamRequestDto.Username,
                unpackJwt.Username);

            return this.Ok();
        }

    }
}