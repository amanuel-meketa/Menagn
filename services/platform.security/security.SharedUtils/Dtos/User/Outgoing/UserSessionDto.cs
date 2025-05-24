namespace security.sharedUtils.Dtos.User.Outgoing
{
    public class UserSessionDto
    {
        public string Id { get; set; }
        public string? IpAddress { get; set; }
        public string? Start { get; set; }
        public string? LastAccess { get; set; }
    }
}