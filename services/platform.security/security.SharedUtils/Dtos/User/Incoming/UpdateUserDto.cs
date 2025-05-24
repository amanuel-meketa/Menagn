using security.sharedUtils.Dtos.User.Common;

namespace security.sharedUtils.Dtos.User.Incoming
{
    public class UpdateUserDto : UserDto
    {
        public bool? EmailVerified { get; set; }
        public bool? IsLocked { get; set; }
    }
}
