using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using epm_api.Dtos.EpmPackages;

namespace epm_api.Packages.Dtos.EpmPackages
{
    public class UploadPackageRequestDto
    {
        [Required]
        public string PackageName { get; set; }

        [Required]
        public string PackageVersion { get; set; }

        [Required]
        public IReadOnlyCollection<PackageFilesRequestDto> PackageFiles { get; set; }
    }
}
