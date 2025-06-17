using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.ApplicationUserDtos.AccountUserDtos
{
    public class LogoutRequestDto
    {
        public string UserId { get; set; }
        public string Token { get; set; } // Optional: for token blacklisting
        public string? DeviceId { get; set; } // Optional: for device-specific logout
    }
}
