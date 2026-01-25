using approvals.application.DTOs.EnumDtos;
using System.ComponentModel.DataAnnotations;

namespace approvals.application.DTOs.StageInstance
{
    public class StageActionDto
    {
        [Required]
        public Guid UserId { get; set; }

        public string? Comment { get; set; }

        [Required]
        public StageInstanceStatusDto Action { get; set; } 
    }
}
