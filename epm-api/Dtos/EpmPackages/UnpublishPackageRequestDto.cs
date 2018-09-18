using System.ComponentModel.DataAnnotations;

namespace epm_api.Dtos.EpmPackages
{
    public class UnpublishPackageRequestDtos
    {
        [Required]
        public string PackageName { get; set; }
    }
}
