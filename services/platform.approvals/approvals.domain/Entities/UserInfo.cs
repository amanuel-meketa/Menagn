using System.ComponentModel.DataAnnotations;

namespace approvals.domain.Entities
{
    public class UserInfo
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;
    }
}
