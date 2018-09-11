using System.ComponentModel.DataAnnotations;

namespace epm_api.Dtos.Teams
{
    public class CreateTeamRequestDto
    {
        [Required]
        public string TeamName { get; set; }

        [Required]
        public bool Private { get; set; }
    }
}
