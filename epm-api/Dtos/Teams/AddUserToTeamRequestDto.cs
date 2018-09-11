using System.ComponentModel.DataAnnotations;

namespace epm_api.Dtos.Teams
{
    public class AddUserToTeamRequestDto
    {
        [Required]
        public string TeamName { get; set; }

        [Required]
        public string NewUser { get; set; }

        [Required]
        public bool IsAdmin { get; set; }
    }
}
