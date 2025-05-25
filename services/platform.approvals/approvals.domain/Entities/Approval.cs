
namespace approvals.domain.Entities
{
    public class Approval
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
        public string? Status { get; set; }
    }
}
