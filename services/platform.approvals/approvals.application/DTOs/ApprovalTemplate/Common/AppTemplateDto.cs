namespace approvals.application.DTOs.ApplicationType.Common
{
    public class AppBaseDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public class AppTemplateDto : AppBaseDto
    {
        public Guid Id { get; set; }
    }
}
