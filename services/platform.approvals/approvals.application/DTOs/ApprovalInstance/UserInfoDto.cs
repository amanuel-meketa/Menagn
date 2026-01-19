using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace approvals.application.DTOs.ApprovalInstance
{
    public class UserInfoDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;
    }
}
