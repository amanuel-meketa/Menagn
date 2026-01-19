

namespace approvals.application.DTOs.ApprovalInstance
{
    public class AppTemplateDto
    {
        public Guid TemplateId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
}
