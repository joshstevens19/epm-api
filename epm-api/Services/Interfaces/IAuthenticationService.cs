using System.Threading.Tasks;
using epm_api.Dtos;
using epm_api.Models;

namespace epm_api.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<UsersEntity> LoginAsync(string username, string password);
        Task<UsersEntity> RegisterAsync(UsersEntity userEntity);
    }
}