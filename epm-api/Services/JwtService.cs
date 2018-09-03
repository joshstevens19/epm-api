using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using epm_api.Models;
using epm_api.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace epm_api.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public string GenerateToken(UsersEntity user, int expiryMinutes = 30)
        {
            byte[] symmetricKey = Convert.FromBase64String(_configuration["SecurityKey"]);

            Claim[] claims = new[]
            {
                new Claim("emailAddress", user.EmailAddress),
                new Claim("firstName", user.FirstName), 
                new Claim("lastName", user.LastName), 
                new Claim("introduction", user.Introduction)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(symmetricKey);
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: creds);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string UnPackJWT(string jwtToken)
        {
            // to do
            return string.Empty;
        }
    }
}
