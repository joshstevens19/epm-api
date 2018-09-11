using System;
using System.Net;
using System.Threading.Tasks;
using epm_api.Dtos;
using epm_api.Dtos.Extentions;
using epm_api.Models;
using epm_api.Services;
using epm_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace epm_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ApiController
    {
        private readonly IJwtService _jwtService;
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IJwtService jwtService, IAuthenticationService authenticationService)
        {
            this._jwtService = jwtService;
            this._authenticationService = authenticationService;
        }

        /// <summary>
        /// Validates if a jwt is still valid 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
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
        [HttpPost]
        [Route(template: "login")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody] LoginRequestDto loginRequestDto)
        {
            int expiryMinutes = 30;
            if (loginRequestDto.ExpiryMinutes.HasValue)
            {
                expiryMinutes = loginRequestDto.ExpiryMinutes.Value;
            }

            UsersEntity user = await this._authenticationService.LoginAsync(loginRequestDto.Username, loginRequestDto.Password);

            if (user == null)
            {
                return this.BadRequest();
            }

            string jwtToken = this._jwtService.GenerateToken(user, expiryMinutes);
            DateTime expiryDate = DateTime.Now.AddMinutes(expiryMinutes);

            return this.Ok(new JwtDetails(jwtToken, expiryDate));
        }

        [HttpPost]
        [Route(template: "logout")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult Post()
        {
            return this.Ok();
        }

        [HttpPost]
        [Route(template: "register")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> PostRegister([FromBody] RegisterRequestDto registerRequestDto)
        {
            UsersEntity user = await this._authenticationService.RegisterAsync(registerRequestDto.ToEntity());
            if (user == null)
            {
                return this.BadRequest();
            }

            const int expiryMinutes = 30;
            string jwtToken = this._jwtService.GenerateToken(user, expiryMinutes);
            DateTime expiryDate = DateTime.Now.AddMinutes(expiryMinutes);

            return this.Ok(new JwtDetails(jwtToken, expiryDate));
        }
    }
}
