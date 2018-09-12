using System.Collections.Generic;
using System.Threading.Tasks;
using epm_api.Models;

namespace epm_api.Services.Interfaces
{
    public interface IStarService
    {
        Task StarPackage(string packageName, string jwtUsername);
        Task UnstarPackage(string packageName, string jwtUsername);
        Task<IReadOnlyList<string>> GetStarredProjects(string jwtUsername);
    }
}