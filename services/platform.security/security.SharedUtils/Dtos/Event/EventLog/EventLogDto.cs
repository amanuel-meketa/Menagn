namespace security.sharedUtils.Dtos.Event.Event
{
    public class EventLogDto
    {
        public long Time { get; set; }
        public string? Type { get; set; }
        public string? UserId { get; set; }
        public string? SessionId { get; set; }
        public string? IpAddress { get; set; }
        public EventLogDetailsDto? Details { get; set; }
    }

}
