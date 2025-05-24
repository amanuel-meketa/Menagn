namespace security.sharedUtils.Dtos.Event.Event
{
    public class EventLogDetailsDto
    {
        public string? Username { get; set; }
        public string? AuthMethod { get; set; }
        public string? TokenId { get; set; }
        public string? GrantType { get; set; }
        public string? Scope { get; set; }
    }
}
