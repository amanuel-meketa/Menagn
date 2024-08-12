namespace security.sharedUtils.Dtos.User.Outgoing
{
    public class GetUserDto
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
    }
}
