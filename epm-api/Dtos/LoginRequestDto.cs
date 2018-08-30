using System.ComponentModel.DataAnnotations;

namespace epm_api.Dtos
{
    public class LoginRequestDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public int? ExpiryMinutes { get; set; }
    }
}
