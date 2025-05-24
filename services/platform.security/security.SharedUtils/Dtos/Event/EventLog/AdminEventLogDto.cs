namespace security.sharedUtils.Dtos.Event.Event
{
    public class AdminEventLogDto
    {
        public long Time { get; set; }
        public AuthDetailsDto? AuthDetails { get; set; }
        public string? OperationType { get; set; }
        public string? ResourceType { get; set; }
        public string? ResourcePath { get; set; }
        public string? Representation { get; set; }
    }

}
