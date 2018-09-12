using System.ComponentModel.DataAnnotations;

namespace epm_api.Dtos.Stars
{
    public class StarProjectRequestDto
    {
        [Required]
        public string PackageName { get; set; }
    }
}
