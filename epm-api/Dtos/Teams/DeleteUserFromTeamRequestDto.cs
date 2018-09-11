using System.ComponentModel.DataAnnotations;

namespace epm_api.Dtos.Teams
{
    public class DeleteUserFromTeamRequestDto
    {
        [Required]
        public string TeamName { get; set; }

        [Required]
        public string DeleteUsername { get; set; }
    }
}
