using System;
using System.Net;
using System.Threading.Tasks;
using epm_api.Dtos;
using epm_api.Models;
using epm_api.Services;
using epm_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace epm_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenicationController : ApiController
    {
        private readonly IJwtService _jwtService;
        private readonly IAuthenticationService _authenticationService;

        public AuthenicationController(IJwtService jwtService, IAuthenticationService authenticationService)
        {
            this._jwtService = jwtService;
            this._authenticationService = authenticationService;
        }

        /// <summary>
        /// Validates if a jwt is still valid 
        /// </summary>
        /// <returns></returns>
        [Route(template: "validate")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult Get()
        {
            return this.Ok();
        }

        /// <summary>
        /// Logins the user in using there username and password
        /// </summary>
        /// <returns></returns>
        [Route(template: "login")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody] LoginRequestDto loginRequestDto)
        {
            int expiryMinutes = 30;
            if (loginRequestDto.ExpiryMinutes.HasValue)
            {
                expiryMinutes = loginRequestDto.ExpiryMinutes.Value;
            }

            if (!await this._authenticationService.Login(loginRequestDto.Username, loginRequestDto.Password))
                return this.BadRequest();

            string jwtToken = this._jwtService.GenerateToken(loginRequestDto.Username, expiryMinutes);
            DateTime expiryDate = DateTime.Now.AddMinutes(expiryMinutes);

            return this.Ok(new JwtDetails(jwtToken, expiryDate));
        }

        [Route(template: "logout")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult PostLogout()
        {
            return this.Ok();
        }
    }
}
