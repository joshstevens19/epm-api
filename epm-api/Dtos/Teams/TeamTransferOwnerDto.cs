using System.ComponentModel.DataAnnotations;

namespace epm_api.Dtos.Teams
{
    public class TeamTransferOwnerDto
    {
        [Required]
        public string TeamName { get; set; }

        [Required]
        public string NewOwner { get; set; }
    }
}
