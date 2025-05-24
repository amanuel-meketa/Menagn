using security.sharedUtils.Dtos.User.Common;

namespace security.sharedUtils.Dtos.User.Incoming
{
    public class CreateUserDto : UserDto
    {
        public string? Username { get; set; }
    }
}
