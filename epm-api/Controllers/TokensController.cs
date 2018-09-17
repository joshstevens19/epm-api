using System;
using System.Linq;
using System.Net;
using epm_api.Dtos.Tokens;
using epm_api.Models;
using epm_api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace epm_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : AuthorizeApiController
    {
        private readonly IJwtService _jwtService;

        public TokensController(IJwtService jwtService)
        {
            this._jwtService = jwtService;
        }

        /// <summary>
        /// Validates if a jwt is still valid 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route(template: "validate")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult Get()
        {
            return this.Ok();
        }

        /// <summary>
        /// Validates if a jwt is still valid 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route(template: "refresh")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult Post([FromBody] RefreshTokenDto refreshTokenDto)
        {
            int expiryMinutes = 30;
            DateTime expiryDate = DateTime.Now.AddMinutes(expiryMinutes);

            string jwtToken = this._jwtService.RefreshToken(User.Claims.ToArray(), expiryMinutes);

            return this.Ok(new JwtDetails(jwtToken, expiryDate));
        }

    }
}
