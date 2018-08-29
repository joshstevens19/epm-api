using System;
using System.Threading.Tasks;
using epm_api.Services.Interfaces;

namespace epm_api.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private IJwtService _jwtService;
        public AuthenticationService(IJwtService jwtService)
        {
            this._jwtService = jwtService;
        }

        public async Task<Boolean> Login(string username, string password)
        {
            return await Task.FromResult(true);
        }
    }
}

