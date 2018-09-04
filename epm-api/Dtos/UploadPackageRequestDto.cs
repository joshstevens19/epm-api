using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace epm_api.Dtos
{
    public class UploadPackageRequestDto
    {
        [Required]
        public string PackageName { get; set; }

        [Required]
        public string PackageVersion { get; set; }

        [Required]
        public IReadOnlyList<PackageFilesRequestDto> PackageFiles { get; set; }
    }
}
