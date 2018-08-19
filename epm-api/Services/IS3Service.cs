using System.Collections.Generic;
using System.Threading.Tasks;
using epm_api.Models;

namespace epm_api.Services
{
    public interface IS3Service
    {
        Task<string> GetLatestVersionOfPackge(string packageName);
        Task<IReadOnlyCollection<PackageFile>> GetPackageFilesDetails(string packageName, string version);
    }
}