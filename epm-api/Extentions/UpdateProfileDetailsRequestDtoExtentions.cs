using epm_api.Dtos.Profile;
using epm_api.Models;

namespace epm_api.Extentions
{
    public static class UpdateProfileDetailsRequestDtoExtentions
    {
        public static UsersEntity ToEntity(this UpdateProfileDetailsRequestDto dto, string username)
        {
            return new UsersEntity()
            {
                Username = username,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Introduction = dto.Introduction,
            };
        }

    }
}
