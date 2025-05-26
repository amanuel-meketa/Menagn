namespace approvals.application.DTOs.ApplicationType
{
    public class ApplicationTypeDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid? FormSchemaJson { get; set; }
        public Guid? ApprovaleDefinitionJson { get; set; }
    }
}
