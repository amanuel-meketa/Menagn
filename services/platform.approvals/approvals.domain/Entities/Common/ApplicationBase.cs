namespace approvals.domain.Entities.Common
{
    public abstract class EntityBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public abstract class ApplicationBase : EntityBase
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
       
    }

    public class Application : ApplicationBase
    {
        public bool IsActive { get; set; }
    }
}
