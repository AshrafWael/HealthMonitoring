using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.ApplicationUserDtos.AccountUserDtos
{
    public class ResetPasswordDto
    {
        [Required]
        public string email { get; set; } = null!;

        public string CurrentPassword { get; set; } = null!;

        public string NewPassword { get; set; } = null!;
    }
}
