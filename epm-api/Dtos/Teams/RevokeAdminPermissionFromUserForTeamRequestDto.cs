using System.ComponentModel.DataAnnotations;

namespace epm_api.Dtos.Teams
{
    public class RevokeAdminPermissionFromUserForTeamRequestDto
    {
        [Required]
        public string TeamName { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
