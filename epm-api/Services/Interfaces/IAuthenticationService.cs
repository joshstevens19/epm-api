using System.Threading.Tasks;

namespace epm_api.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> Login(string username, string password);
    }
}