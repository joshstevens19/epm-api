using System.ComponentModel.DataAnnotations;

namespace epm_api.Dtos.EpmPackages
{
    public class DeprecatePackageRequestDto
    {
        [Required]
        public string PackageName { get; set; }
    }
}
