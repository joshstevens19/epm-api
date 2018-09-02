using System.Collections.Generic;
using System.Threading.Tasks;
using epm_api.Models;

namespace epm_api.Services.Interfaces
{
    public interface IPackageService
    {
        Task<PackageFiles> GetPackageFilesAsync(string packageName, string version);
    }
}