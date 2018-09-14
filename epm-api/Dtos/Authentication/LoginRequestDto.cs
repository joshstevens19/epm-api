using System.ComponentModel.DataAnnotations;

namespace epm_api.Dtos.Authentication
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
