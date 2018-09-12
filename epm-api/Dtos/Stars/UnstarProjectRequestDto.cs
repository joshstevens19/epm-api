using System.ComponentModel.DataAnnotations;

namespace epm_api.Dtos.Stars
{
    public class UnstarProjectRequestDto
    {
        [Required]
        public string PackageName { get; set; }
    }
}
