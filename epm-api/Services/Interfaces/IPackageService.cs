using System.Collections.Generic;
using System.Threading.Tasks;
using epm_api.Models;

namespace epm_api.Services.Interfaces
{
    public interface IPackageService
    {
        Task<PackageFiles> GetPackageFilesAsync(string packageName, string version, string jwtUsername);
        Task UploadPackageAsync(PackageFiles packageFiles, EthereumPmMetaData ethereumPmMetaData, string jwtUsername);
        Task AddAdminUserToPackage(string packageName, string username, string jwtUsername);
        Task AddUserToPackage(string packageName, string username, string jwtUsername);
        Task RemoveUserFromPackage(string packageName, string username, string jwtUsername);
        Task RemoveAdminPermissonFromUserForPackage(string packageName, string username, string jwtUsername);
        Task TransferPackageOwner(string packageName, string username, string jwtUsername);
        Task UpdateDeprecateValueInPackage(string packageName, string jwtUsername, bool deprecate);
        Task UnpublishPackage(string packageName, string jwtUsername);
        Task<IReadOnlyList<string>> GetAdminUsers(string packageName, string jwtUsername);
    }
}