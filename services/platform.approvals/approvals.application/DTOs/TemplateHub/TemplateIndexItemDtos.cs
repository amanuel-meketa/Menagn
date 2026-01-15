
namespace approvals.application.DTOs.TemplateHub
{
    public class TemplateIndexItemDtos
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = "Unnamed Template";
        public string Category { get; set; } = "General";
        public string Description { get; set; } = string.Empty;
        public int Stages { get; set; }
    }
}
