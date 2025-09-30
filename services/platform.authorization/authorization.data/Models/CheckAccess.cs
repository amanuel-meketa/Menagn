
namespace authorization.data.Models
{
    public class CheckAccess
    {
        public required string UserId { get; set; }
        public required string Resource { get; set; }
        public required string Relation { get; set; }
    }
}
