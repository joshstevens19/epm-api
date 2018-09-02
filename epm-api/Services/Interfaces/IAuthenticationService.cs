using System.Threading.Tasks;

namespace epm_api.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> LoginAsync(string username, string password);
    }
}