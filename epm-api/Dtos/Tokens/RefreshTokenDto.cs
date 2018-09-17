using System.ComponentModel.DataAnnotations;

namespace epm_api.Dtos.Tokens
{
    public class RefreshTokenDto
    {
        [Required]
        public string CurrentJwtToken { get; set; }
    }
}
