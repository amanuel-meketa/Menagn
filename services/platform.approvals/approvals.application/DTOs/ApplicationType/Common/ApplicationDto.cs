namespace approvals.application.DTOs.ApplicationType.Common
{
    public class ApplicationBaseDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public class ApplicationDto : ApplicationBaseDto
    {
        public Guid Id { get; set; }
    }
}
