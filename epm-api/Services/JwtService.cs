using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using epm_api.Models;
using epm_api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace epm_api.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        // claims consts
        private const string ClaimUsername = "username";
        private const string ClaimFirstName = "firstName";
        private const string ClaimLastName = "lastName";
        private const string ClaimIntroduction = "introduction";

        public JwtService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public string GenerateToken(UsersEntity user, int expiryMinutes = 30)
        {
            byte[] symmetricKey = Convert.FromBase64String(_configuration["SecurityKey"]);

            Claim[] claims = new[]
            {
                new Claim(ClaimUsername, user.Username),
                new Claim(ClaimFirstName, user.FirstName), 
                new Claim(ClaimLastName, user.LastName), 
                new Claim(ClaimIntroduction, user.Introduction)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(symmetricKey);
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                // issuer: "yourdomain.com",
                // audience: "yourdomain.com", // sort domain later
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: creds);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public UnpackedJwt UnpackJwtClaimsToProfile(IList<Claim> claims)
        {
            Claim emailAddress = this.GetClaim(claims, ClaimUsername);
            Claim firstName = this.GetClaim(claims, ClaimFirstName);
            Claim lastName = this.GetClaim(claims, ClaimLastName);
            Claim introduction = this.GetClaim(claims, ClaimIntroduction);

            return new UnpackedJwt(
                emailAddress?.Value, 
                firstName?.Value, 
                lastName?.Value, 
                introduction?.Value
            );
        }

        private Claim GetClaim(IEnumerable<Claim> claims, string claimType)
        {
            return claims.FirstOrDefault(c => c.Type == claimType);
        }
    }
}
