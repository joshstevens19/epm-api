using System;
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
        private const string ClaimEmailAddress = "emailAddress";
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
                new Claim(ClaimEmailAddress, user.EmailAddress),
                new Claim(ClaimFirstName, user.FirstName), 
                new Claim(ClaimLastName, user.LastName), 
                new Claim(ClaimIntroduction, user.Introduction)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(symmetricKey);
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                // audience: "yourdomain.com", // sort domain later
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: creds);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Profile UnPackJwtToProfile(string jwtToken)
        {
            JwtSecurityToken decodedJwt = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);

            Claim emailAddress = this.GetClaim(decodedJwt, ClaimEmailAddress);
            Claim firstName = this.GetClaim(decodedJwt, ClaimFirstName);
            Claim lastName = this.GetClaim(decodedJwt, ClaimLastName);
            Claim introduction = this.GetClaim(decodedJwt, ClaimIntroduction);

            return new Profile(
                emailAddress?.Value, 
                firstName?.Value, 
                lastName?.Value, 
                introduction?.Value
            );
        }

        private Claim GetClaim(JwtSecurityToken decodedJwt, string claimType)
        {
            return decodedJwt.Claims.FirstOrDefault(c => c.Type == claimType);
        }
    }
}
