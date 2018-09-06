using epm_api.Models;

namespace epm_api.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(UsersEntity user, int expiryMinutes = 30);
        Profile UnPackJwtToProfile(string jwtToken);
    }
}