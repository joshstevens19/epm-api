using System.ComponentModel.DataAnnotations;

namespace epm_api.Dtos
{
    public class RegisterRequestDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Introduction { get; set; }
    }
}
