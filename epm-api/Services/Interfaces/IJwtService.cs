using System.Collections.Generic;
using System.Security.Claims;
using epm_api.Models;

namespace epm_api.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(UsersEntity user, int expiryMinutes = 30);
        UnpackedJwt UnpackJwtClaimsToProfile(IList<Claim> claims);
    }
}