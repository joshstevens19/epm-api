using System.Collections.Generic;
using System.Threading.Tasks;
using epm_api.Models;

namespace epm_api.Services.Interfaces
{
    public interface IVersionService
    {
        Task<string> GetLatestVersionOfPackgeAsync(string packageName);
    }
}