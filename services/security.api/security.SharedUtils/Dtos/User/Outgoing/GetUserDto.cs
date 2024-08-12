using security.sharedUtils.Dtos.User.Common;

namespace security.sharedUtils.Dtos.User.Outgoing
{
    public class GetUserDto : UserDto
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
    }
}
