namespace epm_api.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(string username, int expiryMinutes = 30);
        string UnPackJWT(string jwtToken);
    }
}